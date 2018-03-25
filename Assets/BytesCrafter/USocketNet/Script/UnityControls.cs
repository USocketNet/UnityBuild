using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using BytesCrafter.USocketNet;

public class UnityControls : MonoBehaviour//, SocketInterface
{
	private USocketNet netScpt = null;
	public USocketNet netScript
	{
		get {
			if(netScpt == null)
			{
				netScpt = GameObject.FindObjectOfType<USocketNet> ();
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

	[Header("PUBLIC MESSAGE")]
	public InputField pubMsgContent = null;
	public DebugViewer publicViewer = null;

	[Header("PRIVATE MESSAGE")]
	public InputField priMsgContent = null;
	public InputField priMsgReceiver = null;
	public DebugViewer privateViewer = null;

	[Header("CHANNEL MESSAGE")]
	public InputField chanMsgContent = null;
	public DebugViewer channelViewer = null;


	[Header("Server Room")]
	public InputField roomname = null;
	public Transform spawnPoint = null;





	[Header("PING MECHANISM")]
	public Text pingSocket = null; void Update()
	{
		pingSocket.text = netScript.PingCount + " ms";
	}

	void Awake()
	{
		netScript.ListenMessagesEvent (ListenOnMessage);
	}

	private void ListenOnMessage(MsgJson msgJson)
	{
		if(msgJson.msgtype == MsgType.Public)
		{
			publicViewer.Logs(msgJson.sender + ": " + msgJson.content);
		}

		else if(msgJson.msgtype == MsgType.Private)
		{
			privateViewer.Logs(msgJson.sender + ": " + msgJson.content);
		}

		else
		{
			channelViewer.Logs(msgJson.sender + ": " + msgJson.content);
		}
	}

	//Connecting to server with callbacks.
	public void ConnectToServer()
	{
		netScript.ConnectToServer (username.text, (ConnStat conStat, ConnJson conJson) =>
			{
				if(conStat == ConnStat.Connected)
				{
					ChangeCanvas(1);
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
					ChangeCanvas(0);
				}
			});
	}

	#region MESSAGINGS
	//Send a public message on the server.
	public void SendPublicMessage()
	{
		netScript.SendPublicMessage (pubMsgContent.text, (MsgStat msgStat) =>
		{
				if(msgStat == MsgStat.Success)
				{
					publicViewer.Logs("Me: " + pubMsgContent.text);
					pubMsgContent.text = string.Empty;
				}
		});
	}

	//Send a private message on the server.
	public void SendPrivateMessage()
	{
		netScript.SendPrivateMessage (priMsgReceiver.text, priMsgContent.text, (MsgStat msgStat, MsgJson msgJson) =>
		{
			privateViewer.Logs("Me: " + pubMsgContent.text);
			priMsgContent.text = string.Empty;
			priMsgReceiver.text = string.Empty;
		});
	}

	public void SendChannelMessage()
	{
		netScript.SendChannelMessage (chanMsgContent.text, (MsgStat msgStat, MsgJson msgJson) =>
		{
			channelViewer.Logs("Me: " + msgJson.content);
			chanMsgContent.text = string.Empty;
		});
	}
	#endregion

	public void AutoJoinServerRoom()
	{
		netScript.AutoJoinChannel ("Default", 100, (ChannelJson roomJson) =>
			{
				ChangeCanvas(2);
				netScript.Instantiate (0, spawnPoint.position, spawnPoint.rotation);
			});
	}

	public void CreateServerRoom()
	{
		netScript.CreateServerRoom (roomname.text, "Default", 2, (ChannelJson roomJson) => 
			{
				//debugViewer.Logs("Room Created: " + " : " + roomJson.cname + " : " + roomJson.maxconnect);
				//plist = new List<UnityPlayer> (); InstantiateThis (netScript.Identity, true);

				ChangeCanvas(2);
				netScript.Instantiate (0, spawnPoint.position, spawnPoint.rotation);

				//Instatiate All
			});
	}

	public void JoinServerRoom()
	{
		netScript.JoinServerRoom (roomname.text, (ChannelJson roomJson) =>
			{
				//debugViewer.Logs("Room Joined: " + " : " + roomJson.cname + " : " + roomJson.maxconnect);
				//plist = new List<UnityPlayer> (); 

				//for (int i = 0; i < roomJson.users.Length; i++)
				//{
				//	if (netScript.Identity == roomJson.users [i].identity)
				//	{ InstantiateThis (netScript.Identity, true); }

				//	else { InstantiateThis (roomJson.users [i].identity, false); }
				//}

				ChangeCanvas(2);
				netScript.Instantiate (0, spawnPoint.position, spawnPoint.rotation);

				//Instatiate All
			});
	}

	public void LeaveServerRoom()
	{
		netScript.LeaveServerRoom ();
		ChangeCanvas(1);
	}
}