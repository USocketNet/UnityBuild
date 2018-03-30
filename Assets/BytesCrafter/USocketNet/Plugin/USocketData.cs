using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BytesCrafter.USocketNet;

public class USocket
{
	private static USocket udata = null;
	public static USocket Instance
	{
		get
		{
			if(udata == null)
			{
				udata = new USocket ();
			}

			return udata;
		}
	}

	/// <summary>
	/// The usocket nets client in this local machine.
	/// </summary>
	public List<USocketNet> usocketNets = new List<USocketNet>();

	/// <summary>
	/// The socket view of all the other machine's.
	/// </summary>
	public List<USocketView> socketIdentities = new List<USocketView>();
}