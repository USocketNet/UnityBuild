using UnityEngine;
using BytesCrafter.USocketNet;

[RequireComponent(typeof(Rigidbody))]
public class SimpleControl : MonoBehaviour
{
	public bool autoNavigate = false;
	public bool onControl = false;

	public string socketId = string.Empty;
	public string instanceId = string.Empty;

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

	void Awake()
	{
		onControl = true;
	}

	public int curTrackPoint = 0;
	public TrackPoints trackPoints = null;
	private Vector2 curVpos = Vector2.zero;
	void Update () 
	{
		socketId = socketView.Identity;
		instanceId = socketView.Instance;

		if (socketView.Identity == string.Empty)
			return;

		if(socketView.IsLocalUser)
		{
			if(autoNavigate)
			{
				if(trackPoints == null)
				{
					trackPoints = FindObjectOfType<TrackPoints> ();
					curTrackPoint = Random.Range (0, trackPoints.transform.childCount -1);
					camObject.enabled = false;
					return;
				}

				else
				{
					if(Vector3.Distance(trackPoints.trackPoints[curTrackPoint].position, transform.position) > 1f)
					{
						transform.position = Vector3.MoveTowards(transform.position, trackPoints.trackPoints[curTrackPoint].position, posSpeed * Time.deltaTime);
					}

					else
					{
						if(curTrackPoint < trackPoints.trackPoints.Length - 1)
						{
							curTrackPoint += 1;
						}

						else
						{
							curTrackPoint = 0;
						}
					}
				}
			}

			else
			{
				camObject.enabled = false;

				if(!onControl)
					return;

				camObject.enabled = true;
				rigObject.useGravity = true;
				rigObject.detectCollisions = true;

				if(Application.isMobilePlatform)
				{
					if(Input.touchCount == 1)
					{
						if(Input.GetTouch(0).phase == TouchPhase.Began)
						{
							curVpos = Input.GetTouch (0).position;
						}

						if(Input.GetTouch(0).phase == TouchPhase.Moved)
						{
							Vector2 magni = new Vector2 (
								Mathf.Clamp(Input.GetTouch (0).position.x - curVpos.x, -1f, 1f),
								Mathf.Clamp(Input.GetTouch (0).position.y - curVpos.y, -1f, 1f)
							);

							transform.Translate(Vector3.forward * magni.y * posSpeed * Time.deltaTime);
							transform.Rotate(Vector3.up * magni.x * rotSpeed * Time.deltaTime);
						}
					}

					if(Input.touchCount >= 2 && IsGrounded)
					{
						rigObject.AddForce (Vector3.up * jumpHeight, ForceMode.Impulse);
					}
				}

				else
				{
					transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * posSpeed * Time.deltaTime);
					transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime);

					if(IsGrounded && Input.GetKeyDown(KeyCode.Space))
					{
						rigObject.AddForce (Vector3.up * jumpHeight, ForceMode.Impulse);
					}
				}
			}
		}

		else
		{
			camObject.enabled = false;
			rigObject.useGravity = false;
			rigObject.detectCollisions = false;
		}
	}
}