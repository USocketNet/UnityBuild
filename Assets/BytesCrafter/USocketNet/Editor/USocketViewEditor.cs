#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using BytesCrafter.USocketNet;
using BytesCrafter.USocketNet.Serializables;

namespace BytesCrafter.USocketNet
{
	[CustomEditor(typeof(USocketView))]
	public class USocketViewEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUIUtility.labelWidth = 70f;
			OnUSocketView ();
		}

		private void OnUSocketView()
		{
			USocketView view = (USocketView)target;

			#region Header Helpbox
			EditorGUILayout.Separator ();
			EditorGUILayout.HelpBox("USocketView is something like NetworkView in Photon and NetworkIdentity in Unity. " +
				"The main difference is I combine all the possible datas like transform, animator, and state. " +
				"Just to make things easier and much easy to understand. You can always enable and disable sync data to " +
				"minimized network traffic or maximized data sync qualities.", MessageType.Info);
			EditorGUILayout.Separator ();
			#endregion

			#region POSITION AREA
			EditorGUILayout.Separator ();
			view.position.synchronize = EditorGUILayout.Toggle ("POSITION", view.position.synchronize);
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			if(view.position.synchronize)
			{
				//EditorGUILayout.BeginHorizontal ();
				//GUILayout.Space (25f);
				//view.position.sendRate = EditorGUILayout.IntSlider ("Sync Rate", Convert.ToInt16(view.position.sendRate), 1, 30);
				//EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				view.position.syncMode = (SocketSync)EditorGUILayout.EnumPopup ("Sync Mode", view.position.syncMode);
				EditorGUILayout.EndHorizontal ();

				if(view.position.syncMode != SocketSync.Realtime)
				{
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.position.interpolation = EditorGUILayout.Slider ("Interpolation", view.position.interpolation, 1f, 30f);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.position.speed = EditorGUILayout.Slider ("Speed", view.position.speed, 1f, 30f);
					EditorGUILayout.EndHorizontal ();
				}
			}
			#endregion

			#region ROTATION AREA
			EditorGUILayout.Separator ();
			view.rotation.synchronize = EditorGUILayout.Toggle ("ROTATION", view.rotation.synchronize);
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			if(view.rotation.synchronize)
			{
				//EditorGUILayout.BeginHorizontal ();
				//GUILayout.Space (25f);
				//view.rotation.sendRate = EditorGUILayout.IntSlider ("Sync Rate", Convert.ToInt16(view.rotation.sendRate), 1, 30);
				//EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				view.rotation.syncMode = (SocketSync)EditorGUILayout.EnumPopup ("Sync Mode", view.rotation.syncMode);
				EditorGUILayout.EndHorizontal ();

				if(view.rotation.syncMode != SocketSync.Realtime)
				{
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.rotation.interpolation = EditorGUILayout.Slider ("Interpolation", view.position.interpolation, 1f, 30f);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.rotation.speed = EditorGUILayout.Slider ("Speed", view.rotation.speed, 1f, 30f);
					EditorGUILayout.EndHorizontal ();
				}

				EditorGUILayout.Separator ();
				EditorGUILayout.Separator ();
			}
			#endregion

			#region SCALE AREA
			EditorGUILayout.Separator ();
			view.scale.synchronize = EditorGUILayout.Toggle ("SCALE", view.scale.synchronize);
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			if(view.scale.synchronize)
			{
				//EditorGUILayout.BeginHorizontal ();
				//GUILayout.Space (25f);
				//view.scale.sendRate = EditorGUILayout.IntSlider ("Sync Rate", Convert.ToInt16(view.scale.sendRate), 1, 30);
				//EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				view.scale.syncMode = (SocketSync)EditorGUILayout.EnumPopup ("Sync Mode", view.scale.syncMode);
				EditorGUILayout.EndHorizontal ();

				if(view.scale.syncMode != SocketSync.Realtime)
				{
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.scale.interpolation = EditorGUILayout.Slider ("Slope", view.scale.interpolation, 1f, 30f);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.scale.speed = EditorGUILayout.Slider ("Speed", view.scale.speed, 1f, 30f);
					EditorGUILayout.EndHorizontal ();
				}

				EditorGUILayout.Separator ();
				EditorGUILayout.Separator ();
			}
			#endregion

			#region ANIMATOR AREA
			view.animator.synchronize = EditorGUILayout.Toggle ("ANIMATOR", view.animator.synchronize);
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			if(view.animator.synchronize)
			{
				#region References.
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f); view.animator.reference = (Animator)EditorGUILayout.ObjectField ("Reference:", view.animator.reference, typeof(Animator), true);
				if(view.animator.reference != null)
				{
					if(GUILayout.Button("Remove"))
					{
						view.animator.reference = null;
						view.animator.parameters = new List<AnimTypes> ();
					}
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				if(view.animator.reference == null)
				{
					EditorGUILayout.HelpBox ("Please set the reference of the animator to sync or kindly toggle off the animator to disregard synching animator data.", MessageType.Warning);
				}
				GUILayout.Space (25f);
				EditorGUILayout.EndHorizontal ();
				#endregion

				#region Parameters
				if(view.animator.reference != null)
				{
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (25f); EditorGUILayout.LabelField("Parameters:");
					EditorGUILayout.EndHorizontal ();
				}
				for(int i = 0; i < view.animator.parameters.Count; i++)
				{
					EditorGUILayout.Separator ();
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.animator.parameters[i].apValue = EditorGUILayout.TextField ("Name", view.animator.parameters[i].apValue); GUILayout.Space (12f);
					view.animator.parameters[i].apType = (APType)EditorGUILayout.EnumPopup ("Type", view.animator.parameters[i].apType); GUILayout.Space (12f);
					if(GUILayout.Button("X"))
					{
						view.animator.parameters.RemoveAt (i);
					}
					GUILayout.Space (50f);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);

					EditorGUILayout.EndHorizontal ();
				}

				if(view.animator.reference != null)
				{
					EditorGUILayout.Separator ();
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (25f);
					if(view.animator.parameters.Count == 0)
					{
						EditorGUILayout.HelpBox ("Please set the parameters found on the referenced animator or kindly toggle off the animator to disregard synching animator data.", MessageType.Warning);
					}
					GUILayout.Space (25f);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (25f);
					if(GUILayout.Button("Add"))
					{
						view.animator.parameters.Add (new AnimTypes(APType.Float));
					}
					GUILayout.Space (25f);
					if(GUILayout.Button("Reset"))
					{
						view.animator.parameters = new List<AnimTypes> ();
					}
					GUILayout.Space (25f);
					EditorGUILayout.EndHorizontal ();
				}

				#endregion
			}
			#endregion

			#region STATES AREA
			EditorGUILayout.Separator ();
			view.states.synchronize = EditorGUILayout.Toggle ("STATES", view.states.synchronize);
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			if(view.states.synchronize)
			{
				//EditorGUILayout.BeginHorizontal ();
				//GUILayout.Space (25f);
				//view.states.sendRate = EditorGUILayout.IntSlider ("Sync Rate", Convert.ToInt16(view.states.sendRate), 1, 30);
				//EditorGUILayout.EndHorizontal ();

				//EditorGUILayout.Separator ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				EditorGUILayout.LabelField("Capacity: " + view.states.syncValue.Count + " items");

				if(GUILayout.Button("Add"))
				{
					view.states.syncValue.Add (string.Empty);
				}

				if(view.states.syncValue.Count > 0)
				{
					if(GUILayout.Button("Remove"))
					{
						view.states.syncValue.RemoveAt(view.states.syncValue.Count - 1);
					}
				}

				GUILayout.Space (25f);
				EditorGUILayout.EndHorizontal ();
			}
			#endregion

			#region CHILDS AREA
			EditorGUILayout.Separator ();
			view.childs.synchronize = EditorGUILayout.Toggle ("CHILDS", view.childs.synchronize);
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			if(view.childs.synchronize)
			{
				//EditorGUILayout.BeginHorizontal ();
				//GUILayout.Space (25f);
				//view.childs.sendRate = EditorGUILayout.IntSlider ("Sync Rate", Convert.ToInt16(view.childs.sendRate), 1, 30);
				//EditorGUILayout.EndHorizontal ();

				//EditorGUILayout.Separator ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				EditorGUILayout.LabelField("Capacity: " + view.childs.childList.Count + " childs");
				EditorGUILayout.EndHorizontal ();


				EditorGUILayout.Separator ();
				for(int i = 0; i < view.childs.childList.Count; i++)
				{
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.Separator ();
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.childs.childList[i].reference = (Transform)EditorGUILayout.ObjectField ("Transform", view.childs.childList[i].reference, typeof(Transform), true); GUILayout.Space (12f);
					view.childs.childList[i].position.synchronize = EditorGUILayout.Toggle (view.childs.childList[i].position.synchronize); GUILayout.Space (1f);
					EditorGUILayout.LabelField("Position");
					view.childs.childList[i].rotation.synchronize = EditorGUILayout.Toggle (view.childs.childList[i].rotation.synchronize); GUILayout.Space (1f);
					EditorGUILayout.LabelField("Rotation");
					view.childs.childList[i].scale.synchronize = EditorGUILayout.Toggle (view.childs.childList[i].scale.synchronize); GUILayout.Space (1f);
					EditorGUILayout.LabelField("Scale");
					if(GUILayout.Button("X"))
					{
						view.childs.childList.RemoveAt (i);
					}
					GUILayout.Space (50f);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.EndHorizontal ();
				}
				EditorGUILayout.Separator ();


				EditorGUILayout.BeginHorizontal ();
				if(GUILayout.Button("Add"))
				{
					view.childs.childList.Add (new ViewChilds());
				}

				if(view.childs.childList.Count > 0)
				{
					if(GUILayout.Button("Remove"))
					{
						view.childs.childList.RemoveAt(view.childs.childList.Count - 1);
					}
				}

				GUILayout.Space (25f);
				EditorGUILayout.EndHorizontal ();
			}

			else
			{
				if(view.childs.childList.Count > 0)
				{
					view.childs.childList = new List<ViewChilds>();
				}
			}
			#endregion

			if(GUI.changed)
			{
				EditorUtility.SetDirty ( target );
			}
		}
	}
}

#endif