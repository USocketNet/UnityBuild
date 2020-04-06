using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
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

        public WPToken wptoken = new WPToken();
        public void Authenticate(string uname, string pword, USocketClient usnClient, Action<Response> callback) 
        {
            if( usnClient.bind.restapiUrl == string.Empty )
			{
				usnClient.Debug(Debugs.Warn, "ConnectionError", "Please fill up USocketNet restapi field on this USocketClient: " + usnClient.name);
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
                callback( new Response() );
            }

            else
            {
                string bytes = Encoding.UTF8.GetString( request.downloadHandler.data );
                Response response = JsonUtility.FromJson<Response>(bytes);

                if( response.success )
                {
                    wptoken = new WPToken(response.data.id, response.data.session);
                    callback( response );
                }

                else
                {
                    callback( new Response() );
                }
            }
        }
    }
}
