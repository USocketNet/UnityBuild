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

		public void Starts()
		{
			//ENCRYPTIONS & HANDLERS
			encoder = new Encoder();
			decoder = new Decoder();
			parser = new Parser();

			handlers = new Dictionary<string, List<Action<SocketIOEvent>>>();
			eventQueue = new Queue<SocketIOEvent>();
			eventQueueLock = new object();

			ackQueue = new Queue<Packet>();
			ackQueueLock = new object();
			ackList = new List<Ack>();
		}

		public void Stops()
		{
			//ENCRYPTIONS & HANDLERS
			encoder = null;
			decoder = null;
			parser = null;

			handlers = null;
			eventQueue = null;
			eventQueueLock = null;

			ackQueue = null;
			ackQueueLock = null;
			ackList = null;
		}
	}
}