using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BytesCrafter.USocketNet;
using BytesCrafter.USocketNet.Serials;

public class Demoguy : USocketNet
{
	#region Variablles and References

	[Header("CANVAS DISPLAYS")]
	public List<CanvasGroup> canvasGroup = new List<CanvasGroup>();
	public void ChangeCanvas(int index)
	{
		canvasGroup.ForEach ((CanvasGroup cgroup) => {
			cgroup.alpha = 0f;
			cgroup.gameObject.SetActive(false);
		});

		canvasGroup [index].alpha = 1f;
		canvasGroup [index].gameObject.SetActive (true);
	}

	[Header("Connections")]
	public InputField username = null;
	public InputField password = null;

	#endregion

	#region CONNECTION

	//Connecting to server with callbacks.
	public void ConnectToServer()
	{
		//STEP3: You now need to authenticate the user with username and password to ask for the
		// server for a token to be able for us to connect to the websocket port.
		USocketNet.Core.Authenticate (username.text, password.text, (BC_USN_Response response) =>
		{
			if( response.success )
			{
				// netScript.Connect( (ConStat conStat) => {
				// 	if( conStat == ConStat.Success ) {
				// 		ChangeCanvas(1);
				// 	}

				// 	UnityEngine.Debug.Log("WPID: " + response.data.id + " SNID: " + response.data.session + " Response: " + conStat.ToString() + " SID:" + netScript.Identity);
				// });
				
			}

			else
			{
				
			}
		});
	}

	//Disconnecting to server.
	public void DisconnectFromServer()
	{
		//netScript.Disconnect ();
		ChangeCanvas(0);
	}

	#endregion

	/// <summary>
	/// STEP1: Important! You need to declare 'Config' variable to set your restapi and websocket url.
	/// The said class can be derived from this namespace: BytesCrafter.USocketNet.Serials;
	/// </summary>
	public Config serverConfig = new Config(); 

	void Awake()
	{
		//STEP2: Important! Next is to initialize the 'USocketNet' instance with the 'Config' which is
		// is previously declared as a variable and set the values before app starts.
		USocketNet.Initialized(serverConfig);
	}



	

	

	

















	

	[Header("PUBLIC MESSAGE")]
	public InputField pubMsgContent = null;
	public MessageDisplay publicViewer = null;

	[Header("PRIVATE MESSAGE")]
	public InputField priMsgContent = null;
	public InputField priMsgReceiver = null;
	public MessageDisplay privateViewer = null;

	[Header("CHANNEL MESSAGE")]
	public InputField chanMsgContent = null;
	public MessageDisplay channelViewer = null;

	[Header("Server Room")]
	public InputField roomname = null;

	[Header("PING MECHANISM")]
	public Text pingSocket = null;
}