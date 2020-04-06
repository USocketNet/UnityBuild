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
        private USocketClient usnClient = null;
        public BC_USN_RestApi( USocketClient reference )
        {
            usnClient = reference;
        }

        public Response_Data GetUserData
        {
            get 
            {
                return curUser; 
            }
        }
        private Response_Data curUser = new Response_Data();
        public void Authenticate(string uname, string pword, USocketClient usnClient, Action<Response> callback) 
        {
            if( usnClient.bind.restapiUrl == string.Empty )
			{
				usnClient.Debug(BC_USN_Debug.Warn, "RestApi", "Please fill up RestApi url on this USocketClient: " + usnClient.name);
				callback( new Response() );
				return;
			}
            
            usnClient.StartCoroutine( Authenticating(uname, pword, callback) );
        }
        
        IEnumerator Authenticating( string uname, string pword, Action<Response> callback ) 
        {            
            WWWForm creds = new WWWForm();
            creds.AddField("UN", uname);
            creds.AddField("PW", pword);

            string rapi = usnClient.bind.restapiUrl;
            string startString = rapi[0] == 'h' && rapi[1] == 't' && rapi[2] == 't' && rapi[3] == 'p' ? "" : "http://";
            string endString = rapi[rapi.Length - 1] == '/' ? "" : "/";
            var request = UnityWebRequest.Post( startString + rapi + endString + "wp-json/usocketnet/v1/auth", creds);
            
            yield return request.SendWebRequest();
            
            if ( request.isNetworkError || request.isHttpError )
            {
                usnClient.Debug(BC_USN_Debug.Error, "RestApi", "The Rest API url return 404 Not found. Please check and try again.");
                callback( new Response() );
            }

            else
            {
                string bytes = Encoding.UTF8.GetString( request.downloadHandler.data );
                Response response = JsonUtility.FromJson<Response>(bytes);

                if( response.success )
                {
                    usnClient.Debug(BC_USN_Debug.Log, "RestApi", "Welcome! " +response.data.dname+ " [" +response.data.email+ "]" );
                    curUser = response.data;
                    callback( response );
                }

                else
                {
                    usnClient.Debug(BC_USN_Debug.Warn, "RestApi", response.message);
                    callback( new Response() );
                }
            }
        }
    }
}
