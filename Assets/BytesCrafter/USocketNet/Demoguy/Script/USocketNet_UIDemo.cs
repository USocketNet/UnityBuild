using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BytesCrafter.USocketNet;
using BytesCrafter.USocketNet.Compression;

public class USocketNet_UIDemo : MonoBehaviour
{
	private USocketClient netScpt = null;
	public USocketClient netScript
	{
		get {
			if(netScpt == null)
			{
				netScpt = GameObject.FindObjectOfType<USocketClient> ();
			}

			return netScpt;
		}
	}

	[Header("CANVAS DISPLAYS")]
	public List<CanvasGroup> canvasGroup = new List<CanvasGroup>();
	public void ChangeCanvas(int index)
	{
		canvasGroup.ForEach ((CanvasGroup cgroup) => {
			cgroup.alpha = 0f;
			cgroup.gameObject.SetActive(false);
		});

		canvasGroup [index].alpha = 1f;
		canvasGroup [index].gameObject.SetActive (true);
	}

	[Header("Connections")]
	public InputField username = null;
	public InputField password = null;

	[Header("PUBLIC MESSAGE")]
	public InputField pubMsgContent = null;
	public USocketNet_MsgLog publicViewer = null;

	[Header("PRIVATE MESSAGE")]
	public InputField priMsgContent = null;
	public InputField priMsgReceiver = null;
	public USocketNet_MsgLog privateViewer = null;

	[Header("CHANNEL MESSAGE")]
	public InputField chanMsgContent = null;
	public USocketNet_MsgLog channelViewer = null;

	[Header("Server Room")]
	public InputField roomname = null;

	[Header("PING MECHANISM")]
	public Text pingSocket = null;
	float timer = 0f;

	public List<Transform> spawnPoint = null;

	public string gameVariant = "Default";

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.F5))
		{
			netScript.localSockets.ForEach ((USocketView viewss) => {

				if(viewss.IsLocalUser)
				{
					TankHealth thealth = viewss.GetComponent<TankHealth>();
					if(viewss != null)
					{
						if(thealth.m_Dead)
						{
							int spawning = Random.Range (0, spawnPoint.Count);
							viewss.transform.position = spawnPoint[0].position;

							viewss.TriggerEvents ("Revived", "", (Returned returned) => {
								if(returned == Returned.Success)
								{
									thealth.Revived ();
								}
							});

						}

					}
				}
			});
		}

		pingSocket.text = netScript.PingCount + " ms";
		//Ping ASD = new Ping ();

		timer += Time.deltaTime;

		if(timer > 2f)
			return;

		if(netScript.localSockets.Count < 10)
			return;
	}

	#region LISTENERS

	void Awake()
	{
		netScript.ListenConnectionStatus (listenOnConnection);
		netScript.ListenMessagesEvent (ListenOnMessage);
		netScript.ListenMatchJoined (ListenOnMatchJoined);
		netScript.ListenMatchLeaved (listenOnLeaved);
	}

	private void listenOnConnection(ConnStat conStat)
	{
		if(conStat == ConnStat.Connected || conStat == ConnStat.Reconnected)
		{
			ChangeCanvas(1);
		}

		else
		{
			ChangeCanvas(0);
		}
	}

	private void ListenOnMessage(MsgJson msgJson)
	{
		if(msgJson.mt == MsgType.Public)
		{
			publicViewer.Logs(msgJson.sd + ": " + msgJson.ct);
		}

		else if(msgJson.mt == MsgType.Private)
		{
			privateViewer.Logs(msgJson.sd + ": " + msgJson.ct);
		}

		else
		{
			channelViewer.Logs(msgJson.sd + ": " + msgJson.ct);
		}
	}

	private void ListenOnMatchJoined(PeerJson peerJson)
	{
		channelViewer.Logs(peerJson.id + " had joined.");
	}

	private void listenOnLeaved(PeerJson peerJson)
	{
		channelViewer.Logs(peerJson.id + " had leaved.");
	}

	#endregion

	#region CONNECTION

	//Connecting to server with callbacks.
	public void ConnectToServer()
	{
		netScript.ConnectToServer (username.text, password.text, (ConnStat conStat, ConnAuth connAuth) =>
			{
				if(conStat == ConnStat.Connected && connAuth == ConnAuth.Success)
				{
					publicViewer.Logs("Connected with id: " + netScpt.Identity);
				}
			});
	}

	//Disconnecting to server with callbacks.
	public void DisconnectFromServer()
	{
		netScript.DisconnectFromServer ((ConnStat connStat) => 
			{
				if(connStat == ConnStat.Disconnected)
				{
					publicViewer.Logs("Disconnected from the server.");
				}
			});
	}

	#endregion

	#region MESSAGINGS
	//Send a public message on the server.
	public void SendPublicMessage()
	{
		netScript.SendPublicMessage (pubMsgContent.text, (Returned msgStat) =>
		{
				if(msgStat == Returned.Success)
				{
					publicViewer.Logs("Me: " + pubMsgContent.text);
					pubMsgContent.text = string.Empty;
				}
		});
	}

	//Send a private message on the server.
	public void SendPrivateMessage()
	{
		netScript.SendPrivateMessage (priMsgReceiver.text, priMsgContent.text, (Returned msgStat) =>
		{
			privateViewer.Logs("Me: " + pubMsgContent.text);
			priMsgContent.text = string.Empty;
			priMsgReceiver.text = string.Empty;
		});
	}

	public void SendChannelMessage()
	{
		netScript.SendChannelMessage (chanMsgContent.text, (Returned msgStat) =>
		{
			channelViewer.Logs("Me: " + chanMsgContent.text);
			chanMsgContent.text = string.Empty;
		});
	}
	#endregion

	#region CHANNELS

	public void AutoJoinServerRoom()
	{
		netScript.AutoMatchChannel (gameVariant, 10, (MatchRes matchRes, MatchMake matchMake) =>
			{
				if(matchRes == MatchRes.Success)
				{
					ChangeCanvas(2);
					netScript.Instantiate (0, Vector3.zero, Quaternion.identity, null); // spawnPoint.position, spawnPoint.rotation);
				}
			});
	}

	public void CreateServerRoom()
	{
		netScript.CreateChannel (roomname.text, gameVariant, 2, (Returned returned, ChannelJson roomJson) => 
			{
				if(returned == Returned.Success)
				{
					ChangeCanvas(2);
					netScript.Instantiate (0, Vector3.zero, Quaternion.identity, null);
				}
			});
	}

	public void JoinServerRoom()
	{
		netScript.JoinChannel (roomname.text, gameVariant, (Returned returned, ChannelJson roomJson) =>
			{
				if(returned == Returned.Success)
				{
					ChangeCanvas(2);
					netScript.Instantiate (0, Vector3.zero, Quaternion.identity, null);
				}
			});
	}

	public void LeaveServerRoom()
	{
		netScript.LeaveChannel ((Returned returned, ChannelJson roomJson) =>
			{
				if(returned == Returned.Success)
				{
					ChangeCanvas(1);
				}
			});
		
	}

	#endregion
}