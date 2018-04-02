using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BytesCrafter.USocketNet;

public class Med_Char_Control : MonoBehaviour
{
	[Header("MOVEMENT")]
	public float jumpHeight = 3f;
	public Vector2 moveSpeed = Vector2.one;
	public float rotationSpeed = 3f;

	[Header("REFERENCES")]
	public Animator animator = null;
	public Rigidbody rigidbody = null;
	public Camera camObject = null;
	public USocketView socketView = null;

	[Header("DEBUGGER")]
	public string debugger = string.Empty;

	public Vector2 direction
	{
		get
		{
			return new Vector2(Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
		}
	}

	public bool jumping
	{
		get
		{
			return Input.GetKeyDown(KeyCode.Space);
		}
	}

	public bool normalAttack
	{
		get
		{
			return Input.GetMouseButtonDown(0);
		}
	}

	public bool superAttack
	{
		get
		{
			return Input.GetMouseButtonDown(1);
		}
	}

	void Update ()
	{
		if(socketView.IsLocalUser)
		{
			AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo (0);

			//Movement
			if(animStateInfo.fullPathHash == Animator.StringToHash("Primary.Walking"))
			{
				transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * moveSpeed.x * Time.deltaTime);
				transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * moveSpeed.y * Time.deltaTime);
			}

			//Rotation
			transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);

			//Animation
			animator.SetFloat("Horizontal", direction.x);
			animator.SetFloat("Vertical", direction.y);



			if(animStateInfo.fullPathHash == Animator.StringToHash("Primary.Walking"))
			{
				if(jumping)
				{
					rigidbody.velocity = new Vector3 (0f, jumpHeight, 0f);
					animator.SetTrigger("Jump");
				}

				if(normalAttack)
				{
					animator.SetTrigger("NAttack");
				}

				if(superAttack)
				{
					animator.SetTrigger("SAttack");
				}
			}

			debugger = animStateInfo.fullPathHash.ToString();

			camObject.enabled = true;
			rigidbody.useGravity = true;
		}

		else
		{
			camObject.enabled = false;
			rigidbody.useGravity = false;
		}


	}
}