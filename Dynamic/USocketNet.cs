using System; 
using System.Threading; 
using System.Collections; 
using System.Collections.Generic;

using SocketIO;
using WebSocketSharp;
using UnityEngine;

namespace BytesCrafter.USocketNet
{
    public class USocketNet : MonoBehaviour
    {
        #region ExposedInterface

        [Header("NETWORK SETTINGS")]
        public Bindings bindings = new Bindings();

        [Header("INSTANCE PREFAB")]
        public List<SocketView> socketPrefabs = new List<SocketView>();

        [Header("SERVER MESSAGE")]
        public string debugLog = string.Empty;

        #endregion

        #region CheckerInterface

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

        public int PingServer
        {
            get
            {
                return pingValue;
            }
        }

        #endregion

        #region PingMechanism

        private int pingValue = 0;
        /*
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
            timer = 1.2f;
        }
        */

        #endregion

        #region CallbackEvents // ------------------------------------- SERVER AUTHORITATIVE EVENTS! ------------------------------------ //

        private MessageCallback messageCallback = null;
        private void OnMessageReceived(SocketIOEvent _eventArgs)
        {
            MsgJson msgJson = JsonUtility.FromJson<MsgJson>(_eventArgs.data.ToString());
            Debug.Log("OnEvent:" + "MESSAGE: " + " Sender: " + msgJson.sender + " Content: " + msgJson.content + " Receiver: " + msgJson.content + " Type: " + msgJson.content);
            if (messageCallback != null) { messageCallback(msgJson); }
        }

        private RoomCallback joinedCallback = null;
        private void OnRoomJoined(SocketIOEvent _eventArgs)
        {
            RoomJson roomJson = JsonUtility.FromJson<RoomJson>(_eventArgs.data.ToString());
            Debug.Log("OnEvent:" + "JOINED: " + " ID: " + roomJson.identity + " NAME: " + roomJson.cname + " VAR: " + roomJson.variant + " MAX: " + roomJson.maxconnect + " CUR: " + roomJson.users.Length);
            if (joinedCallback != null) { joinedCallback(roomJson); }
        }

        private RoomCallback leavedCallback = null;
        private void OnRoomLeaved(SocketIOEvent _eventArgs)
        {
            RoomJson roomJson = JsonUtility.FromJson<RoomJson>(_eventArgs.data.ToString());
            Debug.Log("OnEvent:" + "LEAVE: " + " ID: " + roomJson.identity + " NAME: " + roomJson.cname + " VAR: " + roomJson.variant + " MAX: " + roomJson.maxconnect + " CUR: " + roomJson.users.Length);
            if (leavedCallback != null) { leavedCallback(roomJson); }
        }

        private List<SocketView> socketIdentities = new List<SocketView>();
        private void OnInstancePeer(SocketIOEvent _eventArgs)
        {
            Debug.Log("OnRoomEvent: " + "Instance from Peers!" + " Content: " + _eventArgs.data.ToString());
            Instances instances = JsonUtility.FromJson<Instances>(_eventArgs.data.ToString());

            SocketView curUser = Instantiate(socketPrefabs[instances.prefabIndex], instances.pos.ToVector, instances.rot.ToQuaternion);
            curUser.IsLocalUser = false; curUser.socketId = instances.identity; curUser.uSocketNet = this;
            socketIdentities.Add(curUser);
        }

        private void OnStateReceived(SocketIOEvent _eventArgs)
        {
            if (_eventArgs.data.keys.Exists(x => x == "identity"))
            {
                string identity = ""; _eventArgs.data.ToDictionary().TryGetValue("identity", out identity);

                if (socketIdentities.Exists(x => x.socketId == identity && x.IsLocalUser == false))
                {
                    SocketView sockId = socketIdentities.Find(x => x.socketId == identity);
                    sockId.Downdate(_eventArgs.data.ToString());
                }
            }
        }

        #endregion

        #region ConnectServer // ------------------------------------------ CONNECTING TO THE SERVER! ------------------------------------------ //

        private Action<ConnStat, ConnJson> connectCallback = null; private bool connectReturn = false;
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

            else { connectCallback(ConnStat.Connected, new ConnJson()); Debug.LogWarning("Connection failed: Already connected to the server!"); }
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
                if (!connectReturn) { connectCallback(ConnStat.InternetAccess, new ConnJson()); }
            }

            else
            {
                ForceDisconnect(); connectCallback(ConnStat.Maintainance, new ConnJson());
            }
        }

        private void OnServerConnect(JSONObject jsonObject)
        {
            StopCoroutine("ConnectingToServer"); connectReturn = true;
            ConnJson connJson = JsonSerializer.ToObject<ConnJson>(jsonObject.ToString());
            lastUsername = connJson.username;
            connectCallback(ConnStat.Connected, connJson);
        }

        #endregion

        #region DisconnectServer // ---------------------------------------- DISCONNECTING FROM THE SERVER! --------------------------------------- //

        private Action<ConnStat> disconnectCallback = null; private bool disconnectReturn = false;
        public void DisconnectFromServer(Action<ConnStat> disconCallback)
        {
            disconnectCallback = disconCallback; disconnectReturn = false;

            if (threadset.websocket.IsConnected)
            {
                StartCoroutine(DisconnectingToServer());
            }

            else { disconnectCallback(ConnStat.Maintainance); Debug.LogWarning("Disconnection failed: Not connected to the server!"); }
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
            StopCoroutine("DisconnectingToServer"); disconnectReturn = true;
            ForceDisconnect(); disconnectCallback(ConnStat.Disconnected); pingValue = 0;
        }

        private void OnUserRejected(SocketIOEvent _eventArgs)
        {
            ConnJson connJson = JsonUtility.FromJson<ConnJson>(_eventArgs.data.ToString());
            ForceDisconnect(); disconnectCallback(ConnStat.Disconnected); pingValue = 0;
        }

        #endregion

        #region ReconnectServer // ----------------------------------------- RECONNECTING FROM THE SERVER! --------------------------------------- //

        private Action<ConnStat, ConnJson> connectionStats = null; private bool onListeningReturn = false;

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
                if (connectionStats != null) { connectionStats(connStat, null); }
                else { Debug.LogWarning(connStat.ToString() + " - Add a callback to ListenConnectionStatus. Currently Null!"); }
            }
        }

        private void OnServerReconnect(JSONObject jsonObject)
        {
            onListeningReturn = true; ConnJson connJson = JsonSerializer.ToObject<ConnJson>(jsonObject.ToString());
            if (connectionStats != null) { connectionStats(ConnStat.Reconnected, connJson); }
            else { Debug.LogWarning("Reconnected - Add a callback to ListenConnectionStatus. Currently Null!"); }
        }

        #endregion

        #region LobbyMessage // ----------------------------------------- SENDING PUBLIC MESSAGE! --------------------------------------- //

        private Action<MsgStat, MsgJson> lobbyCallback = null; private bool lobbyReturned = false;
        public void SendLobbyMessage(string message, Action<MsgStat, MsgJson> _lobbyCallback)
        {
            lobbyCallback = _lobbyCallback; lobbyReturned = false;

            if (threadset.websocket.IsConnected)
            {
                MsgJson msgJson = new MsgJson(message, Sending.Lobby.ToString());
                string sendData = JsonUtility.ToJson(msgJson);
                SendEmit("message", new JSONObject(sendData), OnLobbyMessageSent);
                StartCoroutine(SendingLobbyMessage());
            }

            else
            {
                MsgJson msgJson = new MsgJson();
                lobbyCallback(MsgStat.Failed, msgJson);
                Debug.LogWarning("Lobby Message Failed: Not connected to the server!");
            }
        }

        IEnumerator SendingLobbyMessage()
        {
            yield return new WaitForSeconds(bindings.connectDelay);

            if (!lobbyReturned)
            {
                MsgJson msgJson = new MsgJson(); if (!threadset.websocket.IsConnected)
                { lobbyCallback(MsgStat.Failed, msgJson); }
            }
        }

        private void OnLobbyMessageSent(JSONObject jsonObject)
        {
            MsgJson msgJson = JsonSerializer.ToObject<MsgJson>(jsonObject.ToString());
            lobbyReturned = true; lobbyCallback(MsgStat.Success, msgJson);
        }

        #endregion

        #region RoomMessage // ------------------------------------ SENDING ROOM MESSAGE! ---------------------------------- //

        private Action<MsgStat, MsgJson> roomCallback = null; private bool roomReturned = false;
        public void SendRoomMessage(string message, Action<MsgStat, MsgJson> _roomCallback)
        {
            roomCallback = _roomCallback; roomReturned = false;

            if (threadset.websocket.IsConnected)
            {
                MsgJson msgJson = new MsgJson(message, Sending.Channel.ToString());
                string sendData = JsonUtility.ToJson(msgJson);
                SendEmit("message", new JSONObject(sendData), OnRoomMessageSent);
                StartCoroutine(SendingRoomMessage());
            }

            else
            {
                MsgJson msgJson = new MsgJson();
                roomCallback(MsgStat.Failed, msgJson);
                Debug.LogWarning("Room Message Failed: Not connected to the server!");
            }
        }

        IEnumerator SendingRoomMessage()
        {
            yield return new WaitForSeconds(bindings.connectDelay);

            if (!roomReturned)
            {
                MsgJson msgJson = new MsgJson(); if (!threadset.websocket.IsConnected)
                { roomCallback(MsgStat.Failed, msgJson); }
            }
        }

        private void OnRoomMessageSent(JSONObject jsonObject)
        {
            MsgJson msgJson = JsonSerializer.ToObject<MsgJson>(jsonObject.ToString());
            roomReturned = true; roomCallback(MsgStat.Success, msgJson);
        }

        #endregion

        #region PrivateMessage // ---------------------------------------- SENDING PRIVATE MESSAGE! --------------------------------------- //

        private Action<MsgStat, MsgJson> privateMessage = null; private bool privateReturned = false;
        public void SendPrivateMessage(string receiver, string message, Action<MsgStat, MsgJson> _privateMessage)
        {
            privateMessage = _privateMessage; privateReturned = false;

            if (threadset.websocket.IsConnected)
            {
                MsgJson msgJson = new MsgJson(message, Sending.Socket.ToString(), receiver);
                string sendData = JsonUtility.ToJson(msgJson);
                SendEmit("message", new JSONObject(sendData), OnPrivateMessageSent);
                StartCoroutine(SendingPrivateMessage());
            }

            else
            {
                MsgJson msgJson = new MsgJson();
                privateMessage(MsgStat.Failed, msgJson);
                Debug.LogWarning("Private Message Failed: Not connected to the server!");
            }
        }

        IEnumerator SendingPrivateMessage()
        {
            yield return new WaitForSeconds(bindings.connectDelay);

            if (!privateReturned)
            {
                MsgJson msgJson = new MsgJson(); if (!threadset.websocket.IsConnected)
                { privateMessage(MsgStat.Failed, msgJson); }
            }
        }

        private void OnPrivateMessageSent(JSONObject jsonObject)
        {
            MsgJson msgJson = JsonSerializer.ToObject<MsgJson>(jsonObject.ToString());
            privateReturned = true; privateMessage(MsgStat.Failed, msgJson);
        }

        #endregion

        #region AutoChannel // ------------------------------------------- AUTO JOIN CHANNEL OR ROOM! ------------------------------------------ //

        private Action<RoomJson> autoRoomCallback = null; private bool autoReturned = false;
        public void AutoJoinServerRoom(string variant, int maxconnect, Action<RoomJson> autoCallback)
        {
            autoRoomCallback = autoCallback; autoReturned = false;

            if (threadset.websocket.IsConnected)
            {
                RoomJson roomJson = new RoomJson(variant, maxconnect);
                string sendData = JsonUtility.ToJson(roomJson);
                SendEmit("auto", new JSONObject(sendData), OnAutoServerRoom);
                StartCoroutine(AutoServerRoom(roomJson));
            }

            else
            {
                Debug.LogWarning("Private Lobby Message Failed: Not connected to the server!");
            }
        }

        IEnumerator AutoServerRoom(RoomJson roomJson)
        {
            yield return new WaitForEndOfFrame(); //yield return new WaitForSeconds (approaches.callbackWait);
            if (!autoReturned)
            {
                if (!threadset.websocket.IsConnected) { autoRoomCallback(roomJson); }
            }
        }

        private void OnAutoServerRoom(JSONObject jsonObject)
        {
            RoomJson roomJson = JsonSerializer.ToObject<RoomJson>(jsonObject.ToString());
            autoReturned = true; autoRoomCallback(roomJson);
        }

        #endregion

        #region CreateChannel // ------------------------------------------- CREATING CHANNEL OR ROOM! ------------------------------------------ //

        private Action<RoomJson> createCallback = null; private bool createReturned = false;
        public void CreateServerRoom(string channelName, string variant, int maxconnect, Action<RoomJson> _createCallback)
        {
            createCallback = _createCallback; createReturned = false;

            if (threadset.websocket.IsConnected)
            {
                RoomJson roomJson = new RoomJson(channelName, variant, maxconnect);
                string sendData = JsonUtility.ToJson(roomJson);
                SendEmit("create", new JSONObject(sendData), OnCreatedServerRoom);
                StartCoroutine(CreatingServerRoom(roomJson));
            }

            else
            {
                Debug.LogWarning("Lobby Message Failed: Not connected to the server!");
            }
        }

        IEnumerator CreatingServerRoom(RoomJson roomJson)
        {
            yield return new WaitForEndOfFrame(); //yield return new WaitForSeconds (approaches.callbackWait);
            if (!createReturned)
            {
                if (!threadset.websocket.IsConnected) { createCallback(roomJson); }
            }
        }

        private void OnCreatedServerRoom(JSONObject jsonObject)
        {
            RoomJson roomJson = JsonSerializer.ToObject<RoomJson>(jsonObject.ToString());
            createReturned = true; createCallback(roomJson);
        }

        #endregion

        #region JoinChannel // ------------------------------------------- JOINING CHANNEL OR ROOM! ------------------------------------------ //

        private Action<RoomJson> joinCallback = null; private bool joinReturned = false;
        public void JoinServerRoom(string channelId, Action<RoomJson> _joinCallback)
        {
            joinCallback = _joinCallback; joinReturned = false;

            if (threadset.websocket.IsConnected)
            {
                RoomJson roomJson = new RoomJson(channelId);
                string sendData = JsonUtility.ToJson(roomJson);
                SendEmit("join", new JSONObject(sendData), OnJoinedServerRoom);
                StartCoroutine(JoiningServerRoom(roomJson));
            }
            else { Debug.Log("Disconnection failed: You are not connected to the server!"); }
        }

        IEnumerator JoiningServerRoom(RoomJson roomJson)
        {
            yield return new WaitForEndOfFrame();
            if (!joinReturned)
            {
                if (joinCallback != null) { joinCallback(null); }
            }
        }

        private void OnJoinedServerRoom(JSONObject jsonObject)
        {
            joinReturned = true;
            RoomJson roomJson = JsonSerializer.ToObject<RoomJson>(jsonObject.ToString());
            if (joinCallback != null) { joinCallback(roomJson); }
        }

        #endregion

        #region LeaveChannel // ------------------------------------------- LEAVING CHANNEL OR ROOM! ------------------------------------------ //

        public void LeaveServerRoom()
        {
            if (threadset.websocket.IsConnected)
            {
                SendEmit("leave", OnLeaveServerRoom);
            }
            else { Debug.Log("Disconnection failed: You are not connected to the server!"); }
        }

        private void OnLeaveServerRoom(JSONObject jsonObject)
        {
            Debug.Log("Room Leaved: " + jsonObject.ToString());
        }

        #endregion

        #region OngoingCodes // ----------------------------------------- SENDING TRANSFORM ROOM VALUE! --------------------------------------- //

        public void StateSync(StateJson stateJson)
        {
            string sendData = JsonUtility.ToJson(stateJson);
            SendEmit("transtate", new JSONObject(sendData));
        }

        public void Instantiate(int prefabIndex, Vector3 position, Quaternion rotation)
        {
            Instances intance = new Instances(prefabIndex, position, rotation.eulerAngles);
            string sendData = JsonUtility.ToJson(intance);
            SendEmit("instance", new JSONObject(sendData), OnInstantiateThis);
        }

        private void OnInstantiateThis(JSONObject jsonObject)
        {
            Debug.Log("OnRoomEvent: " + "Instance from Local! " + " Content: " + jsonObject.ToString());
            Instances instances = JsonSerializer.ToObject<Instances>(jsonObject.ToString());

            SocketView curUser = Instantiate(socketPrefabs[instances.prefabIndex], instances.pos.ToVector, instances.rot.ToQuaternion);
            curUser.IsLocalUser = true; curUser.socketId = instances.identity; curUser.uSocketNet = this;
            socketIdentities.Add(curUser);
        }

        #endregion

        #region UnityInterface

        //Initialized custom websocket approaches.
        void Awake()
        {
            DontDestroyOnLoad(this); //Prevent destroying the instance of this script.

            //- CUSTOM INITIALIZATION - CUSTOM INITIALIZATION - CUSTOM INITIALIZATION - CUSTOM INITIALIZATION - CUSTOM INITIALIZATION -//
            threadset.IsInitialized = true; Application.runInBackground = bindings.runOnBackground;
            //- CUSTOM INITIALIZATION - CUSTOM INITIALIZATION - CUSTOM INITIALIZATION - CUSTOM INITIALIZATION - CUSTOM INITIALIZATION -//

            //- ENCRYPTIONS & HANDLERS - ENCRYPTIONS & HANDLERS - ENCRYPTIONS & HANDLERS - ENCRYPTIONS & HANDLERS - ENCRYPTIONS & HANDLERS -//
            queueCoder.encoder = new Encoder(); queueCoder.decoder = new Decoder(); queueCoder.parser = new Parser();
            queueCoder.handlers = new Dictionary<string, List<Action<SocketIOEvent>>>(); queueCoder.eventQueue = new Queue<SocketIOEvent>();
            queueCoder.eventQueueLock = new object(); queueCoder.ackQueue = new Queue<Packet>();
            queueCoder.ackQueueLock = new object(); queueCoder.ackList = new List<Ack>();
            //- ENCRYPTIONS & HANDLERS - ENCRYPTIONS & HANDLERS - ENCRYPTIONS & HANDLERS - ENCRYPTIONS & HANDLERS - ENCRYPTIONS & HANDLERS -//

            //- WEB SOCKET INITIALIZATION - WEB SOCKET INITIALIZATION - WEB SOCKET INITIALIZATION - WEB SOCKET INITIALIZATION - WEB SOCKET INITIALIZATION - //
            threadset.websocket = new WebSocket("ws://" + bindings.serverUrl + ":" + bindings.serverPort + "/socket.io/?EIO=4&transport=websocket");
            threadset.websocket.OnOpen += OnOpen; threadset.websocket.OnError += OnError; threadset.websocket.OnClose += OnClose;
            threadset.websocket.OnMessage += OnMessage; threadset.wsCheck = false; threadset.IsConnected = false;
            threadset.packetId = 0; threadset.socketId = null; threadset.autoConnect = true;
            //- WEB SOCKET INITIALIZATION - WEB SOCKET INITIALIZATION - WEB SOCKET INITIALIZATION - WEB SOCKET INITIALIZATION - WEB SOCKET INITIALIZATION - //

            //- LISTENERS AND EVENTS - LISTENERS AND EVENTS - LISTENERS AND EVENTS - LISTENERS AND EVENTS - LISTENERS AND EVENTS - LISTENERS AND EVENTS -//
            CallbackOn("disconnected", OnUserRejected);

            CallbackOn("messaged", OnMessageReceived);
            CallbackOn("joined", OnRoomJoined);
            CallbackOn("leaved", OnRoomLeaved);

            CallbackOn("instanced", OnInstancePeer);
            CallbackOn("transtated", OnStateReceived);

            //ADD MORE HERE!

            //- LISTENERS AND EVENTS - LISTENERS AND EVENTS - LISTENERS AND EVENTS - LISTENERS AND EVENTS - LISTENERS AND EVENTS - LISTENERS AND EVENTS -//
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
            if (socketThread != null) { socketThread.Abort(); }
            if (pingThread != null) { pingThread.Abort(); }
        }

        void Update()
        {
            //Pinging();

            if (threadset.IsInitialized)
            {
                lock (queueCoder.eventQueueLock)
                {
                    while (queueCoder.eventQueue.Count > 0) { EmitEvent(queueCoder.eventQueue.Dequeue()); }
                }

                lock (queueCoder.ackQueueLock)
                {
                    while (queueCoder.ackQueue.Count > 0)
                    {
                        Packet packet = queueCoder.ackQueue.Dequeue();
                        Ack ack; for (int i = 0; i < queueCoder.ackList.Count; i++)
                        {
                            if (queueCoder.ackList[i].packetId != packet.id) { continue; }
                            ack = queueCoder.ackList[i]; queueCoder.ackList.RemoveAt(i); ack.Invoke(packet.json); return;
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
                            StartCoroutine(OnListeningConnectionStatus(ConnStat.Connected)); threadset.autoConnect = true;
                        }
                    }

                    else
                    {
                        EmitEvent("disconnect");

                        if (threadset.autoConnect) { StartCoroutine(OnListeningConnectionStatus(ConnStat.Maintainance)); } //User disconnection.
                        else { StartCoroutine(OnListeningConnectionStatus(ConnStat.Disconnected)); } //Server disconnection.
                    }
                }

                if (queueCoder.ackList.Count == 0) { return; }
                if (DateTime.Now.Subtract(queueCoder.ackList[0].time).TotalSeconds < threadset.ackExpireTime)
                { return; }
                queueCoder.ackList.RemoveAt(0);
            }
        }

        #endregion

        #region WebSocketThreading

        private Threadset threadset = new Threadset();
        private QueueCoder queueCoder = new QueueCoder();

        private Thread socketThread;
        private void RunSocketThread(object obj)
        {
            WebSocket webSocket = (WebSocket)obj;

            while (threadset.IsConnected)
            {
                if (webSocket.IsConnected) { Thread.Sleep(threadset.reconDelay); }
                else { webSocket.Connect(); }
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

        private void OnOpen(object sender, EventArgs e) { EmitEvent("open"); }
        private void OnError(object sender, ErrorEventArgs e) { EmitEvent("error"); }
        private void OnClose(object sender, CloseEventArgs e) { EmitEvent("close"); }
        private void OnMessage(object sender, MessageEventArgs e)
        {
            Packet packet = queueCoder.decoder.Decode(e); switch (packet.enginePacketType)
            {
                case EnginePacketType.OPEN: HandleOpen(packet); break;
                case EnginePacketType.CLOSE: EmitEvent("close"); break;
                case EnginePacketType.PING: HandlePing(); break;
                case EnginePacketType.PONG: HandlePong(); break;
                case EnginePacketType.MESSAGE: HandleMessage(packet); break;
            }
        }

        private void HandleOpen(Packet packet) { threadset.socketId = packet.json["sid"].str; EmitEvent("open"); }
        private void HandlePing() { EmitPacket(new Packet(EnginePacketType.PONG)); }
        private void HandlePong() { threadset.wsPonging = true; threadset.wsPinging = false; }
        private void HandleMessage(Packet packet)
        {
            if (packet.json == null) { return; }

            if (packet.socketPacketType == SocketPacketType.ACK)
            {
                for (int i = 0; i < queueCoder.ackList.Count; i++)
                {
                    if (queueCoder.ackList[i].packetId != packet.id) { continue; }
                    lock (queueCoder.ackQueueLock) { queueCoder.ackQueue.Enqueue(packet); }
                    return;
                }
            }

            if (packet.socketPacketType == SocketPacketType.EVENT)
            {
                SocketIOEvent e = queueCoder.parser.Parse(packet.json);
                lock (queueCoder.eventQueueLock) { queueCoder.eventQueue.Enqueue(e); }
            }
        }

        #endregion

        #region EmitterInterface

        private void EmitEvent(string type)
        {
            EmitEvent(new SocketIOEvent(type));
        }

        private void EmitEvent(SocketIOEvent eventArgs)
        {
            if (!queueCoder.handlers.ContainsKey(eventArgs.name)) { return; }
            foreach (Action<SocketIOEvent> handler in queueCoder.handlers[eventArgs.name])
            { try { handler(eventArgs); } catch (Exception except) { /*Debug.LogWarning(except.Message);*/ debugLog = except.Message; } }
        }


        private void EmitMessage(int id, string raw)
        {
            EmitPacket(new Packet(EnginePacketType.MESSAGE, SocketPacketType.EVENT, 0, "/", id, new JSONObject(raw)));
        }

        private void EmitPacket(Packet packet)
        {
            try { threadset.websocket.Send(queueCoder.encoder.Encode(packet)); }
            catch (SocketIOException ex) { if (ex != null) { } }
        }

        #endregion

        #region MethodsInterface

        private void CallbackOn(string events, Action<SocketIOEvent> callback)
        {
            if (!queueCoder.handlers.ContainsKey(events)) { queueCoder.handlers[events] = new List<Action<SocketIOEvent>>(); }
            queueCoder.handlers[events].Add(callback);
        }

        private void CallbackOff(string events, Action<SocketIOEvent> callback)
        {
            if (!queueCoder.handlers.ContainsKey(events))
            { Debug.LogWarning("No callbacks registered for event: " + events); debugLog = events; return; }

            List<Action<SocketIOEvent>> eventList = queueCoder.handlers[events];
            if (!eventList.Contains(callback))
            { Debug.LogWarning("Couldn't remove callback action for event: " + events); debugLog = events; return; }

            eventList.Remove(callback); if (eventList.Count == 0) { queueCoder.handlers.Remove(events); }
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