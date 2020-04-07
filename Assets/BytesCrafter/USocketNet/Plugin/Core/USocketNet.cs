using System;
using System.Collections.Generic;

using UnityEngine;
using BytesCrafter.USocketNet;

namespace BytesCrafter.USocketNet
{
	public class USocketNet
	{
		private static USocketNet udata = null;
		public static USocketNet Instance
		{
			get
			{
				if(udata == null)
				{
					udata = new USocketNet ();
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


	}
}