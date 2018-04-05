using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using BytesCrafter.USocketNet;

public class AutoCamSet : MonoBehaviour
{
	public AutoCam autoCam = null;
	public FreeLookCam freeCam = null;

	void Update ()
	{
		if(USocket.Instance.usocketNets.Count > 0)
		{
			if(USocket.Instance.usocketNets[0].localSockets.Count > 0)
			{
				if(autoCam != null)
				{
					autoCam.SetTarget(USocket.Instance.usocketNets [0].localSockets [0].transform);
				}

				if(freeCam != null)
				{
					freeCam.SetTarget(USocket.Instance.usocketNets [0].localSockets [0].transform);
				}
			}
		}
	}
}
