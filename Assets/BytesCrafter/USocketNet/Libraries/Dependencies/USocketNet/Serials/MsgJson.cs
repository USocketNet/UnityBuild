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
		public string sd = string.Empty;
		public string ct = string.Empty;
		public MsgType mt = MsgType.Public;
		public string rv = string.Empty; 

		public MsgJson() { }

		public MsgJson(string _content, MsgType _msgtype)
		{
			ct = _content; mt = _msgtype; 
		}

		public MsgJson(string _content, MsgType _msgtype, string _receiver)
		{
			ct = _content; mt = _msgtype; rv = _receiver; 
		}
	}
}