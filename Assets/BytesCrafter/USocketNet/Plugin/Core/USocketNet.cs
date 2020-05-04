
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

		/// <summary>
		/// Get the initialized config for this specific USocketNet core instance.
		/// </summary>
		/// <value>{ restapiUrl, serverUrl, debugOnLog, runOnBackground }</value>
		public static Config config
		{
			get {
				return staticConfig;
			}
		}
		private static Config staticConfig = new Config();  

		/// <summary>
		/// Check if this current USocketNet core instance is initialize or not.
		/// You need to initialize the instance before trying to pass commands. 
		/// You just need to call, USocketNet.Initialize(config, callback);
		/// </summary>
		/// <value> bool: true or false. </value>
		public static bool isInitialized
		{
			get {
				return staticConfig != null ? true : false;
			}
		}

		/// <summary>
		/// Current core instance of USocketNet. This will also return false if 
		/// this instance is actually null or not initialized else will return 
		/// the reference of the core instance.
		/// </summary>
		/// <value>bool: USocketNet or null.</value>
		public static USocketNet Core
		{
			get {
				if(isInitialized && baseRefs != null) {
					return baseRefs;
				} else {
					return null;
				}
			}
		}
		private static USocketNet baseRefs;

		/// <summary>
		/// Static logging of USocketNet and will only work for if USocketNet.config.debugOnLog
		/// is enable or equal to true. During build, it is best to disable that bool.
		/// </summary>
		/// <param name="log">Type of Log if Log, Warn, or Error.</param>
		/// <param name="title">Main Category of this log.</param>
		/// <param name="info">Short description of this log.</param>
		public static void Log ( Logs log, string title, string info ) 
		{
			if(title == "disable")
				return;

			logger.Push(log, title, info);
		}
		private static BC_USN_Logger logger;

		/// <summary>
		/// Check whether the current core instance is authenticated from its restapiUrl 
		/// or not. Call USocketNet.Core.Authenticate(username, password, callback); 
		/// </summary>
		/// <value></value>
		public bool isAuthenticated
        {
            get {
                return restApi.isAuthenticated;
            }
        }

		/// <summary>
		/// If this core instance is previously authenticated to server, this object 
		/// will have a value or if not will and is not authenticated return null. 
		/// </summary>
		/// <value> { session, cookie, avatar, id, uname, dname, email, roles[] } or null. </value>
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

		/// <summary>
		/// To be able to pass commands to this core instance, you need to call this 
		/// function and pass the client configuration for this core instance.
		/// </summary>
		/// <param name="refsConfig"> Type of USocketNet.Config </param>
		public static void Initialized(Config refsConfig)
		{
			staticConfig = refsConfig;
			Application.runInBackground = USocketNet.config.runOnBackground;

			logger = new BC_USN_Logger();
			baseRefs = new GameObject("USocketNet").AddComponent<USocketNet>();
		}

		#endregion

		#region Class Components
			void Awake()
			{
				DontDestroyOnLoad(this);
			}

			/// <summary>
			/// Authenticate the user to your RestApi host.
			/// </summary>
			/// <param name="uname">Username.</param>
			/// <param name="pword">Password.</param>
			/// <param name="callback">BC_USN_Response.</param>
			public void Authenticate( string uname, string pword, Action<BC_USN_Response> callback )
			{
				restApi.Authenticate(uname, pword, (BC_USN_Response response) => {
					callback(response);
				});
			}

			public void VerifyProject( string secretkey, Action<ProjectObject> callback )
			{
				restApi.VerifyProject(secretkey, (ProjectObject response) => {
					callback(response);
				});
			}

			/// <summary>
			/// Signout the user and will delete previous user authentication data. 
			/// This will also disconnect all connected USNClient instances.
			/// </summary>
			public void SignOut()
			{
				if( restApi.isAuthenticated ) {
					restApi.Deauthenticate();
					MasterDisconnect();
					MessageDisconnect();
					MatchDisconnect();
				} else {
					USocketNet.Log(Logs.Warn, "RestApi", "You are not authenticated thus you dont have to sign out.");
				}
				
			}

			/// <summary>
			/// Connect you Master client to server.
			/// </summary>
			/// <param name="appsecret">App Secret Key from your backend. </param>
			/// <param name="callback">Callback that will return typeof ConStat.</param>
			public void MasterConnect(Action<ConStat> callback )
			{
				if( masterClient == null ) {
					masterClient = new GameObject("MasterClient").AddComponent<MasterClient>();
					masterClient.Connect(callback);
				} else {
					if(masterClient.IsConnected) {
						callback( ConStat.Online );
					} else {
						callback( ConStat.Connecting );
					}
				}
			}

			/// <summary>
			/// Forcibly close all USocketNet clients from the existing connection 
			/// to the server caused by Masters disconnection. Includes: Message and Game client.
			/// </summary>
			public void MasterDisconnect()
			{
				if(masterClient == null)
					return;

				masterClient.Disconnect();
				Destroy(masterClient.gameObject);
			}

			/// <summary>
			/// This is the Master Client of this USocketNet core. 
			/// Use this if you have any query to server.
			/// </summary>
			/// <value></value>
			public MasterClient Master
			{
				get {
					if(isInitialized) {
						return masterClient;
					} else {
						return null;
					}
				}
			} private MasterClient masterClient;

			public bool IsMasterConnected {
				get {
					if(masterClient != null) {
						return masterClient.IsConnected ? true : false;
					} else {
						return false;
					}
				}
			}

			/// <summary>
			/// Add and Connect a USocketNet Message client for listening or dealing with messages.
			/// </summary>
			/// <param name="appsecret">App Secret Key from your backend. </param>
			/// <param name="callback">Callback that will return typeof ConStat.</param>
			public void MessageConnect(string appsecret, Action<ConStat> callback )
			{
				messageClient = new GameObject("MessageClient").AddComponent<MessageClient>();
				messageClient.Connect(callback);
			}

			/// <summary>
			/// Removed and Disconnect USocketNet Message client from the server.
			/// </summary>
			public void MessageDisconnect()
			{
				if(messageClient == null)
					return;

				messageClient.Disconnect();
				Destroy(messageClient.gameObject);
			}

			public MessageClient Message {
				get {
					if(isInitialized) {
						return messageClient;
					} else {
						return null;
					}
				}
			} private MessageClient messageClient;

			public bool IsMessageConnected {
				get {
					if(messageClient != null) {
						return messageClient.IsConnected ? true : false;
					} else {
						return false;
					}
				}
			}

			/// <summary>
			/// Add and Connect a USocketNet Game client for listening or dealing with messages.
			/// </summary>
			/// <param name="appsecret">App Secret Key from your backend. </param>
			/// <param name="callback">Callback that will return typeof ConStat.</param>
			public void MatchConnect(string appsecret, Action<ConStat> callback )
			{
				matchClient = new GameObject("MatchClient").AddComponent<MatchClient>();
				matchClient.Connect(appsecret, callback);
			}

			/// <summary>
			/// Removed and Disconnect USocketNet Game client from the server.
			/// </summary>
			public void MatchDisconnect()
			{
				if(matchClient == null)
					return;

				matchClient.Disconnect();
				Destroy(matchClient.gameObject);
			}

			public MatchClient Match {
				get {
					if(isInitialized) {
						return matchClient;
					} else {
						return null;
					}
				}
			} private MatchClient matchClient;

			public bool IsMatchConnected {
				get {
					if(matchClient != null) {
						return matchClient.IsConnected ? true : false;
					} else {
						return false;
					}
				}
			}

		#endregion

	}
}