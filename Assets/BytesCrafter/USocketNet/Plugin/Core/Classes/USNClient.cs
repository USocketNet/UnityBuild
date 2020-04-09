
#region License
/*
 * USNClient.cs
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
using UnityEngine;

using BytesCrafter.USocketNet.Serials;
using BytesCrafter.USocketNet.Networks;

namespace BytesCrafter.USocketNet
{
	public class USNClient : MonoBehaviour
	{
		protected BC_USN_WebSocket bc_usn_websocket = new BC_USN_WebSocket();

		#region VARIABLES

		/// <summary>
		/// This is the host and client config.
		/// </summary>
		/// <returns></returns>
		public Config config = new Config();

		/// <summary>
		/// Determines whether this client is connected to web socket server.
		/// </summary>
		/// <returns><c>true</c> if this client is connected; otherwise, <c>false</c>.</returns>
		public bool IsConnected 
		{
            get 
			{
                return bc_usn_websocket.isConnected;
            }
        }

		/// <summary>
		/// Sockets identity of the current user's connection to the server.
		/// </summary>
		/// <returns> Socket Identity, Not Connected, No Internet Access. </returns>
		public string Identity
		{
			get
			{
				return bc_usn_websocket.getSocketId;
			}
		}

		/// <summary>
		/// Get the Token that your RestApi respond.
		/// </summary>
		/// <value></value>
		public BC_USN_Token GetToken {
			get {
				return USocketNet.User.token;
			}
		}

		#endregion

		#region MONOBEHAVIOUR
		

		void Awake()
		{
			bc_usn_websocket = new BC_USN_WebSocket();
			//Client INITIALIZATION config before it run.
			DontDestroyOnLoad(this);
		}

		void Update()
		{
			if(bc_usn_websocket == null)
				return;

			bc_usn_websocket.Update();
		}

		void OnApplicationQuit()
		{
			if(bc_usn_websocket == null)
				return;

			bc_usn_websocket.ForceDisconnect();
		}

		void OnDestroy()
		{
			if(bc_usn_websocket == null)
				return;

			bc_usn_websocket.AbortConnection();
		}

		#endregion

		

		// void Start() 
		// {
		// 	OnStart(true);
		// }

		// protected virtual void OnStart( bool auto )
		// {
		// 	USocketNet.Log(Logs.Log, "Starting", "");
		// }

		public void OnConnection(bool recon)
		{
			OnConnect(recon);
		}

		public void OnDisconnection(bool auto)
		{
			OnDisconnect(auto);
		}

		protected virtual void OnConnect(bool recon)
		{
			Debug.Log("OnConnect on USNClient from " + this.name);
		}

		protected virtual void OnDisconnect(bool auto)
		{
			Debug.Log("OnDisconnect on USNClient from " + this.name);
		}
	}
}