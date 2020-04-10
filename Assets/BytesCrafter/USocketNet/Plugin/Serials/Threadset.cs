
#region License
/*
 * cs
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
using WebSocketSharp;

namespace BytesCrafter.USocketNet.Networks
{
	[System.Serializable]
	public class Threadset
	{
		private WebSocket curWebsocket = null; //An instance of Web Socket for connection.

		public WebSocket websocket {
			get {
				return curWebsocket;
			}
		}

		public bool isWsConnected {
			get {
				return curWebsocket.IsConnected;
			}
		}

		public void Close()
		{
			curWebsocket.Close();
		}

		public void Send(string data)
		{
			curWebsocket.Send(data);
		}

		public string socketId { get; set; }
		public int packetId; 

		public bool isConnected { get { return connected; } set { connected = value; } }
		private volatile bool connected;

		public bool wsCheck { get { return wscheck; } set { wscheck = value; } }
		private volatile bool wscheck;

		public bool isInitialized { get { return wsinit; } set { wsinit = value; } }
		private volatile bool wsinit;

		public bool wsPinging { get { return wspinging; } set { wspinging = value; } }
		private volatile bool wspinging; 

		public bool wsPonging { get { return wsponging; } set { wsponging = value; } }
		private volatile bool wsponging;

		public int reconDelay {
			get {
				return 5;
			}
		}
		public float ackExpireTime {
			get {
				return 1800f;
			}
		}
		public float pingInterval {
			get {
				return 25f;
			}
		}

		public float pingTimeout {
			get {
				return 60f;
			}
		}

		public void Start(string appsecret, string port, EventHandler OnOpen, EventHandler<ErrorEventArgs> OnError, EventHandler<CloseEventArgs> OnClose, EventHandler<MessageEventArgs> OnPacket) 
		{
			string hostUrl = USocketNet.config.serverUrl + ":" + port;
			string sioPath = "/socket.io/?EIO=4&transport=websocket";
			string usrTok = "&wpid=" + USocketNet.User.token.wpid + "&snid=" + USocketNet.User.token.snid + "&apid=" + appsecret;

			curWebsocket = new WebSocket("ws://" + hostUrl + sioPath + usrTok);
			curWebsocket.OnOpen += OnOpen;
			curWebsocket.OnError += OnError;
			curWebsocket.OnClose += OnClose;
			curWebsocket.OnMessage += OnPacket;

			wsCheck = false;
			isConnected = true;
			packetId = 0;
			socketId = string.Empty;
			isInitialized = true;
		}

		public void Reset()
		{
			isInitialized = false;
			curWebsocket = null;

			wsCheck = false;
			isConnected = false;

			packetId = 0;
			socketId = string.Empty;
		}
	}
}