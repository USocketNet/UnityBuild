
using UnityEngine;
using System.Collections.Generic;

namespace BytesCrafter.USocketNet
{
	[RequireComponent(typeof(Rigidbody))]
	public class USocketView : MonoBehaviour
	{
		#region Parameters

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

		[Range(1f, 30f)] public float sendRate = 15f;

		[HideInInspector] public string socketId = string.Empty;
		[HideInInspector] public bool IsLocalUser = false;
		[HideInInspector] public Rigidbody rigidbody = null;

		[Header("MAIN TRANSFORM")]
		public TransAxis position = new TransAxis();
		public TransAxis rotation = new TransAxis();
		public TransAxis scale = new TransAxis();
		public StateList states = new StateList();

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

		#endregion

		void Awake()
		{
			targetPos = transform.position;
			targetRot = transform.rotation.eulerAngles;
			targetSize = transform.localScale;

			rigidbody = GetComponent<Rigidbody>();
			if (IsLocalUser)
			{
				rigidbody.useGravity = true;
				//rigidbody.isKinematic = false;

			}
			else
			{
				//rigidbody.isKinematic = true;
				rigidbody.useGravity = false;
			}
		}
	}
}
