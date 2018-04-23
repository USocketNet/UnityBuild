//#if UNITY_EDITOR

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
			float scrWidth = EditorGUIUtility.currentViewWidth;
			EditorGUIUtility.fieldWidth = 29f;
			USocketView view = (USocketView)target;

			OnHeaderDraw ();
			OnPositionDraw (view, scrWidth);
			OnRotationDraw (view, scrWidth);
			OnScaleDraw (view, scrWidth);
			OnStateDraw (view, scrWidth);
			OnAnimatorDraw (view, scrWidth);
			OnChildDraw (view, scrWidth);

			if(GUI.changed)
			{
				EditorUtility.SetDirty ( target );
			}
		}

		private void OnHeaderDraw()
		{
			GUILayout.BeginVertical (EditorStyles.helpBox);
			EditorGUILayout.HelpBox("USocketView is something like NetworkView in Photon and NetworkIdentity in Unity. " +
				"The main difference is I combine all the possible datas like transform, animator, and state. " +
				"Just to make things easier and much easy to understand. You can always enable and disable sync data to " +
				"minimized network traffic or maximized data sync qualities.", MessageType.Info);
			GUILayout.Space (7f);
			GUILayout.EndVertical ();
			GUILayout.Space (7f);
		}

		private void OnPositionDraw(USocketView view, float scrWidth)
		{
			if(view.position.synchronize)
			{
				GUILayout.BeginVertical (EditorStyles.helpBox);
			}
			GUILayout.BeginVertical (EditorStyles.helpBox);
			view.position.synchronize = EditorGUILayout.Toggle ("POSITION", view.position.synchronize);
			GUILayout.Space (2f);
			GUILayout.EndVertical ();
			GUILayout.Space (7f);

			if(view.position.synchronize)
			{
				//EditorGUILayout.BeginHorizontal ();
				//GUILayout.Space (25f);
				//view.position.sendRate = EditorGUILayout.IntSlider ("Sync Rate", Convert.ToInt16(view.position.sendRate), 1, 30);
				//EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				GUILayout.Label("Target Axis");
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (50f);
				GUILayout.Label("x: ", GUILayout.MaxWidth(12f));
				view.position.axises.xAxis = EditorGUILayout.Toggle (view.position.axises.xAxis);
				GUILayout.Label("y: ", GUILayout.MaxWidth(12f));
				view.position.axises.yAxis = EditorGUILayout.Toggle (view.position.axises.yAxis);
				GUILayout.Label("z: ", GUILayout.MaxWidth(12f));
				view.position.axises.zAxis = EditorGUILayout.Toggle (view.position.axises.zAxis);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.Separator ();

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

			GUILayout.Space (7f);
			if(view.position.synchronize)
			{
				GUILayout.EndVertical ();
			}
		}

		private void OnRotationDraw(USocketView view, float scrWidth)
		{
			if(view.rotation.synchronize)
			{
				GUILayout.BeginVertical (EditorStyles.helpBox);
			}
			GUILayout.BeginVertical (EditorStyles.helpBox);
			view.rotation.synchronize = EditorGUILayout.Toggle ("ROTATION", view.rotation.synchronize);
			GUILayout.Space (2f);
			GUILayout.EndVertical ();
			GUILayout.Space (7f);

			if(view.rotation.synchronize)
			{
				//EditorGUILayout.BeginHorizontal ();
				//GUILayout.Space (25f);
				//view.rotation.sendRate = EditorGUILayout.IntSlider ("Sync Rate", Convert.ToInt16(view.rotation.sendRate), 1, 30);
				//EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				GUILayout.Label("Target Axis");
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (50f);
				GUILayout.Label("x: ", GUILayout.MaxWidth(12f));
				view.rotation.axises.xAxis = EditorGUILayout.Toggle (view.rotation.axises.xAxis);
				GUILayout.Label("y: ", GUILayout.MaxWidth(12f));
				view.rotation.axises.yAxis = EditorGUILayout.Toggle (view.rotation.axises.yAxis);
				GUILayout.Label("z: ", GUILayout.MaxWidth(12f));
				view.rotation.axises.zAxis = EditorGUILayout.Toggle (view.rotation.axises.zAxis);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.Separator ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				view.rotation.syncMode = (SocketSync)EditorGUILayout.EnumPopup ("Sync Mode", view.rotation.syncMode);
				EditorGUILayout.EndHorizontal ();

				if(view.rotation.syncMode != SocketSync.Realtime)
				{
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.rotation.interpolation = EditorGUILayout.Slider ("Interpolation", view.rotation.interpolation, 1f, 30f);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.rotation.speed = EditorGUILayout.Slider ("Speed", view.rotation.speed, 1f, 30f);
					EditorGUILayout.EndHorizontal ();
				}
			}

			GUILayout.Space (7f);
			if(view.rotation.synchronize)
			{
				GUILayout.EndVertical ();
			}
		}

		private void OnScaleDraw(USocketView view, float scrWidth)
		{
			if(view.scale.synchronize)
			{
				GUILayout.BeginVertical (EditorStyles.helpBox);
			}
			GUILayout.BeginVertical (EditorStyles.helpBox);
			view.scale.synchronize = EditorGUILayout.Toggle ("SCALE", view.scale.synchronize);
			GUILayout.Space (2f);
			GUILayout.EndVertical ();
			GUILayout.Space (7f);

			if(view.scale.synchronize)
			{
				//EditorGUILayout.BeginHorizontal ();
				//GUILayout.Space (25f);
				//view.scale.sendRate = EditorGUILayout.IntSlider ("Sync Rate", Convert.ToInt16(view.scale.sendRate), 1, 30);
				//EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				GUILayout.Label("Target Axis");
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (50f);
				GUILayout.Label("x: ", GUILayout.MaxWidth(12f));
				view.scale.axises.xAxis = EditorGUILayout.Toggle (view.scale.axises.xAxis);
				GUILayout.Label("y: ", GUILayout.MaxWidth(12f));
				view.scale.axises.yAxis = EditorGUILayout.Toggle (view.scale.axises.yAxis);
				GUILayout.Label("z: ", GUILayout.MaxWidth(12f));
				view.scale.axises.zAxis = EditorGUILayout.Toggle (view.scale.axises.zAxis);
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.Separator ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				view.scale.syncMode = (SocketSync)EditorGUILayout.EnumPopup ("Sync Mode", view.scale.syncMode);
				EditorGUILayout.EndHorizontal ();

				if(view.scale.syncMode != SocketSync.Realtime)
				{
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.scale.interpolation = EditorGUILayout.Slider ("Interpolation", view.scale.interpolation, 1f, 30f);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (50f);
					view.scale.speed = EditorGUILayout.Slider ("Speed", view.scale.speed, 1f, 30f);
					EditorGUILayout.EndHorizontal ();
				}
			}

			GUILayout.Space (7f);
			if(view.scale.synchronize)
			{
				GUILayout.EndVertical ();
			}
		}

		private void OnStateDraw(USocketView view, float scrWidth)
		{
			if(view.states.synchronize)
			{
				GUILayout.BeginVertical (EditorStyles.helpBox);
			}
			GUILayout.BeginVertical (EditorStyles.helpBox);
			view.states.synchronize = EditorGUILayout.Toggle ("STATE", view.states.synchronize);
			GUILayout.Space (2f);
			GUILayout.EndVertical ();
			GUILayout.Space (7f);

			if(view.states.synchronize)
			{
				//EditorGUILayout.BeginHorizontal ();
				//GUILayout.Space (25f);
				//view.states.sendRate = EditorGUILayout.IntSlider ("Sync Rate", Convert.ToInt16(view.states.sendRate), 1, 30);
				//EditorGUILayout.EndHorizontal ();

				//EditorGUILayout.Separator ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (scrWidth/10);
				EditorGUILayout.LabelField("Capacity: " + view.states.syncValue.Count + " items");

				if(GUILayout.Button("Add", GUILayout.MaxWidth(49f)))
				{
					view.states.syncValue.Add (string.Empty);
				}

				if(view.states.syncValue.Count > 0)
				{
					if(GUILayout.Button("Remove", GUILayout.MaxWidth(75f)))
					{
						view.states.syncValue.RemoveAt(view.states.syncValue.Count - 1);
					}
				}

				EditorGUILayout.EndHorizontal ();
			}

			GUILayout.Space (7f);
			if(view.states.synchronize)
			{
				GUILayout.EndVertical ();
			}
		}

		private void OnAnimatorDraw(USocketView view, float scrWidth)
		{
			if(view.animator.synchronize)
			{
				GUILayout.BeginVertical (EditorStyles.helpBox);
			}
			GUILayout.BeginVertical (EditorStyles.helpBox);
			view.animator.synchronize = EditorGUILayout.Toggle ("ANIMATOR", view.animator.synchronize);
			GUILayout.Space (2f);
			GUILayout.EndVertical ();
			GUILayout.Space (7f);

			if(view.animator.synchronize)
			{
				#region References.
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				GUILayout.Label("Reference", GUILayout.MaxWidth(70f));
				view.animator.reference = (Animator)EditorGUILayout.ObjectField (view.animator.reference, typeof(Animator), true);
				if(view.animator.reference != null)
				{
					if(GUILayout.Button("Remove", GUILayout.MaxWidth(75f)))
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
					GUILayout.Space (40f);
					GUILayout.Label("Name", GUILayout.MaxWidth(70f));
					view.animator.parameters[i].apValue = EditorGUILayout.TextField (view.animator.parameters[i].apValue);
					GUILayout.Label("Type", GUILayout.MaxWidth(70f));
					view.animator.parameters[i].apType = (APType)EditorGUILayout.EnumPopup (view.animator.parameters[i].apType);
					GUILayout.Space (3f);
					if(GUILayout.Button("X"))
					{
						view.animator.parameters.RemoveAt (i);
					}
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
			}

			GUILayout.Space (7f);
			if(view.animator.synchronize)
			{
				GUILayout.EndVertical ();
			}
		}

		private void OnChildDraw(USocketView view, float scrWidth)
		{
			if(view.childs.synchronize)
			{
				GUILayout.BeginVertical (EditorStyles.helpBox);
			}
			GUILayout.BeginVertical (EditorStyles.helpBox);
			view.childs.synchronize = EditorGUILayout.Toggle ("CHILDS", view.childs.synchronize);
			GUILayout.Space (2f);
			GUILayout.EndVertical ();
			GUILayout.Space (7f);

			if(view.childs.synchronize)
			{
				//EditorGUILayout.BeginHorizontal ();
				//GUILayout.Space (25f);
				//view.childs.sendRate = EditorGUILayout.IntSlider ("Sync Rate", Convert.ToInt16(view.childs.sendRate), 1, 30);
				//EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (25f);
				EditorGUILayout.LabelField("Capacity: " + view.childs.childList.Count + " childs");
				EditorGUILayout.EndHorizontal ();


				EditorGUILayout.Separator ();
				for(int i = 0; i < view.childs.childList.Count; i++)
				{
					EditorGUILayout.Separator ();
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (40f);
					view.childs.childList[i].reference = (Transform)EditorGUILayout.ObjectField (view.childs.childList[i].reference, typeof(Transform), true);
					GUILayout.Space (12f);
					GUILayout.Label("Position", GUILayout.MaxWidth(70f));
					view.childs.childList[i].position.synchronize = EditorGUILayout.Toggle (view.childs.childList[i].position.synchronize);
					GUILayout.Label("Rotation", GUILayout.MaxWidth(70f));
					view.childs.childList[i].rotation.synchronize = EditorGUILayout.Toggle (view.childs.childList[i].rotation.synchronize);
					GUILayout.Label("Scale", GUILayout.MaxWidth(70f));
					view.childs.childList[i].scale.synchronize = EditorGUILayout.Toggle (view.childs.childList[i].scale.synchronize);

					if(GUILayout.Button("X"))
					{
						view.childs.childList.RemoveAt (i);
					}
					EditorGUILayout.EndHorizontal ();
				}
				EditorGUILayout.Separator ();


				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (scrWidth/10);

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

				GUILayout.Space (scrWidth/10);
				EditorGUILayout.EndHorizontal ();
			}

			else
			{
				if(view.childs.childList.Count > 0)
				{
					view.childs.childList = new List<ViewChilds>();
				}
			}

			GUILayout.Space (7f);
			if(view.childs.synchronize)
			{
				GUILayout.EndVertical ();
			}
		}
	}
}

//#endif