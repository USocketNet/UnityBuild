

#region License
/*
 * MessageClient.cs
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
    public class MessageClient : USNClient
    {
        /// <summary>
		/// Connects to server using user specific credentials.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void Connect( Action<ConStat> callback )
		{
			if( USocketNet.config.serverUrl == string.Empty )
			{
				USocketNet.Log(Logs.Warn, "ConnectionError", "Please fill up USocketNet host field on this USocketClient: " + name);
				callback( ConStat.Invalid );
				return;
			}

			bc_usn_websocket.InitConnection(string.Empty, USocketNet.config.serverPort.message, this, callback);
		}

        /// <summary>
		/// Disconnect to server using user specific credentials.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void Disconnect()
		{
			bc_usn_websocket.ForceDisconnect();
		}

		public void JoinServerWide() { //svr-ID -> ID=cluster id

		}

		public void JoinProjectWide(string eventID) { //prj-ID -> ID=project id
			
		}

		public void JoinVariantWide(string eventID) { //prj-pID-vID -> pID=parent id and vID=variant id.
			
		}

		public void JoinMatchWide(string eventID) { //mat-mtid -> mtid=match id from server.
			
		}

		public void JoinGroupWide(string eventID) { //grp-cID -> cID=custom id what user want.
			
		}

		/// <summary>
		/// Send any type of human message to the other clients.
		/// </summary>
		/// <param name="msgContent"></param>
		/// <param name="callback"></param>
		public void SendMessage(string msgContent, Action<MsgRes> callback) 
		{
			string sendData = JsonUtility.ToJson(new MsgRaw(msgContent));
			bc_usn_websocket.SendEmit("svr", new JSONObject(sendData), (JSONObject jsonObj) => {
				callback( JSONProcessor.ToObject<MsgRes>(jsonObj.ToString()) );
			});
		}

		/// <summary>
		/// Send any type of human message to specific client.
		/// </summary>
		/// <param name="msgContent"></param>
		/// <param name="userID"></param>
		/// <param name="callback"></param>
		public void SendMessage(string msgContent, string userID, Action<MsgRes> callback) 
		{
			string sendData = JsonUtility.ToJson(new MsgRaw(msgContent, userID));
			bc_usn_websocket.SendEmit("pri", new JSONObject(sendData), (JSONObject jsonObj) => {
				callback( JSONProcessor.ToObject<MsgRes>(jsonObj.ToString()) );
			});
		}





        protected override void OnConnect(bool recon)
        {
            Debug.Log("OnConnect on MessageClient from " + this.name);
        }

        protected override void OnDisconnect(bool recon)
        {
            Debug.Log("OnDisconnect on MessageClient from " + this.name);
        }
    }
}

