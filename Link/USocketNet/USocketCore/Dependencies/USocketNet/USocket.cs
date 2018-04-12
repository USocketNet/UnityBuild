﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BytesCrafter.USocketNet;

namespace BytesCrafter.USocketNet
{
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

		public int synchRate = 0;
		public float uploadRate = 0f;
		public float downloadRate = 0f;

		/// <summary>
		/// The usocket nets client in this local machine.
		/// </summary>
		public List<USocketClient> usocketNets = new List<USocketClient>();

		/// <summary>
		/// The socket view of all the other machine's.
		/// </summary>
		public List<USocketView> socketIdentities = new List<USocketView>();
	}
}