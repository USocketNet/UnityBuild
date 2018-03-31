using System; 
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
		[SerializeField] Bindings bindings = new Bindings();

		[Header("INSTANCE PREFAB")]
		public List<USocketView> socketPrefabs = new List<USocketView>();
		[HideInInspector]
		public List<USocketView> localSockets = new List<USocketView> ();

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

		[HideInInspector] public bool Subscribed = false;

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
			if(!bindings.debugOnLog)
				return;

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

		//Done! User rejections if authKey is not valid!
		private void OnUserRejected(SocketIOEvent _eventArgs)
		{
			ConnJson connJson = JsonUtility.FromJson<ConnJson>(_eventArgs.data.ToString());

			ForceDisconnect();
			disconnectCallback(ConnStat.Disconnected);
			pingValue = 0;

			DebugLog (Debugs.Error, "ConnectForbidden", "Your authentication key is not authorized!");
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
			StopCoroutine ("OnListeningConnectionStatus");
			ConnJson connJson = JsonSerializer.ToObject<ConnJson>(jsonObject.ToString());
			onListeningReturn = true;

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
			StopCoroutine (SendingPublicMessage());
			MsgCallback msgJson = JsonSerializer.ToObject<MsgCallback>(jsonObject.ToString());
			publicReturned = true;
			publicCallback(MsgStat.Success);

			DebugLog (Debugs.Log, "PubMsgSuccess", "Successfully sent a lobby message!");
		}

		#endregion

		#region PrivateMessage - Done!

		private bool privateReturned = false;
		private Action<MsgStat> privateMessage = null;

		public void SendPrivateMessage(string receiver, string message, Action<MsgStat> _privateMessage)
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
				privateMessage(MsgStat.Failed);
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
					privateMessage(MsgStat.Failed);
					DebugLog (Debugs.Error, "PrivateMsgFailed", "You are currently disconnected to the server!");
				}
			}
		}

		private void OnPrivateMessageSent(JSONObject jsonObject)
		{
			StopCoroutine (SendingPrivateMessage());
			MsgJson msgJson = JsonSerializer.ToObject<MsgJson>(jsonObject.ToString());
			privateReturned = true;
			privateMessage(MsgStat.Failed);

			DebugLog (Debugs.Log, "PrivateMsgSuccess", "Successfully sent a private message!");
		}

		#endregion

		#region ChannelMessage - Done!

		private bool channelReturned = false;
		private Action<MsgStat> channelCallback = null;

		public void SendChannelMessage(string message, Action<MsgStat> _channelCallback)
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
				channelCallback(MsgStat.Failed);
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
					channelCallback(MsgStat.Failed);
					DebugLog (Debugs.Error, "ChannelMsgFailed", "You are currently disconnected to the server!");
				}
			}
		}

		private void OnChannelMessageSent(JSONObject jsonObject)
		{
			StopCoroutine (SendingChannelMessage());
			MsgJson msgJson = JsonSerializer.ToObject<MsgJson>(jsonObject.ToString());
			channelReturned = true;
			channelCallback(MsgStat.Success);

			DebugLog (Debugs.Log, "ChannelMsgSuccess", "Successfully sent a channel message!");
		}

		#endregion

		#region ListenMessage - Done!

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

		#region AutoChannel - Done!

		private bool autoChannelReturned = false;
		private Action<Returned, ChannelJson> autoChannelCallback = null;

		public void AutoJoinChannel(string variant, int maxconnect, Action<Returned, ChannelJson> _autoChannelCallback)
		{
			autoChannelCallback = _autoChannelCallback;
			autoChannelReturned = false;

			if (threadset.websocket.IsConnected)
			{
				ChannelJson channelJson = new ChannelJson(variant, maxconnect);
				string sendData = JsonUtility.ToJson(channelJson);
				SendEmit("auto", new JSONObject(sendData), OnAutoServerChannel);
				StartCoroutine(AutoServerChannel());
			}

			else
			{
				autoChannelCallback(Returned.Error, new ChannelJson(string.Empty));
				DebugLog (Debugs.Error, "AutoChannelFailed", "You are currently disconnected to the server!");
			}
		}

		private IEnumerator AutoServerChannel()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!autoChannelReturned)
			{
				if (!threadset.websocket.IsConnected)
				{
					autoChannelCallback(Returned.Failed, new ChannelJson(string.Empty));
					DebugLog (Debugs.Error, "AutoChannelFailed", "You are currently disconnected to the server!");
				}
			}
		}

		//var newUser = {};
		//newUser.id = curUser.identity;
		//newUser.itc = [];

		//newUser.pfb = '';
		//newUser.itc = '';
		//newUser.pos = '';
		//newUser.rot = '';
		//newUser.sca = '';
		//newUser.sta = '';

		private void OnAutoServerChannel(JSONObject jsonObject)
		{
			StopCoroutine (AutoServerChannel());
			ChannelJson channelJson = JsonSerializer.ToObject<ChannelJson>(jsonObject.ToString());
			autoChannelReturned = true;
			autoChannelCallback(Returned.Success, channelJson);
			Subscribed = false;

			DebugLog (Debugs.Log, "AutoChannelSuccess", "Successfully auto match a channel!");
		}

		#endregion

		#region CreateChannel - Done!

		private Action<Returned, ChannelJson> createCallback = null; private bool createReturned = false;
		public void CreateServerRoom(string channelName, string variant, int maxconnect, Action<Returned, ChannelJson> _createCallback)
		{
			createCallback = _createCallback;
			createReturned = false;

			if (threadset.websocket.IsConnected)
			{
				ChannelJson channelJson = new ChannelJson(channelName, variant, maxconnect);
				string sendData = JsonUtility.ToJson(channelJson);
				SendEmit("create", new JSONObject(sendData), OnCreatedServerRoom);
				StartCoroutine(CreatingServerRoom());
			}

			else
			{
				createCallback(Returned.Error, new ChannelJson(String.Empty));
				DebugLog (Debugs.Error, "CreateChannelFailed", "You are currently disconnected to the server!");
			}
		}

		IEnumerator CreatingServerRoom()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!createReturned)
			{
				if (!threadset.websocket.IsConnected)
				{
					createCallback(Returned.Failed, new ChannelJson(String.Empty));
					DebugLog (Debugs.Error, "CreateChannelFailed", "You are currently disconnected to the server!");
				}
			}
		}

		private void OnCreatedServerRoom(JSONObject jsonObject)
		{
			StopCoroutine (CreatingServerRoom());
			ChannelJson channelJson = JsonSerializer.ToObject<ChannelJson>(jsonObject.ToString());
			createReturned = true;
			createCallback(Returned.Success, channelJson);
			Subscribed = false;

			DebugLog (Debugs.Log, "CreateChannelSuccess", "Successfully create match a channel!");
		}

		#endregion

		#region JoinChannel - Done!

		private Action<Returned, ChannelJson> joinCallback = null;
		private bool joinReturned = false;

		public void JoinServerRoom(string channelId, Action<Returned, ChannelJson> _joinCallback)
		{
			joinCallback = _joinCallback;
			joinReturned = false;

			if (threadset.websocket.IsConnected)
			{
				ChannelJson channelJson = new ChannelJson(channelId);
				string sendData = JsonUtility.ToJson(channelJson);
				SendEmit("join", new JSONObject(sendData), OnJoinedServerRoom);
				StartCoroutine(JoiningServerRoom());
			}

			else
			{
				joinCallback(Returned.Error, new ChannelJson(String.Empty));
				DebugLog (Debugs.Error, "JoinChannelFailed", "You are currently disconnected to the server!");
			}
		}

		IEnumerator JoiningServerRoom()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!joinReturned)
			{
				if (joinCallback != null)
				{
					joinCallback(Returned.Failed, new ChannelJson(String.Empty));
					DebugLog (Debugs.Error, "JoinChannelFailed", "You are currently disconnected to the server!");
				}
			}
		}

		private void OnJoinedServerRoom(JSONObject jsonObject)
		{
			StopCoroutine (JoiningServerRoom());
			ChannelJson channelJson = JsonSerializer.ToObject<ChannelJson>(jsonObject.ToString());
			joinReturned = true;
			joinCallback(Returned.Success, channelJson);
			Subscribed = false;

			DebugLog (Debugs.Log, "JoinChannelSuccess", "Successfully join match a channel!");
		}

		#endregion

		#region LeaveChannel - Done!

		private Action<Returned, ChannelJson> leaveCallback = null;
		private bool leaveReturned = false;

		public void LeaveChannel(Action<Returned, ChannelJson> _leaveCallback)
		{
			leaveCallback = _leaveCallback;
			leaveReturned = false;

			if (threadset.websocket.IsConnected)
			{
				string sendData = JsonUtility.ToJson(Identity);
				SendEmit("leave", new JSONObject(sendData), OnLeaveMatch);
				StartCoroutine (LeavingMatch());
			}

			else
			{
				leaveCallback (Returned.Error, new ChannelJson(String.Empty));
				DebugLog(Debugs.Error, "LeaveMatchError", "You are not connected to the server!");
			}
		}

		IEnumerator LeavingMatch()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!leaveReturned)
			{
				if (leaveCallback != null)
				{
					leaveCallback(Returned.Failed, new ChannelJson(String.Empty));
					DebugLog (Debugs.Warn, "LeaveChannelFailed", "Server did not respond to leave event!");
				}
			}
		}

		private void OnLeaveMatch(JSONObject jsonObject)
		{
			StopCoroutine (LeavingMatch());
			leaveReturned = true;
			leaveCallback (Returned.Success, new ChannelJson(String.Empty));
			Subscribed = false;

			USocket.Instance.socketIdentities.ForEach ((USocketView sockView) => {
				Destroy(sockView.gameObject);
			});
			USocket.Instance.socketIdentities = new List<USocketView> ();

			localSockets.ForEach ((USocketView sockView) => {
				Destroy(sockView.gameObject);
			});
			localSockets = new List<USocketView> ();

			DebugLog(Debugs.Log, "LeaveMatchSuccess", "You successfully leaved a match!");
		}

		#endregion

		#region MatchMakingJoinedEvent - Ongoing!

		private Action<PeerJson> joinedCallback = null;

		public void ListenMatchJoined(Action<PeerJson> _joinedCallback)
		{
			joinedCallback = _joinedCallback;
		}

		private void OnChannelJoined(SocketIOEvent _eventArgs)
		{
			PeerJson peerJson = JsonUtility.FromJson<PeerJson>(_eventArgs.data.ToString());

			if (joinedCallback != null)
			{
				joinedCallback(peerJson);
			}

			DebugLog(Debugs.Log, "OnMatchEvent: JOINED", "ID: " + peerJson.id + " OBJ: " + peerJson.obj.Count);
		}

		#endregion

		#region MatchMakingLeavedEvent - Done!

		private Action<PeerJson> leavedCallback = null;

		public void ListenMatchLeaved(Action<PeerJson> _leavedCallback)
		{
			leavedCallback = _leavedCallback;
		}

		private void OnRoomLeaved(SocketIOEvent _eventArgs)
		{
			PeerJson peerJson = JsonUtility.FromJson<PeerJson>(_eventArgs.data.ToString());

			if(USocket.Instance.socketIdentities.Exists(x => x.Identity == peerJson.id))
			{
				//all instance of the current peer!
				USocket.Instance.socketIdentities.ForEach ((USocketView obj) => {
					if(obj.Identity == peerJson.id)
					{
						Destroy(obj.gameObject);
					}
				});
				USocket.Instance.socketIdentities.RemoveAll (x => x == null);
			}

			if (leavedCallback != null)
			{
				leavedCallback(peerJson);
			}

			DebugLog(Debugs.Log, "OnMatchEvent: LEAVED", "ID: " + peerJson.id + " OBJ: " + peerJson.obj.Count);
		}
		//instance event.
		#endregion

		#region INSTANTIATIONS - Done!

		private Action<Returned> instanceCallback = null;
		private bool instanceReturned = false;

		public void Instantiate(int prefabIndex, Vector3 position, Quaternion rotation, Action<Returned> _instanceCallback)
		{
			instanceCallback = _instanceCallback;
			instanceReturned = false;

			if (threadset.websocket.IsConnected)
			{
				Instances intance = new Instances(localSockets.Count + DateTime.Now.ToString(), prefabIndex, position, rotation);
				intance.itc = intance.GetHashCode () + "~" + localSockets.Count;
				string sendData = JsonUtility.ToJson(intance);
				SendEmit("instance", new JSONObject(sendData), OnInstantiate);
				StartCoroutine (InstantiatingInstance());
			}

			else
			{
				instanceCallback (Returned.Error);
				DebugLog(Debugs.Error, "InstantiateError", "You are not connected to the server!");
			}
		}

		IEnumerator InstantiatingInstance()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!instanceReturned)
			{
				if (instanceCallback != null)
				{
					instanceCallback(Returned.Failed);
					DebugLog (Debugs.Warn, "InstantiateFailed", "Server did not respond to instantiate event!");
				}
			}
		}

		//Instantiate from LOCAL.
		private void OnInstantiate(JSONObject jsonObject)
		{
			StopCoroutine (InstantiatingInstance());
			instanceReturned = true;

			Instances instances = JsonSerializer.ToObject<Instances>(jsonObject.ToString());
			Instantiating (instances, true);

			if(instanceCallback != null)
			{
				instanceCallback (Returned.Success);
			}

			DebugLog(Debugs.Log, "OnEvent: INSTANCE LOCAL", "ID: " + instances.identity + "POS: " + instances.pos);
		}

		//Instantiate froms SERVER.
		private void OnInstancePeer(SocketIOEvent _eventArgs)
		{
			Instances instances = JsonUtility.FromJson<Instances>(_eventArgs.data.ToString());

			if(!USocket.Instance.usocketNets.Exists(x => x.Identity == instances.identity))
			{
				if(!USocket.Instance.socketIdentities.Exists(x => x.Instance == instances.itc))
				{
					Instantiating (instances, false);

					DebugLog(Debugs.Log, "OnEvent: INSTANCE SERVER", "ID: " + instances.identity + "POS: " + instances.pos);
				}
			}
		}

		//Instantiating Mechanism.
		private USocketView Instantiating(Instances instances, bool isLocalUser)
		{
			USocketView curUser = Instantiate(socketPrefabs[instances.pfb], VectorJson.ToVector3(instances.pos), VectorJson.ToQuaternion(instances.rot));
			curUser.IsLocalUser = isLocalUser;
			curUser.Identity = instances.identity;
			curUser.Instance = instances.itc;
			curUser.uSocketNet = this;

			if(isLocalUser)
			{
				localSockets.Add(curUser);
			}

			else
			{
				USocket.Instance.socketIdentities.Add(curUser);
			}

			return curUser;
		}

		#endregion

		#region SYNCHRONIZATION - Optimization!

		private void SynchingOutbound()
		{
			USocket.Instance.socketIdentities.Remove (null);
			localSockets.Remove (null);

			bindings.sendTimer += Time.deltaTime;

			if (bindings.sendTimer >= (1f / bindings.mainSendRate))
			{
				SyncJsons syncJsons = new SyncJsons ();//Identity);

				if (localSockets.Count > 0)
				{
					foreach(USocketView usocket in localSockets)
					{
						syncJsons.obj.Add (usocket.GetViewData());
					}
				}

				string sendData = JsonUtility.ToJson (syncJsons);
				SendEmit ("transtate", new JSONObject (sendData), SynchingSockets);

				bindings.sendTimer = 0f;
			}
		}

		private void SynchingSockets(JSONObject jsonObject)
		{
			ChanUsers chanUsers = JsonSerializer.ToObject<ChanUsers>(jsonObject.ToString());

			//SYNCHRONIZE AND INSTANTIATE!
			chanUsers.users.ForEach ((PeerJson peerJson) => 
				{
					if(peerJson.id == Identity)
						return;

					//Check if user id is other machine must!.
					if(!USocket.Instance.usocketNets.Exists(x => x.Identity == peerJson.id))
					{
						peerJson.obj.ForEach ((ObjJson objJson) => 
							{
								if (USocket.Instance.socketIdentities.Exists(x => x.Identity == peerJson.id && x.Instance == objJson.id))
								{
									USocket.Instance.socketIdentities.Find(x => x.Identity == peerJson.id && x.Instance == objJson.id).SetViewTarget(objJson);
								}

								else
								{
									Instances instan = new Instances(objJson.id, objJson.pfb, VectorJson.ToVector3(objJson.pos), VectorJson.ToQuaternion(objJson.rot));
									instan.identity = peerJson.id;
									Instantiating(instan, false).SetViewTarget(objJson);
								}
							});
					}
				});
			
			//SYNCHRONIZE AND DESTROY IF NOT EXIST!
			USocket.Instance.socketIdentities.ForEach((USocketView toDestroy) =>{

				if(!toDestroy.IsLocalUser)
				{
					if(!chanUsers.users.Exists(x => x.id == toDestroy.Identity))
					{
						Destroy(toDestroy.gameObject);
					}
				}
			});
			USocket.Instance.socketIdentities.RemoveAll (x => x == null);
		}

		private void SynchingInbound()
		{
			USocket.Instance.socketIdentities.ForEach ((USocketView peers) => 
				{
					//Check if entry is null, just remove it.
					if (peers == null)
					{
						USocket.Instance.socketIdentities.Remove(peers);
						return;
					};

					peers.SyncViewData();
				});
		}

		#endregion

		#region UnityInterface - Done!

		//Initilaized all the required mechanisms.
		void Awake()
		{
			if(!USocket.Instance.socketIdentities.Exists(x => x.GetInstanceID() == GetInstanceID()))
			{
				USocket.Instance.usocketNets.Add (this);
			}

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

			//CallbackOn("open", OnReceivedOpen);
			//CallbackOn("close", OnReceivedClose);
			//CallbackOn("error", OnReceivedError);

			//LISTENERS AND EVENTS
			CallbackOn("disconnected", OnUserRejected);
			CallbackOn("messaged", OnMessageReceived);
			CallbackOn("joined", OnChannelJoined);
			CallbackOn("instanced", OnInstancePeer);
			CallbackOn("leaved", OnRoomLeaved);
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
			if(threadset.websocket == null)
				return;

			if (threadset.websocket.IsConnected)
			{
				SynchingOutbound ();
				SynchingInbound ();
				Pinging();
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

		public void OnReceivedOpen(SocketIOEvent e)
		{
			DebugLog(Debugs.Warn, "[SocketIO]", " Open received: " + e.name + " " + e.data);
		}

		public void OnReceivedError(SocketIOEvent e)
		{
			DebugLog(Debugs.Warn, "[SocketIO]", " Error received: " + e.name + " " + e.data);
		}

		public void OnReceivedClose(SocketIOEvent e)
		{	
			DebugLog(Debugs.Warn, "[SocketIO]", " Close received: " + e.name + " " + e.data);
		}

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