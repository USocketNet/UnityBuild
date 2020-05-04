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
	public void ChangeCanvas(int index) {
		canvasGroup.ForEach ((CanvasGroup cgroup) => {
			cgroup.alpha = 0f;
			cgroup.gameObject.SetActive(false);
		});

		canvasGroup [index].alpha = 1f;
		canvasGroup [index].gameObject.SetActive (true);
	}
	public CanvasGroup masterDisplay;
	private void ShowMaster(bool yes) {
		if(yes) {
			masterDisplay.alpha = 1f;
			masterDisplay.gameObject.SetActive (true);
		} else {
			masterDisplay.alpha = 0f;
			masterDisplay.gameObject.SetActive (false);
		}
	}

	[Header("PING MECHANISM")]
	public Text pingSocket = null;

	[Header("Connections")]
	public InputField username = null;
	public InputField password = null;
	public InputField appsecret = null;

	#endregion

	#region CONNECTION

	public void SignIn() {
		//STEP3: You now need to authenticate the user with username and password to ask for the
		// server for a token to be able for us to connect to the websocket port.
		USocketNet.Core.Authenticate (username.text, password.text, (BC_USN_Response response) => {
			if( response.success ) {
				USocketNet.Log(Logs.Log, "Demogguy", "WPID: " + response.data.wpid + " SNID: " + response.data.snid 
					+ " Response: " + " Welcome! :" + response.data.uname);
				ChangeCanvas(1);
			} else {
				USocketNet.Log(Logs.Log, "Demoguy", "RestAPI failed to respond properly, check the url.");
			}
		});
	}

	public void Select() {
		USocketNet.Core.VerifyProject (appsecret.text, (ProjectObject response) => {
			if( response.success ) {
				USocketNet.Log(Logs.Log, "Demogguy", "Name: " + response.data.name + " Desc: " + response.data.desc 
					+ " Match User: " + response.data.matchcap + " Capacity! :" + response.data.capacity);
				ChangeCanvas(2);
			} else {
				USocketNet.Log(Logs.Log, "Demoguy", "RestAPI failed to respond properly, check the url.");
			}
		});
	}

	public void SignOut() {
		//OPTIONAL: You can call SignOut to forcibly close all client associated with this USocketNet
		// instance. Also, the if Reauthentication on app load will be postpone.
		USocketNet.Core.SignOut ();
		ChangeCanvas(0);
	}

	public void ConnectToServer() {
		USocketNet.Core.MasterConnect(appsecret.text, (ConStat conStat) => {
			if( conStat == ConStat.Success ) {
				//Enable master display.
				ShowMaster(true);
				USocketNet.Log(Logs.Log, "Demoguy", "You are now connected with id: " + USocketNet.Core.Master.Identity );
			} else {
				USocketNet.Log(Logs.Log, "Demoguy", "Connection failed on status of: " + conStat.ToString() );
			}
		});
	}

	public void DisconnectFromServer() {
		//Hide master display.
		ShowMaster(false);
		USocketNet.Core.MasterDisconnect();
	}

	#endregion

	/// <summary>
	/// STEP1: Important! You need to declare 'Config' variable to set your restapi and websocket url.
	/// The said class can be derived from this namespace: BytesCrafter.USocketNet.Serials;
	/// </summary>
	public Config serverConfig = new Config(); 

	void Awake() {
		ChangeCanvas(0);

		//STEP2: Important! Next is to initialize the 'USocketNet' instance with the 'Config' which is
		// is previously declared as a variable and set the values before app starts.
		USocketNet.Initialized(serverConfig);
	}

	[Header("MESSAGING")]
	public Toggle chatBotEnabled = null;
	public InputField priMsgContent = null;
	public InputField priMsgReceiver = null;
	public MessageDisplay privateViewer = null;
	public Text msgPrefabItem = null;
	public Transform msgPrefabParent = null;

	public void ConnectMessageClient() {
		USocketNet.Core.MessageConnect(appsecret.text, (ConStat conStat) => {
			if( conStat == ConStat.Success ) {
				ChangeCanvas(3);
				USocketNet.Core.Message.ListensOnMessage(MsgType.pub, OnPublicMessage);
			}
			USocketNet.Log(Logs.Log, "Demogguy", "Connection to Chat Server return: " + conStat.ToString() );
		});
	}

	private void OnPublicMessage(MsgJson msgJson) {
		if(msgPrefabParent.childCount > 60) {
			Text last = msgPrefabParent.GetChild(msgPrefabParent.childCount-2).GetComponent<Text>();
			last.transform.SetAsFirstSibling();
			last.text = msgJson.username  + " (" + msgJson.datestamp +"): " + msgJson.message;
		} else {
			Text msgView = Instantiate(msgPrefabItem.gameObject, msgPrefabParent).GetComponent<Text>();
			msgView.transform.SetAsFirstSibling();
			msgView.text = msgJson.username  + " (" + msgJson.datestamp +"): " + msgJson.message;
			msgView.gameObject.SetActive(true);
		}
		
	}

	public void SendMessage() {
		USocketNet.Core.Message.SendMessage(MsgType.pub, priMsgContent.text, (MsgRes msgRes) => {
			if(msgRes.status == RStats.Success) {
				priMsgContent.text = string.Empty;
			}
			USocketNet.Log(Logs.Log, "Demogguy", "Auto Public Message Send: " + msgRes.status.ToString() );
		});
	}

	public void DisconnectMessageClient() {
		ChangeCanvas(2);
		USocketNet.Core.MessageDisconnect();
	}


	public void ConnectMatchClient() {
		USocketNet.Core.MatchConnect(appsecret.text, (ConStat conStat) => {
			USocketNet.Log(Logs.Log, "Demogguy", "Connection to Game Server return: " + conStat.ToString() );
		});
	}

	public void DisconnectMatchClient() {
		USocketNet.Core.MatchDisconnect();
	}

	float timer = 0f;

	void Update() {
		if(USocketNet.Core.IsMasterConnected) {
			pingSocket.text = USocketNet.Core.Master.GetPingInMS + "ms";
		}

		if(USocketNet.Core.IsMessageConnected)
		{	
			timer += Time.deltaTime;
			if( timer > 2f) {
				if(chatBotEnabled != null) {
					if(chatBotEnabled.isOn) {
						//Send Random Message.
						USocketNet.Core.Message.SendMessage(MsgType.pub, RandomMessage, (MsgRes msgRes) => {
							USocketNet.Log(Logs.Log, "Demogguy", "Manual Public Message Send: " + msgRes.status.ToString() );
						});
					}
				}
				timer = 0f;
			}
		}
	}

	string RandomMessage {
		get {
			string result = string.Empty;
			string charList = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			for ( var i = 0; i < 15; i++ ) {
				result += charList[Random.Range(0, 20)];
			}
			return result;
		}
	}
	

	

	

















	

	[Header("PUBLIC MESSAGE")]
	public InputField pubMsgContent = null;
	public MessageDisplay publicViewer = null;

	

	[Header("CHANNEL MESSAGE")]
	public InputField chanMsgContent = null;
	public MessageDisplay channelViewer = null;

	[Header("Server Room")]
	public InputField roomname = null;

	
}