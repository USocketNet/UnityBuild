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
		public void Connect( Action<ConStat> callback )
		{
			if( config.serverUrl == string.Empty )
			{
				USocketNet.Log(Logs.Warn, "ConnectionError", "Please fill up USocketNet host field on this USocketClient: " + name);
				callback( ConStat.Error );
				return;
			}

			if (!bc_usn_websocket.isConnected)
			{
				bc_usn_websocket.InitConnection(USocketNet.config.serverPort.master, callback);
			}

			else
			{
				USocketNet.Log(Logs.Warn, "ConnectionSuccess", "Already connected to the server!");
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
    }
}

