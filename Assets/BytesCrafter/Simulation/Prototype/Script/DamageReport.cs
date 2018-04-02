
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageReport : MonoBehaviour
{
	public Vector3 eulerRot;
	public Quaternion rots;

	void LateUpdate()
	{
		eulerRot = transform.eulerAngles;
		rots = transform.rotation;
	}


	public PlayerController playCtrl = null;
	public Collider collider = null;

	public void DamageTaken(string playerId, float damage)
	{
		Debug.Log (transform.root.name + " was hit on the " + name);
		playCtrl.DamageTaken (playerId, damage);
	}
}