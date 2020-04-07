
#region License
/*
 * BC_USN_RestApi.cs
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
using UnityEngine.Networking;

using System;
using System.Text;
using System.Collections;

using BytesCrafter.USocketNet.Serializables;
using BytesCrafter.USocketNet.Toolsets;

namespace BytesCrafter.USocketNet.RestApi {
    public class BC_USN_RestApi 
    {
        private USNClient usnClient = null;
        public BC_USN_RestApi( USNClient reference )
        {
            usnClient = reference;
        }

        public BC_USN_Response_Data GetUserData
        {
            get 
            {
                return curUser; 
            }
        }
        private BC_USN_Response_Data curUser = new BC_USN_Response_Data();
        public void Authenticate(string uname, string pword, USNClient usnClient, Action<BC_USN_Response> callback) 
        {
            if( usnClient.config.restapiUrl == string.Empty )
			{
				USocketNet.Log(Logs.Warn, "RestApi", "Please fill up RestApi url on this USNClient: " + usnClient.name);
				callback( new BC_USN_Response() );
				return;
			}
            
            usnClient.StartCoroutine( Authenticating(uname, pword, callback) );
        }
        
        IEnumerator Authenticating( string uname, string pword, Action<BC_USN_Response> callback ) 
        {            
            WWWForm creds = new WWWForm();
            creds.AddField("UN", uname);
            creds.AddField("PW", pword);

            string rapi = usnClient.config.restapiUrl;
            string startString = rapi[0] == 'h' && rapi[1] == 't' && rapi[2] == 't' && rapi[3] == 'p' ? "" : "http://";
            string endString = rapi[rapi.Length - 1] == '/' ? "" : "/";
            var request = UnityWebRequest.Post( startString + rapi + endString + "wp-json/usocketnet/v1/auth", creds);
            
            yield return request.SendWebRequest();
            
            if ( request.isNetworkError || request.isHttpError )
            {
                USocketNet.Log(Logs.Error, "RestApi", "The Rest API url return 404 Not found. Please check and try again.");
                callback( new BC_USN_Response() );
            }

            else
            {
                string bytes = Encoding.UTF8.GetString( request.downloadHandler.data );
                BC_USN_Response response = JsonUtility.FromJson<BC_USN_Response>(bytes);

                if( response.success )
                {
                    USocketNet.Log(Logs.Log, "RestApi", "Welcome! " +response.data.dname+ " [" +response.data.email+ "]" );
                    curUser = response.data;
                    callback( response );
                }

                else
                {
                    USocketNet.Log(Logs.Warn, "RestApi", response.message);
                    callback( new BC_USN_Response() );
                }
            }
        }
    }
}
