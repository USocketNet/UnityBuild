using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using BytesCrafter.USocketNet;



public class CharSet : MonoBehaviour {

	public ThirdPersonUserControl ctrl = null;
	public USocketView view = null;

	void Update () 
	{
		if(view != null)
		{
			ctrl.enabled = view.IsLocalUser;
		}
	}
}
