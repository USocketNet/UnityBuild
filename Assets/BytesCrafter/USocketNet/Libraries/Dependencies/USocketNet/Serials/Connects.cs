using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet
{
	/// <summary>
	/// Conn auth is an enum handler for the current client authentication.
	/// </summary>
	public enum ConnAuth
	{ 
		/// <summary>
		/// Successfully enter the server.
		/// </summary>
		Success, 
		/// <summary>
		/// Username not found on the server.
		/// </summary>
		NotFound, 
		/// <summary>
		/// Password of username is incorrect.
		/// </summary>
		Incorrect, 
		/// <summary>
		/// For some reason, server cannot process request.
		/// </summary>
		Error,
		/// <summary>
		/// USocketClient is currently performing task.
		/// </summary>
		Busy
	}

	/// <summary>
	/// Conn stat is an enum handler for the current connection init.
	/// </summary>
	public enum ConnStat
	{ 
		/// <summary>
		/// Client had been successfully connected from server.
		/// </summary>
		Connected, 
		/// <summary>
		/// Client had been disconnected from server for some reason.
		/// </summary>
		Disconnected, 
		/// <summary>
		/// Client had been reconnected to the server.
		/// </summary>
		Reconnected, 
		/// <summary>
		/// Server host/port is currently unreachable.
		/// </summary>
		Maintainance, 
		/// <summary>
		/// For some reason, internet activity not detected.
		/// </summary>
		InternetAccess, 
		/// <summary>
		/// Client had been rejected by server for some args.
		/// </summary>
		Rejected
	}
}

namespace BytesCrafter.USocketNet.Serializables
{
	/// <summary>
	/// Conn init.
	/// </summary>
	[System.Serializable]
	public class Credential
	{
		public string ak = string.Empty;
		public string un = string.Empty;
		public string pw = string.Empty;

		public Credential(string authKey, string username, string password)
		{
			ak = authKey;
			un = username;
			pw = password;
		}
	}

	/// <summary>
	/// Rejection callback handler.
	/// </summary>
	[System.Serializable]
	public class Rejection
	{
		public Reject rs = Reject.UnauthorizedDomain;
	}

	public enum Reject
	{
		/// <summary>
		/// For some reason, connection credentials is not detected during client evaluation.
		/// </summary>
		ForbiddenAuthKey,
		/// <summary>
		/// Server expects for userInfo and callback, please dont make any changes on the plugin.
		/// </summary>
		InvalidArguments,
		/// <summary>
		/// The domain or public ip refered for connection is not authorized by server.
		/// </summary>
		UnauthorizedDomain
	}

	[System.Serializable]
	public class ConnRes
	{
		public ConnAuth ca = ConnAuth.Error;
		public string id = string.Empty;
	}


}