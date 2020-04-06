using System; 
using System.Collections; 
using System.Collections.Generic;

using UnityEngine;
using SocketIO;

using BytesCrafter.USocketNet.Serializables;
using BytesCrafter.USocketNet.Networks;
using BytesCrafter.USocketNet.Toolsets;
using BytesCrafter.USocketNet.Overrides;
using BytesCrafter.USocketNet.RestApi;

namespace BytesCrafter.USocketNet
{
	public class USocketClient : MonoBehaviour
	{
		public Bindings bindings = new Bindings();

		public Bindings bind 
		{
			get {
				return bindings;
			}
		}

		public WPToken wptoken {
			get {
				return bc_usn_restapi.wptoken;
			}
		}

		public void Debug ( Debugs log, string title, string info ) {
			bc_usn_logger.Push(log, title, info);
		}

		private BC_USN_Logger bc_usn_logger = null;
		private BC_USN_RestApi bc_usn_restapi = null;
		private BC_USN_WebSocket bc_usn_websocket = null;

		void Awake()
		{
			//Put all child script to be initialized as CLASS.
			bc_usn_logger = new BC_USN_Logger( this );
			bc_usn_restapi = new BC_USN_RestApi( this );
			bc_usn_websocket = new BC_USN_WebSocket( this );

			//Put all child script to be initialized as METHOD
			bc_usn_websocket.Initialize();

			//Client INITIALIZATION options before it run.
			if(bindings.dontDestroyOnLoad)
			{
				DontDestroyOnLoad(this);
			}
			Application.runInBackground = bindings.runOnBackground;
		}

		void Update()
		{
			bc_usn_websocket.Update( this );
		}

		void OnApplicationQuit()
		{
			bc_usn_websocket.ForceDisconnect();
		}

		void OnDestroy()
		{
			bc_usn_websocket.AbortConnection();
		}

		public void Authenticate( string uname, string pword, Action<Response> callback ) {
			bc_usn_restapi.Authenticate(uname, pword, this, (Response response) => {
				if( response.success ) {
					Debug(Debugs.Log, "Code: " + response.code, " Message: " + response.message);
					callback(response);
				}
			});
		}

		/// <summary>
		/// Connects to server using user specific credentials.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void ConnectToServer( Action<ConStat> callback )
		{
			if( bindings.serverUrl == string.Empty || bindings.serverPort == string.Empty )
			{
				bc_usn_logger.Push(Debugs.Warn, "ConnectionError", "Please fill up USocketNet host field on this USocketClient: " + name);
				callback( ConStat.Error );
				return;
			}

			if (!bc_usn_websocket.isConnected)
			{
				bc_usn_websocket.InitConnection();
			}

			else
			{
				bc_usn_logger.Push(Debugs.Warn, "ConnectionSuccess", "Already connected to the server!");
			}
		}


		//Check for all logging.
		//Decide default blocking of user interface.
		//Have a connect listener please.










		
	}
}