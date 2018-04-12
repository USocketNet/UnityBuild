using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet
{
	[System.Serializable]
	public class MsgCallback
	{
		public Returned msgStat = Returned.Success;
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
}