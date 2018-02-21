using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using BytesCrafter.Multiplayer;

public class UnityControls : MonoBehaviour//, SocketInterface
{
	[Header("Network Component")]
	public UnitedMultiplayer netScript = null;
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
	public Text pingWebs = null;
	private float timer = 0f;

	void Update()
	{
		if(netScript.isConnect)
		{
			if(timer > 0f)
			{
				timer -= Time.deltaTime;
			}

			else
			{
				timer = 3f;
				//Ping here
				netScript.CheckPing(OnPingReceived);
				StartCoroutine (PingTest());
			}
		}
	}

	IEnumerator PingTest()
	{
		Ping pingtest = new Ping ("112.205.155.231");
		yield return pingtest.isDone;
		pingWebs.text = pingtest.time + "ms";
	}

	private void OnPingReceived(PingJson pingJson)
	{
		System.TimeSpan timeSpan = DateTime.Now.Subtract (Convert.ToDateTime (pingJson.datetime));
		//System.TimeSpan timeSpan = System.DateTime.Now.Subtract (System.Convert.ToDateTime (pingJson.datetime));
		pingSocket.text = timeSpan.Milliseconds + "ms";
	}

	void Start()
	{
		netScript.ListenConnectionStatus (ListeningConnectionStats);
		SocketCallback.Access.unityCtrl = this;
	}

	//This will receive a callback for ecery connection changes.
	private void ListeningConnectionStats(ConnStat connStat, ConnJson connJson)
	{
		debugViewer.Logs ("Connection Status: " + connStat.ToString());
	}

	//Connecting to server with callbacks.
	public void ConnectToServer()
	{
		netScript.ConnectToServer (username.text, OnConnectServer);
	}

	private void OnConnectServer(ConnRes connRes, ConnJson connJson)
	{
		debugViewer.Logs (connRes.ToString() + " : " + connJson.success + " : " + connJson.connIdentity);
	}

	//Disconnecting to server with callbacks.
	public void DisconnectFromServer()
	{
		netScript.DisconnectFromServer (OnDisconnectServer);
	}

	private void OnDisconnectServer(ConnRes connRes)
	{
		debugViewer.Logs ("Disconnection: " + connRes.ToString());
	}

	//Send a public message on the server.
	public void SendPublicLobbyMessage()
	{
		netScript.SendPublicLobbyMessage (message.text, OnPublicLobbyMessage);
	}

	private void OnPublicLobbyMessage(MsgRes msgRes, MsgJson msgJson)
	{
		debugViewer.Logs(msgRes.ToString() + ": " + msgJson.result);
	}

	//Send a private message on the server.
	public void SendPrivateLobbyMessage()
	{
		netScript.SendPrivateLobbyMessage (receiver.text, message.text, OnPrivateLobbyMessage);
	}

	private void OnPrivateLobbyMessage(MsgRes msgRes, MsgJson msgJson)
	{
		debugViewer.Logs(msgRes.ToString() + ": " + msgJson.result);
	}

	public void CreateServerRoom()
	{
		netScript.CreateServerRoom (roomname.text, OnCreatedRoom);
	}

	private void OnCreatedRoom(RoomJson roomJson)
	{
		debugViewer.Logs("CREATED: " + roomJson.result + " : " + roomJson.roomname);
		UnityPlayer curPlayer = Instantiate (prefab, spawnpoint).GetComponent<UnityPlayer> ();
		curPlayer.transform.position = spawnpoint.position;
		curPlayer.Initialized (netScript.UserIdentity, true, netScript); plist = new List<UnityPlayer> (); plist.Add (curPlayer);
	}
		
	public void JoinServerRoom()
	{
		netScript.JoinServerRoom (roomname.text, OnJoinedRoom);
	}

	private void OnJoinedRoom(RoomJson roomJson)
	{
		debugViewer.Logs("JOINED: " + roomJson.result + " : " + roomJson.roomname);
		UnityPlayer curPlayer = Instantiate (prefab, spawnpoint).GetComponent<UnityPlayer> ();
		curPlayer.transform.position = spawnpoint.position;
		curPlayer.Initialized (netScript.UserIdentity, true, netScript); plist = new List<UnityPlayer> (); plist.Add (curPlayer);

		for (int i = 0; i < roomJson.participants.Length; i++)
		{
			if(plist.Find(x => x.identity == roomJson.participants[i].identity) == null)
			{
				if (netScript.UserIdentity != roomJson.participants [i].identity)
				{
					UnityPlayer newPlayer = Instantiate (prefab.transform, spawnpoint).GetComponent<UnityPlayer> ();
					newPlayer.transform.position = spawnpoint.position;
					newPlayer.Initialized (roomJson.participants [i].identity, false, netScript);
					plist.Add (newPlayer);
					Debug.Log (newPlayer.identity);
				}
			}
		}
	}

	public void OnJoiningUser(RoomJson roomJson)
	{
		for (int i = 0; i < roomJson.participants.Length; i++)
		{
			if(plist.Find(x => x.identity == roomJson.participants[i].identity) == null)
			{
				if(netScript.UserIdentity != roomJson.participants[i].identity)
				{
					UnityPlayer newPlayer = Instantiate (prefab.transform, spawnpoint).GetComponent<UnityPlayer>();
					newPlayer.transform.position = spawnpoint.position;
					newPlayer.Initialized (roomJson.participants [i].identity, false, netScript);
					plist.Add (newPlayer);
					Debug.Log (newPlayer.identity);
				}
			}
		}
	}

	public void LeaveServerRoom()
	{
		netScript.LeaveServerRoom ();
	}




	public void SendPublicRoomMessage()
	{
		netScript.SendPublicRoomMessage (messages.text, room.text);
	}

	public void SendPrivateRoomMessage()
	{
		netScript.SendPrivateRoomMessage (messages.text, receivers.text, room.text);
	}

	[Header("Player Prefab")]
	public Transform prefab = null;
	public Transform spawnpoint = null;
	public List<UnityPlayer> plist = null; //List of Participant in a room.

	public void OnRoomUpdate(PosJson posJson)
	{
		UnityPlayer uPlayer = plist.Find (x => x.identity == posJson.identity);

		if(uPlayer != null)
		{
			uPlayer.transform.position = PosJson.ToVector3 (posJson);
			uPlayer.transform.rotation = PosJson.ToQuaternion (posJson);
		}
	}
}