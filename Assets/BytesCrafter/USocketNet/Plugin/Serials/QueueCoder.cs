#region License
/*
 * QueueCoder.cs
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
using System.Collections.Generic;
using SocketIO;

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