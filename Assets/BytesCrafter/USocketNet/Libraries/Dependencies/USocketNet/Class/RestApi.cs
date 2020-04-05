using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using BytesCrafter.USocketNet.Serializables;

namespace BytesCrafter.USocketNet.RestApi {
    public class BC_USN_RestApi 
    {
        #region Authentication - Ongoing!
        public void Authenticate(string uname, string pword, USocketClient usnClient, Action<Response> callback) {
             usnClient.StartCoroutine( Authenticating(uname, pword, callback) );
        }
        
        IEnumerator Authenticating( string uname, string pword, Action<Response> callback ) {            
            
            WWWForm creds = new WWWForm();
            creds.AddField("UN", uname);
            creds.AddField("PW", pword);

            var request = UnityWebRequest.Post("http://localhost/wp-json/usocketnet/v1/auth", creds);
            yield return request.SendWebRequest();

            if ( request.isNetworkError || request.isHttpError )
            {
                callback( new Response() );
            }

            else
            {
                string bytes = Encoding.UTF8.GetString( request.downloadHandler.data );
                Response response = JsonUtility.FromJson<Response>(bytes);
                callback( response );
            }
        }
		#endregion
    }
}
