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

        public BC_USN_Response_Data GetUserData
        {
            get 
            {
                return curUser; 
            }
        }
        private BC_USN_Response_Data curUser = new BC_USN_Response_Data();
        public void Authenticate(string uname, string pword, USocketClient usnClient, Action<BC_USN_Response> callback) 
        {
            if( usnClient.config.restapiUrl == string.Empty )
			{
				usnClient.Logs(BC_USN_Debug.Warn, "RestApi", "Please fill up RestApi url on this USocketClient: " + usnClient.name);
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
                usnClient.Logs(BC_USN_Debug.Error, "RestApi", "The Rest API url return 404 Not found. Please check and try again.");
                callback( new BC_USN_Response() );
            }

            else
            {
                string bytes = Encoding.UTF8.GetString( request.downloadHandler.data );
                BC_USN_Response response = JsonUtility.FromJson<BC_USN_Response>(bytes);

                if( response.success )
                {
                    usnClient.Logs(BC_USN_Debug.Log, "RestApi", "Welcome! " +response.data.dname+ " [" +response.data.email+ "]" );
                    curUser = response.data;
                    callback( response );
                }

                else
                {
                    usnClient.Logs(BC_USN_Debug.Warn, "RestApi", response.message);
                    callback( new BC_USN_Response() );
                }
            }
        }
    }
}
