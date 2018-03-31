
using UnityEngine;
using System;
using System.Collections.Generic;

namespace BytesCrafter.USocketNet
{
	public class USocketView : MonoBehaviour
	{
		#region Required Reference

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

		#endregion

		#region Public Variables

		[Header("SYNCHRONIZATION")]
		public VectorOption position = new VectorOption();
		public VectorOption rotation = new VectorOption();
		public VectorOption scale = new VectorOption();
		public AnimatorOption animator = new AnimatorOption();
		public StateOption states = new StateOption();

		[HideInInspector] public string Identity = string.Empty;
		[HideInInspector] public string Instance = string.Empty;
		[HideInInspector] public bool IsLocalUser = false;

		#endregion

		#region Private Variables

		private Vector3 targetPos = Vector3.zero;
		private Vector3 targetRot = Vector3.zero;
		private Vector3 targetSize = Vector3.zero;
		private List<string> targetAni = new List<string> ();
		private List<string> targetState = new List<string> ();

		#endregion

		void Awake()
		{
			targetPos = transform.position;
			targetRot = transform.rotation.eulerAngles;
			targetSize = transform.localScale;
			if(animator.parameters.Count > 0)
			{
				if (animator != null)
				{
					if(targetAni.Count == animator.parameters.Count)
					{
						for(int i = 0; i < targetAni.Count; i++)
						{
							if(animator.parameters[i].apType == APType.Float)
							{
								float floatings = System.Convert.ToSingle(targetAni [i]);
								animator.reference.SetFloat(animator.parameters[i].apValue, floatings);
							}

							else if(animator.parameters[i].apType == APType.Int)
							{
								int integers = System.Convert.ToInt16(targetAni [i]);
								animator.reference.SetInteger(animator.parameters[i].apValue, integers);
							}

							else if(animator.parameters[i].apType == APType.Bool)
							{
								bool booleans = System.Convert.ToBoolean(targetAni [i]);
								animator.reference.SetBool(animator.parameters[i].apValue, booleans);
							}

							else
							{

							}
						}
					}
				}
			}
			states.syncValue = targetState;
		}

		public SyncJson GetViewData()
		{
			//Create a minified string version of syncjson.
			SyncJson syncJson = new SyncJson ();

			//On downlink, if string state is equal to f, dont do anything.
			syncJson.states.AddRange (new string[5] { "f", "f", "f", "f", "f" }); //pos, rot, sca, ani, sta

			syncJson.states.Add (Instance);

			//Check positions.
			if (position.synchronize)
			{
				//usocket.position.sendTimer += Time.deltaTime;

				//if (usocket.position.sendTimer >= (1f / usocket.position.sendRate))
				//{
				string newVstate = VectorJson.ToVectorStr (transform.position);

				if(!newVstate.Equals(position.prevVstring))
				{
					syncJson.states [0] = "t";
					syncJson.states.Add (newVstate);

					position.prevVstring = newVstate;
				}

				//	usocket.position.sendTimer = 0f;
				//}
			}			

			//Check rotation.
			if (rotation.synchronize)
			{
				//usocket.rotation.sendTimer += Time.deltaTime;

				//if (usocket.rotation.sendTimer >= (1f / usocket.rotation.sendRate))
				//{
				string newVstate = VectorJson.ToVectorStr (transform.rotation.eulerAngles);

				if(!newVstate.Equals(rotation.prevVstring))
				{
					syncJson.states [1] = "t";
					syncJson.states.Add (newVstate);

					rotation.prevVstring = newVstate;
				}

				//	usocket.rotation.sendTimer = 0f;
				//}
			}	

			//Check scale.
			if (scale.synchronize)
			{
				//usocket.scale.sendTimer += Time.deltaTime;

				//if (usocket.scale.sendTimer >= (1f / usocket.scale.sendRate))
				//{
				string newVstate = VectorJson.ToVectorStr (transform.lossyScale);

				if(!newVstate.Equals(scale.prevVstring))
				{
					syncJson.states [2] = "t";
					syncJson.states.Add (newVstate);

					scale.prevVstring = newVstate;
				}

				//	usocket.scale.sendTimer = 0f;
				//}
			}

			//Check animator.
			if (animator.synchronize)
			{
				if (animator.reference != null)
				{
					//usocket.animator.sendTimer += Time.deltaTime;

					//if (usocket.animator.sendTimer >= (1f / usocket.animator.sendRate))
					//{

					string[] anims = new string[animator.parameters.Count];
					for(int i = 0; i < animator.parameters.Count; i++)
					{
						if(animator.parameters[i].apType == APType.Float)
						{
							anims [i] = animator.reference.GetFloat (animator.parameters[i].apValue) + "";
						}

						else if(animator.parameters[i].apType == APType.Int)
						{
							anims [i] = animator.reference.GetInteger (animator.parameters[i].apValue) + "";
						}

						else if(animator.parameters[i].apType == APType.Bool)
						{
							anims [i] = animator.reference.GetBool (animator.parameters[i].apValue) + "";
						}

						else
						{

						}
					}

					string newCstate = string.Join("~", anims);

					if(!newCstate.Equals(animator.prevVstring))
					{
						syncJson.states [3] = "t";
						syncJson.states.Add (newCstate);

						animator.prevVstring = newCstate;
					}

					//	usocket.animator.sendTimer = 0f;
					//}
				}
			}

			//Check states.
			if (states.synchronize)
			{
				//usocket.states.sendTimer += Time.deltaTime;

				//if (usocket.states.sendTimer >= (1f / usocket.states.sendRate))
				//{
				string newCstate = string.Join("~", states.syncValue.ToArray());

				if(!newCstate.Equals(states.prevVstring))
				{
					syncJson.states [4] = "t";
					syncJson.states.Add (newCstate);

					states.prevVstring = newCstate;
				}

				//	usocket.states.sendTimer = 0f;
				//}
			}

			return syncJson;
		}

		public void SetViewTarget(ObjJson objJson)
		{
			targetPos = VectorJson.ToVector3(objJson.pos);
			targetRot = VectorJson.ToVector3(objJson.rot);
			targetSize = VectorJson.ToVector3(objJson.sca);
			targetAni = new List<string>();
			targetAni.AddRange(objJson.ani.Split('~'));
			targetState = new List<string>();
			targetState.AddRange(objJson.sta.Split('~'));
		}

		public void SyncViewData()
		{
			//Check if socket id is empty, then remove it.
			if (Identity == string.Empty)
				return;

			//Check if local player dont sync.
			if (IsLocalUser)
				return;

			//LocalSync on DownLink
			if (position.synchronize)
			{
				if (position.syncMode == SocketSync.Realtime)
				{
					transform.position = targetPos;
				}

				else if (position.syncMode == SocketSync.AdjustToward)
				{
					float pDamp = Vector3.Distance(transform.position, targetPos);
					transform.position = Vector3.MoveTowards(transform.position, targetPos, 
						(pDamp + position.interpolation) * position.speed * Time.deltaTime);
				}

				else if (position.syncMode == SocketSync.LerpValues)
				{
					float pDamp = Vector3.Distance(transform.position, targetPos);
					transform.position = Vector3.Lerp(transform.position, targetPos, 
						(pDamp + position.interpolation) * position.speed * Time.deltaTime);
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
					transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRot), 
						(rDamp + rotation.interpolation) * rotation.speed * Time.deltaTime);
				}

				else if (rotation.syncMode == SocketSync.LerpValues)
				{
					float rDamp = Quaternion.Angle(transform.rotation, Quaternion.Euler(targetRot));
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRot), 
						(rDamp + rotation.interpolation) * rotation.speed * Time.deltaTime);
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
					transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, 
						(sDamp + scale.interpolation) * scale.speed * Time.deltaTime);
				}

				else if (position.syncMode == SocketSync.LerpValues)
				{
					float sDamp = Vector3.Distance(transform.localScale, targetSize);
					transform.localScale = Vector3.Lerp(transform.localScale, targetSize, 
						(sDamp + scale.interpolation) * scale.speed * Time.deltaTime);
				}
			}

			if (animator.synchronize)
			{
				if (animator.reference != null)
				{
					if(animator.parameters.Count > 0)
					{
						if(targetAni.Count == animator.parameters.Count)
						{
							for(int i = 0; i < targetAni.Count; i++)
							{
								if(animator.parameters[i].apType == APType.Float)
								{
									float floatings = System.Convert.ToSingle(targetAni [i]);
									animator.reference.SetFloat(animator.parameters[i].apValue, floatings);
								}

								else if(animator.parameters[i].apType == APType.Int)
								{
									int integers = System.Convert.ToInt16(targetAni [i]);
									animator.reference.SetInteger(animator.parameters[i].apValue, integers);
								}

								else if(animator.parameters[i].apType == APType.Bool)
								{
									bool booleans = System.Convert.ToBoolean(targetAni [i]);
									animator.reference.SetBool(animator.parameters[i].apValue, booleans);
								}

								else
								{

								}
							}
						}
					}
				}
			}

			if (states.synchronize)
			{
				states.syncValue = targetState;
			}
		}
	}
}
