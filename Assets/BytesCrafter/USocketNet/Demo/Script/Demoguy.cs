using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BytesCrafter.USocketNet;
using BytesCrafter.USocketNet.Serials;
using BytesCrafter.USocketNet.Serializables;

public class Demoguy : USocketNet
{
	#region Variablles and References
	
	public Config serverConfig; 

	public USocketClient netScpt = null;
	public USocketClient netScript
	{
		get {
			if(netScpt == null)
			{
				netScpt = GameObject.FindObjectOfType<USocketClient> ();
			}

			return netScpt;
		}
	}

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
		netScript.Authenticate (username.text, password.text, (BC_USN_Response response) =>
			{
				if( response.success )
				{
					netScript.Connect( (ConStat conStat) => {
						if( conStat == ConStat.Success ) {
							ChangeCanvas(1);
						}

						UnityEngine.Debug.Log("WPID: " + response.data.id + " SNID: " + response.data.session + " Response: " + conStat.ToString() + " SID:" + netScript.Identity);
					});
					
				}

				else
				{
					
				}
			});
	}

	//Disconnecting to server.
	public void DisconnectFromServer()
	{
		netScript.Disconnect ();
		ChangeCanvas(0);
	}

	#endregion

	void Awake()
	{
		USocketNet.Initialized(serverConfig);
		Debug.Log("AAAAAAAAAAAAAAAA");
		//UnityEngine.Debug.LogWarning("sTARRTING @ " + auto);
	}


	// #region LISTENERS SAMPLE
	// protected override void OnConnection(bool auto)
	// {
	// 	UnityEngine.Debug.LogWarning("OnReconnection @ " + auto);
	// }

	// protected override void OnDisconnection(bool auto)
	// {
	// 	UnityEngine.Debug.LogWarning("OnDisconnection @ " + auto);
	// }
	// #endregion


	

	

	

















	

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