using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BytesCrafter.USocketNet;
using BytesCrafter.USocketNet.Serials;
using BytesCrafter.USocketNet.Toolsets;

public class Demoguy : MonoBehaviour
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
	public InputField appsecret = null;

	#endregion

	#region CONNECTION

	public void SignIn()
	{
		//STEP3: You now need to authenticate the user with username and password to ask for the
		// server for a token to be able for us to connect to the websocket port.
		USocketNet.Core.Authenticate (username.text, password.text, (BC_USN_Response response) =>
		{
			if( response.success ) {
				USocketNet.Log(Logs.Warn, "Demogguy", "WPID: " + response.data.id + " SNID: " + response.data.session 
					+ " Response: " + " Welcome! :" + response.data.uname);
				ChangeCanvas(1);
			} else {
				USocketNet.Log(Logs.Warn, "Demogguy", "Failed connection to websocket server.");
			}
		});
	}

	public void SignOut()
	{
		//OPTIONAL: You can call SignOut to forcibly close all client associated with this USocketNet
		// instance. Also, the if Reauthentication on app load will be postpone.
		USocketNet.Core.SignOut ();
		ChangeCanvas(0);
	}

	public void ConnectToServer()
	{
		USocketNet.Core.Connect(appsecret.text, (ConStat conStat) => {
			if( conStat == ConStat.Success ) {
				ChangeCanvas(2);
				USocketNet.Log(Logs.Warn, "Demogguy", "You are now connected with id: " + USocketNet.Core.Master.Identity );
			} else {
				USocketNet.Log(Logs.Warn, "Demogguy", "You did not connect successfully: " + conStat.ToString() );
			}
		});
	}

	public void DisconnectFromServer()
	{
		USocketNet.Core.Disconnect();
		ChangeCanvas(1);
	}

	#endregion

	/// <summary>
	/// STEP1: Important! You need to declare 'Config' variable to set your restapi and websocket url.
	/// The said class can be derived from this namespace: BytesCrafter.USocketNet.Serials;
	/// </summary>
	public Config serverConfig = new Config(); 

	void Awake()
	{
		ChangeCanvas(0);

		//STEP2: Important! Next is to initialize the 'USocketNet' instance with the 'Config' which is
		// is previously declared as a variable and set the values before app starts.
		USocketNet.Initialized(serverConfig);
	}

	[Header("MESSAGING")]
	public InputField priMsgContent = null;
	public InputField priMsgReceiver = null;
	public MessageDisplay privateViewer = null;

	public void AddChatClient()
	{
		USocketNet.Core.AddChatClient(appsecret.text, (ConStat conStat) => {
			USocketNet.Log(Logs.Warn, "Demogguy", "Connection to Chat Server return: " + conStat.ToString() );
		});
	}

	public void SendMessage()
	{

	}

	public void RemoveChatClient()
	{
		USocketNet.Core.RemoveChatClient();
	}


	public void AddGameClient()
	{
		USocketNet.Core.AddGameClient(appsecret.text, (ConStat conStat) => {
			USocketNet.Log(Logs.Warn, "Demogguy", "Connection to Game Server return: " + conStat.ToString() );
		});
	}

	public void RemoveGameClient()
	{
		USocketNet.Core.RemoveGameClient();
	}



	

	

	

















	

	[Header("PUBLIC MESSAGE")]
	public InputField pubMsgContent = null;
	public MessageDisplay publicViewer = null;

	

	[Header("CHANNEL MESSAGE")]
	public InputField chanMsgContent = null;
	public MessageDisplay channelViewer = null;

	[Header("Server Room")]
	public InputField roomname = null;

	[Header("PING MECHANISM")]
	public Text pingSocket = null;
}