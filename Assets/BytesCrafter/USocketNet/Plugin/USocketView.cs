
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

		[HideInInspector] public string Identity = string.Empty;
		[HideInInspector] public string Instance = string.Empty;
		[HideInInspector] public bool IsLocalUser = false;

		#endregion

		#region Parameters

		[Header("SYNCHRONIZATION")]
		public VectorOption position = new VectorOption();
		public VectorOption rotation = new VectorOption();
		public VectorOption scale = new VectorOption();
		public StateOption states = new StateOption();

		[HideInInspector] public Vector3 targetPos = Vector3.zero;
		[HideInInspector] public Vector3 targetRot = Vector3.zero;
		[HideInInspector] public Vector3 targetSize = Vector3.zero;
		[HideInInspector] public List<string> targetState = new List<string> ();

		#endregion

		void Awake()
		{
			targetPos = transform.position;
			targetRot = transform.rotation.eulerAngles;
			targetSize = transform.localScale;
		}


	}
}
