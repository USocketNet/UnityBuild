
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BytesCrafter.USocketNet
{
	public class USocketView : MonoBehaviour
	{
		[HideInInspector] public string Identity = string.Empty;
		[HideInInspector] public string Instance = string.Empty;
		[HideInInspector] public bool IsLocalUser = false;

		#region Public Variables

		[Header("SYNCHRONIZATION")]
		public VectorOption position = new VectorOption();
		public VectorOption rotation = new VectorOption();
		public VectorOption scale = new VectorOption();
		public AnimatorOption animator = new AnimatorOption();
		public StateOption states = new StateOption();
		public SocketChild childs = new SocketChild();

		#endregion

		#region Private Variables

		private Vector3 targetPos = Vector3.zero;
		private Vector3 targetRot = Vector3.zero;
		private Vector3 targetSize = Vector3.zero;
		private List<string> targetAni = new List<string> ();
		private List<string> targetState = new List<string> ();
		private ChildTrans targetChilds = new ChildTrans();

		#endregion

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

		void Awake()
		{
			//Parent position set default!
			targetPos = transform.position;

			//Parent rotation set default!
			targetRot = transform.rotation.eulerAngles;

			//Parent scale set default!
			targetSize = transform.localScale;

			//Parent animator set default!
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

			//Parent states set default!
			states.syncValue = targetState;

			//Parent childs set default!
			targetChilds.lists = new List<ChildTran> ();
			for(int i = 0; i < childs.childList.Count; i++)
			{
				if(childs.childList[i].reference != null)
				{
					targetChilds.lists.Add (new ChildTran());
					targetChilds.lists[i].position = childs.childList[i].reference.position;
					targetChilds.lists[i].rotation = childs.childList[i].reference.rotation;
					targetChilds.lists[i].scale = childs.childList[i].reference.localScale;
				}
			}
		}

		void Update()
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

			if(childs.synchronize)
			{
				for(int i = 0; i < childs.childList.Count; i++)
				{
					if(childs.childList[i].reference != null)
					{
						if (childs.childList[i].position.synchronize)
						{
							if (childs.childList[i].position.syncMode == SocketSync.Realtime)
							{
								childs.childList[i].reference.position = targetPos;
							}

							else if (childs.childList[i].position.syncMode == SocketSync.AdjustToward)
							{
								float pDamp = Vector3.Distance(childs.childList[i].reference.position, targetChilds.lists[i].position);
								transform.position = Vector3.MoveTowards(childs.childList[i].reference.position, targetChilds.lists[i].position, 
									(pDamp + childs.childList[i].position.interpolation) * childs.childList[i].position.speed * Time.deltaTime);
							}

							else if (childs.childList[i].position.syncMode == SocketSync.LerpValues)
							{
								float pDamp = Vector3.Distance(childs.childList[i].reference.position, targetChilds.lists[i].position);
								childs.childList[i].reference.position = Vector3.Lerp(childs.childList[i].reference.position, targetChilds.lists[i].position, 
									(pDamp + childs.childList[i].position.interpolation) * childs.childList[i].position.speed * Time.deltaTime);
							}
						}

						if (childs.childList[i].rotation.synchronize)
						{
							if (childs.childList[i].rotation.syncMode == SocketSync.Realtime)
							{
								childs.childList[i].reference.rotation = targetChilds.lists[i].rotation;
							}

							else if (childs.childList[i].rotation.syncMode == SocketSync.AdjustToward)
							{
								float rDamp = Quaternion.Angle(childs.childList[i].reference.rotation, targetChilds.lists[i].rotation);
								childs.childList[i].reference.rotation = Quaternion.RotateTowards(childs.childList[i].reference.rotation, targetChilds.lists[i].rotation, 
									(rDamp + childs.childList[i].rotation.interpolation) * childs.childList[i].rotation.speed * Time.deltaTime);
							}

							else if (childs.childList[i].rotation.syncMode == SocketSync.LerpValues)
							{
								float rDamp = Quaternion.Angle(childs.childList[i].reference.rotation, targetChilds.lists[i].rotation);
								childs.childList[i].reference.rotation = Quaternion.Lerp(childs.childList[i].reference.rotation, targetChilds.lists[i].rotation, 
									(rDamp + childs.childList[i].rotation.interpolation) * childs.childList[i].rotation.speed * Time.deltaTime);
							}
						}

						if (childs.childList[i].scale.synchronize)
						{
							if (childs.childList[i].scale.syncMode == SocketSync.Realtime)
							{
								childs.childList[i].reference.localScale = targetChilds.lists[i].scale;
							}

							else if (childs.childList[i].scale.syncMode == SocketSync.AdjustToward)
							{
								float sDamp = Vector3.Distance(childs.childList[i].reference.localScale, targetChilds.lists[i].scale);
								childs.childList[i].reference.localScale = Vector3.MoveTowards(childs.childList[i].reference.localScale, targetChilds.lists[i].scale, 
									(sDamp + childs.childList[i].scale.interpolation) * childs.childList[i].scale.speed * Time.deltaTime);
							}

							else if (childs.childList[i].position.syncMode == SocketSync.LerpValues)
							{
								float sDamp = Vector3.Distance(childs.childList[i].reference.localScale, targetChilds.lists[i].scale);
								childs.childList[i].reference.localScale = Vector3.Lerp(childs.childList[i].reference.localScale, targetChilds.lists[i].scale, 
									(sDamp + childs.childList[i].scale.interpolation) * childs.childList[i].scale.speed * Time.deltaTime);
							}
						}
					}
				}
			}
		}

		public SyncJson GetViewData()
		{
			//Create a minified string version of syncjson.
			SyncJson syncJson = new SyncJson ();

			//On downlink, if string state is equal to f, dont do anything.
			syncJson.states.AddRange (new string[6] { "f", "f", "f", "f", "f", "f" }); //pos, rot, sca, ani, sta, chi

			//Check positions values if theres an update.
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

			//Check rotation values if theres an update.
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

			//Check scale values if theres an update.
			if (scale.synchronize)
			{
				//usocket.scale.sendTimer += Time.deltaTime;

				//if (usocket.scale.sendTimer >= (1f / usocket.scale.sendRate))
				//{
				string newVstate = VectorJson.ToVectorStr (transform.localScale);

				if(!newVstate.Equals(scale.prevVstring))
				{
					syncJson.states [2] = "t";
					syncJson.states.Add (newVstate);

					scale.prevVstring = newVstate;
				}

				//	usocket.scale.sendTimer = 0f;
				//}
			}

			//Check animator values if theres an update.
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

			//Check states values if theres an update.
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

			//Check check childs values if theres an update.
			if (childs.synchronize)
			{
				//usocket.childs.sendTimer += Time.deltaTime;

				//if (usocket.childs.sendTimer >= (1f / usocket.childs.sendRate))
				//{

				//if(GetViewChildData() is Touch Behaviour synchronizable)
				//{
					if(childs.childList.Count > 0)
					{
						string newCstate = string.Join("`", GetViewChildData());

						if(!newCstate.Equals(string.Empty))
						{
							if(!newCstate.Equals(childs.prevVstring))
							{
								syncJson.states [5] = "t";
								syncJson.states.Add (newCstate);

								childs.prevVstring = newCstate;
							}
						}
					}
				//}

				//	usocket.childs.sendTimer = 0f;
				//}
			}

			//Check if sync json are all false send empty array.
			if(syncJson.states.Count > 6)
			{
				syncJson.states.Add (Instance);
			}

			else
			{
				syncJson = new SyncJson ();
			}
			return syncJson;
		}

		private string[] GetViewChildData()
		{
			List<string> childDatas = new List<string> ();

			for(int i = 0; i < childs.childList.Count; i++)
			{
				List<string> childOne = new List<string> ();
				childOne.AddRange (new string[3]{"f", "f", "f"});

				if(childs.childList[i].reference != null)
				{
					//Check child positions.
					if (childs.childList[i].position.synchronize)
					{
						string newVstate = VectorJson.ToVectorStr (childs.childList[i].reference.position);

						if(!newVstate.Equals(childs.childList[i].position.prevVstring))
						{
							childOne[0] = "t";
							childOne.Add (newVstate);
							childs.childList[i].position.prevVstring = newVstate;
						}
					}			

					//Check child rotation.
					if (childs.childList[i].rotation.synchronize)
					{
						string newVstate = VectorJson.ToQuaternionStr (childs.childList[i].reference.rotation);

						if(!newVstate.Equals(childs.childList[i].rotation.prevVstring))
						{
							childOne[1] = "t";
							childOne.Add (newVstate);
							childs.childList[i].rotation.prevVstring = newVstate;
						}
					}	

					//Check child scale.
					if (childs.childList[i].scale.synchronize)
					{
						string newVstate = VectorJson.ToVectorStr (childs.childList[i].reference.localScale);

						if(!newVstate.Equals(childs.childList[i].scale.prevVstring))
						{
							childOne[2] = "t";
							childOne.Add (newVstate);
							childs.childList[i].scale.prevVstring = newVstate;
						}
					}
				}

				//Submit id so that some child option to not update. or the index. yet if deleted problem.
				//if(childOne.Count > 3)
				//{
					childDatas.Add (string.Join("^", childOne.ToArray()));
				//}
			}

			return childDatas.ToArray ();
		}

		public void SetViewTarget(ObjJson objJson)
		{
			if (!objJson.pos.Equals ("f"))
			{
				targetPos = VectorJson.ToVector3(objJson.pos);
			}
			if (!objJson.rot.Equals ("f"))
			{
				targetRot = VectorJson.ToVector3(objJson.rot);
			}
			if (!objJson.sca.Equals ("f"))
			{
				targetSize = VectorJson.ToVector3(objJson.sca);
			}
			if(!objJson.ani.Equals("f"))
			{
				targetAni = new List<string>();
				targetAni.AddRange(objJson.ani.Split('~'));
			}
			if(!objJson.sta.Equals("f"))
			{
				targetState = new List<string>();
				targetState.AddRange(objJson.sta.Split('~'));
			}
			if(!objJson.chi.Equals("f"))
			{
				string[] childish = objJson.chi.Split('`');
				for(int i = 0; i < childish.Length; i++)
				{
					string[] childit = childish [i].Split('^');

					if(childit.Length > 3)
					{
						int childindex = 3;

						if(childit[0].Equals("t"))
						{
							targetChilds.lists[i].position = VectorJson.ToVector3 (childit[childindex]);
							childindex += 1;
						}

						if(childit[1].Equals("t"))
						{
							targetChilds.lists[i].rotation = VectorJson.ToQuaternion (childit[childindex]);
							childindex += 1;
						}

						if(childit[2].Equals("t"))
						{
							targetChilds.lists[i].scale = VectorJson.ToVector3 (childit[childindex]);
						}
					}
				}
			}
		}

		private Triggered triggerSubscribers = null;
		public void ListenTriggers(Triggered triggersListener)
		{
			//if (IsLocalUser)
			//{
				StartCoroutine (SettingTriggers(triggersListener));
			//}
		}

		private IEnumerator SettingTriggers(Triggered triggersListener)
		{
			yield return new WaitForSeconds (1f);
			triggerSubscribers = triggersListener;
			socketNet.ListenTriggersEvent(ReceivedTriggers);
		}

		private void ReceivedTriggers(TriggerJson tJson)
		{
			Debug.LogError(JsonUtility.ToJson(tJson));
			//tJson.id is id of the cur user other action.
			if(Instance == tJson.itc)
			{
				triggerSubscribers (tJson);
			}
		}

		private int maxTriggers = 3; //Only on failed
		private int curTriggers = 0;
		public void TriggerEvents(string key, string content, Action<Returned> callback)
		{
			TriggerJson trigger = new TriggerJson ();
			trigger.itc = Instance;
			trigger.tKy = key;
			trigger.tVl = content;

			socketNet.SendTriggerEvent (trigger, (Returned returned) => {

				Debug.LogWarning(returned.ToString());

				if(returned == Returned.Failed)
				{
					if(curTriggers == maxTriggers)
					{
						if(callback != null)
						{
							curTriggers = 0;
							callback(returned);
						}
					}

					else
					{
						curTriggers += 1;
						TriggerEvents(key, content, callback);
					}
				}

				else
				{
					if(callback != null)
					{
						callback(returned);
					}
				}
			});
		}
	}
}
