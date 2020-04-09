

#region License
/*
 * ChatClient.cs
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
using BytesCrafter.USocketNet.Toolsets;

namespace BytesCrafter.USocketNet
{
    public class ChatClient : USNClient
    {
        /// <summary>
		/// Connects to server using user specific credentials.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void Connect( string appsecret, Action<ConStat> callback )
		{
			if( config.serverUrl == string.Empty )
			{
				USocketNet.Log(Logs.Warn, "ConnectionError", "Please fill up USocketNet host field on this USocketClient: " + name);
				callback( ConStat.Invalid );
				return;
			}

			if (!bc_usn_websocket.isConnected)
			{
				bc_usn_websocket.InitConnection(appsecret, USocketNet.config.serverPort.chat, this, callback);
			}

			else
			{
				USocketNet.Log(Logs.Warn, "ConnectionSuccess", "Already connected to the server!");
				callback( ConStat.Error );
			}
		}

        /// <summary>
		/// Disconnect to server using user specific credentials.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void Disconnect()
		{
			bc_usn_websocket.ForceDisconnect();
		}

        protected override void OnConnect(bool recon)
        {
            Debug.Log("OnConnect on ChatClient from " + this.name);
        }

        protected override void OnDisconnect(bool recon)
        {
            Debug.Log("OnDisconnect on ChatClient from " + this.name);
        }
    }
}

