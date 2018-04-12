using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet.Serializables
{
	[System.Serializable]
	public class SocketChild
	{
		//Synchronized or bypass this data!
		public bool synchronize = true;

		public List<ViewChilds> childList = new List<ViewChilds>();

		//Rate of sync per seconds timespan.
		[Range(1f, 30f)] public float sendRate = 15f;

		//sendTimer for synching looper.
		[HideInInspector] public float sendTimer = 0f;

		[HideInInspector] public string prevVstring = string.Empty;
	}

	[System.Serializable]
	public class ViewChilds
	{
		public Transform reference = null;
		public VectorOption position = new VectorOption();
		public VectorOption rotation = new VectorOption();
		public VectorOption scale = new VectorOption();
	}

	[System.Serializable]
	public class ChildTrans
	{
		public List<ChildTran> lists = new List<ChildTran>();
	}

	[System.Serializable]
	public class ChildTran
	{
		public Vector3 position = Vector3.zero;
		public Quaternion rotation = Quaternion.identity;
		public Vector3 scale = Vector3.zero;
	}
}