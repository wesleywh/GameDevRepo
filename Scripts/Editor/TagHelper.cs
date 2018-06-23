using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;              //for making this an editor

namespace CyberBullet.Editors {
    
    public class TagHelper : EditorWindow {

        #region Inputs
        GameObject targetObject = null;
        public string tagToApply = "Untagged";
        #endregion

        bool running = false;
        string helpBox = "Select your target objects.";
        GenericEditor editor = null;
        bool completed = false;

        [MenuItem("Cyber Bullet/Helpers/Tag To All Children")]
        private static void GDR_GeneratePlayer()
        {
            EditorWindow window = EditorWindow.GetWindow (typeof(TagHelper));//create an new windows
            window.minSize = new Vector2(400,500);
            window.Show ();
        }
        void OnEnable() 
        {
            editor = new GenericEditor ();
            editor.InitTextures ();
        }
        void OnGUI() 
        {
            editor.DrawBox ();
            editor.DrawTitle (" - CyberBullet - ","Apply Tag To All Children");
            if (targetObject == null) {
                editor.DrawHelpBox ("Select your target gameobject to apply tag to.");
            } else {
                editor.DrawHelpBox (helpBox);
            }
            ObjectInput ();
            Options ();
        }
        void ObjectInput() 
        {
            GUILayout.BeginArea(new Rect(20, 130, Screen.width-40, 100));
            targetObject = EditorGUILayout.ObjectField("Target:", targetObject, typeof(GameObject), true, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300)) as GameObject;
            GUILayout.EndArea ();
            GUILayout.BeginArea(new Rect(20, 150, Screen.width-40, 100));
            tagToApply = EditorGUILayout.TagField("Tag To Apply:", tagToApply,GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300));
            GUILayout.EndArea ();
        }
        void Options() 
        {
            if (targetObject == null)
                return;
            if (running == false && targetObject != null && completed == false)
            {
                helpBox = "Great you have your object! Now click the button.";
            }
            else if (completed == true)
            {
                helpBox = "Tag applied.";
            }
            if (targetObject != null)
            {
                GUILayout.BeginArea(new Rect(30, 300, Screen.width - 70, 30));
                if (GUILayout.Button("Apply Tag To Gameobject & Children", GUILayout.Height(30)))
                {
                    running = true;
                    ApplyTagToChildren();
                }
                GUILayout.EndArea();
            }
        }
        //Use recursion to find all children and children of children endlessly
        void ApplyTagToChildren()
        {
            targetObject.tag = tagToApply;
            foreach (Transform child in targetObject.transform)
            {
                child.tag = tagToApply;
            }
        }
    }
}