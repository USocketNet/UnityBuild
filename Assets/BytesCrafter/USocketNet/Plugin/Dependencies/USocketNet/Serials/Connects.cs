using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet
{
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
}