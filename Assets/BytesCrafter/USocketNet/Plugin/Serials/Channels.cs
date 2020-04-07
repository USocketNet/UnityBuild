using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet
{
	[System.Serializable]
	public class ChannelJson
	{
		public string id = string.Empty;
		public string cn = string.Empty;
		public string vt = string.Empty;
		public int mc = 0;
		public string ct = string.Empty;
		public List<PeerJson> us;

		public ChannelJson(string _channelName)
		{
			cn = _channelName;
		}

		public ChannelJson(string _channelName, string _variant)
		{
			cn = _channelName;
			vt = _variant;
		}

		public ChannelJson(string _variant, int _maxconnect)
		{
			vt = _variant;
			mc = _maxconnect;
		}

		public ChannelJson(string _channelName, string _variant, int _maxconnect)
		{
			cn = _channelName;
			vt = _variant;
			mc = _maxconnect;
		}
	}
}

namespace BytesCrafter.USocketNet.Serializables
{
	[System.Serializable]
	public class ChanReturn
	{
		public bool rt = false;
		public string ch = string.Empty;
	}
}