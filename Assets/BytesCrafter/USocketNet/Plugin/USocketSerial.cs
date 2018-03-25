
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

	#endregion

	#region REQUIRED CLASSES

	[System.Serializable]
	public class Bindings
	{
		[Header("AUTHENTICATION")]
		public string authenKey = "SeCuReHaSkEy123";
		public string serverUrl = "localhost";
		public string serverPort = "3000";

		[Header("SETTINGS")]
		[Range(0f, 1f)] public float connectDelay = 0.01f;
		[Range(1f, 10f)] public float pingFrequency = 1f;
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

	[System.Serializable] public class MsgJson
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

	#endregion

	#region CHANNEL JSON FORMAT

	[System.Serializable]
	public class ChannelJson
	{
		public string identity = string.Empty;
		public string cname = string.Empty;
		public string variant = "Default";
		public int maxconnect = 0;
		public string created = string.Empty;
		public List<ChannelUser> users;

		public ChannelJson(string _identity)
		{
			identity = _identity;
		}

		public ChannelJson(string _variant, int _maxconnect)
		{
			variant = _variant; maxconnect = _maxconnect;
		}

		public ChannelJson(string _channelName, string _variant, int _maxconnect)
		{
			cname = _channelName; variant = _variant; maxconnect = _maxconnect;
		}
	}

	[System.Serializable]
	public class ChannelUser
	{
		public string identity = string.Empty;
		public List<string> states = new List<string>();
	}

	#endregion

	#region TRANSFORM & STATES JSON

	public enum SocketSync
	{
		Realtime, AdjustToward, LerpValues
	}

	[System.Serializable]
	public class TransAxis
	{
		public bool synchronize = true;
		[Range(1f, 30f)] public float sendRate = 15f;
		[HideInInspector] public float sendTimer = 0f;
		public SocketSync syncMode = SocketSync.Realtime;
	}

	[System.Serializable]
	public class StateList
	{
		public bool synchronize = true;
		[Range(1f, 30f)] public float sendRate = 1f;
		[HideInInspector] public float sendTimer = 0f;
		public List<string> syncValue = new List<string>();
	}

	[System.Serializable]
	public class StateJson
	{
		public string identity = string.Empty;
		public List<string> states = new List<string>();
	}

	[System.Serializable]
	public class TVector
	{
		public float x = 0f;
		public float y = 0f;
		public float z = 0f;

		public static string ToVectorStr(Vector3 vector3)
		{
			return vector3.x + "~" + vector3.y + "~" + vector3.z;
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

		public string ToVectorStr()
		{
			return x + "," + y + "," + z;
		}

		public void FromVectorStr(string vectorStr)
		{
			string[] vectorValues = vectorStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

			if (vectorValues.Length == 3)
			{
				x = Convert.ToSingle(vectorValues[0]);
				y = Convert.ToSingle(vectorValues[1]);
				z = Convert.ToSingle(vectorValues[2]);
			}
		}

		public void FromVector3(Vector3 vector3)
		{
			x = vector3.x;
			y = vector3.y;
			z = vector3.z;
		}

		public void FromQuaternion(Quaternion quaternion)
		{
			x = quaternion.eulerAngles.x;
			y = quaternion.eulerAngles.y;
			z = quaternion.eulerAngles.z;
		}

		public Vector3 ToVector
		{
			get
			{
				return new Vector3(x, y, z);
			}
		}

		public Quaternion ToQuaternion
		{
			get
			{
				return Quaternion.Euler(new Vector3(x, y, z));
			}
		}
	}

	#endregion


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
			pos.FromVector3(position);
			rot.FromVector3(rotation);
		}
	}

}