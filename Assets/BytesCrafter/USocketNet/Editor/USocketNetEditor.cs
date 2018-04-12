#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using BytesCrafter.USocketNet;

namespace BytesCrafter.USocketNet
{
	[CustomEditor(typeof(USocketClient))]
	public class USocketNetEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			OnSocketNet ();
			//base.OnInspectorGUI ();
			EditorGUIUtility.labelWidth = 70f;
		}

		private void OnSocketNet()
		{
			USocketClient net = (USocketClient)target;

			#region Header Helpbox
			EditorGUILayout.Separator ();
			EditorGUILayout.HelpBox("USocketNet is something like NetworkManager where you can call connect, diconnect, automatch, create, join, " +
				"send public message, send private message to a specific USocketNet instance, send message to the whole peers of the current channel " +
				"currently subscribe to, triggers and listen to a custom events that you made. Click on 'Show Debugging Window' to know more about the " +
				"current network statistics and object instances.", MessageType.Info);
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (100f);
			if(GUILayout.Button("Show Debugging Window"))
			{
				USocketNetWindow.ShowWindow ();
			}
			GUILayout.Space (100f);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			#endregion

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.serverUrl = EditorGUILayout.TextField ("Address", net.bindings.serverUrl);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.serverPort = EditorGUILayout.TextField ("Port", net.bindings.serverPort);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.connectDelay = EditorGUILayout.Slider("Connect Delay", net.bindings.connectDelay, 1f, 7f);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			net.bindings.mainSendRate = EditorGUILayout.IntSlider ("Override Sync Rate", net.bindings.mainSendRate, 1, 30);
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
			net.bindings.runOnBackground = EditorGUILayout.Toggle ("Run on Background", net.bindings.runOnBackground);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (25f);
			EditorGUILayout.LabelField("USocketView Prefabs:");
			EditorGUILayout.EndHorizontal ();
			for(int i = 0; i < net.socketPrefabs.Count; i++)
			{
				EditorGUILayout.Separator ();
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (50f);
				if(net.socketPrefabs[i] == null)
				{
					net.socketPrefabs[i] = (USocketView)EditorGUILayout.ObjectField ("Name: Nothing!", null, typeof(USocketView), true); GUILayout.Space (12f);
				}

				else
				{
					net.socketPrefabs[i] = (USocketView)EditorGUILayout.ObjectField ("Name: " + net.socketPrefabs[i].name, net.socketPrefabs[i], typeof(USocketView), true); GUILayout.Space (12f);
				}

				if(GUILayout.Button("X"))
				{
					net.socketPrefabs.RemoveAt (i);
				}
				GUILayout.Space (50f);
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (50f);
			if(GUILayout.Button("Add Prefab"))
			{
				net.socketPrefabs.Add (null);
			}
			GUILayout.Space (50f);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();

			if(GUI.changed)
			{
				EditorUtility.SetDirty ( target );
			}
		}
	}
}

#endif