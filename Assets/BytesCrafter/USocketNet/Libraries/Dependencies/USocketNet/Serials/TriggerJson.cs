using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet
{
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
}