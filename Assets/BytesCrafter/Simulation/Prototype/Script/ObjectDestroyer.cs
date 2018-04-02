using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
	void Awake ()
	{
		Destroy (this.gameObject, 2.0f);
	}
}