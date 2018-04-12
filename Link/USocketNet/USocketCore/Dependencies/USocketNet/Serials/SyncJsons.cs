using System;
using System.Collections.Generic;
using UnityEngine;

using BytesCrafter.USocketNet.Serializables;

namespace BytesCrafter.USocketNet
{
	[System.Serializable]
	public class PeerJson
	{
		public string id = string.Empty;
		public List<ObjJson> obj = new List<ObjJson>();
	}
}

namespace BytesCrafter.USocketNet.Serializables
{
	[System.Serializable]
	public class SyncJsons
	{
		public List<SyncJson> obj = new List<SyncJson>();
	}

	[System.Serializable]
	public class SyncJson
	{
		public List<string> states = new List<string>();
	}

	[System.Serializable]
	public class ChanUsers
	{
		public List<PeerJson> us = new List<PeerJson>();
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
}