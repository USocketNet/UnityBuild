
#region License
/*
 * Response.cs
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
    public class BC_USN_Response
    {
        public string code = "error";
        public string message = string.Empty;
        public BC_USN_Response_Data data = null;

        public BC_USN_Response(string shortCode, string errorMsg) {
            code = shortCode;
            message = errorMsg;
        }

        public bool success {
            get {
                return code == "success" ? true : false;
            }
        }
    }

    [System.Serializable]
    public class BC_USN_Response_Data
    {
        public string session = string.Empty;
        public string cookie = string.Empty;
        public string avatar = string.Empty;
        public string id = string.Empty;
        public string uname = string.Empty;
        public string dname = string.Empty;
        public string email = string.Empty;
        public string[] roles;

        public  BC_USN_Token token {
            get {
                return new BC_USN_Token(id, session);
            }
        }
        
    }
}
