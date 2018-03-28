using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BytesCrafter.USocketNet;

public class CharChanger : MonoBehaviour
{
	private USocketNet socketNet = null;
	public USocketNet uSocketNet
	{
		get
		{
			if(socketNet == null)
			{
				socketNet = GameObject.FindObjectOfType<USocketNet> ();
			}

			return socketNet;
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			uSocketNet.localSockets [0].GetComponent<UnityPlayer> ().onControl = true;
			uSocketNet.localSockets [1].GetComponent<UnityPlayer> ().onControl = false;
		}

		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			uSocketNet.localSockets [0].GetComponent<UnityPlayer> ().onControl = false;
			uSocketNet.localSockets [1].GetComponent<UnityPlayer> ().onControl = true;
		}
	}
}