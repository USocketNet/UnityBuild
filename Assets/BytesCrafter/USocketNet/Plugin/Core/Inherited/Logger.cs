
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
    public enum Logs
    { 
        Log, 
        Warn, 
        Error 
    }

    public class BC_USN_Logger
    {
        public void Push(Logs logType, string title, string details) 
        {
            switch( logType ) 
            {
                case Logs.Log:
                    Debug.Log(title + " - " + details);
                    break;
                case Logs.Warn:
                    Debug.LogWarning(title + " - " + details);
                    break;
                case Logs.Error:
                    Debug.LogError(title + " - " + details);
                    break;
                default:
                    break;
            }
        }
    }
}