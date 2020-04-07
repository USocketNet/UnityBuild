
#region License
/*
 * USocketNet.cs
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
using BytesCrafter.USocketNet.RestApi;
using BytesCrafter.USocketNet.Serials;
using BytesCrafter.USocketNet.Toolsets;

namespace BytesCrafter.USocketNet
{
	public class USocketNet : MonoBehaviour
	{
		#region  Static Components
		public static Config config
		{
			get {
				return staticConfig;
			}
		}
		private static Config staticConfig = new Config();  

		public static bool isInitialized
		{
			get {
				return staticConfig != null ? true : false;
			}
		}

		public static USocketNet Core
		{
			get {
				if(isInitialized) {
					return baseRefs;
				} else {
					return null;
				}
			}
		}
		private static USocketNet baseRefs;

		public static void Log ( Logs log, string title, string info ) 
		{
			logger.Push(log, title, info);
		}
		private static BC_USN_Logger logger;

		public bool isAuthenticated
        {
            get
            {
                return restApi.isAuthenticated;
            }
        }

		public static BC_USN_Response_Data User
		{
			get {
				if(restApi.isAuthenticated) {
					return restApi.curUser;
				} else {
					return null;
				}
			}
		}
		private static BC_USN_RestApi restApi = new BC_USN_RestApi();

		public static void Initialized(Config refsConfig)
		{
			staticConfig = refsConfig;
			logger = new BC_USN_Logger();
			baseRefs = new GameObject("USocketNet").AddComponent<USocketNet>();
		}

		#endregion

		void Awake()
		{
			DontDestroyOnLoad(this);
			Application.runInBackground = USocketNet.config.runOnBackground;
		}

		/// <summary>
		/// Authenticate the user to your Rest Api host.
		/// </summary>
		/// <param name="uname">Username.</param>
		/// <param name="pword">Password.</param>
		/// <param name="callback">BC_USN_Response.</param>
		public void Authenticate( string uname, string pword, Action<BC_USN_Response> callback )
		{
			restApi.Authenticate(this, uname, pword, (BC_USN_Response response) => {
				if( response.success ) {
					callback(response);
				}
			});
		}
		
		public void SignOut()
		{
			if( restApi.isAuthenticated ) {
				restApi.Deauthenticate();
				Disconnect();
			} else {
				USocketNet.Log(Logs.Warn, "RestApi", "You are not authenticated thus you dont have to sign out.");
			}
			
		}

		public MasterClient Master
		{
			get {
				if(isInitialized) {
					return masterClient;
				} else {
					return null;
				}
			}
		}
		private MasterClient masterClient;

		public void Connect( Action<ConStat> callback )
		{
			if( masterClient == null ) {
				masterClient = new GameObject("MasterClient").AddComponent<MasterClient>();
				masterClient.Connect(callback);
			} else {
				callback( ConStat.Online );
			}
			
		}

		public void Disconnect()
		{
			masterClient.Disconnect();
			Destroy(masterClient.gameObject);

			RemoveChatClient();
			RemoveGameClient();
		}

		private ChatClient chatClient;

		public void AddChatClient( Action<ConStat> callback )
		{
			chatClient = new GameObject("ChatClient").AddComponent<ChatClient>();
			chatClient.Connect(callback);
		}

		public void RemoveChatClient()
		{
			chatClient.Disconnect();
			Destroy(chatClient.gameObject);
		}

		private GameClient gameClient;

		public void AddGameClient( Action<ConStat> callback )
		{
			gameClient = new GameObject("GameClient").AddComponent<GameClient>();
			gameClient.Connect(callback);
		}

		public void RemoveGameClient()
		{
			gameClient.Disconnect();
			Destroy(gameClient.gameObject);
		}
	}
}