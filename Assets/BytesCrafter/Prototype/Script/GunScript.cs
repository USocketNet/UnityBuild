using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunScript : MonoBehaviour
{
	[Header("Gus Profile")]
	public bool autoMatic = true;
	public float frequency = 0.5f;
	public float timer = 0f;

	[Header("Object Reference")]
	public Transform muzzlePart = null;
	public AudioSource audioSource = null;
	public Transform gunShot = null;

	public void Shoot(float shootDistance)
	{

		if (autoMatic)
		{
			timer += Time.deltaTime;

			if(timer > frequency)
			{
				timer = 0f;
			}

			else
			{
				return;
			}
		}

		//Show muzzle flash.

		//Play sounds effect.
		audioSource.Play();

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit = new RaycastHit (); //float height = 0f;

		if(Physics.Raycast(ray, out hit, shootDistance))
		{
			if(hit.collider != null)
			{
				DamageReport dr = hit.collider.GetComponent<DamageReport> ();
				if(dr != null)
				{
					if(dr.playCtrl.GetInstanceID() != this.GetInstanceID())
					{
						dr.DamageTaken (gameObject.name, 100f);
						Debug.DrawLine (muzzlePart.position, hit.point, Color.green, Time.deltaTime);
					}

					else
					{
						Debug.DrawLine (muzzlePart.position, hit.point, Color.red, Time.deltaTime);
					}
				}

				else
				{
					Debug.DrawLine (muzzlePart.position, hit.point, Color.black, Time.deltaTime);
				}

				Instantiate (gunShot, hit.point, Quaternion.identity);
			}
		}

		else
		{
			Debug.DrawLine (muzzlePart.position, Camera.main.transform.forward * shootDistance, Color.blue, Time.deltaTime);
		}
	}
}
