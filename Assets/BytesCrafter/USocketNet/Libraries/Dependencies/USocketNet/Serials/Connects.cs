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
		/// Successfully enter the server. Code: 0
		/// </summary>
		Success, 
		/// <summary>
		/// Username not found on the server. Code: 1
		/// </summary>
		NotFound, 
		/// <summary>
		/// Password of username is incorrect. Code: 2
		/// </summary>
		Incorrect, 
		/// <summary>
		/// Application id dont exist on the database. Code: 3
		/// </summary>
		Invalid, 
		/// <summary>
		/// Server found invalid arguments thus rejecting conn. Code: 4
		/// </summary>
		Rejected,
		/// <summary>
		/// For some reason, server cannot process request. Code: 5
		/// </summary>
		Error,
		/// <summary>
		/// Users account is currently online. Code: 6
		/// </summary>
		Online,
		/// <summary>
		/// USocketClient is currently performing task. Code: 7
		/// </summary>
		Busy,
		/// <summary>
		/// Game is currenly inactive and block want new connection. Code: 8
		/// </summary>
		Inactive,
		/// <summary>
		/// Game is currenly at its limit and cant accept new connection Code: 9
		/// </summary>
		Overload
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
		public string UN = string.Empty;
		public string PW = string.Empty;

		public Credential(string username, string password)
		{
			UN = username;
			PW = password;
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