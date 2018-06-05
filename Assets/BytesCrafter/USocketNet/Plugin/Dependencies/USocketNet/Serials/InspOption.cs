﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet.Serializables
{
	public enum SocketSync
	{
		Realtime, AdjustToward, LerpValues
	}

	public enum SyncGroup
	{
		Single, Multiple
	}

	[System.Serializable]
	public class SocketAxis
	{
		public bool xAxis = true;
		public bool yAxis = true;
		public bool zAxis = true;

		public SocketAxis(bool all)
		{
			xAxis = all;
			yAxis = all;
			zAxis = all;
		}
	}

	[System.Serializable]
	public class Floatings
	{
		public int xPoints = 4;
		public int yPoints = 4;
		public int zPoints = 4;
	}

	[System.Serializable]
	public class VectorOption
	{
		//Synchronized or bypass this data!
		public bool synchronize = true;

		public SocketAxis axises = new SocketAxis(true);
		public Floatings floatings = new Floatings();

		//Rate of sync per seconds timespan.
		[Range(1f, 30f)] public float sendRate = 15f;

		public float interpolation = 3f;
		public float speed = 3f;

		//sendTimer for synching looper.
		[HideInInspector] public float sendTimer = 0f;

		[HideInInspector] public string prevVstring = string.Empty;

		//Current sync mode to follow.
		public SocketSync syncMode = SocketSync.Realtime;
	}

	[System.Serializable]
	public class StateOption
	{
		public bool synchronize = true;
		[Range(1f, 30f)] public float sendRate = 1f;
		[HideInInspector] public float sendTimer = 0f;
		[HideInInspector] public string prevVstring = string.Empty;

		public List<string> syncValue = new List<string>();
	}

	public enum APType
	{
		Float, Int, Bool
	}

	[System.Serializable]
	public class AnimatorOption
	{
		public bool synchronize = true;
		[Range(1f, 30f)] public float sendRate = 1f;
		public Animator reference = null;
		public List<AnimTypes> parameters = new List<AnimTypes> ();
		[HideInInspector] public float sendTimer = 0f;
		[HideInInspector] public string prevVstring = string.Empty;
	}

	[System.Serializable]
	public class AnimTypes
	{
		public APType apType = APType.Float;
		public string apValue = string.Empty;

		public AnimTypes(APType _apType)
		{
			apType = _apType;
		}
	}
}