
#region License
/*
 * ProjectObject.cs
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
    public class ProjectObject
    {
        public bool success {
            get {
                return status == "success" ? true : false;
            }
        }
        public string status = string.Empty;

        public string message = string.Empty;

         public ProjectObject(string shortCode, string errorMsg) {
            status = shortCode;
            message = errorMsg;
        }

        public ProjectObjectRaw data = new ProjectObjectRaw();
    }

    [System.Serializable]
    public class ProjectObjectRaw
    {
        public string name = string.Empty;
        public string desc = string.Empty;
        public string url = string.Empty;
        public string status = string.Empty;
        public string matchcap = string.Empty;
        public string capacity = string.Empty;
        public string parent = string.Empty;
    }
}
