using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet.Networks
{
	[System.Serializable]
	public class Bindings
	{
		[Header("SERVER SETTINGS")]
		public string authenKey = "SeCuReHaSkEy123";
		public string serverUrl = "localhost";
		public string serverPort = "3000";

		[Header("CLIENT SETTINGS")]
		[Range(1f, 10f)] public float connectDelay = 0.49f;
		[Range(1f, 60f)] public float pingFrequency = 1f;

		//Rate of sync per seconds timespan.
		[Range(1f, 30f)] public int mainSendRate = 30;
		[HideInInspector] public float sendTimer = 0f;

		public bool debugOnLog = true;
		public bool runOnBackground = true;
	}
}