using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using BytesCrafter.USocketNet;

public class UnityControls : MonoBehaviour//, SocketInterface
{
	[Header("Network Component")]
	public USocketNet netScript = null;
	public DebugViewer debugViewer = null;

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

	[Header("Server Room")]
	public Text pingSocket = null;

	void Update()
	{
		pingSocket.text = netScript.PingServer + " ms";
	}

	//Connecting to server with callbacks.
	public void ConnectToServer()
	{
		netScript.ConnectToServer (username.text, (ConnStat conStat, ConnJson conJson) =>
		{
			debugViewer.Logs ("Callback: " + conStat.ToString() + " ID: " + conJson.identity  + " UN: " + conJson.username + " CN: " + conJson.curcid + " DT: " + conJson.logdate);
		});
	}

	//Disconnecting to server with callbacks.
	public void DisconnectFromServer()
	{
		netScript.DisconnectFromServer ((ConnStat connStat) => 
		{
			debugViewer.Logs ("Callback: " + connStat.ToString());
		});
	}

	//Send a public message on the server.
	public void SendPublicLobbyMessage()
	{
		netScript.SendLobbyMessage (message.text, (MsgStat msgStat, MsgJson msgJson) =>
		{
			debugViewer.Logs(msgStat.ToString() + "  " + msgJson.sender + ": " + msgJson.content);
		});
	}

	//Send a private message on the server.
	public void SendPrivateLobbyMessage()
	{
		netScript.SendPrivateMessage (receiver.text, message.text, (MsgStat msgStat, MsgJson msgJson) =>
		{
			debugViewer.Logs(msgStat.ToString() + "  " + msgJson.sender + ": " + msgJson.content);
		});
	}

	public void SendPublicRoomMessage()
	{
		netScript.SendRoomMessage (messages.text, (MsgStat msgStat, MsgJson msgJson) =>
			{
				debugViewer.Logs(msgStat.ToString() + "  " + msgJson.sender + ": " + msgJson.content);
			});
	}

	public void SendPrivateRoomMessage()
	{
		netScript.SendPrivateMessage (receivers.text, messages.text, (MsgStat msgStat, MsgJson msgJson) =>
			{
				debugViewer.Logs(msgStat.ToString() + "  " + msgJson.sender + ": " + msgJson.content);
			});
	}

	public void CreateServerRoom()
	{
		netScript.CreateServerRoom (roomname.text, "Default", 2, (RoomJson roomJson) => 
			{
				debugViewer.Logs("Room Created: " + " : " + roomJson.cname + " : " + roomJson.maxconnect);
				//plist = new List<UnityPlayer> (); InstantiateThis (netScript.Identity, true);
			});
	}

	public void AutoJoinServerRoom()
	{
		netScript.AutoJoinServerRoom ("Default", 100, (RoomJson roomJson) =>
		{
			debugViewer.Logs("Room Auto: " + " : " + roomJson.cname + " : " + roomJson.maxconnect);
			//plist = new List<UnityPlayer> (); InstantiateThis (netScript.Identity, true);
		});
	}

	public void JoinServerRoom()
	{
		netScript.JoinServerRoom (roomname.text, (RoomJson roomJson) =>
			{
				debugViewer.Logs("Room Joined: " + " : " + roomJson.cname + " : " + roomJson.maxconnect);
				//plist = new List<UnityPlayer> (); 

				//for (int i = 0; i < roomJson.users.Length; i++)
				//{
				//	if (netScript.Identity == roomJson.users [i].identity)
				//	{ InstantiateThis (netScript.Identity, true); }

				//	else { InstantiateThis (roomJson.users [i].identity, false); }
				//}
			});
	}

	public void LeaveServerRoom()
	{
		netScript.LeaveServerRoom ();
	}


	public Transform spawnPoint = null;
	public void SpawnUser()
	{
		netScript.Instantiate (0, spawnPoint.position, spawnPoint.rotation);
	}
}