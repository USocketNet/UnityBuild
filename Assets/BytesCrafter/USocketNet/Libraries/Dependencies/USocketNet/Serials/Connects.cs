using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet
{
	/// <summary>
	/// Conn auth is an enum handler for the current client authentication.
	/// </summary>
	public enum ConStat
	{ 
		/// <summary>
		/// Successfully enter the server. Code: 0
		/// </summary>
		Success, 
		/// <summary>
		/// Application id dont exist on the database. Code: 1
		/// </summary>
		Invalid, 
		/// <summary>
		/// Server found invalid arguments thus rejecting conn. Code: 2
		/// </summary>
		Rejected,

		/// <summary>
		/// Please check your socket host and port. Code: 3
		/// </summary>
		Error,


		
		/// <summary>
		/// Game is currenly inactive and block want new connection. Code: 4
		/// </summary>
		Inactive,
		/// <summary>
		/// Game is currenly at its limit and cant accept new connection Code: 5
		/// </summary>
		Overload,
		/// <summary>
		/// Users account is currently online. Code: 6
		/// </summary>
		Online,

		/// <summary>
		/// USocketClient is currently performing task. Code: 7
		/// </summary>
		Busy,
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
		public string id = string.Empty;
	}


}