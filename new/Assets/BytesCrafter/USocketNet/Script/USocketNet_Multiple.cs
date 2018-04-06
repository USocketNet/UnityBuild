using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BytesCrafter.USocketNet;

public class USocketNet_Multiple : MonoBehaviour
{
	[Header("USOCKETNET INSTANCE")]
	public GameObject gobjects = null;
	private int current = 0;

	IEnumerator Connecting ()
	{
		USocketNet socket = Instantiate (gobjects).GetComponent<USocketNet> ();
		yield return socket;
		 
		ConnectAndJoin (socket);
	}

	private void ConnectAndJoin(USocketNet socket)
	{
		socket.ConnectToServer ("", (ConnStat conStat, ConnJson conJson) =>
			{
				if(conStat == ConnStat.Connected)
				{
					socket.GetComponent<USocketNet_UIDemo>().ChangeCanvas(1);

					for(int index = 0; index < USocket.Instance.usocketNets.Count; index++)
					{
						USocket.Instance.usocketNets[index].GetComponent<Canvas> ().enabled = false;
						foreach(USocketView sockV in USocket.Instance.usocketNets[index].localSockets)
						{
							sockV.GetComponent<USocketNet_CtrlDemo> ().onControl = false;
						}
					}

					USocket.Instance.usocketNets[USocket.Instance.usocketNets.Count - 1].GetComponent<Canvas> ().enabled = true;
					foreach(USocketView sockV in USocket.Instance.usocketNets[USocket.Instance.usocketNets.Count - 1].localSockets)
					{
						sockV.GetComponent<USocketNet_CtrlDemo> ().onControl = true;
					}

					socket.AutoMatchChannel ("Default", 10, (Returned returned, ChannelJson channelJson) =>
						{
							if(returned == Returned.Success)
							{
								socket.GetComponent<USocketNet_UIDemo>().ChangeCanvas(2);
								current = USocket.Instance.usocketNets.Count - 1;
								SetActiveSockets();
							}
						});
				}

				if(conStat == ConnStat.Disconnected)
				{
					ConnectAndJoin (socket);
				}
			});
	}

	private void SetActiveSockets()
	{
		if (USocket.Instance.usocketNets.Count == 0)
			return;

		for(int index = 0; index < USocket.Instance.usocketNets.Count; index++)
		{
			USocket.Instance.usocketNets[index].GetComponent<Canvas> ().enabled = false;
			foreach(USocketView sockV in USocket.Instance.usocketNets[index].localSockets)
			{
				sockV.GetComponent<USocketNet_CtrlDemo> ().onControl = false;
			}
		}

		instance = 0;
		USocket.Instance.usocketNets[current].GetComponent<Canvas> ().enabled = true;
		foreach(USocketView sockV in USocket.Instance.usocketNets[current].localSockets)
		{
			sockV.GetComponent<USocketNet_CtrlDemo> ().onControl = false;
		}
		if(USocket.Instance.usocketNets[current].localSockets.Count > 0)
		{
			USocket.Instance.usocketNets[current].localSockets[instance].GetComponent<USocketNet_CtrlDemo> ().onControl = true;
		}
	}

	void Update()
	{
		//Connect and Join!
		if(Input.GetKeyDown(KeyCode.F1))
		{
			StartCoroutine ( Connecting () );
		}

		//Switch Active Socket.
		if(Input.GetKeyDown(KeyCode.F2))
		{
			int sockInt = USocket.Instance.usocketNets.FindIndex (x => x.GetComponent<Canvas> ().enabled);

			if(sockInt == USocket.Instance.usocketNets.Count - 1)
			{
				current = 0;
			}

			else
			{
				current += 1;
			}

			Debug.Log ("AAAAAAAA: " + current);

			SetActiveSockets ();
		}

		//Instatiate a player.
		if(Input.GetKeyDown(KeyCode.F3))
		{
			USocket.Instance.usocketNets[current].Instantiate (0, Vector3.zero, Quaternion.identity, (Returned rets) => 
				{
					if(rets == Returned.Success)
					{
						if(USocket.Instance.usocketNets [current].localSockets.Count == 0)
						{
							instance = 0;
						}

						else
						{
							instance = USocket.Instance.usocketNets [current].localSockets.Count - 1;
						}

						USocketNet_CtrlDemo uPlayer = null;

						for(int i = 0; i < USocket.Instance.usocketNets[current].localSockets.Count; i++)
						{
							uPlayer = USocket.Instance.usocketNets[current].localSockets[i].GetComponent<USocketNet_CtrlDemo> ();

							if(uPlayer != null)
							{
								uPlayer.onControl = false;
							}

						}
						if(uPlayer != null)
						{
							USocket.Instance.usocketNets[current].localSockets[instance].GetComponent<USocketNet_CtrlDemo> ().onControl = true;
						}
					}
				}); // spawnPoint.position, spawnPoint.rotation);
		}

		//Switch Instances.
		if(Input.GetKeyDown(KeyCode.F4))
		{
			USocket.Instance.usocketNets[current].GetComponent<Canvas> ().enabled = true;

			int playCtrl = USocket.Instance.usocketNets[current].localSockets.FindIndex (x => x.GetComponent<USocketNet_CtrlDemo> ().onControl == true);

			Debug.Log ("Control: " + playCtrl);

			if(playCtrl < USocket.Instance.usocketNets[current].localSockets.Count - 1)
			{
				instance += 1;
			}

			else
			{
				instance = 0;
			}

			for(int i = 0; i < USocket.Instance.usocketNets[current].localSockets.Count; i++)
			{
				if(instance == i)
				{
					USocket.Instance.usocketNets[current].localSockets[i].GetComponent<USocketNet_CtrlDemo> ().onControl = true;
				}

				else
				{
					USocket.Instance.usocketNets[current].localSockets[i].GetComponent<USocketNet_CtrlDemo> ().onControl = false;
				}
			}
		}
	}

	private int instance = 0;
}