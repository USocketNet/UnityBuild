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

	[Header("Communication - Lobby")]
	public InputField receiver = null;
	public InputField message = null;

	[Header("Communication - Room")]
	public InputField room = null;
	public InputField receivers = null;
	public InputField messages = null;

	[Header("Server Room")]
	public InputField roomname = null;
	public Transform spawnPoint = null;

	public DebugViewer debugViewer = null;

	[Header("Server Room")]
	public Text pingSocket = null;

	void Update()
	{
		pingSocket.text = netScript.PingCount + " ms";
	}

	//Connecting to server with callbacks.
	public void ConnectToServer()
	{
		if(username.text != string.Empty)
		{
			netScript.ConnectToServer (username.text, (ConnStat conStat, ConnJson conJson) =>
				{
					if(conStat == ConnStat.Connected)
					{
						ChangeCanvas(1);
					}
				});
		}
	}

	//Disconnecting to server with callbacks.
	public void DisconnectFromServer()
	{
		netScript.DisconnectFromServer ((ConnStat connStat) => 
			{
				ChangeCanvas(0);
			});
	}

	#region MESSAGINGS
	//Send a public message on the server.
	public void SendPublicLobbyMessage()
	{
		netScript.SendPublicMessage (message.text, (MsgStat msgStat, MsgJson msgJson) =>
		{
			debugViewer.Logs(msgJson.sender + ": " + msgJson.content);
		});
	}

	//Send a private message on the server.
	public void SendPrivateLobbyMessage()
	{
		netScript.SendPrivateMessage (receiver.text, message.text, (MsgStat msgStat, MsgJson msgJson) =>
		{
			debugViewer.Logs(msgJson.sender + ": " + msgJson.content);
		});
	}

	public void SendPublicRoomMessage()
	{
		netScript.SendChannelMessage (messages.text, (MsgStat msgStat, MsgJson msgJson) =>
		{
			debugViewer.Logs(msgJson.sender + ": " + msgJson.content);
		});
	}
	#endregion

	public void AutoJoinServerRoom()
	{
		netScript.AutoJoinChannel ("Default", 100, (RoomJson roomJson) =>
			{
				//debugViewer.Logs("Room Auto: " + " : " + roomJson.cname + " : " + roomJson.maxconnect);
				//plist = new List<UnityPlayer> (); InstantiateThis (netScript.Identity, true);

				ChangeCanvas(2);
				netScript.Instantiate (0, spawnPoint.position, spawnPoint.rotation);

				//Instatiate All
			});
	}

	public void CreateServerRoom()
	{
		netScript.CreateServerRoom (roomname.text, "Default", 2, (RoomJson roomJson) => 
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
		netScript.JoinServerRoom (roomname.text, (RoomJson roomJson) =>
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