
using UnityEngine;

namespace BytesCrafter.USocketNet.Networks
{
	[System.Serializable]
	public class BC_USN_Option
	{
		[Header("HOST SETTINGS")]
		public string restapiUrl = "localhost";
		public string serverUrl = "localhost";
		public string serverPort = "19090";

		[Header("CLIENT SETTINGS")]
		public bool debugOnLog = true;
		public bool dontDestroyOnLoad = true;
		public bool runOnBackground = true;
	}
}