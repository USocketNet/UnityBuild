
using System;
using System.Collections.Generic;

using SocketIO;
using UnityEngine;
using WebSocketSharp;

namespace BytesCrafter.USocketNet
{
	#region EASY SERIALIZER

	public class JsonSerializer
	{
		public static T ToObject<T>(string jsonString)
		{
			string trimmed = jsonString.TrimStart ('[');
			trimmed = trimmed.TrimEnd (']');
			return JsonUtility.FromJson<T> (trimmed);
		}

		public static string FromObject(object jsonObject)
		{
			return JsonUtility.ToJson(jsonObject);
		}
	}

	public enum Returned
	{
		Success, Failed, Error
	}

	[System.Serializable]
	public class Returning
	{
		public Returned returned = Returned.Failed;
	}

	#endregion

	#region REQUIRED CLASSES

	[System.Serializable]
	public class Bindings
	{
		[Header("SERVER SETTINGS")]
		public string authenKey = "SeCuReHaSkEy123";
		public string serverUrl = "localhost";
		public string serverPort = "3000";

		[Header("CLIENT SETTINGS")]
		[Range(0f, 1f)] public float connectDelay = 0.49f;
		[Range(1f, 10f)] public float pingFrequency = 1f;

		//Rate of sync per seconds timespan.
		[Range(1f, 30f)] public int mainSendRate = 30;
		[HideInInspector] public float sendTimer = 0f;

		public bool debugOnLog = true;
		public bool runOnBackground = true;
	}

	[System.Serializable]
	public class Threadset
	{
		public WebSocket websocket = null; //An instance of Web Socket for connection.

		public bool wsCheck { get { return wscheck; } set { wscheck = value; } }
		private volatile bool wscheck;

		public bool IsConnected { get { return connected; } set { connected = value; } }
		private volatile bool connected;

		public string socketId { get; set; }
		public int packetId; 

		public bool IsInitialized { get { return wsinit; } set { wsinit = value; } }
		private volatile bool wsinit;

		public bool autoConnect { get { return autoConn; } set { autoConn = value; } }
		private volatile bool autoConn;

		public bool wsPinging { get { return wspinging; } set { wspinging = value; } }
		private volatile bool wspinging; 

		public bool wsPonging { get { return wsponging; } set { wsponging = value; } }
		private volatile bool wsponging;

		public int reconDelay = 5;
		public float ackExpireTime = 1800f;
		public float pingInterval = 25f;
		public float pingTimeout = 60f;
	}

	[System.Serializable]
	public class QueueCoder
	{
		public Dictionary<string, List<Action<SocketIOEvent>>> handlers;
		public Queue<SocketIOEvent> eventQueue; 
		public object eventQueueLock;
		public Queue<Packet> ackQueue;
		public object ackQueueLock;
		public List<Ack> ackList; 

		public Encoder encoder;
		public Decoder decoder;
		public Parser parser;
	}

	#endregion

	#region CONNECTIONS

	public enum ConnStat
	{ 
		Connected, Disconnected, Reconnected, Maintainance, InternetAccess
	}

	[System.Serializable]
	public class UserJson
	{
		public string identity = string.Empty;
		public string username = string.Empty;

		public UserJson(string _identity, string _username)
		{
			identity = _identity; username = _username;
		}
	}

	[System.Serializable]
	public class ConnJson
	{
		public string identity = string.Empty; //Current connection id!
		public string username = string.Empty; //user name!
		public string curcid = string.Empty; //channel id!
		public string logdate = string.Empty; //datetime logged!
	}

	#endregion

	#region SEND MESSAGE

	public enum MsgStat
	{ 
		Success, Failed, Error
	}

	[System.Serializable]
	public class MsgCallback
	{
		public MsgStat msgStat = MsgStat.Success;
	}

	public delegate void MsgListener(MsgJson msgJson);

	public enum MsgType
	{ 
		Public, Private, Channel
	}

	[System.Serializable]
	public class MsgJson
	{
		public string sender = string.Empty;
		public string content = string.Empty;
		public MsgType msgtype = MsgType.Public;
		public string receiver = string.Empty; 

		public MsgJson() { }

		public MsgJson(string _content, MsgType _msgtype)
		{
			content = _content; msgtype = _msgtype; 
		}

		public MsgJson(string _content, MsgType _msgtype, string _receiver)
		{
			content = _content; msgtype = _msgtype; receiver = _receiver; 
		}
	}

	public delegate void Triggered(TriggerJson tJson);

	[System.Serializable]
	public class TriggerJson
	{
		public string id = string.Empty; //SocketNet from server!
		public string itc = string.Empty; //SocketView from local.
		public string tKy = string.Empty; //SocketView from local.
		public string tVl = string.Empty; //SocketView from local.

		public TriggerJson() {}

		public TriggerJson(string _instance, string _triggerKey, string _triggerValue)
		{
			itc = _instance;
			tKy = _triggerKey; 
			tVl = _triggerValue; 
		}
	}

	#endregion

	#region CHANNEL JSON FORMAT

	[System.Serializable]
	public class ChannelJson
	{
		public string id = string.Empty;
		public string cn = string.Empty;
		public string vt = string.Empty;
		public int mc = 0;
		public string ct = string.Empty;
		public List<PeerJson> us;

		public ChannelJson(string _identity)
		{
			id = _identity;
		}

		public ChannelJson(string _variant, int _maxconnect)
		{
			vt = _variant; mc = _maxconnect;
		}

		public ChannelJson(string _channelName, string _variant, int _maxconnect)
		{
			cn = _channelName; vt = _variant; mc = _maxconnect;
		}
	}

	[System.Serializable]
	public class ChanUsers
	{
		public List<PeerJson> users = new List<PeerJson>();
	}

	[System.Serializable]
	public class PeerJson
	{
		public string id = string.Empty;
		public List<ObjJson> obj = new List<ObjJson>();
	}

	[System.Serializable]
	public class ObjJson
	{
		public string id = string.Empty;
		public int pfb = 0;
		public string pos = string.Empty;
		public string rot = string.Empty;
		public string sca = string.Empty;
		public string ani = string.Empty;
		public string sta = string.Empty;
		public string chi = string.Empty;
	}



	#endregion

	#region TRANSFORM & STATES JSON

	public enum SocketSync
	{
		Realtime, AdjustToward, LerpValues
	}

	[System.Serializable]
	public class VectorOption
	{
		//Synchronized or bypass this data!
		public bool synchronize = true;

		//Rate of sync per seconds timespan.
		[Range(1f, 30f)] public float sendRate = 15f;

		public float interpolation = 3f;
		public float speed = 3f;

		//sendTimer for synching looper.
		[HideInInspector] public float sendTimer = 0f;

		[HideInInspector] public string prevVstring = string.Empty;

		//Current sync mode to follow.
		public SocketSync syncMode = SocketSync.Realtime;
	}

	[System.Serializable]
	public class VectorJson
	{
		public float x = 0f;
		public float y = 0f;
		public float z = 0f;

		private static string Minified(float floatings, int significant)
		{
			string final = "";
			bool foundDot = false;
			int countingSigna = 0;
			string axis = floatings.ToString ();
			for (int i = 0; i < axis.Length; i++)
			{
				if(foundDot)
				{
					if(countingSigna < significant) //significant
					{
						final = final + axis [i];
						countingSigna += 1;
					}

					else
					{
						break;
					}
				}

				else
				{
					if(significant == 0)
					{
						if(axis[i] == '.')
						{
							if(axis.ToLower().IndexOf('e') != -1)
							{
								bool foundEs = false;
								for (int it = 0; it < axis.Length; it++)
								{
									if(axis[it] == 'E')
									{
										foundEs = true;
									}

									if(foundEs)
									{
										final = final + axis [it];
									}
								}

								break;
							}

							else
							{
								break;
							}
						}

						else
						{
							final = final + axis [i];
						}
					}

					if(significant > 0)
					{
						if(axis.ToLower().IndexOf('e') != -1)
						{
							if(axis[i] == '.')
							{
								bool foundEs = false;
								for (int it = i; it < axis.Length; it++)
								{
									if(axis[it] == 'E')
									{
										foundEs = true;
									}

									if(foundEs)
									{
										final = final + axis [it];
									}
								}

								break;
							}

							else
							{
								final = final + axis [i];
							}

						}

						else
						{
							final = final + axis [i];

							if(axis[i] == '.')
							{
								foundDot = true;
							}
						}
					}
				}
			}

			return final;
		}

		public static string ToVectorStr(Vector3 vector3)
		{
			return Minified(vector3.x, 3) + "~" + Minified(vector3.y, 3) + "~" + Minified(vector3.z, 3);
		}

		public static Vector3 ToVector3(string vectorStr)
		{
			string[] vectorValues = vectorStr.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);

			if (vectorValues.Length == 3)
			{
				return new Vector3
					(
						Convert.ToSingle(vectorValues[0]),
						Convert.ToSingle(vectorValues[1]),
						Convert.ToSingle(vectorValues[2])
					);
			}

			else
			{
				return Vector3.zero;
			}
		}

		public static string ToQuaternionStr(Quaternion rotation)
		{
			return Minified(rotation.eulerAngles.x, 0) + "~" + Minified(rotation.eulerAngles.y, 0) + "~" + Minified(rotation.eulerAngles.z, 0);
		}

		public static Quaternion ToQuaternion(string vectorStr)
		{
			string[] vectorValues = vectorStr.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);

			if (vectorValues.Length == 3)
			{
				Vector3 eulerValue = new Vector3
					(
						Convert.ToSingle(vectorValues[0]),
						Convert.ToSingle(vectorValues[1]),
						Convert.ToSingle(vectorValues[2])
					);

				return Quaternion.Euler(eulerValue);
			}

			else
			{
				return Quaternion.identity;
			}
		}
	}

	[System.Serializable]
	public class StateOption
	{
		public bool synchronize = true;
		[Range(1f, 30f)] public float sendRate = 1f;
		[HideInInspector] public float sendTimer = 0f;
		[HideInInspector] public string prevVstring = string.Empty;

		public List<string> syncValue = new List<string>();
	}

	[System.Serializable]
	public class SyncJsons
	{
		public string identity = string.Empty;
		public List<SyncJson> obj = new List<SyncJson>();
	}

	[System.Serializable]
	public class SyncJson
	{
		public List<string> states = new List<string>();
	}

	[System.Serializable]
	public class AnimatorOption
	{
		public bool synchronize = true;
		[Range(1f, 30f)] public float sendRate = 1f;
		public Animator reference = null;
		public List<AnimTypes> parameters = new List<AnimTypes> ();
		[HideInInspector] public float sendTimer = 0f;
		[HideInInspector] public string prevVstring = string.Empty;
	}

	public enum APType
	{
		Float, Int, Bool
	}

	[System.Serializable]
	public class AnimTypes
	{
		public APType apType = APType.Float;
		public string apValue = string.Empty;

		public AnimTypes(APType _apType)
		{
			apType = _apType;
		}
	}

	#endregion

	[System.Serializable]
	public class SocketChild
	{
		//Synchronized or bypass this data!
		public bool synchronize = true;

		public List<ViewChilds> childList = new List<ViewChilds>();

		//Rate of sync per seconds timespan.
		[Range(1f, 30f)] public float sendRate = 15f;

		//sendTimer for synching looper.
		[HideInInspector] public float sendTimer = 0f;

		[HideInInspector] public string prevVstring = string.Empty;
	}

	[System.Serializable]
	public class ViewChilds
	{
		public Transform reference = null;
		public VectorOption position = new VectorOption();
		public VectorOption rotation = new VectorOption();
		public VectorOption scale = new VectorOption();
	}

	[System.Serializable]
	public class ChildTrans
	{
		public List<ChildTran> lists = new List<ChildTran>();
	}

	[System.Serializable]
	public class ChildTran
	{
		public Vector3 position = Vector3.zero;
		public Quaternion rotation = Quaternion.identity;
		public Vector3 scale = Vector3.zero;
	}

	[System.Serializable]
	public class Instances
	{
		//Socket Identity of this client. by Server.
		public string identity = string.Empty;

		//Object instance id of the client. by Client.
		public string itc = string.Empty;

		//What prefab to instantiate from the list. by Client.
		public int pfb = 0;

		//Initial instance information from the prefab. by Client.
		public string pos = string.Empty;
		public string rot = string.Empty;
		//public VectorJson sca = new VectorJson();
		//public VectorJson sta = new VectorJson();

		public Instances(string instance, int prefab, Vector3 position, Quaternion rotation)
		{
			itc = instance; pfb = prefab;
			pos = VectorJson.ToVectorStr(position);
			rot = VectorJson.ToQuaternionStr(rotation);
		}
	}

}