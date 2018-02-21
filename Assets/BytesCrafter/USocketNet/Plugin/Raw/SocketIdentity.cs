
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BytesCrafter.USocketNet;

public class SocketIdentity : MonoBehaviour
{
	private SocketPrototype socketNet = null;
	public SocketPrototype uSocketNet
	{
		set
		{
			socketNet = value;
		}
	}

	[HideInInspector] public string socketId = string.Empty;
	[HideInInspector] public bool IsLocalUser = false;

	[Range(1f, 30f)] public float sendRate = 12f;
	public bool syncOnTrans = true;

	[Header("IF TRANSFORM")]

	public TransAxis position = new TransAxis();
	public TransAxis rotation = new TransAxis();
	public TransAxis scale = new TransAxis();

	[Header("ELSE CUSTOMS")]
	public CustomState states = new CustomState();

	private float timer = 0f;
	private List<string> lastState = new List<string>();
	private TransJson lastTrans = new TransJson();
	void Update()
	{
		if (socketId == string.Empty)
			return;

		timer += Time.deltaTime;

		if(timer < (1f/sendRate))
			return;

		if(IsLocalUser)
		{
			if(syncOnTrans)
			{
				//Send data as transform.
				TransJson trasJson = new TransJson();

				if(position.synchronize)
				{
					if(!lastTrans.Position.Equals(transform.position))
					{
						trasJson.AddPosition (transform.position);
						position.syncStatus = "Synchronizing...";
					}

					else
					{
						position.syncStatus = "Waiting...";
					}
				}

				if(rotation.synchronize)
				{
					if(!lastTrans.Rotation.Equals(transform.rotation.eulerAngles))
					{
						trasJson.AddRotation (transform.rotation.eulerAngles);
						rotation.syncStatus = "Synchronizing...";
					}

					else
					{
						rotation.syncStatus = "Waiting...";
					}
				}

				if(scale.synchronize)
				{
					if(!lastTrans.Scale.Equals(transform.lossyScale))
					{
						trasJson.AddScale (transform.lossyScale);
						scale.syncStatus = "Synchronizing...";
					}

					else
					{
						scale.syncStatus = "Waiting...";
					}
				}

				if(socketNet != null)
				{
					socketNet.TranSync (trasJson);
					lastTrans = trasJson;
				}

				//Note: First check if values is same then dont send.
			}

			else
			{
				//Send data as transform.
				StateJson stateJson = new StateJson();
				stateJson.states = states.syncValue;

				if(states.synchronize)
				{
					if(lastState.Equals(stateJson.states))
					{
						states.syncStatus = "Synchronizing...";
					}

					else
					{
						states.syncStatus = "Waiting...";
					}
				}

				if(socketNet != null)
				{
					socketNet.StateSync (stateJson);
					lastState = stateJson.states;
				}
			}
		}

		timer = 0f;
	}

	public void Downdate(string jsonData)
	{
		if(!IsLocalUser)
		{
			if(syncOnTrans)
			{
				TransJson transJson = JsonUtility.FromJson<TransJson>(jsonData);

				if(position.synchronize && !transJson.Position.Equals(Vector3.zero))
				{
					if(position.syncMode == SocketSync.Realtime)
					{
						transform.position = transJson.Position;
					}

					else if(position.syncMode == SocketSync.AdjustToward)
					{
						transform.position = Vector3.MoveTowards (transform.position, transJson.Position, Time.deltaTime);
					}

					else if(position.syncMode == SocketSync.LerpValues)
					{
						transform.position = Vector3.Lerp (transform.position, transJson.Position, Time.deltaTime);
					}
				}

				if(rotation.synchronize && !transJson.Rotation.Equals(Vector3.zero))
				{
					if(rotation.syncMode == SocketSync.Realtime)
					{
						transform.rotation = Quaternion.Euler(transJson.Rotation);
					}

					else if(rotation.syncMode == SocketSync.AdjustToward)
					{
						transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(transJson.Rotation), Time.deltaTime);
					}

					else if(rotation.syncMode == SocketSync.LerpValues)
					{
						transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler(transJson.Rotation), Time.deltaTime);
					}
				}

				if(scale.synchronize && !transJson.Scale.Equals(Vector3.zero))
				{
					if(scale.syncMode == SocketSync.Realtime)
					{
						transform.localScale = transJson.Scale;
					}

					else if(scale.syncMode == SocketSync.AdjustToward)
					{
						transform.localScale = Vector3.MoveTowards (transform.localScale, transJson.Scale, Time.deltaTime);
					}

					else if(position.syncMode == SocketSync.LerpValues)
					{
						transform.localScale = Vector3.Lerp (transform.localScale, transJson.Scale, Time.deltaTime);
					}
				}
			}

			else
			{
				StateJson stateJson = JsonUtility.FromJson<StateJson>(jsonData);

				if(states.synchronize)
				{
					states.syncValue = stateJson.states;
				}
			}
		}
	}
}

public enum SocketSync
{
	Realtime, AdjustToward, LerpValues
}

[System.Serializable]
public class TransAxis
{
	public bool synchronize = true;
	public SocketSync syncMode = SocketSync.Realtime;
	public string syncStatus = "Nothing";
}

[System.Serializable]
public class CustomState
{
	public bool synchronize = true;
	public List<string> syncValue = new List<string>();
	public string syncStatus = "Nothing";
}

[System.Serializable]
public class StateJson
{
	public string identity = string.Empty;
	public List<string> states = new List<string>();
}

[System.Serializable]
public class TransJson
{
	public string identity = string.Empty;
	public string stat = string.Empty;
	public TVector pos = new TVector ();
	public TVector rot = new TVector ();
	public TVector sca = new TVector ();

	public void AddPosition(Vector3 vector3)
	{
		pos.FromVector (vector3);
	}

	public void AddRotation(Vector3 vector3)
	{
		rot.FromVector (vector3);
	}

	public void AddScale(Vector3 vector3)
	{
		sca.FromVector (vector3);
	}

	public void AddCustom(string state)
	{
		stat = state;
	}

	public Vector3 Position
	{
		get
		{
			return pos.ToVector;
		}
	}

	public Vector3 Rotation
	{
		get
		{
			return rot.ToVector;
		}
	}

	public Vector3 Scale
	{
		get
		{
			return sca.ToVector;
		}
	}

	public string State
	{
		get
		{
			return stat;
		}
	}
}

[System.Serializable]
public class TVector
{
	public float x = 0f;
	public float y = 0f;
	public float z = 0f;

	public void FromVector(Vector3 vector3)
	{
		x = vector3.x; y = vector3.y; z = vector3.z;
	}

	public Vector3 ToVector
	{
		get
		{
			return new Vector3 (x, y, z);
		}
	}

	public Quaternion ToQuaternion
	{
		get
		{
			return Quaternion.Euler ( new Vector3 (x, y, z) );
		}
	}
}

[System.Serializable]
public class Instances
{
	public string identity = string.Empty;
	public int prefabIndex = 0;
	public TVector pos = new TVector();
	public TVector rot = new TVector();

	public Instances(int index, Vector3 position, Vector3 rotation)
	{
		prefabIndex = index;
		pos.FromVector (position);
		rot.FromVector (rotation);
	}
}