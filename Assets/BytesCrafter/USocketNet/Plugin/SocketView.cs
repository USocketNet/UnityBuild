
using UnityEngine;
using System.Collections.Generic;

namespace BytesCrafter.USocketNet
{
	[RequireComponent(typeof(Rigidbody))]
	public class SocketView : MonoBehaviour
	{
		#region Parameters

		private USocketNet socketNet = null;
		public USocketNet uSocketNet
		{
			set
			{
				if(socketNet == null)
				{
					socketNet = value;
				}
			}
		}

		[Range(1f, 30f)] public float sendRate = 15f;

		[HideInInspector] public string socketId = string.Empty;
		[HideInInspector] public bool IsLocalUser = false;
		[HideInInspector] public Rigidbody rigidbody = null;

		[Header("MAIN TRANSFORM")]
		public TransAxis position = new TransAxis();
		public TransAxis rotation = new TransAxis();
		public TransAxis scale = new TransAxis();
		public StateList states = new StateList();

		//[Header("CHILDS TRANSFORM")]
		private float mainTimer = 0f;
		//public float posTimer = 0f;
		//public float rotTimer = 0f;
		//public float scaTimer = 0f;
		//public float staTimer = 0f;

		[Header("POSITION STATUS")]
		public Vector3 targetPos = Vector3.zero;
		public float posInterpolation = 3f;
		public float posSpeed = 3f;

		[Header("ROTATION STATUS")]
		public Vector3 targetRot = Vector3.zero;
		public float rotInterpolation = 3f;
		public float rotSpeed = 3f;

		[Header("SCALE STATUS")]
		public Vector3 targetSize = Vector3.zero;
		public float sizeInterpolation = 3f;
		public float sizeSpeed = 3f;

		#endregion

		void Awake()
		{
			targetPos = transform.position;
			targetRot = transform.rotation.eulerAngles;
			targetSize = transform.localScale;

			rigidbody = GetComponent<Rigidbody>();
			if (IsLocalUser)
			{
				rigidbody.useGravity = true;
				rigidbody.isKinematic = false;

			}
			else
			{
				rigidbody.isKinematic = true;
				rigidbody.useGravity = false;
			}
		}

		void Update()
		{
			if (socketId == string.Empty)
				return;

			mainTimer += Time.deltaTime;

			if (mainTimer < (1f / sendRate))
				return;

			if (IsLocalUser)
			{
				//Send data as transform.
				StateJson stateJson = new StateJson();
				stateJson.states.AddRange( new string[4] { "f", "f", "f", "f" } ); //pos, rot, sca, sta

				if (position.synchronize)
				{
					//posTimer += Time.deltaTime;

					//if (posTimer >= (1f / position.sendRate))
					//{
					stateJson.states[0] = "t";
					stateJson.states.Add(TVector.ToVectorStr(transform.position));
					//posTimer = 0f; //Debug.Log("Position");
					//}
				}

				if (rotation.synchronize)
				{
					//rotTimer += Time.deltaTime;

					//if (rotTimer >= (1f / rotation.sendRate))
					//{
					stateJson.states[1] = "t";
					stateJson.states.Add(TVector.ToVectorStr(transform.rotation.eulerAngles));
					//rotTimer = 0f; //Debug.LogWarning("Rotation");
					//}
				}

				if (scale.synchronize)
				{
					//scaTimer += Time.deltaTime;

					//if (scaTimer >= (1f / scale.sendRate))
					//{
					stateJson.states[2] = "t";
					stateJson.states.Add(TVector.ToVectorStr(transform.lossyScale));
					//scaTimer = 0f; //Debug.LogError("Scale");
					//}
				}

				if (states.synchronize)
				{
					//staTimer += Time.deltaTime;

					//if (staTimer >= (1f / states.sendRate))
					//{
					stateJson.states[3] = "t";
					stateJson.states.AddRange(states.syncValue.ToArray());
					//staTimer = 0f; //Debug.LogError("States");
					//}
				}

				//Note: First check if values is same then dont send.

				//if (socketNet != null)
				//{
				socketNet.StateSync(stateJson);
				//}

				mainTimer = 0f;
			}

			else
			{
				if (position.synchronize)
				{
					if (position.syncMode == SocketSync.Realtime)
					{
						transform.position = targetPos;
					}

					else if (position.syncMode == SocketSync.AdjustToward)
					{
						float pDamp = Vector3.Distance(transform.position, targetPos);
						transform.position = Vector3.MoveTowards(transform.position, targetPos, (pDamp + posInterpolation) * posSpeed * Time.deltaTime);
					}

					else if (position.syncMode == SocketSync.LerpValues)
					{
						float pDamp = Vector3.Distance(transform.position, targetPos);
						transform.position = Vector3.Lerp(transform.position, targetPos, (pDamp + posInterpolation) * posSpeed * Time.deltaTime);
					}
				}

				if (rotation.synchronize)
				{
					if (rotation.syncMode == SocketSync.Realtime)
					{
						transform.rotation = Quaternion.Euler(targetRot);
					}

					else if (rotation.syncMode == SocketSync.AdjustToward)
					{
						float rDamp = Quaternion.Angle(transform.rotation, Quaternion.Euler(targetRot));
						transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRot), (rDamp + rotInterpolation) * rotSpeed * Time.deltaTime);
					}

					else if (rotation.syncMode == SocketSync.LerpValues)
					{
						float rDamp = Quaternion.Angle(transform.rotation, Quaternion.Euler(targetRot));
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRot), (rDamp + rotInterpolation) * rotSpeed * Time.deltaTime);
					}
				}

				if (scale.synchronize)
				{
					if (scale.syncMode == SocketSync.Realtime)
					{
						transform.localScale = targetSize;
					}

					else if (scale.syncMode == SocketSync.AdjustToward)
					{
						float sDamp = Vector3.Distance(transform.localScale, targetSize);
						transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, (sDamp + sizeInterpolation) * sizeSpeed * Time.deltaTime);
					}

					else if (position.syncMode == SocketSync.LerpValues)
					{
						float sDamp = Vector3.Distance(transform.localScale, targetSize);
						transform.localScale = Vector3.Lerp(transform.localScale, targetSize, (sDamp + sizeInterpolation) * sizeSpeed * Time.deltaTime);
					}
				}
			}
		}

		public void Downdate(string jsonData)
		{
			if (!IsLocalUser)
			{
				StateJson stateJson = JsonUtility.FromJson<StateJson>(jsonData);
				int currentIndex = 4;

				if (position.synchronize && stateJson.states[0].Equals("t"))
				{
					targetPos = TVector.ToVector3(stateJson.states[currentIndex]);
					currentIndex += 1;
				}

				if (rotation.synchronize && stateJson.states[1].Equals("t"))
				{
					targetRot = TVector.ToVector3(stateJson.states[currentIndex]);
					currentIndex += 1;
				}

				if (scale.synchronize && stateJson.states[2].Equals("t"))
				{
					targetSize = TVector.ToVector3(stateJson.states[currentIndex]);
					currentIndex += 1;
				}

				if (states.synchronize && stateJson.states[3].Equals("t"))
				{
					states.syncValue = new List<string>();

					for (int i = currentIndex; i < stateJson.states.Count; i++)
					{
						states.syncValue.Add(stateJson.states[i]);
					}
				}
			}
		}
	}
}
