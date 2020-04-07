
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
using System.Collections.Generic;

using UnityEngine;
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
				if(isInitialized) {
					return staticConfig;
				} else {
					return null;
				}
			}
		}
		private static Config staticConfig;  

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
			if(isInitialized)
			{
				logger.Push(Logs.Warn, "USocketNet", "The USN core instance is not yet initialize.");
				return;
			}

			logger.Push(log, title, info);
		}
		private static BC_USN_Logger logger;

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
	}
}