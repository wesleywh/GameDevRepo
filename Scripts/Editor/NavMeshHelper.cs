using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;              //for making this an editor

namespace CyberBullet.Editors {
    public class NavMeshHelper : EditorWindow {

        #region Inputs
        GameObject targetObject = null;
        #endregion

        bool btn_generated = false;
        bool running = false;
        string helpBox = "Add a gameobject to start scan, no changes will be made yet.";
        GenericEditor editor = null;

        [MenuItem("Cyber Bullet/Helpers/NavMesh Static Setup")]
        private static void GDR_GeneratePlayer()
        {
            EditorWindow window = EditorWindow.GetWindow (typeof(NavMeshHelper));//create an new windows
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
            editor.DrawTitle (" - CyberBullet - ","NavMesh Setup Helper");
            if (targetObject == null) {
                editor.DrawHelpBox ("Add the object whos children you want to set the navmesh properties on.");
            } else {
                editor.DrawHelpBox (helpBox);
            }
            ObjectInput ();
            Options ();
        }
        void ObjectInput() 
        {
            GUILayout.BeginArea(new Rect(20, 130, Screen.width-40, 100));
            targetObject = EditorGUILayout.ObjectField("Object:", targetObject, typeof(GameObject), true, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300)) as GameObject;
            GUILayout.EndArea ();
        }
        void Options() 
        {
            if (targetObject == null)
                return;
            GUILayout.BeginArea (new Rect (30, 200, Screen.width - 70, 30));
            if (running == false && btn_generated == false)
            {
                helpBox = "Great you have your object! Now click the desired attributes you want to apply.";
            }
            else if (btn_generated == false)
            {
                helpBox = "Setting all children to Navigation Static, Please wait...";
            }
            else
            {
                helpBox = "Wait a few moments for this to change the child properties. Verify it's complete by checking one of the children.";
            }
            if (GUILayout.Button ("Make All Children Navigation Walkable", GUILayout.Height (30))) {
                GenerateNavMesh(0);
                btn_generated = true;
            }
            GUILayout.EndArea ();
            GUILayout.BeginArea (new Rect (30, 250, Screen.width - 70, 30));
            if (GUILayout.Button ("Make All Children Navigation Jump", GUILayout.Height (30))) {
                GenerateNavMesh(2);
                btn_generated = true;
            }
            GUILayout.EndArea ();
            GUILayout.BeginArea (new Rect (30, 300, Screen.width - 70, 30));
            if (GUILayout.Button ("Make All Children Navigation Not Walkable", GUILayout.Height (30))) {
                GenerateNavMesh(1);
                btn_generated = true;
            }
            GUILayout.EndArea ();
        }
        //Use recursion to find all children and children of children endlessly
        List<Transform> GetAllChildren(Transform target)
        {
            List<Transform> allChildren = new List<Transform>();
            foreach (Transform child in target)
            {
                if (child.childCount > 0)
                {
                    allChildren.AddRange(GetAllChildren(child));
                }
                else
                {
                    allChildren.Add(child);
                }
            }
            return allChildren;
        }
        void GenerateNavMesh(int layer)
        {
            foreach (Transform child in GetAllChildren(targetObject.transform))
            {
                GameObjectUtility.SetStaticEditorFlags(child.gameObject, StaticEditorFlags.NavigationStatic);
                GameObjectUtility.SetNavMeshLayer(child.gameObject, layer);
            }
        }
    }
}