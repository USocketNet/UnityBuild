﻿using System; 
using System.Threading; 
using System.Collections; 
using System.Collections.Generic;

using SocketIO;
using WebSocketSharp;
using UnityEngine;

using BytesCrafter.USocketNet;

namespace BytesCrafter.USocketNet
{
	public class USocketNet : MonoBehaviour
	{
		#region ExposedInterface - Ongoing!

		[Header("NETWORK SETTINGS")]
		public Bindings bindings = new Bindings();

		[Header("INSTANCE PREFAB")]
		public List<USocketView> socketPrefabs = new List<USocketView>();
		private List<USocketView> socketIdentities = new List<USocketView>();

		#endregion

		#region CheckerInterface - Done!

		/// <summary>
		/// Determines whether this client is connected to web socket server.
		/// </summary>
		/// <returns><c>true</c> if this client is connected; otherwise, <c>false</c>.</returns>
		public bool IsConnected
		{
			get { return threadset.IsConnected; }
		}

		/// <summary>
		/// Sockets identity of the current user's connection to the server.
		/// </summary>
		/// <returns> Socket Identity, Not Connected, No Internet Access. </returns>
		public string Identity
		{
			get
			{
				if (threadset.IsConnected) { return threadset.socketId; }
				else { return "Not Connected"; }
			}
			//get{ if(Internet) { if(threadset.IsConnected) { return threadset.socketId; } 
			//else{ return "Not Connected"; } } else { return "No Internet Access"; } }
		}

		/// <summary>
		/// Gets the current ping count in ms or millisecond.
		/// </summary>
		/// <value>The ping count in ms.</value>
		public int PingCount
		{
			get
			{
				return pingValue;
			}
		}
		private int pingValue = 0;
		private DateTime lastPing = DateTime.Now;
		private float timer = 0f;
		private void Pinging()
		{
			if (threadset.IsConnected)
			{
				if (timer > 0f)
				{
					timer -= Time.deltaTime;
				}

				else
				{
					lastPing = DateTime.Now;
					SendEmit("sping", OnPingReceived);
				}
			}
		}
		private void OnPingReceived(JSONObject jsonObject)
		{
			TimeSpan timeSpan = DateTime.Now.Subtract(lastPing);
			pingValue = timeSpan.Milliseconds;
			timer = bindings.pingFrequency;
		}

		enum Debugs { Log, Warn, Error }
		private void DebugLog(Debugs debugs, string title, string details)
		{
			if(debugs == Debugs.Warn)
			{
				Debug.LogWarning(title + " - " + details);
			}

			else if(debugs == Debugs.Error)
			{
				Debug.LogError(title + " - " + details);
			}

			else
			{
				Debug.Log(title + " - " + details);
			}
		}

		#endregion

		#region ConnectServer - Done!

		private bool connectReturn = false;
		private Action<ConnStat, ConnJson> connectCallback = null;

		public void ConnectToServer(string username, Action<ConnStat, ConnJson> connCallback)
		{
			connectCallback = connCallback; connectReturn = false;

			if (!threadset.IsConnected)
			{
				threadset.IsConnected = true; threadset.autoConnect = false;
				socketThread = new Thread(RunSocketThread); socketThread.Start(threadset.websocket);
				pingThread = new Thread(RunPingThread); pingThread.Start(threadset.websocket);
				StartCoroutine(ConnectingToServer(username));
			}

			else
			{
				connectCallback(ConnStat.Connected, new ConnJson());
				DebugLog(Debugs.Error, "ConnectionFailed", "Already connected to the server!");
			}
		}

		private string lastUsername = string.Empty;
		IEnumerator ConnectingToServer(string username)
		{
			lastUsername = username;
			yield return IsConnected;
			yield return new WaitForSeconds(bindings.connectDelay);

			if (threadset.websocket.IsConnected)
			{
				UserJson userJson = new UserJson(bindings.authenKey, username);
				string sendData = JsonUtility.ToJson(userJson);
				SendEmit("connect", new JSONObject(sendData), OnServerConnect);

				yield return new WaitForSeconds(bindings.connectDelay);

				if (!connectReturn)
				{
					connectCallback(ConnStat.InternetAccess, new ConnJson());
					DebugLog(Debugs.Error, "ConnectionNoInternet", "Detected no internet connection!");
				}
			}

			else
			{
				ForceDisconnect();
				connectCallback(ConnStat.Maintainance, new ConnJson());
				DebugLog(Debugs.Error, "ConnectionMaintainance", "Server seems to be shutdown or offline!");
			}
		}

		private void OnServerConnect(JSONObject jsonObject)
		{
			StopCoroutine("ConnectingToServer");
			connectReturn = true;

			ConnJson connJson = JsonSerializer.ToObject<ConnJson>(jsonObject.ToString());
			lastUsername = connJson.username;
			connectCallback(ConnStat.Connected, connJson);
		}

		#endregion

		#region DisconnectServer - Done!

		private bool disconnectReturn = false;
		private Action<ConnStat> disconnectCallback = null;

		public void DisconnectFromServer(Action<ConnStat> disconCallback)
		{
			disconnectCallback = disconCallback;
			disconnectReturn = false;

			if (threadset.websocket.IsConnected)
			{
				StartCoroutine(DisconnectingToServer());
			}

			else
			{
				disconnectCallback(ConnStat.Disconnected);
				DebugLog (Debugs.Warn, "CantDisconnect", "You are not connected to server!");
			}
		}

		IEnumerator DisconnectingToServer()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!disconnectReturn)
			{
				ForceDisconnect();
				disconnectCallback(ConnStat.Disconnected);
			}
		}

		private void OnServerDisconnect(JSONObject jsonObject)
		{
			StopCoroutine("DisconnectingToServer");
			disconnectReturn = true;

			ForceDisconnect();
			disconnectCallback(ConnStat.Disconnected);
			pingValue = 0;
		}

		#endregion

		#region ReconnectServer - Done!

		private Action<ConnStat, ConnJson> connectionStats = null;
		private bool onListeningReturn = false;

		public void ListenConnectionStatus(Action<ConnStat, ConnJson> connectionStatus)
		{
			connectionStats = connectionStatus;
		}

		IEnumerator OnListeningConnectionStatus(ConnStat connStat)
		{
			onListeningReturn = false;

			if (connStat == ConnStat.Reconnected)
			{
				UserJson userJson = new UserJson(bindings.authenKey, lastUsername);
				string sendData = JsonUtility.ToJson(userJson);
				SendEmit("reconnect", new JSONObject(sendData), OnServerReconnect);
			}

			yield return new WaitForSeconds(bindings.connectDelay);

			if (!onListeningReturn)
			{
				if (connectionStats != null)
				{
					connectionStats(connStat, null);
				}
			}

			DebugLog (Debugs.Warn, "ConnectionStatus", "You are currently " + connStat.ToString() + " to the server!");
		}

		private void OnServerReconnect(JSONObject jsonObject)
		{
			onListeningReturn = true;
			ConnJson connJson = JsonSerializer.ToObject<ConnJson>(jsonObject.ToString());

			if (connectionStats != null)
			{
				connectionStats(ConnStat.Reconnected, connJson);
			}
		}

		#endregion

		#region PublicMessage - Done!

		private bool publicReturned = false;
		private Action<MsgStat> publicCallback = null;

		public void SendPublicMessage(string message, Action<MsgStat> _publicCallback)
		{
			publicCallback = _publicCallback;
			publicReturned = false;

			if (threadset.websocket.IsConnected)
			{
				MsgJson msgJson = new MsgJson(message, MsgType.Public);
				string sendData = JsonUtility.ToJson(msgJson);
				SendEmit("message", new JSONObject(sendData), OnPublicMessageSent);
				StartCoroutine(SendingPublicMessage());
			}

			else
			{
				MsgJson msgJson = new MsgJson();
				publicCallback(MsgStat.Failed);
				DebugLog (Debugs.Error, "PubMsgFailed", "You are currently disconnected!");
			}
		}

		IEnumerator SendingPublicMessage()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!publicReturned)
			{
				MsgJson msgJson = new MsgJson();
				if (!threadset.websocket.IsConnected)
				{
					publicCallback(MsgStat.Failed);
					DebugLog (Debugs.Error, "PubMsgFailed", "You are currently disconnected!");
				}
			}
		}

		private void OnPublicMessageSent(JSONObject jsonObject)
		{
			StopCoroutine ("SendingLobbyMessage");

			MsgCallback msgJson = JsonSerializer.ToObject<MsgCallback>(jsonObject.ToString());
			publicReturned = true;
			publicCallback(MsgStat.Success);

			DebugLog (Debugs.Log, "PubMsgSuccess", "Successfully sent a lobby message!");
		}

		#endregion

		#region PrivateMessage - Done!

		private bool privateReturned = false;
		private Action<MsgStat, MsgJson> privateMessage = null;

		public void SendPrivateMessage(string receiver, string message, Action<MsgStat, MsgJson> _privateMessage)
		{
			privateMessage = _privateMessage; privateReturned = false;

			if (threadset.websocket.IsConnected)
			{
				MsgJson msgJson = new MsgJson(message, MsgType.Private, receiver);
				string sendData = JsonUtility.ToJson(msgJson);
				SendEmit("message", new JSONObject(sendData), OnPrivateMessageSent);
				StartCoroutine(SendingPrivateMessage());
			}

			else
			{
				MsgJson msgJson = new MsgJson();
				privateMessage(MsgStat.Failed, msgJson);
				DebugLog (Debugs.Error, "PrivateMsgFailed", "You are currently disconnected to the server!");
			}
		}

		IEnumerator SendingPrivateMessage()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!privateReturned)
			{
				MsgJson msgJson = new MsgJson();

				if (!threadset.websocket.IsConnected)
				{
					privateMessage(MsgStat.Failed, msgJson);
					DebugLog (Debugs.Error, "PrivateMsgFailed", "You are currently disconnected to the server!");
				}
			}
		}

		private void OnPrivateMessageSent(JSONObject jsonObject)
		{
			MsgJson msgJson = JsonSerializer.ToObject<MsgJson>(jsonObject.ToString());
			privateReturned = true;
			privateMessage(MsgStat.Failed, msgJson);

			DebugLog (Debugs.Log, "PrivateMsgSuccess", "Successfully sent a private message!");
		}

		#endregion

		#region ChannelMessage - Done!

		private bool channelReturned = false;
		private Action<MsgStat, MsgJson> channelCallback = null;

		public void SendChannelMessage(string message, Action<MsgStat, MsgJson> _channelCallback)
		{
			channelCallback = _channelCallback;
			channelReturned = false;

			if (threadset.websocket.IsConnected)
			{
				MsgJson msgJson = new MsgJson(message, MsgType.Channel);
				string sendData = JsonUtility.ToJson(msgJson);
				SendEmit("message", new JSONObject(sendData), OnChannelMessageSent);
				StartCoroutine(SendingChannelMessage());
			}

			else
			{
				MsgJson msgJson = new MsgJson();
				channelCallback(MsgStat.Failed, msgJson);
				DebugLog (Debugs.Error, "ChannelMsgFailed", "You are currently disconnected to the server!");
			}
		}

		IEnumerator SendingChannelMessage()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!channelReturned)
			{
				MsgJson msgJson = new MsgJson();

				if (!threadset.websocket.IsConnected)
				{
					channelCallback(MsgStat.Failed, msgJson);
					DebugLog (Debugs.Error, "ChannelMsgFailed", "You are currently disconnected to the server!");
				}
			}
		}

		private void OnChannelMessageSent(JSONObject jsonObject)
		{
			MsgJson msgJson = JsonSerializer.ToObject<MsgJson>(jsonObject.ToString());
			channelReturned = true;
			channelCallback(MsgStat.Success, msgJson);

			DebugLog (Debugs.Log, "ChannelMsgSuccess", "Successfully sent a channel message!");
		}

		#endregion

		#region ListenerMessage - Done!

		private MsgListener messageCallback = null;

		public void ListenMessagesEvent(MsgListener _messageCallback)
		{
			messageCallback = _messageCallback;
		}

		private void OnMessageReceived(SocketIOEvent _eventArgs)
		{
			MsgJson msgJson = JsonUtility.FromJson<MsgJson>(_eventArgs.data.ToString());
			DebugLog(Debugs.Log, "OnEvent: MESSAGE", "Sender: " + msgJson.sender + " Type: " + msgJson.content);

			if (messageCallback != null)
			{
				messageCallback(msgJson);
			}
		}

		#endregion



		#region AutoChannel - OnGoing!

		private bool autoReturned = false;
		private Action<ChannelJson> autoChannelCallback = null;

		public void AutoJoinChannel(string variant, int maxconnect, Action<ChannelJson> autoCallback)
		{
			autoChannelCallback = autoCallback;
			autoReturned = false;

			if (threadset.websocket.IsConnected)
			{
				ChannelJson channelJson = new ChannelJson(variant, maxconnect);
				string sendData = JsonUtility.ToJson(channelJson);
				SendEmit("auto", new JSONObject(sendData), OnAutoServerChannel);
				StartCoroutine(AutoServerChannel(channelJson));
			}

			else
			{
				DebugLog (Debugs.Error, "AutoChannelFailed", "You are currently disconnected to the server!");
			}
		}

		IEnumerator AutoServerChannel(ChannelJson channelJson)
		{
			yield return new WaitForEndOfFrame();

			if (!autoReturned)
			{
				if (!threadset.websocket.IsConnected)
				{
					autoChannelCallback(channelJson);
					DebugLog (Debugs.Error, "AutoChannelFailed", "You are currently disconnected to the server!");
				}
			}
		}

		private void OnAutoServerChannel(JSONObject jsonObject)
		{
			ChannelJson channelJson = JsonSerializer.ToObject<ChannelJson>(jsonObject.ToString());
			autoReturned = true;
			autoChannelCallback(channelJson);

			DebugLog (Debugs.Log, "AutoChannelSuccess", "Successfully auto match a channel!");
		}

		#endregion

		#region CreateChannel - OnGoing!

		private Action<ChannelJson> createCallback = null; private bool createReturned = false;
		public void CreateServerRoom(string channelName, string variant, int maxconnect, Action<ChannelJson> _createCallback)
		{
			createCallback = _createCallback; createReturned = false;

			if (threadset.websocket.IsConnected)
			{
				ChannelJson channelJson = new ChannelJson(channelName, variant, maxconnect);
				string sendData = JsonUtility.ToJson(channelJson);
				SendEmit("create", new JSONObject(sendData), OnCreatedServerRoom);
				StartCoroutine(CreatingServerRoom(channelJson));
			}

			else
			{
				DebugLog (Debugs.Error, "CreateChannelFailed", "You are currently disconnected to the server!");
			}
		}

		IEnumerator CreatingServerRoom(ChannelJson channelJson)
		{
			yield return new WaitForEndOfFrame(); //yield return new WaitForSeconds (approaches.callbackWait);
			if (!createReturned)
			{
				if (!threadset.websocket.IsConnected)
				{
					createCallback(channelJson);
					DebugLog (Debugs.Error, "CreateChannelFailed", "You are currently disconnected to the server!");
				}
			}
		}

		private void OnCreatedServerRoom(JSONObject jsonObject)
		{
			ChannelJson channelJson = JsonSerializer.ToObject<ChannelJson>(jsonObject.ToString());
			createReturned = true;
			createCallback(channelJson);

			DebugLog (Debugs.Log, "CreateChannelSuccess", "Successfully create match a channel!");
		}

		#endregion

		#region JoinChannel - OnGoing!

		private Action<ChannelJson> joinCallback = null;
		private bool joinReturned = false;

		public void JoinServerRoom(string channelId, Action<ChannelJson> _joinCallback)
		{
			joinCallback = _joinCallback; joinReturned = false;

			if (threadset.websocket.IsConnected)
			{
				ChannelJson channelJson = new ChannelJson(channelId);
				string sendData = JsonUtility.ToJson(channelJson);
				SendEmit("join", new JSONObject(sendData), OnJoinedServerRoom);
				StartCoroutine(JoiningServerRoom(channelJson));
			}

			else
			{
				DebugLog (Debugs.Error, "JoinChannelFailed", "You are currently disconnected to the server!");
			}
		}

		IEnumerator JoiningServerRoom(ChannelJson channelJson)
		{
			yield return new WaitForEndOfFrame();

			if (!joinReturned)
			{
				if (joinCallback != null)
				{
					joinCallback(null);
					DebugLog (Debugs.Error, "JoinChannelFailed", "You are currently disconnected to the server!");
				}
			}
		}

		private void OnJoinedServerRoom(JSONObject jsonObject)
		{
			ChannelJson channelJson = JsonSerializer.ToObject<ChannelJson>(jsonObject.ToString());
			joinReturned = true;
			joinCallback(channelJson);

			DebugLog (Debugs.Log, "JoinChannelSuccess", "Successfully join match a channel!");
		}

		#endregion

		#region LeaveChannel - OnGoing!

		public void LeaveServerRoom()
		{
			if (threadset.websocket.IsConnected)
			{
				SendEmit("leave", OnLeaveServerRoom);
			}

			else
			{
				DebugLog(Debugs.Error, "LeaveMatchFailed", "You are not connected to the server!");
			}
		}

		#endregion



		#region OngoingCodes // ----------------------------------------- SENDING TRANSFORM ROOM VALUE! --------------------------------------- //

		public void Instantiate(int prefabIndex, Vector3 position, Quaternion rotation)
		{
			Instances intance = new Instances(prefabIndex, position, rotation.eulerAngles);
			string sendData = JsonUtility.ToJson(intance);
			SendEmit("instance", new JSONObject(sendData), OnInstantiate);
		}

		private void OnInstantiate(JSONObject jsonObject)
		{
			Instances instances = JsonSerializer.ToObject<Instances>(jsonObject.ToString());
			USocketView curUser = Instantiate(socketPrefabs[instances.prefabIndex], instances.pos.ToVector, instances.rot.ToQuaternion);
			curUser.IsLocalUser = true;
			curUser.socketId = instances.identity;
			curUser.uSocketNet = this;
			socketIdentities.Add(curUser);
			DebugLog(Debugs.Log, "StartedMatchSuccess", "You successfully started a match!");
		}

		private void Synching()
		{
			socketIdentities.ForEach ((USocketView local) => 
			{
					if (local.socketId == string.Empty)
						return;

					if(local.IsLocalUser) //LocalSync on Uplink
					{
						//Send data as transform.
						StateJson stateJson = new StateJson ();
						stateJson.states.AddRange (new string[4] { "f", "f", "f", "f" }); //pos, rot, sca, sta

						if (local.position.synchronize)
						{
							local.position.sendTimer += Time.deltaTime;

							if (local.position.sendTimer >= (1f / local.position.sendRate))
							{
								stateJson.states [0] = "t";
								stateJson.states.Add (TVector.ToVectorStr (local.transform.position));
								local.scale.sendTimer = 0f;
							}
						}

						if (local.rotation.synchronize)
						{
							local.rotation.sendTimer += Time.deltaTime;

							if (local.rotation.sendTimer >= (1f / local.rotation.sendRate))
							{
								stateJson.states [1] = "t";
								stateJson.states.Add (TVector.ToVectorStr (local.transform.rotation.eulerAngles));
								local.rotation.sendTimer = 0f;
							}
						}

						if (local.scale.synchronize)
						{
							local.scale.sendTimer += Time.deltaTime;

							if (local.scale.sendTimer >= (1f / local.scale.sendRate))
							{
								stateJson.states [2] = "t";
								stateJson.states.Add (TVector.ToVectorStr (local.transform.lossyScale));
								local.scale.sendTimer = 0f;
							}
						}

						if (local.states.synchronize)
						{
							local.states.sendTimer += Time.deltaTime;

							if (local.states.sendTimer >= (1f / local.states.sendRate))
							{
								stateJson.states [3] = "t";
								stateJson.states.AddRange (local.states.syncValue.ToArray ());
								local.states.sendTimer = 0f;
							}
						}

						//Note: First check if values is same then dont send.
						string sendData = JsonUtility.ToJson (stateJson);
						SendEmit ("transtate", new JSONObject (sendData));
					}

					else //LocalSync on DownLink
					{
						if (local.position.synchronize)
						{
							if (local.position.syncMode == SocketSync.Realtime)
							{
								local.transform.position = local.targetPos;
							}

							else if (local.position.syncMode == SocketSync.AdjustToward)
							{
								float pDamp = Vector3.Distance(local.transform.position, local.targetPos);
								local.transform.position = Vector3.MoveTowards(local.transform.position, local.targetPos, (pDamp + local.posInterpolation) * local.posSpeed * Time.deltaTime);
							}

							else if (local.position.syncMode == SocketSync.LerpValues)
							{
								float pDamp = Vector3.Distance(local.transform.position, local.targetPos);
								local.transform.position = Vector3.Lerp(local.transform.position, local.targetPos, (pDamp + local.posInterpolation) * local.posSpeed * Time.deltaTime);
							}
						}

						if (local.rotation.synchronize)
						{
							if (local.rotation.syncMode == SocketSync.Realtime)
							{
								local.transform.rotation = Quaternion.Euler(local.targetRot);
							}

							else if (local.rotation.syncMode == SocketSync.AdjustToward)
							{
								float rDamp = Quaternion.Angle(local.transform.rotation, Quaternion.Euler(local.targetRot));
								local.transform.rotation = Quaternion.RotateTowards(local.transform.rotation, Quaternion.Euler(local.targetRot), (rDamp + local.rotInterpolation) * local.rotSpeed * Time.deltaTime);
							}

							else if (local.rotation.syncMode == SocketSync.LerpValues)
							{
								float rDamp = Quaternion.Angle(local.transform.rotation, Quaternion.Euler(local.targetRot));
								local.transform.rotation = Quaternion.Lerp(local.transform.rotation, Quaternion.Euler(local.targetRot), (rDamp + local.rotInterpolation) * local.rotSpeed * Time.deltaTime);
							}
						}

						if (local.scale.synchronize)
						{
							if (local.scale.syncMode == SocketSync.Realtime)
							{
								local.transform.localScale = local.targetSize;
							}

							else if (local.scale.syncMode == SocketSync.AdjustToward)
							{
								float sDamp = Vector3.Distance(local.transform.localScale, local.targetSize);
								local.transform.localScale = Vector3.MoveTowards(local.transform.localScale, local.targetSize, (sDamp + local.sizeInterpolation) * local.sizeSpeed * Time.deltaTime);
							}

							else if (local.position.syncMode == SocketSync.LerpValues)
							{
								float sDamp = Vector3.Distance(local.transform.localScale, local.targetSize);
								local.transform.localScale = Vector3.Lerp(local.transform.localScale, local.targetSize, (sDamp + local.sizeInterpolation) * local.sizeSpeed * Time.deltaTime);
							}
						}
					}
			});
		}

		#endregion




		#region CallbackEvents // ------------------------------------- SERVER AUTHORITATIVE EVENTS! ------------------------------------ //

		//Done! User rejections if authKey is not valid!
		private void OnUserRejected(SocketIOEvent _eventArgs)
		{
			ConnJson connJson = JsonUtility.FromJson<ConnJson>(_eventArgs.data.ToString());

			ForceDisconnect();
			disconnectCallback(ConnStat.Disconnected);
			pingValue = 0;

			DebugLog (Debugs.Error, "ConnectForbidden", "Your authentication key is not authorized!");
		}

		private Action<ChannelJson> joinedCallback = null;
		private void OnRoomJoined(SocketIOEvent _eventArgs)
		{
			ChannelJson channelJson = JsonUtility.FromJson<ChannelJson>(_eventArgs.data.ToString());
			DebugLog(Debugs.Log, "OnMatchEvent: JOINED", "ID: " + channelJson.identity + " NAME: " + channelJson.cname 
				+ " VAR: " + channelJson.variant + " MAX: " + channelJson.maxconnect + " CUR: " + channelJson.users.Count);

			if (joinedCallback != null)
			{
				joinedCallback(channelJson);
			}
		}

		private Action<ChannelJson> leavedCallback = null;
		private void OnRoomLeaved(SocketIOEvent _eventArgs)
		{
			ChannelJson channelJson = JsonUtility.FromJson<ChannelJson>(_eventArgs.data.ToString());
			DebugLog(Debugs.Log, "OnMatchEvent: LEAVED", "ID: " + channelJson.identity + " NAME: " + channelJson.cname 
				+ " VAR: " + channelJson.variant + " MAX: " + channelJson.maxconnect + " CUR: " + channelJson.users.Count);

			List<string> ids = new List<string> ();
			foreach(ChannelUser chanUser in channelJson.users)
			{
				ids.Add (chanUser.identity);
			}

			socketIdentities.ForEach ((USocketView sockView) => {
				
				if(ids.Find(x => x == sockView.socketId) == null)
				{
					Destroy(sockView.gameObject);
				}
			});

			if (leavedCallback != null)
			{
				leavedCallback(channelJson);
			}
		}

		private void OnInstancePeer(SocketIOEvent _eventArgs)
		{
			DebugLog(Debugs.Log, "OnEvent: INTANCE", "Instance from Peers!" + " Content: " + _eventArgs.data.ToString());
			Instances instances = JsonUtility.FromJson<Instances>(_eventArgs.data.ToString());

			USocketView curUser = Instantiate(socketPrefabs[instances.prefabIndex], instances.pos.ToVector, instances.rot.ToQuaternion);
			curUser.IsLocalUser = false;
			curUser.socketId = instances.identity;
			curUser.uSocketNet = this;
			socketIdentities.Add(curUser);
		}

		private void OnStateReceived(SocketIOEvent _eventArgs)
		{
			if (_eventArgs.data.keys.Exists(x => x == "identity"))
			{
				string identity = "";
				_eventArgs.data.ToDictionary().TryGetValue("identity", out identity);

				if (socketIdentities.Exists(x => x.socketId == identity && x.IsLocalUser == false))
				{
					USocketView sockId = socketIdentities.Find(x => x.socketId == identity);

					StateJson stateJson = JsonUtility.FromJson<StateJson>(_eventArgs.data.ToString());
					int currentIndex = 4;

					if (sockId.position.synchronize && stateJson.states[0].Equals("t"))
					{
						sockId.targetPos = TVector.ToVector3(stateJson.states[currentIndex]);
						currentIndex += 1;
					}

					if (sockId.rotation.synchronize && stateJson.states[1].Equals("t"))
					{
						sockId.targetRot = TVector.ToVector3(stateJson.states[currentIndex]);
						currentIndex += 1;
					}

					if (sockId.scale.synchronize && stateJson.states[2].Equals("t"))
					{
						sockId.targetSize = TVector.ToVector3(stateJson.states[currentIndex]);
						currentIndex += 1;
					}

					if (sockId.states.synchronize && stateJson.states[3].Equals("t"))
					{
						sockId.states.syncValue = new List<string>();

						for (int i = currentIndex; i < stateJson.states.Count; i++)
						{
							sockId.states.syncValue.Add(stateJson.states[i]);
						}
					}
				}
			}
		}

		private void OnLeaveServerRoom(JSONObject jsonObject)
		{
			socketIdentities.ForEach ((USocketView sockView) => {
				Destroy(sockView.gameObject);
			});

			DebugLog(Debugs.Log, "LeaveMatchSuccess", "You successfully leaved a match!");
		}

		#endregion



		#region UnityInterface - Done!

		//Initilaized all the required mechanisms.
		void Awake()
		{
			//CUSTOM INITIALIZATION
			DontDestroyOnLoad(this);
			threadset.IsInitialized = true;
			Application.runInBackground = bindings.runOnBackground;

			//ENCRYPTIONS & HANDLERS
			queueCoder.encoder = new Encoder();
			queueCoder.decoder = new Decoder();
			queueCoder.parser = new Parser();

			queueCoder.handlers = new Dictionary<string, List<Action<SocketIOEvent>>>();
			queueCoder.eventQueue = new Queue<SocketIOEvent>();
			queueCoder.eventQueueLock = new object();

			queueCoder.ackQueue = new Queue<Packet>();
			queueCoder.ackQueueLock = new object();
			queueCoder.ackList = new List<Ack>();

			//WEB SOCKET INITIALIZATION
			threadset.websocket = new WebSocket("ws://" + bindings.serverUrl + ":" + bindings.serverPort + "/socket.io/?EIO=4&transport=websocket");
			threadset.websocket.OnOpen += OnOpen;
			threadset.websocket.OnError += OnError;
			threadset.websocket.OnClose += OnClose;
			threadset.websocket.OnMessage += OnMessage;

			threadset.wsCheck = false;
			threadset.IsConnected = false;
			threadset.packetId = 0;
			threadset.socketId = null;
			threadset.autoConnect = true;

			//LISTENERS AND EVENTS
			CallbackOn("disconnected", OnUserRejected);
			CallbackOn("messaged", OnMessageReceived);
			CallbackOn("joined", OnRoomJoined);
			CallbackOn("leaved", OnRoomLeaved);
			CallbackOn("instanced", OnInstancePeer);
			CallbackOn("transtated", OnStateReceived);
			//ADD MORE HERE!
		}

		//On any case that abruptly closing the app, force to disconnect client.
		void OnApplicationQuit()
		{
			ForceDisconnect();
		}

		//Force client disconnection to server, locally and on the server.
		private void ForceDisconnect()
		{
			//Send a packet to server to immediately close the connection.
			EmitPacket(new Packet(EnginePacketType.MESSAGE, SocketPacketType.DISCONNECT, 0, "/", -1, new JSONObject("")));
			EmitPacket(new Packet(EnginePacketType.CLOSE));

			//threadset.websocket.Close ();
			threadset.IsConnected = false;
			threadset.autoConnect = false;
		}

		//On any case that abruptly destroyed script instance.
		void OnDestroy()
		{
			AbortConnection(); //Forcibly abort connection.
		}

		private void AbortConnection()
		{
			if (socketThread != null)
			{
				socketThread.Abort();
			}

			if (pingThread != null)
			{
				pingThread.Abort();
			}
		}

		void Update()
		{
			if (threadset.websocket.IsConnected)
			{
				Pinging();
				Synching ();
			}

			if (threadset.IsInitialized)
			{
				lock (queueCoder.eventQueueLock)
				{
					while (queueCoder.eventQueue.Count > 0)
					{
						EmitEvent(queueCoder.eventQueue.Dequeue());
					}
				}

				lock (queueCoder.ackQueueLock)
				{
					while (queueCoder.ackQueue.Count > 0)
					{
						Packet packet = queueCoder.ackQueue.Dequeue();
						Ack ack; for (int i = 0; i < queueCoder.ackList.Count; i++)
						{
							if (queueCoder.ackList[i].packetId != packet.id)
							{
								continue;
							}

							ack = queueCoder.ackList[i];
							queueCoder.ackList.RemoveAt(i);
							ack.Invoke(packet.json);
							return;
						}
					}
				}

				//Check for websocket connection status change.
				if (threadset.wsCheck != threadset.websocket.IsConnected)
				{
					//Save the current websocket connetion status as bool.
					threadset.wsCheck = threadset.websocket.IsConnected;

					//If websocket instance is currently connected.
					if (threadset.websocket.IsConnected)
					{
						EmitEvent("connect"); //Send a 'connect' event to server.

						//If reconnection is authorized.
						if (threadset.autoConnect)
						{
							StartCoroutine(OnListeningConnectionStatus(ConnStat.Reconnected));
						}

						else
						{
							StartCoroutine(OnListeningConnectionStatus(ConnStat.Connected));
							threadset.autoConnect = true;
						}
					}

					else
					{
						EmitEvent("disconnect");

						if (threadset.autoConnect) //User disconnection.
						{
							StartCoroutine(OnListeningConnectionStatus(ConnStat.Maintainance));
						}

						else //Server disconnection.
						{
							StartCoroutine(OnListeningConnectionStatus(ConnStat.Disconnected));
						}
					}
				}

				if (queueCoder.ackList.Count == 0)
					return;
				
				if (DateTime.Now.Subtract (queueCoder.ackList [0].time).TotalSeconds < threadset.ackExpireTime)
					return;
				
				queueCoder.ackList.RemoveAt(0);
			}
		}

		#endregion

		#region DefaultSocketEvent - Done!

		private void OnOpen(object sender, EventArgs e) { EmitEvent("open"); }
		private void OnError(object sender, ErrorEventArgs e) { EmitEvent("error"); }
		private void OnClose(object sender, CloseEventArgs e) { EmitEvent("close"); }

		private void OnMessage(object sender, MessageEventArgs e)
		{
			Packet packet = queueCoder.decoder.Decode(e); 

			switch (packet.enginePacketType)
			{
				case EnginePacketType.OPEN: HandleOpen(packet); break;
				case EnginePacketType.CLOSE: EmitEvent("close"); break;
				case EnginePacketType.PING: HandlePing(); break;
				case EnginePacketType.PONG: HandlePong(); break;
				case EnginePacketType.MESSAGE: HandleMessage(packet); break;
			}
		}

		private void HandleOpen(Packet packet)
		{
			threadset.socketId = packet.json["sid"].str;
			EmitEvent("open");
		}

		private void HandlePing()
		{
			EmitPacket(new Packet(EnginePacketType.PONG));
		}

		private void HandlePong()
		{
			threadset.wsPonging = true;
			threadset.wsPinging = false;
		}

		private void HandleMessage(Packet packet)
		{
			if (packet.json == null) { return; }

			if (packet.socketPacketType == SocketPacketType.ACK)
			{
				for (int i = 0; i < queueCoder.ackList.Count; i++)
				{
					if (queueCoder.ackList[i].packetId != packet.id)
					{
						continue;
					}

					lock (queueCoder.ackQueueLock)
					{
						queueCoder.ackQueue.Enqueue(packet);
					}

					return;
				}
			}

			if (packet.socketPacketType == SocketPacketType.EVENT)
			{
				SocketIOEvent e = queueCoder.parser.Parse(packet.json);
				lock (queueCoder.eventQueueLock)
				{
					queueCoder.eventQueue.Enqueue(e);
				}
			}
		}

		#endregion

		#region WebSocketThreading - Done!

		private Threadset threadset = new Threadset();
		private QueueCoder queueCoder = new QueueCoder();

		private Thread socketThread;
		private void RunSocketThread(object obj)
		{
			WebSocket webSocket = (WebSocket)obj;

			while (threadset.IsConnected)
			{
				if (webSocket.IsConnected)
				{
					Thread.Sleep(threadset.reconDelay);
				}

				else
				{
					webSocket.Connect();
				}
			}

			webSocket.Close();
		}

		private Thread pingThread;
		private void RunPingThread(object obj)
		{
			WebSocket webSocket = (WebSocket)obj; DateTime pingStart;
			int timeoutMilis = Mathf.FloorToInt(threadset.pingTimeout * 1000);
			int intervalMilis = Mathf.FloorToInt(threadset.pingInterval * 1000);

			while (threadset.IsConnected)
			{
				if (!threadset.websocket.IsConnected)
				{
					Thread.Sleep(threadset.reconDelay);
				}

				else
				{
					threadset.wsPinging = true; threadset.wsPonging = false;
					EmitPacket(new Packet(EnginePacketType.PING));
					pingStart = DateTime.Now;

					while (webSocket.IsConnected && threadset.wsPinging && (DateTime.Now.Subtract(pingStart).TotalSeconds < timeoutMilis))
					{
						Thread.Sleep(200);
					}

					if (!threadset.wsPonging)
					{
						webSocket.Close();
					}

					Thread.Sleep(intervalMilis);
				}
			}
		}

		#endregion

		#region EmitterInterface - Done!

		private void EmitEvent(string type)
		{
			EmitEvent(new SocketIOEvent(type));
		}

		private void EmitEvent(SocketIOEvent eventArgs)
		{
			if (!queueCoder.handlers.ContainsKey(eventArgs.name))
				return;

			foreach (Action<SocketIOEvent> handler in queueCoder.handlers[eventArgs.name])
			{
				try
				{
					handler(eventArgs);
				}

				catch (Exception except)
				{
					DebugLog(Debugs.Warn, "EmitEventCatch", except.Message);
				}
			}
		}

		private void EmitMessage(int id, string raw)
		{
			EmitPacket(new Packet(EnginePacketType.MESSAGE, SocketPacketType.EVENT, 0, "/", id, new JSONObject(raw)));
		}

		private void EmitPacket(Packet packet)
		{
			try
			{
				threadset.websocket.Send(queueCoder.encoder.Encode(packet));
			}

			catch (SocketIOException ex)
			{
				if (ex != null)
				{
					DebugLog(Debugs.Warn, "EmitPacketCatch", ex.Message);
				}
			}
		}

		#endregion

		#region MethodsInterface - Done!

		private void CallbackOn(string events, Action<SocketIOEvent> callback)
		{
			if (!queueCoder.handlers.ContainsKey(events))
			{
				queueCoder.handlers[events] = new List<Action<SocketIOEvent>>();
			}

			queueCoder.handlers[events].Add(callback);
		}

		private void CallbackOff(string events, Action<SocketIOEvent> callback)
		{
			if (!queueCoder.handlers.ContainsKey(events))
			{
				DebugLog(Debugs.Warn, "CallbackOffMissing", "No callbacks registered for event: " + events);
				return;
			}

			List<Action<SocketIOEvent>> eventList = queueCoder.handlers[events];
			if (!eventList.Contains(callback))
			{
				DebugLog(Debugs.Warn, "CallbackOffCantRemove", "Couldn't remove callback action for event: " + events);
				return;
			}

			eventList.Remove(callback);
			if (eventList.Count == 0)
			{
				queueCoder.handlers.Remove(events);
			}
		}

		private void SendEmit(string events)
		{
			EmitMessage(-1, string.Format("[\"{0}\"]", events));
		}

		private void SendEmit(string events, Action<JSONObject> action)
		{
			EmitMessage(++threadset.packetId, string.Format("[\"{0}\"]", events));
			queueCoder.ackList.Add(new Ack(threadset.packetId, action));
		}

		private void SendEmit(string events, JSONObject data)
		{
			EmitMessage(-1, string.Format("[\"{0}\",{1}]", events, data));
		}

		private void SendEmit(string events, JSONObject data, Action<JSONObject> action)
		{
			EmitMessage(++threadset.packetId, string.Format("[\"{0}\",{1}]", events, data));
			queueCoder.ackList.Add(new Ack(threadset.packetId, action));
		}

		#endregion
	}
}