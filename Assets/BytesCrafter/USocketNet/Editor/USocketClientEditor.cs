//#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using BytesCrafter.USocketNet;

using BytesCrafter.USocketNet.Serializables;
namespace BytesCrafter.USocketNet
{
	[CustomEditor(typeof(USocketClient))]
	public class USocketClientEditor : Editor
	{
		Texture2D headerTexture;
		Rect headerRect;

		public override void OnInspectorGUI()
		{
			//EditorGUIUtility.labelWidth = 70f;
			float scrWidth = EditorGUIUtility.currentViewWidth;
			EditorGUIUtility.fieldWidth = 29f;
			USocketClient net = (USocketClient)target;

			OnHeaderDraw ();
			OnServerDetails (net, scrWidth);
			OnSocketOptions (net, scrWidth);
			OnVewPrefabs (net, scrWidth);

			if(GUI.changed)
			{
				EditorUtility.SetDirty ( target );
			}
		}

		private void OnHeaderDraw()
		{
			GUILayout.BeginVertical (EditorStyles.helpBox);
			EditorGUILayout.HelpBox("USocketClient is something like NetworkManager where you can call connect, diconnect, automatch, create, join, " +
				"send public message, send private message to a specific USocketNet instance, send message to the whole peers of the current channel " +
				"currently subscribe to, triggers and listen to a custom events that you made. Click on 'Show Debugging Window' to know more about the " +
				"current network statistics and object instances.", MessageType.Info);
			GUILayout.Space (7f);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (Screen.width * 0.25f);
			if(GUILayout.Button("Show Debugging Window"))
			{
				USocketNetWindow.ShowWindow ();
			}
			GUILayout.Space (Screen.width * 0.25f);
			EditorGUILayout.EndHorizontal ();

			GUILayout.Space (7f);
			GUILayout.EndVertical ();
		}

		private void OnServerDetails(USocketClient net, float scrWidth)
		{
			EditorGUILayout.BeginVertical (EditorStyles.helpBox);
			EditorGUILayout.BeginHorizontal (EditorStyles.helpBox);
			GUILayout.Space (25f);
			EditorGUILayout.LabelField("Server Information:");
			EditorGUILayout.EndHorizontal ();
			GUILayout.Space (7f);

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (40f);
			net.bindings.serverUrl = EditorGUILayout.TextField ("Address", net.bindings.serverUrl);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (40f);
			net.bindings.serverPort = EditorGUILayout.IntField ("Port", System.Convert.ToInt16(net.bindings.serverPort)) + string.Empty;
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (40f);
			net.bindings.authenKey = EditorGUILayout.PasswordField ("Appkey", net.bindings.authenKey);
			EditorGUILayout.EndHorizontal ();

			GUILayout.Space (7f);
			EditorGUILayout.EndVertical ();
		}

		private void OnSocketOptions(USocketClient net, float scrWidth)
		{
			EditorGUILayout.BeginVertical (EditorStyles.helpBox);
			EditorGUILayout.BeginHorizontal (EditorStyles.helpBox);
			GUILayout.Space (25f);
			EditorGUILayout.LabelField("Connection Options:");
			EditorGUILayout.EndHorizontal ();
			GUILayout.Space (7f);

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.syncGroup = (SyncGroup)EditorGUILayout.EnumPopup ("Sync Group", net.bindings.syncGroup);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.connectDelay = EditorGUILayout.Slider("Connect Delay", net.bindings.connectDelay, 1f, 7f);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.mainSendRate = EditorGUILayout.IntSlider ("Sync Rate", net.bindings.mainSendRate, 1, 30);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.pingFrequency = EditorGUILayout.IntSlider ("Ping Frequency", System.Convert.ToInt16(net.bindings.pingFrequency), 1, 60);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.debugOnLog = EditorGUILayout.Toggle ("Debug Custom Log", net.bindings.debugOnLog);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.dontDestroyOnLoad = EditorGUILayout.Toggle ("Dont Destroy on Load", net.bindings.dontDestroyOnLoad);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.runOnBackground = EditorGUILayout.Toggle ("Run on Background", net.bindings.runOnBackground);
			EditorGUILayout.EndHorizontal ();

			GUILayout.Space (7f);
			EditorGUILayout.EndVertical ();
		}

		private void OnVewPrefabs(USocketClient net, float scrWidth)
		{
			EditorGUILayout.BeginVertical (EditorStyles.helpBox);
			EditorGUILayout.BeginHorizontal (EditorStyles.helpBox);
			GUILayout.Space (25f);
			EditorGUILayout.LabelField("USocketView Prefabs:");
			EditorGUILayout.EndHorizontal ();

			GUILayout.Space (7f);
			for(int i = 0; i < net.socketPrefabs.Count; i++)
			{
				GUILayout.Space (3f);

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (40f);
				GUILayout.Label ("Item " + i);
				if(net.socketPrefabs[i] == null)
				{
					net.socketPrefabs[i] = (USocketView)EditorGUILayout.ObjectField ( null, typeof(USocketView), true);
					GUILayout.Space (7f);
				}

				else
				{
					net.socketPrefabs[i] = (USocketView)EditorGUILayout.ObjectField (net.socketPrefabs[i], typeof(USocketView), true);
					GUILayout.Space (7f);
				}

				if(GUILayout.Button("X", GUILayout.MaxWidth(25f)))
				{
					net.socketPrefabs.RemoveAt (i);
				}
				EditorGUILayout.EndHorizontal ();
			}
			GUILayout.Space (7f);

			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (scrWidth/4);
			if(GUILayout.Button("Add Prefab", GUILayout.MinWidth(149f)))
			{
				net.socketPrefabs.Add (null);
			}
			GUILayout.Space (scrWidth/4);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();
			EditorGUILayout.EndVertical ();

			EditorGUILayout.Separator ();
		}
	}
}

//#endif