
using System;
using System.Collections.Generic;

using SocketIO;
using UnityEngine;
using WebSocketSharp;

namespace BytesCrafter.USocketNet.Toolsets
{
	public class JsonSerializer
	{
		public static T ToObject<T>(string jsonString)
		{
			string trimmed = jsonString.TrimStart ('[');
			trimmed = trimmed.TrimEnd (']');
			return JsonUtility.FromJson<T> (trimmed);
		}

		public static string FromObject(object jsonObject)
		{
			return JsonUtility.ToJson(jsonObject);
		}
	}
}

namespace BytesCrafter.USocketNet
{
	public enum Returned
	{
		Success, Failed, Error
	}

	[System.Serializable]
	public class Returning
	{
		public Returned returned = Returned.Failed;
	}
}