
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BytesCrafter.USocketNet;

public enum PlayerCtlrAction
{
	ChooseAction, GetRequiredComponents, AddDamageReports
}

public class PlayerController : MonoBehaviour
{
	[Header("Data Preview : Debug!")]
	public float height = 0f;

	public float jumpClamped = 0f;

	[Header("State Preview")]
	public float curPivotHeight = 0f;
	public Vector2 lookValue = Vector2.zero;
	public Vector2 analogValue = Vector2.zero;
	public Vector2 currentAxis = Vector2.zero;

	[Header("Controller Profile")]
	[Range(0f, 1f)] public float moveDamping = 0.7f;
	[Range(0f, 12f)] public float jumpHeight = 3f;
	[Range(0f, 1200f)] public float shootDistance = 250f;
	//[Range(0f, 1f)] public float lookDamping = 0.5f;
	public Vector2 camSentivity = new Vector2(120f, 120f);

	[Header("State Boolean")]
	public bool isWatching = false;
	public bool isDead = false;
	public bool isAiming = false;
	public bool isProning = false;
	public bool isCrouching = false;
	public bool isWalking = false;
	public bool isRotating = false;

	[Header("State Speed")]
	public Vector2 proneSpeed = new Vector2(0.40f, 0.7f);
	public Vector2 crouchSpeed = new Vector2(1.0f, 1.5f);
	public Vector2 walkSpeed = new Vector2(2.0f, 3.0f);
	public Vector2 runSpeed = new Vector2(5.0f, 7.0f);

	[Header("Main Components")]
	public AudioListener audiListener = null;
	public Rigidbody rigidBody = null;
	public Animator animator = null;
	public USocketView socket = null;

	[Header("Guns Parameter")]
	public Transform rightHand = null;
	public GunScript gunPrefab = null;
	public List<GunScript> gunsList = new List<GunScript>();

	[Header("Looking Parameter")]
	[Range(0f, 10f)] public float zoomDamping = 5f;
	[Range(0f, 10f)] public float camClippingDamping = 5f;
	public Vector2 lookClampY = new Vector2(25f, 49f);
	public Vector2 cursorPosition = new Vector2(3f, 1f);
	public float pivotStandHeight = 1.0f;
	public float pivotCrouchHeight = 0.49f;
	public float pivotProneHeight = 0.25f;
	public Camera camObject = null;
	public bool showCursor = true;
	public Transform camPivot = null;

	[Header("Object Colliders")]
	public Collider mainCollider = null;
	public Collider[] colliders = null;

	[Header("Script Actions")]
	public PlayerCtlrAction actions = PlayerCtlrAction.ChooseAction;

	public void DamageTaken(string playerId, float damage)
	{
		Debug.Log ("Damage Taken! " + damage + " Points by " + playerId);
	}

	void OnValidate()
	{
		if(actions == PlayerCtlrAction.GetRequiredComponents)
		{
			rigidBody = GetComponent<Rigidbody> ();
			mainCollider = GetComponent<Collider> ();
			colliders = transform.GetChild (0).GetComponentsInChildren<Collider> ();

			actions = PlayerCtlrAction.ChooseAction;
		}

		else if(actions == PlayerCtlrAction.AddDamageReports)
		{
			foreach(Collider collide in colliders)
			{
				DamageReport damageReport = collide.GetComponent<DamageReport>();

				if(damageReport != null)
				{
					damageReport.collider = collide;
					damageReport.playCtrl = this;
				}

				else
				{
					collide.gameObject.AddComponent<DamageReport> ();
				}
			}

			actions = PlayerCtlrAction.ChooseAction;
		}

		camObject.enabled = socket.IsLocalUser;
	}

	private void LookRot()
	{
		
	}

	void Update()
	{
		camObject.enabled = socket.IsLocalUser;
		isWatching = socket.IsLocalUser;
		audiListener.enabled = isWatching;

		if(gunPrefab == null){ gunPrefab = Instantiate(gunsList[0], rightHand); }

		if(!isDead)
		{
			mainCollider.enabled = true;
			rigidBody.useGravity = true;
		}

		foreach(Collider collide in colliders)
		{
			if(isDead)
			{
				if(collide.isTrigger)
				{
					collide.isTrigger = false;
				}
			}

			else
			{
				if(!collide.isTrigger)
				{
					collide.isTrigger = true;
				}
			}
		}

		if(isDead)
		{
			rigidBody.useGravity = false;
			mainCollider.enabled = false;
		}

		animator.enabled = !isDead;

		if(!socket.IsLocalUser) { return; }

		if(!isDead)
		{
			Movement ();
			Looking ();
			Jumping ();
		}
	}

	private void Jumping()
	{
		if(isCrouching || isProning) { return; }

		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(jumpClamped < 0.1f)
			{
				rigidBody.velocity = new Vector3(0f, jumpHeight, 0f);
			}
		}
	}

	public Vector3 offset;
	public Transform hips = null;
	public Transform target = null;

	void FixedUpdate()
	{
		Grounding ();
		CameraAdjust ();

		if(!socket.IsLocalUser) { return; }

		Shooting ();
	}

	private void CameraAdjust()
	{
		bool hitSomething = false; Vector3 defCamPos = new Vector3 (cursorPosition.x, cursorPosition.y, -70f);
		Vector3 worldDefCamPos = camPivot.TransformPoint (defCamPos);
		float rayDistance = Vector3.Distance (worldDefCamPos, camPivot.position);
		RaycastHit hit = new RaycastHit (); Vector3 hitPos = Vector3.zero;

		if(Physics.Raycast(camPivot.position, -camPivot.forward, out hit, rayDistance))
		{
			if(hit.collider != null)
			{
				hitSomething = true; hitPos = camPivot.InverseTransformPoint ( hit.point );
				Debug.DrawLine (camPivot.position, hit.point, Color.green, Time.deltaTime);
			}
		}

		//defCamPos.y = defCamPos.y * (Vector3.Distance(camPivot.position, hitPos) / rayDistance);
		if(hitSomething) { camObject.transform.localPosition = Vector3.Lerp (camObject.transform.localPosition, hitPos, camClippingDamping * Time.deltaTime); }
		else { camObject.transform.localPosition = Vector3.Lerp (camObject.transform.localPosition, defCamPos, camClippingDamping * Time.deltaTime); }
	}

	private void Grounding()
	{
		RaycastHit hit = new RaycastHit (); //float height = 0f;

		if(Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity))
		{
			if(hit.collider != null)
			{
				Debug.DrawLine (transform.position, hit.point, Color.red, Time.deltaTime);
				height = Vector3.Distance(transform.position, hit.point);

				jumpClamped = height / (jumpHeight * 0.1f);

				animator.SetFloat ("JumpHeight", jumpClamped);
			}
		}
	}

	private void Shooting ()
	{
		if(showCursor) { return; }

		animator.SetBool ("Shooting", Input.GetMouseButton(0));
		if(Input.GetMouseButton(0))
		{
			gunPrefab.Shoot (shootDistance);
		}

	}

	private void Looking()
	{
		if(Input.GetKeyDown(KeyCode.Tab))
		{
			showCursor = !showCursor; Cursor.visible = showCursor;
			if(showCursor) { Cursor.lockState = CursorLockMode.None; }
			else { Cursor.lockState = CursorLockMode.Locked; }

			Debug.LogWarning ( " Cursor has been set to " + Cursor.lockState.ToString() );
		}

		if(Input.GetMouseButton(1)) { camObject.fieldOfView = Mathf.Lerp (camObject.fieldOfView, 20f, zoomDamping * Time.deltaTime); isAiming = true; }
		else { camObject.fieldOfView = Mathf.Lerp (camObject.fieldOfView, 60f, zoomDamping * Time.deltaTime); isAiming = false;  }
		animator.SetBool ("isAimingTarget", isAiming);

		if(showCursor) { return; }


		if(isCrouching)
		{ if(isProning) { curPivotHeight = Mathf.Lerp (curPivotHeight, pivotProneHeight, Time.deltaTime); }
		else { curPivotHeight = Mathf.Lerp (curPivotHeight, pivotCrouchHeight, Time.deltaTime); }
		} else { curPivotHeight = Mathf.Lerp (curPivotHeight, pivotStandHeight, Time.deltaTime); }
		camPivot.position = new Vector3 (camPivot.position.x, curPivotHeight, camPivot.position.z);

		float xAxis = Input.GetAxis ("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");

		lookValue = new Vector2 (Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		transform.Rotate (Vector3.up * lookValue.x * camSentivity.x * Time.deltaTime); 

		camPivot.Rotate(-Vector3.right * lookValue.y * camSentivity.y * Time.deltaTime);
		//float clampYmouse = Mathf.Clamp (camPivot.eulerAngles.z, lookClampY.x, lookClampY.y);
		//camPivot.eulerAngles = new Vector3 (camPivot.eulerAngles.x, clampYmouse, camPivot.eulerAngles.z); 

		if(xAxis == 0f && yAxis == 0f)
		{
			if(lookValue.x > 0f) { isRotating = true; }
			else if(lookValue.x < 0f) { isRotating = true; }
			else { isRotating = false; }

			if(isCrouching)
			{
				if(isProning)
				{
					animator.SetBool ("isStandRotating", false);
					animator.SetBool ("isCrouchRotating", false);

					animator.SetFloat ("ProneRotating", lookValue.x);
					animator.SetBool ("isProneRotating", isRotating);

					curPivotHeight = Mathf.Lerp (curPivotHeight, pivotProneHeight, Time.deltaTime);
				}

				else
				{
					animator.SetBool ("isStandRotating", false);
					animator.SetBool ("isProneRotating", false);

					animator.SetFloat ("CrouchRotating", lookValue.x);
					animator.SetBool ("isCrouchRotating", isRotating);

					curPivotHeight = Mathf.Lerp (curPivotHeight, pivotCrouchHeight, Time.deltaTime);
				}
			}

			else
			{
				animator.SetBool ("isCrouchRotating", false);
				animator.SetBool ("isProneRotating", false);

				animator.SetFloat ("StandRotating", lookValue.x);
				animator.SetBool ("isStandRotating", isRotating);

				curPivotHeight = Mathf.Lerp (curPivotHeight, pivotStandHeight, Time.deltaTime);
			}
		}

		else
		{
			isRotating = false;
			animator.SetBool ("isStandRotating", isRotating);
			animator.SetBool ("isCrouchRotating", isRotating);
			animator.SetBool ("isProneRotating", isRotating);
		}
	}

	private void Movement()
	{
		analogValue.x = Input.GetAxis ("Horizontal");
		analogValue.y = Input.GetAxis ("Vertical");

		if (Input.GetKeyDown (KeyCode.LeftControl))
		{
			if(isProning)
			{
				isProning = false; //Make sure to disable proning.
				isCrouching = true;
			}

			else
			{
				if(isCrouching) { isCrouching = false; } //Set to NOT crouching.
				else { isCrouching = true; } //Set to THE crouching.
			}
		}

		if (Input.GetKeyDown (KeyCode.CapsLock))
		{
			isCrouching = true;
			if(isProning) { isProning = false; } //Set to NOT proning.
			else { isProning = true; } //Set to THE proning.
		}

		isWalking = !Input.GetKey (KeyCode.LeftShift);

		animator.SetBool ("Crouching", isCrouching);
		animator.SetBool ("Proning", isProning);
		animator.SetBool ("Walking", isWalking);

		if(isWalking) { currentAxis.x = walkSpeed.x; currentAxis.y = walkSpeed.y; } //Sync Walking Speed.
		else { currentAxis.x = runSpeed.x; currentAxis.y = runSpeed.y; } //Sync Running Speed.

		if(isProning) { if(isCrouching) { currentAxis.x = proneSpeed.x; currentAxis.y = proneSpeed.y; } } //Overide Walking Speed.
		else { if(isCrouching) { currentAxis.x = crouchSpeed.x; currentAxis.y = crouchSpeed.y; } } //Overide Crouching Speed.

		transform.Translate (Vector3.right * moveDamping * currentAxis.x * analogValue.x * Time.deltaTime);
		transform.Translate (Vector3.forward * moveDamping * currentAxis.y * analogValue.y * Time.deltaTime);

		animator.SetFloat ("MoveHaxis", analogValue.x);
		animator.SetFloat ("MoveVaxis", analogValue.y);
	}
}