using System;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

namespace BytesCrafter.USocketNet.Networks
{
	[System.Serializable]
	public class QueueCoder
	{
		public Dictionary<string, List<Action<SocketIOEvent>>> handlers;
		public Queue<SocketIOEvent> eventQueue; 
		public object eventQueueLock;
		public Queue<Packet> ackQueue;
		public object ackQueueLock;
		public List<Ack> ackList; 

		public Encoder encoder;
		public Decoder decoder;
		public Parser parser;
	}
}