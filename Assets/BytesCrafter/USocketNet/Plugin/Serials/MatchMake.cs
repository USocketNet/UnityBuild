using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet
{
	/// <summary>
	/// MatchMake is an enum handler for match make event.
	/// </summary>
	public enum MatchRes
	{ 
		/// <summary>
		/// You successfully subscribed to a channel.
		/// </summary>
		Success, 
		/// <summary>
		/// Failed to subscribed, you are current subrscribed to other channel or channel name exist or full.
		/// </summary>
		Failed, 
		/// <summary>
		/// For some reason, server did not respond.
		/// </summary>
		Error
	}

	[System.Serializable]
	public class MatchMake
	{
		public MatchRes mr = MatchRes.Error;
		public string ci = string.Empty;
	}
}