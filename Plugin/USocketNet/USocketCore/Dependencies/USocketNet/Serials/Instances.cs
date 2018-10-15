using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet.Serializables
{
	[System.Serializable]
	public class Instances
	{
		//Socket Identity of this client. by Server.
		public string id = string.Empty;

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
			itc = instance;
			pfb = prefab;
			pos = VectorJson.ToVectorStr(position);
			rot = VectorJson.ToQuaternionStr(rotation);
		}
	}
}