
using UnityEngine;
using System.Collections.Generic;

namespace BytesCrafter.USocketNet
{
	public class USocketView : MonoBehaviour
	{
		#region Required Reference

		private USocketNet socketNet = null;
		public USocketNet uSocketNet
		{
			set
			{
				if(socketNet == null)
				{
					socketNet = value;
				}
			}
		}

		#endregion

		#region Reaction Parameters

		[HideInInspector] public string socketId = string.Empty;
		[HideInInspector] public bool IsLocalUser = false;

		#endregion

		#region Parameters

		[Header("SYNCHRONIZATION")]
		public VectorOption position = new VectorOption();
		public VectorOption rotation = new VectorOption();
		public VectorOption scale = new VectorOption();
		public StateOption states = new StateOption();

		[Header("POSITION STATUS")]
		public Vector3 targetPos = Vector3.zero;
		public float posInterpolation = 3f;
		public float posSpeed = 3f;

		[Header("ROTATION STATUS")]
		public Vector3 targetRot = Vector3.zero;
		public float rotInterpolation = 3f;
		public float rotSpeed = 3f;

		[Header("SCALE STATUS")]
		public Vector3 targetSize = Vector3.zero;
		public float sizeInterpolation = 3f;
		public float sizeSpeed = 3f;

		[Header("STATE STATUS")]
		public List<string> targetState = new List<string> ();

		#endregion

		void Awake()
		{
			targetPos = transform.position;
			targetRot = transform.rotation.eulerAngles;
			targetSize = transform.localScale;
		}


	}
}
