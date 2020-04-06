using System; 
using UnityEngine;

using BytesCrafter.USocketNet.Serializables;
using BytesCrafter.USocketNet.Networks;
using BytesCrafter.USocketNet.Toolsets;
using BytesCrafter.USocketNet.RestApi;

namespace BytesCrafter.USocketNet
{
	public class USocketClient : MonoBehaviour
	{
		public BC_USN_Option options = new BC_USN_Option();
		private BC_USN_Logger bc_usn_logger = null;
		private BC_USN_RestApi bc_usn_restapi = null;
		private BC_USN_WebSocket bc_usn_websocket = null;

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
				return bc_usn_websocket.socketId;
			}
		}

		public BC_USN_Token GetToken {
			get {
				return bc_usn_restapi.GetUserData.token;
			}
		}

		public void Debug ( BC_USN_Debug log, string title, string info ) {
			if(bc_usn_logger == null)
				return;

			bc_usn_logger.Push(log, title, info);
		}

		void Awake()
		{
			//Put all child script to be initialized as CLASS.
			bc_usn_logger = new BC_USN_Logger( this );
			bc_usn_restapi = new BC_USN_RestApi( this );
			bc_usn_websocket = new BC_USN_WebSocket( this );

			//Client INITIALIZATION options before it run.
			if(options.dontDestroyOnLoad)
			{
				DontDestroyOnLoad(this);
			}
			Application.runInBackground = options.runOnBackground;
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

		public void Authenticate( string uname, string pword, Action<BC_USN_Response> callback ) {
			bc_usn_restapi.Authenticate(uname, pword, this, (BC_USN_Response response) => {
				if( response.success ) {
					Debug(BC_USN_Debug.Log, "Code: " + response.code, " Message: " + response.message);
					callback(response);
				}
			});
		}

		/// <summary>
		/// Connects to server using user specific credentials.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void Connect( Action<ConStat> callback )
		{
			if( options.serverUrl == string.Empty || options.serverPort == string.Empty )
			{
				bc_usn_logger.Push(BC_USN_Debug.Warn, "ConnectionError", "Please fill up USocketNet host field on this USocketClient: " + name);
				callback( ConStat.Error );
				return;
			}

			if (!bc_usn_websocket.isConnected)
			{
				bc_usn_websocket.InitConnection(callback);
			}

			else
			{
				bc_usn_logger.Push(BC_USN_Debug.Warn, "ConnectionSuccess", "Already connected to the server!");
				callback( ConStat.Invalid );
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

		void Start() 
		{
			OnStart(true);
		}

		protected virtual void OnStart( bool auto )
		{
			Debug(BC_USN_Debug.Log, "Starting", "");
		}



		public void OnConnect( bool recon )
		{
			OnConnection( recon );
		}

		protected virtual void OnConnection( bool recon )
		{
			Debug(BC_USN_Debug.Log, "Connect", "");
		}

		public void OnDisconnect( bool auto )
		{
			OnDisconnection( auto );
		}

		protected virtual void OnDisconnection( bool auto )
		{
			Debug(BC_USN_Debug.Log, "Disconnect", "");
		}
	}
}