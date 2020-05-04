
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

using BytesCrafter.USocketNet.Serials;
using BytesCrafter.USocketNet.Toolsets;

namespace BytesCrafter.USocketNet.RestApi {
    public class BC_USN_RestApi 
    {
        public BC_USN_Response_Data curUser
        {
            get 
            {
                return responseData; 
            }
        }
        private BC_USN_Response_Data responseData = new BC_USN_Response_Data();

        public bool isAuthenticated
        {
            get
            {
                return responseData.snid != string.Empty ? true : false;
            }
        }

        public void Authenticate(string uname, string pword, Action<BC_USN_Response> callback) 
        {
            if( USocketNet.config.restapiUrl == string.Empty )
			{
				USocketNet.Log(Logs.Warn, "RestApi", "Please fill up RestApi url on this USocketNet core instance.");
				callback( new BC_USN_Response("urlempty", "Please fill up RestApi url on this USocketNet core instance.") );
				return;
			}
            
            USocketNet.Core.StartCoroutine( Authenticating(uname, pword, callback) );
        }
        
        IEnumerator Authenticating( string uname, string pword, Action<BC_USN_Response> callback ) 
        {            
            WWWForm creds = new WWWForm();
            creds.AddField("UN", uname);
            creds.AddField("PW", pword);

            string rapi = USocketNet.config.restapiUrl;
            string endString = rapi[rapi.Length - 1] == '/' ? "" : "/";
            var request = UnityWebRequest.Post( rapi + endString + "wp-json/usocketnet/v1/user/auth", creds);
            
            yield return request.SendWebRequest();
            
            if ( request.isNetworkError || request.isHttpError )
            {
                USocketNet.Log(Logs.Error, "RestApi", "Request failed with status code of "+request.responseCode+", Try again.");
                callback( new BC_USN_Response("reqerror", "Request failed with status code of "+request.responseCode+", Try again." ) );
            }

            else
            {
                string bytes = Encoding.UTF8.GetString( request.downloadHandler.data );
                BC_USN_Response response = JsonUtility.FromJson<BC_USN_Response>(bytes);

                if( response.success )
                {
                    USocketNet.Log(Logs.Log, "RestApi", "Welcome! " +response.data.dname+ " [" +response.data.email+ "]" );
                    responseData = response.data;
                    callback( response );
                }

                else
                {
                    USocketNet.Log(Logs.Warn, "RestApi", response.message);
                    callback( new BC_USN_Response(response.status, response.message) );
                }
            }
        }

        public void Deauthenticate()
        {
            responseData = new BC_USN_Response_Data();
        }

        public void VerifyProject(string secretkey, Action<ProjectObject> callback) 
        {
            if( USocketNet.config.restapiUrl == string.Empty )
			{
				USocketNet.Log(Logs.Warn, "RestApi", "Please fill up RestApi url on this USocketNet core instance.");
				callback( new ProjectObject("urlempty", "Please fill up RestApi url on this USocketNet core instance.") );
				return;
			}
            
            USocketNet.Core.StartCoroutine( VerifyingProject(secretkey, callback) );
        }

        IEnumerator VerifyingProject( string prjkey, Action<ProjectObject> callback ) 
        {            
            WWWForm creds = new WWWForm();
            creds.AddField("wpid", USocketNet.User.wpid);
            creds.AddField("snid", USocketNet.User.snid);
            creds.AddField("pkey", prjkey);

            string rapi = USocketNet.config.restapiUrl;
            string endString = rapi[rapi.Length - 1] == '/' ? "" : "/";
            var request = UnityWebRequest.Post( rapi + endString + "wp-json/usocketnet/v1/project/verify", creds);
            
            yield return request.SendWebRequest();
            
            if ( request.isNetworkError || request.isHttpError )
            {
                USocketNet.Log(Logs.Error, "RestApi", "Request failed with status code of "+request.responseCode+", Try again.");
                callback( new ProjectObject("reqerror", "Request failed with status code of "+request.responseCode+", Try again." ) );
            }

            else
            {
                string bytes = Encoding.UTF8.GetString( request.downloadHandler.data );
                ProjectObject response = JsonUtility.FromJson<ProjectObject>(bytes);

                if( response.success )
                {
                    USocketNet.Log(Logs.Log, "RestApi", "Play! " +response.data.name+ " [" +response.data.desc+ "]" );
                    //responseData = response.data;
                    callback( response );
                }

                else
                {
                    USocketNet.Log(Logs.Warn, "RestApi", response.message);
                    callback( response );
                }
            }
        }
    }
}
