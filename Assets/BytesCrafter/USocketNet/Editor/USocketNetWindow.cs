//#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using BytesCrafter.USocketNet;

namespace BytesCrafter.USocketNet
{
	public class USocketNetWindow : EditorWindow
	{
		[MenuItem("Bytes Crafter/USocketNet/Show")]
		public static void ShowWindow()
		{
			GetWindow<USocketNetWindow> ("USocketNet");
		}

		[MenuItem("Bytes Crafter/USocketNet/Demo")]
		public static void SurfDemoTutorial()
		{
			Application.OpenURL ("https://www.bytes-crafter.com/portfolio/usocketnet-self-hosted-multiplayer-server-for-unity-and-website/");
		}

		[MenuItem("Bytes Crafter/USocketNet/About")]
		public static void SurfAppWebsite()
		{
			Application.OpenURL ("https://www.bytes-crafter.com/portfolio/usocketnet-self-hosted-multiplayer-server-for-unity-and-website/");
		}

		[MenuItem("Bytes Crafter/More/MobilePOS")]
		public static void VisitMobilePOS()
		{
			Application.OpenURL ("https://www.bytes-crafter.com/portfolio/mopos-point-of-sale-application-for-small-business/");
		}

		[MenuItem("Bytes Crafter/More/DataVice")]
		public static void VisitDataVice()
		{
			Application.OpenURL ("https://www.bytes-crafter.com/portfolio/datavice-user-management-solution-for-unity-or-website/");
		}

		[MenuItem("Bytes Crafter/More/USocketNet")]
		public static void VisitUSocketNet()
		{
			Application.OpenURL ("https://www.bytes-crafter.com/portfolio/usocketnet-self-hosted-multiplayer-server-for-unity-and-website/");
		}

		[MenuItem("Bytes Crafter/More/Digital Store")]
		public static void VisitDigitalStore()
		{
			Application.OpenURL ("http://www.bytes-crafter.com/assets");
		}

		[MenuItem("Bytes Crafter/About")]
		public static void SurfOfficialWebsite()
		{
			Application.OpenURL ("http://www.bytes-crafter.com/about-us");
		}

		void OnInspectorUpdate()
		{
			Repaint ();
		}

		//Texture2D headerTexture;
		//Rect headerRect;

		void OnGUI ()
		{
			/*
			EditorGUIUtility.labelWidth = 70f;

			headerTexture = new Texture2D (1, 1);
			headerTexture.SetPixel (0, 0, Color.grey);
			headerTexture.Apply ();
			headerRect = new Rect ();

			headerRect.x = 0;
			headerRect.y = 0;
			headerRect.width = Screen.width;
			headerRect.height = 49;

			GUI.DrawTexture (headerRect, headerTexture);
			*/

			#region Header Helpbox
			EditorGUILayout.Separator ();
			EditorGUILayout.HelpBox("USocketNet is something like NetworkView in Photon and NetworkIdentity in Unity. " +
				"The main difference is I combine all the possible datas like transform, animator, and state. " +
				"Just to make things easier and much easy to understand. You can always enable and disable sync data to " +
				"minimized network traffic or maximized data sync qualities.", MessageType.Info);
			EditorGUILayout.Separator ();
			#endregion

			EditorGUILayout.Separator ();
			GUILayout.Label ("Network Instances", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("USocketNet Clients: ");
			EditorGUILayout.LabelField (USocketNet.Instance.usocketNets.Count.ToString());
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			int totalLocalViews = 0;
			foreach(USocketClient unets in USocketNet.Instance.usocketNets)
			{
				// totalLocalViews += unets.localSockets.Count;
			}
			EditorGUILayout.LabelField ("USocketView Locals: ");
			EditorGUILayout.LabelField (totalLocalViews.ToString());
			EditorGUILayout.EndHorizontal ();

			// EditorGUILayout.BeginHorizontal ();
			// EditorGUILayout.LabelField ("USocketView Peers: ");
			// EditorGUILayout.LabelField (USocketNet.Instance.socketIdentities.Count.ToString());
			// EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			GUILayout.Label ("Data Traffics", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Sync Rate: ");
			if(USocketNet.Instance.usocketNets.Count > 0)
			{
				EditorGUILayout.LabelField (USocketNet.Instance.synchRate.ToString() + " sync/second");
			}

			else
			{
				EditorGUILayout.LabelField ("0 sync/second");
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Upload Rate: ");
			EditorGUILayout.LabelField (USocketNet.Instance.uploadRate.ToString() + " bytes/sent");
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Download Rate: ");
			EditorGUILayout.LabelField (USocketNet.Instance.downloadRate.ToString() + " bytes/received");
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
		}
	}

}
	
//#endif