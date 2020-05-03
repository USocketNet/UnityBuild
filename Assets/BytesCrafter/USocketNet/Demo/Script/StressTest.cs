
using UnityEngine;
using BytesCrafter.USocketNet;


public class StressTest : MonoBehaviour
{
	public string gameVariant = "Demoguy Stress";
	public USNClient clientPrefab = null;

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F1))
		{
			ConnectCreateMove ();
		}
	}
		
	public void ConnectCreateMove()
	{
		USNClient uClient = Instantiate (clientPrefab.transform).GetComponent<USNClient>();
		// uClient.ConnectToServer("Demoguy", "1234567", (ConStat cs) => {
		// 	if(ca == ConnAuth.Success)
		// 	{
		// 		uClient.AutoMatchChannel(gameVariant, 10, (MatchRes mr, MatchMake mm) => {
		// 			if(mr == MatchRes.Success)
		// 			{
		// 				uClient.Instantiate(0, Vector3.zero, Quaternion.identity, (Returned r) => {
		// 					if(r == Returned.Success)
		// 					{
		// 						Debug.Log("Success!");
		// 					}
		// 				});
		// 			}
		// 		});
		// 	}
		// });
	}

	public void ConnectRandomMove()
	{

	}

	public void ConnectPublicMessage()
	{

	}

	/*
	[Header("USOCKETNET INSTANCE")]
	public GameObject gobjects = null;
	private int current = 0;

	IEnumerator Connecting ()
	{
		USNClient socket = Instantiate (gobjects).GetComponent<USNClient> ();
		yield return socket;
		 
		ConnectAndJoin (socket);
	}

	private void ConnectAndJoin(USNClient socket)
	{

		socket.ConnectToServer ("", "", (ConnStat conStat, ConnAuth connAuth) =>
			{
				if(conStat == ConnStat.Connected)
				{
					socket.GetComponent<USocketNet_UIDemo>().ChangeCanvas(1);

					for(int index = 0; index < USocketNet.Instance.usocketNets.Count; index++)
					{
						USocketNet.Instance.usocketNets[index].GetComponent<Canvas> ().enabled = false;
						foreach(USocketView sockV in USocketNet.Instance.usocketNets[index].localSockets)
						{
							sockV.GetComponent<SimpleControl> ().onControl = false;
						}
					}

					USocketNet.Instance.usocketNets[USocketNet.Instance.usocketNets.Count - 1].GetComponent<Canvas> ().enabled = true;
					foreach(USocketView sockV in USocketNet.Instance.usocketNets[USocketNet.Instance.usocketNets.Count - 1].localSockets)
					{
						sockV.GetComponent<SimpleControl> ().onControl = true;
					}

					socket.AutoMatchChannel ("Default", 10, (Returned returned, ChannelJson channelJson) =>
						{
							if(returned == Returned.Success)
							{
								socket.GetComponent<USocketNet_UIDemo>().ChangeCanvas(2);
								current = USocketNet.Instance.usocketNets.Count - 1;
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
		if (USocketNet.Instance.usocketNets.Count == 0)
			return;

		for(int index = 0; index < USocketNet.Instance.usocketNets.Count; index++)
		{
			USocketNet.Instance.usocketNets[index].GetComponent<Canvas> ().enabled = false;
			foreach(USocketView sockV in USocketNet.Instance.usocketNets[index].localSockets)
			{
				sockV.GetComponent<SimpleControl> ().onControl = false;
			}
		}

		instance = 0;
		USocketNet.Instance.usocketNets[current].GetComponent<Canvas> ().enabled = true;
		foreach(USocketView sockV in USocketNet.Instance.usocketNets[current].localSockets)
		{
			sockV.GetComponent<SimpleControl> ().onControl = false;
		}
		if(USocketNet.Instance.usocketNets[current].localSockets.Count > 0)
		{
			USocketNet.Instance.usocketNets[current].localSockets[instance].GetComponent<SimpleControl> ().onControl = true;
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
			int sockInt = USocketNet.Instance.usocketNets.FindIndex (x => x.GetComponent<Canvas> ().enabled);

			if(sockInt == USocketNet.Instance.usocketNets.Count - 1)
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
			USocketNet.Instance.usocketNets[current].Instantiate (0, Vector3.zero, Quaternion.identity, (Returned rets) => 
				{
					if(rets == Returned.Success)
					{
						if(USocketNet.Instance.usocketNets [current].localSockets.Count == 0)
						{
							instance = 0;
						}

						else
						{
							instance = USocketNet.Instance.usocketNets [current].localSockets.Count - 1;
						}

						SimpleControl uPlayer = null;

						for(int i = 0; i < USocketNet.Instance.usocketNets[current].localSockets.Count; i++)
						{
							uPlayer = USocketNet.Instance.usocketNets[current].localSockets[i].GetComponent<SimpleControl> ();

							if(uPlayer != null)
							{
								uPlayer.onControl = false;
							}

						}
						if(uPlayer != null)
						{
							USocketNet.Instance.usocketNets[current].localSockets[instance].GetComponent<SimpleControl> ().onControl = true;
						}
					}
				}); // spawnPoint.position, spawnPoint.rotation);
		}

		//Switch Instances.
		if(Input.GetKeyDown(KeyCode.F4))
		{
			USocketNet.Instance.usocketNets[current].GetComponent<Canvas> ().enabled = true;

			int playCtrl = USocketNet.Instance.usocketNets[current].localSockets.FindIndex (x => x.GetComponent<SimpleControl> ().onControl == true);

			Debug.Log ("Control: " + playCtrl);

			if(playCtrl < USocketNet.Instance.usocketNets[current].localSockets.Count - 1)
			{
				instance += 1;
			}

			else
			{
				instance = 0;
			}

			for(int i = 0; i < USocketNet.Instance.usocketNets[current].localSockets.Count; i++)
			{
				if(instance == i)
				{
					USocketNet.Instance.usocketNets[current].localSockets[i].GetComponent<SimpleControl> ().onControl = true;
				}

				else
				{
					USocketNet.Instance.usocketNets[current].localSockets[i].GetComponent<SimpleControl> ().onControl = false;
				}
			}
		}
	}

	private int instance = 0;

	*/
}