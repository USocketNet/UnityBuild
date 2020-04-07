#region License
/*
 * Parser.cs
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
using UnityEngine;

namespace SocketIO
{
	public class Parser
	{
		public SocketIOEvent Parse(JSONObject json)
		{
			if (json.Count < 1 || json.Count > 2) {
				throw new SocketIOException("Invalid number of parameters received: " + json.Count);
			}

			if (json[0].type != JSONObject.Type.STRING) {
				throw new SocketIOException("Invalid parameter type. " + json[0].type + " received while expecting " + JSONObject.Type.STRING);
			}

			if (json.Count == 1) {
				return new SocketIOEvent(json[0].str);
			} 

			if (json[1].type != JSONObject.Type.OBJECT) {
				throw new SocketIOException("Invalid argument type. " + json[1].type + " received while expecting " + JSONObject.Type.OBJECT);
			}

			return new SocketIOEvent(json[0].str, json[1]);
		}
	}
}
