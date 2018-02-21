using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Typing
{
	Sending, Receiving
}

public enum Interpol
{
	Realtime, MoveTowards, MoveLerp
}

public class SendScript : MonoBehaviour
{
	public Typing typing = Typing.Sending;
	public float jumpHeight = 3f;

	[Header("Sending")]
	public float posSpeed = 3f;
	public float rotSpeed = 3f;
	public SendScript target = null;

	[Header("Receiving")]
	public Interpol interpol = Interpol.Realtime;
	public float teleportPos = 3f;
	public float teleportRot = 45f;

	[Header("Status")]
	public Vector3 offset = Vector3.zero;
	[Range(1, 30)]
	public int frequency = 15;
	private float timer = 0f;

	void Update()
	{
		if(typing == Typing.Sending)
		{
			transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * posSpeed * Time.deltaTime);
			transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime);

			timer -= Time.deltaTime;

			if(timer < 0f)
			{
				target.Receive (transform.position, transform.rotation.eulerAngles);
				timer = 1f/System.Convert.ToSingle (frequency);
				Debug.Log ("Sent!");
			}

			if(Input.GetKeyDown(KeyCode.Space))
			{
				GetComponent<Rigidbody>().AddForce (Vector3.up * jumpHeight, ForceMode.Impulse);
			}
		}

		else if(typing == Typing.Receiving)
		{
			if(Vector3.Distance(transform.position, tPos) > teleportPos)
			{
				//transform.position = tPos + offset;
			}

			if(Quaternion.Angle(transform.rotation, Quaternion.Euler(tRot)) > teleportRot)
			{
				//transform.rotation = Quaternion.Euler(tRot);
			}

			if(interpol == Interpol.Realtime)
			{
				transform.position = tPos + offset;
				transform.rotation = Quaternion.Euler(tRot);
			}

			else if(interpol == Interpol.MoveTowards)
			{
				float damp = Vector3.Distance (transform.position, tPos + offset);
				transform.position = Vector3.MoveTowards (transform.position, tPos + offset, (damp + posInterpolation) * posSpeed * Time.deltaTime);

				float rDamp = Quaternion.Angle (transform.rotation, Quaternion.Euler(tRot));
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(tRot), (rDamp + rotInterpolation) * rotSpeed * Time.deltaTime);
			}

			else if(interpol == Interpol.MoveLerp)
			{
				float damp = Vector3.Distance (transform.position, tPos + offset);
				transform.position = Vector3.Lerp (transform.position, tPos + offset, (damp + posInterpolation) * posSpeed * Time.deltaTime);

				float rDamp = Quaternion.Angle (transform.rotation, Quaternion.Euler(tRot));
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(tRot), (rDamp + rotInterpolation) * rotSpeed * Time.deltaTime);
			}
		}
	}

	public Vector3 tPos = Vector3.zero;
	public float posInterpolation = 3f;
	public Vector3 tRot = Vector3.zero;
	public float rotInterpolation = 3f;

	public void Receive(Vector3 position, Vector3 rotation)
	{
		tPos = position; tRot = rotation;
	}
}