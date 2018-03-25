using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BytesCrafter.USocketNet;

public class UnityPlayer : MonoBehaviour
{
	[Header("Player Prefs")]
	public float posSpeed = 7.5f;
	public float rotSpeed = 75f;
	public float jumpHeight = 4.9f;

	[Header("Reference")]
	public Camera camObject = null;
	public Rigidbody rigObject = null;
	public USocketView socketView = null;

	private bool IsGrounded
	{
		get{ return Physics.Raycast (transform.position, -Vector3.up, 1.02f); }
	}

	private Vector3 prevPos = Vector3.zero;
	void Update () 
	{
		if (socketView.socketId == string.Empty)
			return;

		if(socketView.IsLocalUser)
		{
			camObject.enabled = true;

			transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * posSpeed * Time.deltaTime);
			transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime);

			if(IsGrounded && Input.GetKeyDown(KeyCode.Space))
			{
				rigObject.AddForce (Vector3.up * jumpHeight, ForceMode.Impulse);
			}
		}

		else
		{
			camObject.enabled = false;

			rigObject.useGravity = false;
			rigObject.detectCollisions = false;
		}
	}
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                