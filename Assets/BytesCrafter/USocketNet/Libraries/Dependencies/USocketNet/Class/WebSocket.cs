using System; 
using System.Collections;
using System.Collections.Generic;

using SocketIO;
using WebSocketSharp;
using System.Threading; 
using UnityEngine;

using BytesCrafter.USocketNet.Toolsets;

namespace BytesCrafter.USocketNet.Networks {
    public class BC_USN_WebSocket
    {
        private USocketClient usnClient = null;
        public BC_USN_WebSocket( USocketClient reference )
        {
            usnClient = reference;
        }

		public string socketId
		{
			get
			{
				return threadset.socketId;
			}
		}

		public bool isConnected 
		{
            get 
			{
                return threadset.IsConnected && threadset.websocket.IsConnected;
            }

            set 
			{
                threadset.IsConnected = value;
            }
        }

		#region Initialization - Done! 
		
        private Threadset threadset = new Threadset();
		private QueueCoder queueCoder = new QueueCoder();
		private Thread socketThread;
		private Thread pingThread;

        public void Update() 
        {
            if(threadset.websocket == null)
				return;

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
						EmitEvent("connect");
						usnClient.OnConnect( true );
					}

					else
					{
						EmitEvent("disconnect");
						usnClient.OnDisconnect( true );
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

		#region Listeners Callback

		private void AddCallback(string events, Action<SocketIOEvent> callback)
		{
			if (!queueCoder.handlers.ContainsKey(events))
			{
				queueCoder.handlers[events] = new List<Action<SocketIOEvent>>();
			}

			queueCoder.handlers[events].Add(callback);
		}

		private void RemoveCallback(string events, Action<SocketIOEvent> callback)
		{
			if (!queueCoder.handlers.ContainsKey(events))
			{
				usnClient.Debug(BC_USN_Debug.Warn, "CallbackOffMissing", "No callbacks registered for event: " + events);
				return;
			}

			List<Action<SocketIOEvent>> eventList = queueCoder.handlers[events];
			if (!eventList.Contains(callback))
			{
				usnClient.Debug(BC_USN_Debug.Warn, "CallbackOffCantRemove", "Couldn't remove callback action for event: " + events);
				return;
			}

			eventList.Remove(callback);

			if (eventList.Count == 0)
			{
				queueCoder.handlers.Remove(events);
			}
		}

		#endregion

		#region Connection Mechanism

        public void InitConnection(Action<ConStat> callback) 
        {
			threadset.IsInitialized = true;
			queueCoder.Starts();
			
			//WEB SOCKET INITIALIZATION	
			string hostUrl = usnClient.options.serverUrl + ":" + usnClient.options.serverPort;
			string sioPath = "/socket.io/?EIO=4&transport=websocket";
			string usrTok = "&wpid=" + usnClient.GetToken.wpid + "&snid=" + usnClient.GetToken.snid;

			threadset.websocket = new WebSocket("ws://" + hostUrl + sioPath + usrTok);
			threadset.websocket.OnOpen += OnOpen;
			threadset.websocket.OnError += OnError;
			threadset.websocket.OnClose += OnClose;
			threadset.websocket.OnMessage += OnPacket;

			threadset.wsCheck = false;
			threadset.IsConnected = true;
			threadset.packetId = 0;
			threadset.socketId = string.Empty;
			threadset.autoConnect = true;

			// AddCallback("open", OnReceivedOpen);
			// AddCallback("close", OnReceivedClose);
			// AddCallback("error", OnReceivedError);

            socketThread = new Thread(RunSocketThread);
            socketThread.Start(threadset.websocket);

            pingThread = new Thread(RunPingThread);
            pingThread.Start(threadset.websocket);

			usnClient.StartCoroutine(WaitingForSocketId(callback));
        }

		IEnumerator WaitingForSocketId(Action<ConStat> callback)
		{
			yield return new WaitUntil(() => threadset.socketId != string.Empty);

			callback( ConStat.Success );
		}

        public void AbortConnection()
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

        public void ForceDisconnect()
		{
			if( threadset.IsInitialized )
			{
				EmitPacket(new Packet(EnginePacketType.MESSAGE, SocketPacketType.DISCONNECT, 0, "/", -1, new JSONObject("")));
				EmitPacket(new Packet(EnginePacketType.CLOSE));
				threadset.websocket.Close ();
			}

			threadset.IsInitialized = false;
			queueCoder.Stops();
			threadset.websocket = null;
			threadset.wsCheck = false;
			threadset.IsConnected = false;
			threadset.packetId = 0;
			threadset.socketId = string.Empty;
			threadset.autoConnect = false;
		}

		#endregion

        #region Socket Event

		private void OnReceivedOpen(SocketIOEvent e)
		{
			usnClient.Debug(BC_USN_Debug.Warn, "[SocketIO]", " Open received: " + e.name + " " + e.data);
		}

		private void OnReceivedError(SocketIOEvent e)
		{
			usnClient.Debug(BC_USN_Debug.Warn, "[SocketIO]", " Error received: " + e.name + " " + e.data);
		}

		private void OnReceivedClose(SocketIOEvent e)
		{	
			usnClient.Debug(BC_USN_Debug.Warn, "[SocketIO]", " Close received: " + e.name + " " + e.data);
		}

		private void OnOpen(object sender, EventArgs e)
		{ 
			EmitEvent("open");
		}

		private void OnError(object sender, ErrorEventArgs e)
		{
			EmitEvent("error");
		}

		private void OnClose(object sender, CloseEventArgs e)
		{
			EmitEvent("close");
		}

		private void OnPacket(object sender, MessageEventArgs e)
		{
			Packet packet = queueCoder.decoder.Decode(e); 

			switch (packet.enginePacketType)
			{
				case EnginePacketType.OPEN: 
					HandleOpen(packet);
					break;
				case EnginePacketType.CLOSE: 
					EmitEvent("close"); 
					break;
				case EnginePacketType.PING: 
					HandlePing (); 
					break;
				case EnginePacketType.PONG: 
					HandlePong(); 
					break;
				case EnginePacketType.MESSAGE: 
					HandleMessage(packet); 
					break;
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
			if (packet.json == null)
				return;

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

        #region Threading Mechanism

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

		private void RunPingThread(object obj)
		{
			WebSocket webSocket = (WebSocket)obj; 
			DateTime pingStart;

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
					threadset.wsPinging = true; 
					threadset.wsPonging = false;
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

        #region Emit Listeners

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
					usnClient.Debug(BC_USN_Debug.Warn, "EmitEventCatch", except.Message);
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
					usnClient.Debug(BC_USN_Debug.Warn, "EmitPacketCatch", ex.Message);
				}
			}
		}

		#endregion
    }
}