
#region License
/*
 * BC_USN_Debug.cs
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

namespace BytesCrafter.USocketNet.Toolsets 
{
    public enum BC_USN_Debug
    { 
        Log, 
        Warn, 
        Error 
    }

    public class BC_USN_Logger
    {
        private USocketClient usnClient = null;
        public BC_USN_Logger( USocketClient reference ) 
        {
            usnClient = reference;
        }

        public void Push(BC_USN_Debug logType, string title, string details) 
        {
            if(!usnClient.config.debugOnLog)
				return;

            switch( logType ) 
            {
                case BC_USN_Debug.Log:
                    Debug.Log(title + " - " + details);
                    break;
                case BC_USN_Debug.Warn:
                    Debug.LogWarning(title + " - " + details);
                    break;
                case BC_USN_Debug.Error:
                    Debug.LogError(title + " - " + details);
                    break;
                default:
                    break;
            }
        }
    }
}