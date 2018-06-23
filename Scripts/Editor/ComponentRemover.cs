using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;              //for making this an editor

namespace CyberBullet.Editors {
    public class ComponentRemover : EditorWindow {

        #region Inputs
        GameObject targetObject = null;
        #endregion

        bool running = false;
        string helpBox = "Select your target objects.";
        GenericEditor editor = null;
        bool completed = false;

        [MenuItem("Cyber Bullet/Helpers/Component Remover")]
        private static void GDR_GeneratePlayer()
        {
            EditorWindow window = EditorWindow.GetWindow (typeof(ComponentRemover));//create an new windows
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
            editor.DrawTitle (" - CyberBullet - ","Component Copier");
            if (targetObject == null) {
                editor.DrawHelpBox ("Select your target to remove all of the components on the target gameobject.");
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
        }
        void Options() 
        {
            if (targetObject == null)
                return;
            if (running == false && targetObject != null && completed == false)
            {
                helpBox = "Great you have your object! Now click the remove button.";
            }
            else if (completed == true)
            {
                helpBox = "Components have been removed from the target!";
            }
            if (targetObject != null)
            {
                GUILayout.BeginArea(new Rect(30, 300, Screen.width - 70, 30));
                if (GUILayout.Button("Remove ALL Components from Target", GUILayout.Height(30)))
                {
                    running = true;
                    CopyComponents();
                }
                GUILayout.EndArea();
            }
        }
        //Use recursion to find all children and children of children endlessly
        void CopyComponents()
        {
            var components = targetObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                DestroyImmediate(component);
            }
            running = false;
            completed = true;
        }
    }
}