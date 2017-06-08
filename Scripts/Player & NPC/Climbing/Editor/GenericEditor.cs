using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System; 

namespace GameDevRepo {
	namespace Editors {
		public class GenericEditor : EditorWindow {
			#region Styling
			Texture2D headerSectionTexture;
			Texture2D blackTexture;
			Texture2D bodySectionTexture;
			Texture2D titleBoxTexture;
			Color headerSectionColor = Color.grey; 
			Color blackBackground = Color.black;
			Color bodySectionColor =new Color(0.5f, 0.5f, 0.55f, 1f);
			Color titleBoxColor =new Color(0.5f, 0.5f, 0.55f, 1f);
			Rect titleSection;
			Rect headerBlackSection;
			Rect bodySection;
			Rect foundSection;
			public GUIStyle TitleStyle = new GUIStyle ();
			public GUIStyle SettingsSytle = new GUIStyle ();
			public Vector2 scrollPosition;
			#endregion

			#region Universal
			public void InitTextures() {
				headerSectionTexture = new Texture2D (1, 1);
				headerSectionTexture.SetPixel (0, 0, headerSectionColor);
				headerSectionTexture.Apply ();

				blackTexture = new Texture2D (1, 1);
				blackTexture.SetPixel (0, 0, blackBackground);
				blackTexture.Apply ();

				bodySectionTexture = new Texture2D (1, 1);
				bodySectionTexture.SetPixel (0, 0, bodySectionColor);
				bodySectionTexture.Apply ();

				titleBoxTexture = new Texture2D (1, 1);
				titleBoxTexture.SetPixel (0, 0, titleBoxColor);
				titleBoxTexture.Apply ();
			}

			public void DrawBox() {
				//Black Background
				bodySection.x = 5;
				bodySection.y = 5;
				bodySection.width = Screen.width-10;
				bodySection.height = Screen.height - 40;
				GUI.DrawTexture (bodySection, blackTexture);

				//Gray box
				bodySection.x = 10;
				bodySection.y = 10;
				bodySection.width = Screen.width-20;
				bodySection.height = Screen.height - 50;
				GUI.DrawTexture (bodySection, bodySectionTexture);
			}

			public void DrawTitle(string Title, string Desc) {
				//Repo Title
				GUILayout.BeginArea(new Rect(0, 0, Screen.width, 80));
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				TitleStyle.normal.textColor = Color.white;
				TitleStyle.fontSize = 25 ;
				TitleStyle.alignment = TextAnchor.UpperLeft;
				GUILayout.Label (Title,TitleStyle);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndArea ();

				//Secondary Title
				GUILayout.BeginArea(new Rect(0, 15, Screen.width, 100));
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				TitleStyle.fontSize = 15 ;
				TitleStyle.fontStyle = FontStyle.Italic;
				TitleStyle.normal.textColor = Color.white;
				TitleStyle.alignment = TextAnchor.MiddleCenter;
				GUILayout.Label (Desc , TitleStyle);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndArea ();

				//Dividing Line
				GUILayout.BeginArea(new Rect(0, 15, Screen.width, 100));
				bodySection.x = 20;
				bodySection.y = 65;
				bodySection.width = Screen.width-40;
				bodySection.height = 4;
				GUI.DrawTexture (bodySection, blackTexture);
				GUILayout.EndArea ();
			}

			public void DrawHelpBox(string message) {
				GUILayout.BeginArea(new Rect(20, 90, Screen.width-40, 100));
				EditorGUILayout.HelpBox (message, MessageType.Info);
				GUILayout.EndArea ();
			}
			#endregion

		}
	}
}