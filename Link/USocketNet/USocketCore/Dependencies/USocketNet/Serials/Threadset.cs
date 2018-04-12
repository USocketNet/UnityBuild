
using WebSocketSharp;

namespace BytesCrafter.USocketNet.Networks
{
	[System.Serializable]
	public class Threadset
	{
		public WebSocket websocket = null; //An instance of Web Socket for connection.

		public bool wsCheck { get { return wscheck; } set { wscheck = value; } }
		private volatile bool wscheck;

		public bool IsConnected { get { return connected; } set { connected = value; } }
		private volatile bool connected;

		public string socketId { get; set; }
		public int packetId; 

		public bool IsInitialized { get { return wsinit; } set { wsinit = value; } }
		private volatile bool wsinit;

		public bool autoConnect { get { return autoConn; } set { autoConn = value; } }
		private volatile bool autoConn;

		public bool wsPinging { get { return wspinging; } set { wspinging = value; } }
		private volatile bool wspinging; 

		public bool wsPonging { get { return wsponging; } set { wsponging = value; } }
		private volatile bool wsponging;

		public int reconDelay = 5;
		public float ackExpireTime = 1800f;
		public float pingInterval = 25f;
		public float pingTimeout = 60f;
	}
}