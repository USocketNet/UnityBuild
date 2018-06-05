using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BytesCrafter.USocketNet;

public class Med_Char_Control : MonoBehaviour
{
	[Header("PROFILE")]
	public float health = 1000f;

	[Header("MOVEMENT")]
	public float jumpHeight = 3f;
	public Vector2 moveSpeed = Vector2.one;
	public float rotationSpeed = 3f;
	public bool onControl = true;
	private bool showCursor = true;

	[Header("REFERENCES")]
	public Animator animator = null;
	public Rigidbody rigidBody = null;
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

	public Vector3 prevPosition = Vector3.zero;
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Tab))
		{
			showCursor = !showCursor; Cursor.visible = showCursor;
			if(showCursor) { Cursor.lockState = CursorLockMode.None; }
			else { Cursor.lockState = CursorLockMode.Locked; }

			Debug.LogWarning ( " Cursor has been set to " + Cursor.lockState.ToString() );
		}

		if(socketView != null)
		{
			if(!socketView.enabled)
			{
				onControl = true;
				camObject.enabled = true;
				rigidBody.useGravity = true;
			}

			else
			{
				if(socketView.IsLocalUser)
				{
					onControl = true;
					camObject.enabled = true;
					rigidBody.useGravity = true;
				}

				else
				{
					onControl = false;
					camObject.enabled = false;
					rigidBody.useGravity = false;
				}
			}
		}

		if(onControl)
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
			//if(prevPosition == Vector3.zero)
			//{
			//	prevPosition = Input.mousePosition;
			//}
			//Vector3 deltaRota = Input.mousePosition - prevPosition;
			//transform.Rotate(Vector3.up * deltaRota.x * rotationSpeed * Time.deltaTime);
			//prevPosition = Input.mousePosition;

			//Animation
			animator.SetFloat("Horizontal", direction.x);
			animator.SetFloat("Vertical", direction.y);

			if(animStateInfo.fullPathHash == Animator.StringToHash("Primary.Walking"))
			{
				if(jumping)
				{
					rigidBody.velocity = new Vector3 (0f, jumpHeight, 0f);
					animator.SetTrigger("Jump");
				}

				if(normalAttack)
				{
					animator.SetTrigger("Attack");
				}

				if(superAttack)
				{
					
				}
			}

			animator.SetBool("Block", Input.GetMouseButton(1));
			debugger = animStateInfo.fullPathHash.ToString();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.transform.root.GetInstanceID() != transform.GetInstanceID())
		{
			Debug.LogError ("Hitter");
			health -= 10f;
		}
	}
}