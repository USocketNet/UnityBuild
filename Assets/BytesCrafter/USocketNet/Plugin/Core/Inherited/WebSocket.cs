
#region License
/*
 * BC_USN_WebSocket.cs
 *
 * Copyright (c) 2020 Bytes Crafter
 *
 * Permission is hereby granted to any person obtaining a copy from our store
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software with restriction to the rights to modify, merge, publish, 
 * distribute, sublicense, and/or sell copies of the Software, and to permit 
 * persons to whom the Software is furnished to do so, subject to the following 
 * conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System; 
using System.Collections;
using System.Collections.Generic;

using SocketIO;
using WebSocketSharp;
using System.Threading; 
using UnityEngine;

using BytesCrafter.USocketNet.Serials;
using BytesCrafter.USocketNet.Toolsets;

namespace BytesCrafter.USocketNet.Networks {
    public class BC_USN_WebSocket
    {
		public string getSocketId
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
                 if(threadset.websocket != null) {
					return threadset.websocket.IsConnected 
						&& threadset.websocket.IsAlive 
						&& threadset.websocket.ReadyState == WebSocketState.Open;
				} else {
					return false;
				}
            }
        }

		#region Initialization - Done! 
		
        private Threadset threadset = new Threadset();
		private QueueCoder queueCoder = new QueueCoder();
		private Thread socketThread;
		private Thread pingThread;

        public void Update() 
        {
			if (threadset.isInitialized)
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
				if (threadset.wsCheck != threadset.isWsConnected)
				{
					//Save the current websocket connetion status as bool.
					threadset.wsCheck = threadset.isWsConnected;

					//If websocket instance is currently connected.
					if (threadset.isWsConnected)
					{
						EmitEvent("connect");
						//curClient.OnConnection( true );
					}

					else
					{
						EmitEvent("disconnect");
						//curClient.OnDisconnection( true );
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

		#region Connection Mechanism
		private USNClient curClient = null;
        public void InitConnection(string appsecret, string port, USNClient usnClient, Action<ConStat> callback) 
        {
			curClient = usnClient;
			queueCoder.Starts();
			threadset.Start(appsecret, port, OnOpen, OnError, OnClose, OnPacket);

			AddCallback("open", OnReceivedOpen);
			AddCallback("close", OnReceivedClose);
			AddCallback("error", OnReceivedError);

			//AddCallback("disconnect", OnDisconnects);
			// AddCallback("reconnect", OnDisconnects);
			// AddCallback("reconnect_attempt", OnDisconnects);
			// AddCallback("reconnecting", OnDisconnects);

			// AddCallback("reconnect_error", OnDisconnects);
			// AddCallback("reconnect_failed", OnDisconnects);
			// AddCallback("reconnecting", OnDisconnects);

			// AddCallback("ping", OnDisconnects);
			//AddCallback("pong", Ponging);
			
			if(USocketNet.config.serverPort.GetServType(port) == ServType.Message) {
				AddCallback("pub", OnPublicMessage);
			}

            socketThread = new Thread(RunSocketThread);
            socketThread.Start(threadset.websocket);

            pingThread = new Thread(RunPingThread);
            pingThread.Start(threadset.websocket);

			USocketNet.Core.StartCoroutine(WaitingForSocketId(callback));
        }

		private Dictionary<string, List<Action<MsgJson>>> eventListeners = new Dictionary<string, List<Action<MsgJson>>>();
		public void ListensOnMessage(MsgType msgType, Action<MsgJson> listener) 
		{
			string mType = msgType.ToString();
			if (!eventListeners.ContainsKey(mType))
			{
				eventListeners[mType] = new List<Action<MsgJson>>();
			}

			eventListeners[mType].Add(listener);
		}

		private void OnPublicMessage(SocketIOEvent e)
		{
			string mType = MsgType.pub.ToString();
			if (!eventListeners.ContainsKey(mType))
				return;
			
			MsgJson msgJson = new MsgJson(JsonUtility.FromJson<MsgRaw>(e.data.ToString()));

			foreach (Action<MsgJson> handler in eventListeners[mType])
			{
				try {
					handler(msgJson);
				} catch (Exception except) {
					USocketNet.Log(Logs.Warn, "disable", "OnPublicMessage ->" + except.Message);
				}
			}
		}

		private void OnDisconnects(SocketIOEvent e)
		{
			//MsgJson msgJson = JsonUtility.FromJson<MsgJson>(_eventArgs.data.ToString());
			USocketNet.Log(Logs.Warn, "[OnDisconnects]", " OnDisconnects received: " + e.name + " " + e.data.ToString());
		}

		IEnumerator WaitingForSocketId(Action<ConStat> callback)
		{	
			DateTime timer = DateTime.Now;

			yield return new WaitUntil(() => (getSocketId != string.Empty && isConnected) || 
				DateTime.Now.Subtract(timer).TotalSeconds > USocketNet.config.connectTimeout );

			if(isConnected) {
				callback( ConStat.Success );
			} else {
				curClient.Destroys();
				callback( ConStat.Error );
			}
		}

        public void AbortConnection()
		{
			if (socketThread != null)
				socketThread.Abort();

			if (pingThread != null)
				pingThread.Abort();
		}

        public void ForceDisconnect()
		{
			if( threadset.isConnected )
			{
				EmitPacket(new Packet(EnginePacketType.MESSAGE, SocketPacketType.DISCONNECT, 0, "/", -1, new JSONObject("")));
				EmitPacket(new Packet(EnginePacketType.CLOSE));
				threadset.Close ();
			}

			queueCoder.Stops();
			threadset.Reset();
		}

		#endregion

		#region Listeners Callback - Done!

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
				USocketNet.Log(Logs.Warn, "CallbackOffMissing", "No callbacks registered for event: " + events);
				return;
			}

			List<Action<SocketIOEvent>> eventList = queueCoder.handlers[events];
			if (!eventList.Contains(callback))
			{
				USocketNet.Log(Logs.Warn, "CallbackOffCantRemove", "Couldn't remove callback action for event: " + events);
				return;
			}

			eventList.Remove(callback);

			if (eventList.Count == 0)
			{
				queueCoder.handlers.Remove(events);
			}
		}

		#endregion

        #region Socket Event - Done!

		private void OnReceivedOpen(SocketIOEvent e)
		{
			USocketNet.Log(Logs.Warn, "disable", " [SocketIO] Open received: " + e.name + " " + e.data.ToString());
		}

		private void OnReceivedError(SocketIOEvent e)
		{
			USocketNet.Log(Logs.Warn, "disable", " [SocketIO] Error received: " + e.name + " " + e.data.ToString());
		}

		private void OnReceivedClose(SocketIOEvent e)
		{	
			USocketNet.Log(Logs.Warn, "disable", " [SocketIO] Close received: " + e.name + " " + e.data.ToString());
		}

		private void OnOpen(object sender, EventArgs e)
		{ 
			EmitEvent("open");
			USocketNet.Log(Logs.Warn, "disable", "[CALLBACK OPEN] " + JsonUtility.ToJson(sender) + " - " + e.ToString());
		}

		private void OnError(object sender, ErrorEventArgs e)
		{
			EmitEvent("error");
			USocketNet.Log(Logs.Warn, "disable", "[CALLBACK ERROR] Message: " + e.Message);
		}

		private void OnClose(object sender, CloseEventArgs e)
		{
			EmitEvent("close");
			USocketNet.Log(Logs.Warn, "disable", "[CALLBACK CLOSE] Code: " + e.Code + " - Reason: " + e.Reason + " - WasClean: " + e.WasClean);
		}

		private void OnPacket(object sender, MessageEventArgs e)
		{
			Packet packet = queueCoder.decoder.Decode(e); 
			USocketNet.Log(Logs.Warn, "disable", packet.enginePacketType.ToString() +"-"+ JsonUtility.ToJson(packet.json));

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
			threadset.wsPinging = true; 
			threadset.wsPonging = false;		
			EmitPacket(new Packet(EnginePacketType.PING));

			threadset.socketId = packet.json["sid"].str;
			EmitEvent("open");
		}

		private void HandlePing()
		{
			EmitPacket(new Packet(EnginePacketType.PONG));
		}

		private DateTime pTimer;
		private int pValue = 0;
		public int getPingInMS {
			get {
				return pValue;
			}
		}

		private void HandlePong()
		{
			threadset.wsPonging = true;
			threadset.wsPinging = false;
			pValue = Convert.ToInt16(DateTime.Now.Subtract(pTimer).TotalMilliseconds);
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

        #region Threading Mechanism - Done!

		private void RunSocketThread(object obj)
		{
			WebSocket webSocket = (WebSocket)obj;

			while (threadset.isConnected)
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

			while (threadset.isConnected)
			{
				if (!threadset.isWsConnected)
				{
					Thread.Sleep(threadset.reconDelay);
				}

				else
				{
					pTimer = DateTime.Now;
					pingStart = DateTime.Now;

					threadset.wsPinging = true; 
					threadset.wsPonging = false;
					EmitPacket(new Packet(EnginePacketType.PING));

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

        #region Emit Listeners - Done!

		public void SendEmit(string events)
		{
			EmitMessage(-1, string.Format("[\"{0}\"]", events));
		}

		public void SendEmit(string events, Action<JSONObject> action)
		{
			EmitMessage(++threadset.packetId, string.Format("[\"{0}\"]", events));
			queueCoder.ackList.Add(new Ack(threadset.packetId, action));
		}

		public void SendEmit(string events, JSONObject data)
		{
			EmitMessage(-1, string.Format("[\"{0}\",{1}]", events, data));
		}

		public void SendEmit(string events, JSONObject data, Action<JSONObject> action)
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
			USocketNet.Log(Logs.Warn, "EmitEventReceived", eventArgs.name + " event = " + " data: " + eventArgs.ToString());

			if (!queueCoder.handlers.ContainsKey(eventArgs.name))
				return;

			foreach (Action<SocketIOEvent> handler in queueCoder.handlers[eventArgs.name])
			{
				try {
					handler(eventArgs);
				} catch (Exception except) {
					USocketNet.Log(Logs.Warn, "disable", "EmitEventCatch ->" + except.Message);
				}
			}
		}

		private void EmitMessage(int id, string raw)
		{
			EmitPacket(new Packet(EnginePacketType.MESSAGE, SocketPacketType.EVENT, 0, "/", id, new JSONObject(raw)));
		}

		private void EmitPacket(Packet packet)
		{
			try {
				threadset.Send(queueCoder.encoder.Encode(packet));
			} catch (SocketIOException ex) {
				if (ex != null) {
					USocketNet.Log(Logs.Warn, "EmitPacketCatch", ex.Message);
				}
			}
		}

		#endregion
    }
}