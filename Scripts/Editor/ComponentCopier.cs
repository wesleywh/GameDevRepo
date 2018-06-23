using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;              //for making this an editor

namespace CyberBullet.Editors {
    public class ComponentCopier : EditorWindow {

        #region Inputs
        GameObject targetObject = null;
        GameObject destinationObject = null;
        #endregion

        bool running = false;
        string helpBox = "Select your target objects.";
        GenericEditor editor = null;
        bool completed = false;

        [MenuItem("Cyber Bullet/Helpers/Component Copier")]
        private static void GDR_GeneratePlayer()
        {
            EditorWindow window = EditorWindow.GetWindow (typeof(ComponentCopier));//create an new windows
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
            if (targetObject == null && destinationObject == null) {
                editor.DrawHelpBox ("Select your target and destination to copy all of the components on the target to the destination gameobject.");
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
            destinationObject = EditorGUILayout.ObjectField("Destination:", destinationObject, typeof(GameObject), true, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300)) as GameObject;
            GUILayout.EndArea ();
        }
        void Options() 
        {
            if (targetObject == null)
                return;
            if (running == false && targetObject != null && destinationObject != null && completed == false)
            {
                helpBox = "Great you have your objects! Now click the copy button.";
            }
            else if (completed == true)
            {
                helpBox = "Components have been copied to the target!";
            }
            if (destinationObject != null && targetObject != null)
            {
                GUILayout.BeginArea(new Rect(30, 300, Screen.width - 70, 30));
                if (GUILayout.Button("Copy Components to Target", GUILayout.Height(30)))
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
                UnityEditorInternal.ComponentUtility.CopyComponent(component);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(destinationObject);
            }
            running = false;
            completed = true;
        }
    }
}