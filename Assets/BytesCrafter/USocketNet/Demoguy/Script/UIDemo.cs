using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BytesCrafter.USocketNet;
using BytesCrafter.USocketNet.Serializables;

public class UIDemo : MonoBehaviour
{
	public USocketClient netScpt = null;
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
	public MessageDisplay publicViewer = null;

	[Header("PRIVATE MESSAGE")]
	public InputField priMsgContent = null;
	public InputField priMsgReceiver = null;
	public MessageDisplay privateViewer = null;

	[Header("CHANNEL MESSAGE")]
	public InputField chanMsgContent = null;
	public MessageDisplay channelViewer = null;

	[Header("Server Room")]
	public InputField roomname = null;

	[Header("PING MECHANISM")]
	public Text pingSocket = null;
	float timer = 0f;

	public List<Transform> spawnPoint = null;

	public string gameVariant = "Default";

	void Update()
	{
		// pingSocket.text = netScript.PingCount + " ms";
		// //Ping ASD = new Ping ();

		// timer += Time.deltaTime;

		// if(timer > 2f)
		// 	return;

		// if(netScript.localSockets.Count < 10)
		// 	return;
	}

	#region LISTENERS

	void Awake()
	{
		// netScript.ListenConnectionStatus (listenOnConnection);
		// netScript.ListenMessagesEvent (ListenOnMessage);
		// netScript.ListenMatchJoined (ListenOnMatchJoined);
		// netScript.ListenMatchLeaved (listenOnLeaved);
	}

	private void listenOnConnection(ConStat conStat)
	{
		// if(conStat == ConnStat.Connected || conStat == ConnStat.Reconnected)
		// {
		// 	ChangeCanvas(1);
		// }

		// else
		// {
		// 	ChangeCanvas(0);
		// }
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
		netScript.Authenticate (username.text, password.text, (Response response) =>
			{
				if( response.success )
				{
					netScript.ConnectToServer( (ConStat conStat) => {
						publicViewer.Logs("WPID: " + response.data.id + " SNID: " + response.data.session + " Response: " + conStat.ToString());
					});
					
				}
			});
	}

	//Disconnecting to server with callbacks.
	public void DisconnectFromServer()
	{
		// netScript.DisconnectFromServer ((ConnStat connStat) => 
		// 	{
		// 		if(connStat == ConnStat.Disconnected)
		// 		{
		// 			publicViewer.Logs("Disconnected from the server.");
		// 		}
		// 	});
	}

	#endregion

	#region MESSAGINGS
	//Send a public message on the server.
	public void SendPublicMessage()
	{
		// netScript.SendPublicMessage (pubMsgContent.text, (Returned msgStat) =>
		// {
		// 		if(msgStat == Returned.Success)
		// 		{
		// 			publicViewer.Logs("Me: " + pubMsgContent.text);
		// 			pubMsgContent.text = string.Empty;
		// 		}
		// });
	}

	//Send a private message on the server.
	public void SendPrivateMessage()
	{
		// netScript.SendPrivateMessage (priMsgReceiver.text, priMsgContent.text, (Returned msgStat) =>
		// {
		// 	privateViewer.Logs("Me: " + pubMsgContent.text);
		// 	priMsgContent.text = string.Empty;
		// 	priMsgReceiver.text = string.Empty;
		// });
	}

	public void SendChannelMessage()
	{
		// netScript.SendChannelMessage (chanMsgContent.text, (Returned msgStat) =>
		// {
		// 	channelViewer.Logs("Me: " + chanMsgContent.text);
		// 	chanMsgContent.text = string.Empty;
		// });
	}
	#endregion

	#region CHANNELS

	public void AutoJoinServerRoom()
	{
		// netScript.AutoMatchChannel (gameVariant, 10, (MatchRes matchRes, MatchMake matchMake) =>
		// 	{
		// 		if(matchRes == MatchRes.Success)
		// 		{
		// 			ChangeCanvas(2);
		// 			netScript.Instantiate (0, Vector3.zero, Quaternion.identity, null); // spawnPoint.position, spawnPoint.rotation);
		// 		}
		// 	});
	}

	public void CreateServerRoom()
	{
		// netScript.CreateChannel (roomname.text, gameVariant, 2, (MatchRes result, MatchMake matchMake) => 
		// 	{
		// 		if(result == MatchRes.Success)
		// 		{
		// 			ChangeCanvas(2);
		// 			netScript.Instantiate (0, Vector3.zero, Quaternion.identity, null);
		// 		}
		// 	});
	}

	public void JoinServerRoom()
	{
		// netScript.JoinChannel (roomname.text, gameVariant, (MatchRes result, MatchMake matchMake) =>
		// 	{
		// 		if(result == MatchRes.Success)
		// 		{
		// 			ChangeCanvas(2);
		// 			netScript.Instantiate (0, Vector3.zero, Quaternion.identity, null);
		// 		}
		// 	});
	}

	public void LeaveServerRoom()
	{
		// netScript.LeaveChannel ((Returned returned, ChannelJson roomJson) =>
		// 	{
		// 		if(returned == Returned.Success)
		// 		{
		// 			ChangeCanvas(1);
		// 		}
		// 	});
		
	}

	#endregion
}