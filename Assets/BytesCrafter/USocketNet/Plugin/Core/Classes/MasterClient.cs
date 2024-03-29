﻿

#region License
/*
 * MasterClient.cs
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
using BytesCrafter.USocketNet.Toolsets;

namespace BytesCrafter.USocketNet
{
    public class MasterClient : USNClient
    {
        /// <summary>
		/// Connects to server using user specific credentials.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void Connect(Action<ConStat> callback )
		{
			if( USocketNet.config.serverUrl == string.Empty )
			{
				USocketNet.Log(Logs.Warn, "ConnectionError", "Please fill up USocketNet host field on this USocketClient: " + name);
				callback( ConStat.Invalid );
				return;
			}

			bc_usn_websocket.InitConnection(string.Empty, USocketNet.config.serverPort.master, this, callback);
		}

        /// <summary>
		/// Disconnect to server using user specific credentials.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void Disconnect()
		{
			bc_usn_websocket.ForceDisconnect();
		}
    }
}

