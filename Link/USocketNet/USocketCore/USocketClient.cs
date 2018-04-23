using System; 
using System.Threading; 
using System.Collections; 
using System.Collections.Generic;

using SocketIO;
using WebSocketSharp;
using UnityEngine;

using UnityEngine.UI;

using BytesCrafter.USocketNet.Serializables;
using BytesCrafter.USocketNet.Networks;
using BytesCrafter.USocketNet.Toolsets;
namespace BytesCrafter.USocketNet
{
	public class USocketClient : MonoBehaviour
	{
		#region ExposedInterface - Ongoing!

		[Header("NETWORK SETTINGS")]
		public Bindings bindings = new Bindings();

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

		[HideInInspector]
		public bool Subscribed = false;

		/// <summary>
		/// Sockets identity of the current user's connection to the server.
		/// </summary>
		/// <returns> Socket Identity, Not Connected, No Internet Access. </returns>
		public string Identity
		{
			get
			{
				//if (threadset.IsConnected) { return threadset.socketId; }
				//else { return "Not Connected"; }
				return netIdentity;
			}
		}
		private string netIdentity = string.Empty;

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

		#region CanvasBlocker - Done!

		private GameObject blocker = null;
		private void UIBlocker(bool show)
		{
			if(show)
			{
				if(blocker == null)
				{
					blocker = new GameObject ("Blocker");
					Canvas cvs = blocker.AddComponent<Canvas> ();
					cvs.renderMode = RenderMode.ScreenSpaceOverlay;
					cvs.sortingOrder = 100;
					blocker.AddComponent<CanvasScaler> ();
					Image img = blocker.AddComponent<Image> ();
					img.color = new Color (0f, 0f, 0f, 0.29f);
				}

				else
				{
					blocker.SetActive(true);
				}
			}

			else
			{
				if(blocker != null)
				{
					blocker.SetActive(false);
				}
			}
		}

		#endregion

		#region ConnectServer - Done!

		private bool connectReturn = false;
		private Action<ConnStat, ConnJson> connectCallback = null;

		public void ConnectToServer(string username, Action<ConnStat, ConnJson> connCallback)
		{
			connectCallback = connCallback;
			connectReturn = false;

			if (!threadset.IsConnected)
			{
				UIBlocker (true);

				threadset.IsConnected = true;
				threadset.autoConnect = false;

				socketThread = new Thread(RunSocketThread);
				socketThread.Start(threadset.websocket);

				pingThread = new Thread(RunPingThread);
				pingThread.Start(threadset.websocket);

				StartCoroutine(ConnectingToServer(username));
			}

			else
			{
				if(connectCallback != null)
				{
					connectCallback(ConnStat.Connected, new ConnJson());
				}
				DebugLog(Debugs.Warn, "ConnectionSuccess", "Already connected to the server!");
			}
		}

		private string lastUsername = string.Empty;
		IEnumerator ConnectingToServer(string username)
		{
			lastUsername = username;
			yield return new WaitForSeconds(bindings.connectDelay);

			if (threadset.websocket.IsConnected)
			{
				UserJson userJson = new UserJson(bindings.authenKey, username);
				string sendData = JsonUtility.ToJson(userJson);
				SendEmit("connect", new JSONObject(sendData), OnServerConnect);

				yield return new WaitForSeconds(bindings.connectDelay);

				if (!connectReturn)
				{
					AbortConnection ();
					UIBlocker (false);

					if(connectCallback != null)
					{
						connectCallback(ConnStat.InternetAccess, new ConnJson());
					}
					DebugLog(Debugs.Error, "ConnectionNoInternet", "Detected no internet connection!");
				}
			}

			else
			{
				AbortConnection ();

				if(connectCallback != null)
				{
					connectCallback(ConnStat.Maintainance, new ConnJson());
				}
				UIBlocker (false);

			}
		}

		private void OnServerConnect(JSONObject jsonObject)
		{
			StopCoroutine("ConnectingToServer");
			connectReturn = true;

			ConnJson connJson = JsonSerializer.ToObject<ConnJson>(jsonObject.ToString());
			lastUsername = connJson.username;
			netIdentity = connJson.identity;
			UIBlocker (false);

			if(connectCallback != null)
			{
				connectCallback(ConnStat.Connected, connJson);
			}
			DebugLog(Debugs.Log, "ConnectionSuccess", "Successfully connected to server!");
		}

		//Done! User rejections if authKey is not valid!
		private void OnUserRejected(SocketIOEvent _eventArgs)
		{
			ForceDisconnect();
			UIBlocker (false);
			pingValue = 0;

			if(disconnectCallback != null)
			{
				disconnectCallback(ConnStat.Disconnected);
			}
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
				UIBlocker (true);
				StartCoroutine(DisconnectingToServer());
			}

			else
			{
				ForceDisconnect();

				if(disconnectCallback != null)
				{
					disconnectCallback(ConnStat.Disconnected);
				}
				DebugLog (Debugs.Warn, "CantDisconnect", "You are not connected to server!");
			}
		}

		IEnumerator DisconnectingToServer()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!disconnectReturn)
			{
				UIBlocker (false);
				ForceDisconnect();

				if(disconnectCallback != null)
				{
					disconnectCallback(ConnStat.Disconnected);
				}
			}
		}

		private void OnServerDisconnect(JSONObject jsonObject)
		{
			StopCoroutine("DisconnectingToServer");
			disconnectReturn = true;
			ForceDisconnect();
			pingValue = 0;
			UIBlocker (false);

			if(disconnectCallback != null)
			{
				disconnectCallback(ConnStat.Disconnected);
			}
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

			if(connStat != ConnStat.Connected)
			{
				Subscribed = false;
			}

			if (connStat == ConnStat.Reconnected)
			{
				UIBlocker (true);
				UserJson userJson = new UserJson(bindings.authenKey, lastUsername);
				string sendData = JsonUtility.ToJson(userJson);
				SendEmit("reconnect", new JSONObject(sendData), OnServerReconnect);
			}

			yield return new WaitForSeconds(bindings.connectDelay);

			if (!onListeningReturn)
			{
				UIBlocker (false);

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
			UIBlocker (false);

			if (connectionStats != null)
			{
				connectionStats(ConnStat.Reconnected, connJson);
			}
		}

		#endregion

		#region PublicMessage - Done!

		private bool publicReturned = false;
		private Action<Returned> publicCallback = null;

		public void SendPublicMessage(string message, Action<Returned> _publicCallback)
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
				if(publicCallback != null)
				{
					publicCallback(Returned.Failed);
				}
				DebugLog (Debugs.Error, "PubMsgError", "You are currently disconnected to the server!");
			}
		}

		IEnumerator SendingPublicMessage()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!publicReturned)
			{
				if (publicCallback != null)
				{
					publicCallback(Returned.Failed);
				}

				DebugLog (Debugs.Warn, "PubMsgFailed", "Server did not respond to leave event!");
			}
		}

		private void OnPublicMessageSent(JSONObject jsonObject)
		{
			StopCoroutine (SendingPublicMessage());
			publicReturned = true;

			MsgCallback msgJson = JsonSerializer.ToObject<MsgCallback>(jsonObject.ToString());
			if(msgJson.msgStat == Returned.Success)
			{
				if(publicCallback != null)
				{
					publicCallback(Returned.Success);
				}
				DebugLog (Debugs.Log, "PubMsgSuccess", "Successfully sent a lobby message!");
			}

			else
			{
				if(publicCallback != null)
				{
					publicCallback(Returned.Failed);
				}
				DebugLog (Debugs.Log, "PubMsgFailed", "Server did failed to send to peers.");
			}
		}

		#endregion

		#region PrivateMessage - Done!

		private bool privateReturned = false;
		private Action<Returned> privateMessage = null;

		public void SendPrivateMessage(string receiver, string message, Action<Returned> _privateMessage)
		{
			privateMessage = _privateMessage;
			privateReturned = false;

			if (threadset.websocket.IsConnected)
			{
				MsgJson msgJson = new MsgJson(message, MsgType.Private, receiver);
				string sendData = JsonUtility.ToJson(msgJson);
				SendEmit("message", new JSONObject(sendData), OnPrivateMessageSent);
				StartCoroutine(SendingPrivateMessage());
			}

			else
			{
				if(privateMessage != null)
				{
					privateMessage(Returned.Failed);
				}
				DebugLog (Debugs.Error, "PriMsgError", "You are currently disconnected to the server!");
			}
		}

		IEnumerator SendingPrivateMessage()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!privateReturned)
			{
				if (privateMessage != null)
				{
					privateMessage(Returned.Failed);
				}
				DebugLog (Debugs.Warn, "PriMsgFailed", "Server did not respond to leave event!");
			}
		}

		private void OnPrivateMessageSent(JSONObject jsonObject)
		{
			StopCoroutine (SendingPrivateMessage());
			privateReturned = true;

			MsgCallback msgJson = JsonSerializer.ToObject<MsgCallback>(jsonObject.ToString());
			if(msgJson.msgStat == Returned.Success)
			{
				if(privateMessage != null)
				{
					privateMessage(Returned.Success);
				}
				DebugLog (Debugs.Log, "PriMsgSuccess", "Successfully sent a private message!");
			}

			else
			{
				if(privateMessage != null)
				{
					privateMessage(Returned.Failed);
				}
				DebugLog (Debugs.Log, "PriMsgFailed", "Server did failed to send to peers.");
			}
		}

		#endregion

		#region ChannelMessage - Done!

		private bool channelReturned = false;
		private Action<Returned> channelCallback = null;

		public void SendChannelMessage(string message, Action<Returned> _channelCallback)
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
				if(channelCallback != null)
				{
					channelCallback(Returned.Failed);
				}
				DebugLog (Debugs.Error, "ChaMsgError", "You are currently disconnected to the server!");
			}
		}

		IEnumerator SendingChannelMessage()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!channelReturned)
			{
				if (channelCallback != null)
				{
					channelCallback(Returned.Failed);
				}
				DebugLog (Debugs.Warn, "ChaMsgFailed", "Server did not respond to leave event!");
			}
		}

		private void OnChannelMessageSent(JSONObject jsonObject)
		{
			StopCoroutine (SendingChannelMessage());
			channelReturned = true;

			MsgCallback msgJson = JsonSerializer.ToObject<MsgCallback>(jsonObject.ToString());
			if(msgJson.msgStat == Returned.Success)
			{
				if(channelCallback != null)
				{
					channelCallback(Returned.Success);
				}
				DebugLog (Debugs.Log, "ChaMsgSuccess", "Successfully sent a lobby message!");
			}

			else
			{
				if(channelCallback != null)
				{
					channelCallback(Returned.Failed);
				}
				DebugLog (Debugs.Warn, "ChaMsgFailed", "Server did failed to send to peers.");
			}
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
			if (messageCallback != null)
			{
				messageCallback(msgJson);
			}
			DebugLog(Debugs.Log, "OnEvent: MESSAGE", "Sender: " + msgJson.sender + " Type: " + msgJson.msgtype + " Content: " + msgJson.content);
		}

		#endregion

		#region AutoChannel - Done!

		private bool autoChannelReturned = false;
		private Action<Returned, ChannelJson> autoChannelCallback = null;

		public void AutoMatchChannel(string variant, int maxconnect, Action<Returned, ChannelJson> _autoChannelCallback)
		{
			autoChannelCallback = _autoChannelCallback;
			autoChannelReturned = false;

			if (threadset.websocket.IsConnected)
			{
				ChannelJson channelJson = new ChannelJson(variant, maxconnect);
				string sendData = JsonUtility.ToJson(channelJson);
				SendEmit("auto", new JSONObject(sendData), OnAutoChannel);
				StartCoroutine(AutoingChannel());
			}

			else
			{
				if(autoChannelCallback != null)
				{
					autoChannelCallback(Returned.Error, null);
				}
				DebugLog (Debugs.Error, "AutoChannelError", "You are currently disconnected to the server!");
			}
		}

		private IEnumerator AutoingChannel()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!autoChannelReturned)
			{
				if (autoChannelCallback != null)
				{
					autoChannelCallback(Returned.Failed, null);
				}

				DebugLog (Debugs.Error, "AutoChannelFailed", "Server did not respond to leave event!");
			}
		}

		private void OnAutoChannel(JSONObject jsonObject)
		{
			StopCoroutine (AutoingChannel());
			autoChannelReturned = true;
			Subscribed = true;

			ChannelJson channelJson = JsonSerializer.ToObject<ChannelJson>(jsonObject.ToString());
			if(autoChannelCallback != null)
			{
				autoChannelCallback(Returned.Success, channelJson);
			}
			DebugLog (Debugs.Log, "AutoChannelSuccess", "Successfully auto match a channel!");
		}

		#endregion

		#region CreateChannel - Done!

		private Action<Returned, ChannelJson> createCallback = null; private bool createReturned = false;
		public void CreateChannel(string channelName, string variant, int maxconnect, Action<Returned, ChannelJson> _createCallback)
		{
			createCallback = _createCallback;
			createReturned = false;

			if (threadset.websocket.IsConnected)
			{
				ChannelJson channelJson = new ChannelJson(channelName, variant, maxconnect);
				string sendData = JsonUtility.ToJson(channelJson);
				SendEmit("create", new JSONObject(sendData), OnCreatedChannel);
				StartCoroutine(CreatingChannel());
			}

			else
			{
				if(createCallback != null)
				{
					createCallback(Returned.Error, new ChannelJson(string.Empty));
				}
				DebugLog (Debugs.Error, "CreateChannelError", "You are currently disconnected to the server!");
			}
		}

		IEnumerator CreatingChannel()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!createReturned)
			{
				if (createCallback != null)
				{
					createCallback(Returned.Failed, new ChannelJson(string.Empty));
				}
				DebugLog (Debugs.Warn, "CreateChannelFailed", "Server did not respond to leave event!");
			}
		}

		private void OnCreatedChannel(JSONObject jsonObject)
		{
			StopCoroutine (CreatingChannel());
			createReturned = true;

			ChanReturn channelJson = JsonSerializer.ToObject<ChanReturn>(jsonObject.ToString());
			if(channelJson.rt)
			{
				Subscribed = true;
				if(createCallback != null)
				{
					createCallback(Returned.Success, JsonUtility.FromJson<ChannelJson>(channelJson.ch) );
				}
				DebugLog (Debugs.Log, "CreateChannelSuccess", "Successfully create match a channel!");
			}

			else
			{
				if(createCallback != null)
				{
					createCallback(Returned.Failed, null);
				}
				DebugLog (Debugs.Warn, "CreateChannelFailed", "Channel name is already exist or full or not same variant!");
			}
		}

		#endregion

		#region JoinChannel - Done!

		private Action<Returned, ChannelJson> joinCallback = null;
		private bool joinReturned = false;

		public void JoinChannel(string channelName, string variant, Action<Returned, ChannelJson> _joinCallback)
		{
			joinCallback = _joinCallback;
			joinReturned = false;

			if (threadset.websocket.IsConnected)
			{
				ChannelJson channelJson = new ChannelJson(channelName, variant);
				string sendData = JsonUtility.ToJson(channelJson);
				SendEmit("join", new JSONObject(sendData), OnJoinedChannel);
				StartCoroutine(JoiningChannel());
			}

			else
			{
				if(joinCallback != null)
				{
					joinCallback(Returned.Error, new ChannelJson(string.Empty));
				}
				DebugLog (Debugs.Error, "JoinChannelError", "You are currently disconnected to the server!");
			}
		}

		IEnumerator JoiningChannel()
		{
			yield return new WaitForSeconds(bindings.connectDelay);

			if (!joinReturned)
			{
				if (joinCallback != null)
				{
					joinCallback(Returned.Failed, new ChannelJson(string.Empty));
				}
				DebugLog (Debugs.Warn, "JoinChannelFailed", "Server did not respond to leave event!");
			}
		}

		private void OnJoinedChannel(JSONObject jsonObject)
		{
			StopCoroutine (JoiningChannel());
			joinReturned = true;

			ChanReturn channelJson = JsonSerializer.ToObject<ChanReturn>(jsonObject.ToString());
			if(channelJson.rt)
			{
				Subscribed = true;
				if(joinCallback != null)
				{
					joinCallback(Returned.Success, JsonUtility.FromJson<ChannelJson>(channelJson.ch) );
				}
				DebugLog (Debugs.Log, "JoinChannelSuccess", "Successfully join match a channel!");
			}

			else
			{
				if(joinCallback != null)
				{
					joinCallback(Returned.Failed, null);
				}
				DebugLog (Debugs.Warn, "JoinChannelFailed", "Channel name is does not exist or full or not same variant!");
			}
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
				if(leaveCallback != null)
				{
					leaveCallback (Returned.Error, new ChannelJson(string.Empty));
				}
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
					leaveCallback(Returned.Failed, new ChannelJson(string.Empty));
				}
				DebugLog (Debugs.Warn, "LeaveChannelFailed", "Server did not respond to leave event!");
			}
		}

		private void OnLeaveMatch(JSONObject jsonObject)
		{
			StopCoroutine (LeavingMatch());
			leaveReturned = true;
			Subscribed = false;
			leaveCallback (Returned.Success, null);

			//Must only destroy peers that is assoc with this net.
			USocket.Instance.socketIdentities.ForEach ((USocketView sockView) => {
				Destroy(sockView.gameObject);
			});
			USocket.Instance.socketIdentities = new List<USocketView> ();

			//For each local socket view of this socket net must be destroy.
			localSockets.ForEach ((USocketView sockView) => {
				Destroy(sockView.gameObject);
			});
			localSockets = new List<USocketView> ();

			LeavedCallback leavedCb = JsonSerializer.ToObject<LeavedCallback>(jsonObject.ToString());
			if(leavedCb.result == LeavedResult.Updated)
			{
				DebugLog(Debugs.Log, "LeaveMatchSuccess", "You successfully leaved a match! Previous channel has been updated.");
			}

			else
			{
				DebugLog(Debugs.Log, "LeaveMatchSuccess", "You successfully leaved a match! Previous channel has been closed.");
			}
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

		private void OnChannelLeaved(SocketIOEvent _eventArgs)
		{
			PeerJson peerJson = JsonUtility.FromJson<PeerJson>(_eventArgs.data.ToString());

			//Destroy only socket views that is assoc in thi socket net.
			if(USocket.Instance.socketIdentities.Exists(x => x.Identity == peerJson.id))
			{
				//all instance of the socket net's current peer!
				USocket.Instance.socketIdentities.ForEach ((USocketView obj) => {
					if(obj.Identity == peerJson.id)
					{
						Destroy(obj.gameObject);
					}
				});
				USocket.Instance.socketIdentities.RemoveAll (x => x == null);
			}

			//Destroy all sockets views assoc in this socket net.
			//localSockets.ForEach ((USocketView sockView) => {
			//	Destroy(sockView.gameObject);
			//});
			//localSockets = new List<USocketView> ();

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
				string itcss = localSockets.Count + intance.GetHashCode ().ToString();
				intance.itc = itcss[1] + itcss[0] + itcss[1] + itcss[2] + "";

				string sendData = JsonUtility.ToJson(intance);
				SendEmit("instance", new JSONObject(sendData), OnInstantiate);
				StartCoroutine (InstantiatingInstance());
			}

			else
			{
				if (instanceCallback != null)
				{
					instanceCallback (Returned.Error);
				}
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
				}
				DebugLog (Debugs.Warn, "InstantiateFailed", "Server did not respond to instantiate event!");
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
			DebugLog(Debugs.Log, "OnEvent: INSTANCE LOCAL", "ID: " + instances.identity + " POS: " + instances.pos);
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
			USocket.Instance.synchRate = bindings.mainSendRate;
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
						SyncJson syncJson = usocket.GetViewData ();
						if(syncJson.states.Count > 0)
						{
							syncJsons.obj.Add (syncJson);
						}
					}
				}

				string sendData = "0";
				if(syncJsons.obj.Count > 0)
				{
					sendData = JsonUtility.ToJson (syncJsons);
				}

				USocket.Instance.uploadRate = sendData.Length;
				SendEmit ("transtate", new JSONObject (sendData), SynchingSockets);

				bindings.sendTimer = 0f;
			}
		}

		private void SynchingSockets(JSONObject jsonObject)
		{
			ChanUsers chanUsers = JsonSerializer.ToObject<ChanUsers>(jsonObject.ToString());
			USocket.Instance.downloadRate = jsonObject.ToString().Length;

			//SYNCHRONIZE AND INSTANTIATE!
			chanUsers.us.ForEach ((PeerJson peerJson) => 
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
					if(!chanUsers.us.Exists(x => x.id == toDestroy.Identity))
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
				});
		}

		#endregion

		#region TriggeringEvents - Ongoing!

		private Action<Returned> triggerCallback = null;
		private bool triggerReturned = false;

		public void SendTriggerEvent(TriggerJson tJson, Action<Returned> tCallback)
		{
			triggerCallback = tCallback;
			triggerReturned = false;

			if (threadset.websocket.IsConnected)
			{
				string sendData = JsonUtility.ToJson(tJson);
				SendEmit("trigger", new JSONObject(sendData), OnTriggerCallback);
				StartCoroutine(SendingTriggerEvent());
			}

			else
			{
				if(triggerCallback != null)
				{
					triggerCallback(Returned.Error);
				}
				DebugLog (Debugs.Error, "TriggerEventError", "You are currently disconnected to the server!");
			}
		}

		IEnumerator SendingTriggerEvent()
		{
			yield return new WaitForEndOfFrame();

			if (!triggerReturned)
			{
				if (!threadset.websocket.IsConnected)
				{
					if (triggerCallback != null)
					{
						triggerCallback (Returned.Failed);
					}
					DebugLog (Debugs.Error, "TriggerEventFailed", "The server did not respond to the event!");
				}
			}
		}

		private void OnTriggerCallback(JSONObject jsonObject)
		{
			StopCoroutine (SendingTriggerEvent());
			Returning returned = JsonSerializer.ToObject<Returning>(jsonObject.ToString());
			triggerReturned = true;

			if(triggerCallback != null)
			{
				triggerCallback(returned.returned);
			}
			DebugLog (Debugs.Log, "TriggerEventSuccess", "Successfully sent a event trigger!");
		}

		#endregion

		#region TriggeringESents - Ongoing!

		private Triggered triggerListens = null;
		public void ListenTriggersEvent(Triggered triggersListener)
		{
			triggerListens += triggersListener;
		}

		private void OnTriggersReceived(SocketIOEvent _eventArgs)
		{
			TriggerJson tJson = JsonUtility.FromJson<TriggerJson>(_eventArgs.data.ToString());

			if (triggerListens != null)
			{
				triggerListens(tJson);
			}
			DebugLog(Debugs.Log, "OnTriggerEvent: RECEIVED", "ID: " + tJson.itc + " Key: " + tJson.tKy +  " Content: " + tJson.tVl);
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
			CallbackOn("triggered", OnTriggersReceived);
			CallbackOn("leaved", OnChannelLeaved);
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
				if(Subscribed)
				{
					SynchingOutbound ();
					SynchingInbound ();
				}

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

		private void OnReceivedOpen(SocketIOEvent e)
		{
			DebugLog(Debugs.Warn, "[SocketIO]", " Open received: " + e.name + " " + e.data);
		}

		private void OnReceivedError(SocketIOEvent e)
		{
			DebugLog(Debugs.Warn, "[SocketIO]", " Error received: " + e.name + " " + e.data);
		}

		private void OnReceivedClose(SocketIOEvent e)
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

		#region EventInterface - Done!

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