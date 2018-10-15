using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet.Serializables
{
	public enum LeavedResult
	{ 
		Updated, Closed
	}

	[System.Serializable]
	public class LeavedCallback
	{
		public LeavedResult result = LeavedResult.Updated;
	}
}