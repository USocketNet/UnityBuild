

#region License
/*
 * Ports.cs
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

namespace BytesCrafter.USocketNet.Serials
{
    [System.Serializable]
    public class Ports
    {
        public string master
        {
            get {
                return masterPort.ToString();
            }
        }
        private int masterPort = 19090;

         public string chat
        {
            get {
                return chatPort.ToString();
            }
        }
        private int chatPort = 6060;

         public string game
        {
            get {
                return gamePort.ToString();
            }
        }
        private int gamePort = 9090;

        public ServType GetServType(string curPort)
        {
            if(curPort == master) {
                return ServType.Master;
            } else if(curPort == chat) {
                return ServType.Chat;
            } else if(curPort == game) {
                return ServType.Game;
            } else {
                return ServType.None;
            }
        }
    }
}