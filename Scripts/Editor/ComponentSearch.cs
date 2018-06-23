using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;              //for making this an editor
using System.Linq;              //for combining gameobject arrays
using EditorCoroutines;         //For a coroutine in the editor
using System.Reflection;        //for getting varibales from scripts and script names

namespace CyberBullet.Editors {
    public class ComponentSearch : EditorWindow {

        #region Inputs
        public string Search = "";
        #endregion

        #region Internal Use
        string helpBox = "When done click search and wait for the results to come back.";
        GenericEditor editor = null;
        private bool searching = false;
        private bool found_objects = false;
        private Vector2 scrollPos;
        private List<GameObject> found_targets = new List<GameObject>();
        #endregion

        #region Core
        [MenuItem("Cyber Bullet/Helpers/Search All Components")]
        private static void GDR_GenerateComponentSearch()
        {
            EditorWindow window = EditorWindow.GetWindow (typeof(ComponentSearch));//create an new windows
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
            editor.DrawTitle (" - CyberBullet - ","Search All Components");
            if (string.IsNullOrEmpty(Search) == true) {
                editor.DrawHelpBox ("In the search box type what you're wanting to find. Can be items in a component or the component itself.");
            } else {
                editor.DrawHelpBox (helpBox);
            }
            ObjectInput ();
            Options ();
            if (found_objects == true)
            {
                GUILayout.BeginArea(new Rect(20, 180, Screen.width-40, Screen.height-230));
                EditorGUILayout.BeginHorizontal();
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width-40), GUILayout.Height(Screen.height-230));
                foreach (GameObject go in found_targets)
                {
                    if (GUILayout.Button(go.name, GUILayout.Height(20)))
                    {
                        Selection.activeGameObject = go;
                    }
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea ();
            }
        }
        void ObjectInput() 
        {
            GUILayout.BeginArea(new Rect(20, 130, Screen.width-40, 100));
            Search = EditorGUILayout.TextField("Search For:", Search,GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300));
            GUILayout.EndArea ();
        }
        #endregion

        #region Logic For GUI
        void Options() 
        {
            if (searching == true && found_objects == false)
            {
                helpBox = "Searching, please wait...";
            }
            else if (found_objects == true)
            {
                helpBox = "Completed!";
                GUILayout.BeginArea(new Rect(30, 150, Screen.width - 70, 30));
                if (GUILayout.Button("Clear Results", GUILayout.Height(30)))
                {
                    Search = "";
                    searching = false;
                    found_objects = false;
                }
                GUILayout.EndArea();
            }
            else
            {
                GUILayout.BeginArea(new Rect(30, 150, Screen.width - 70, 30));
                if (GUILayout.Button("Begin Search", GUILayout.Height(30)))
                {
                    searching = true;
                    found_objects = false;
                    EditorCoroutines.EditorCoroutines.StartCoroutine(SearchComponents(), this);
                }
                GUILayout.EndArea();
            }
        }
        #endregion

        #region Search Logic 
        private IEnumerator SearchComponents()
        {
            yield return new WaitForSeconds(0.1f);
            if (string.IsNullOrEmpty(Search) == false)
            {
                found_targets = FindMatchingGO();
            }
            found_objects = true;
            searching = false;
            Debug.Log("Completed Search - Found Matching Targets: "+found_targets.Count);
            yield return null;
        }
             
        List<GameObject> FindMatchingGO()
        {
            List<GameObject> targets = new List<GameObject>();
            object[] objs = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object obj in objs)
            {
                GameObject go = (GameObject)obj;
                if (go.name.ToLower().Contains(Search.ToLower()))
                {
                    targets.Add(go);
                }
                else if (StringInComponents(go))
                {
                    targets.Add(go);
                }
            }
            return targets;
        }

        bool StringInComponents(GameObject obj)
        {
            foreach (Component component in obj.GetComponents(typeof(Component)))
            {
                try {
                    if (component.GetType() == typeof(Transform))
                    {
                        continue;
                    }
                    if (component.GetType().Name.ToLower().Contains(Search.ToLower()))
                    {
                        return true;
                    }
                    else if (ComponentFeildValueContainString(component, obj))
                    {
                        return true;
                    }
                }
                catch (System.Exception ex)
                {
                    continue;
                }
            }

            return false;
        }

        bool ComponentFeildValueContainString(Component component, GameObject target)
        {
            System.Type objType = component.GetType();
            BindingFlags flags = BindingFlags.Instance |
                                 BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Static |
                                 BindingFlags.FlattenHierarchy |
                                 BindingFlags.Default |
                                 BindingFlags.ExactBinding |
                                 BindingFlags.GetField |
                                 BindingFlags.GetProperty |
                                 BindingFlags.IgnoreReturn |
                                 BindingFlags.IgnoreCase |
                                 BindingFlags.OptionalParamBinding |
                                 BindingFlags.PutDispProperty |
                                 BindingFlags.PutRefDispProperty;
            var properties = objType.GetFields(flags);
            foreach (var prop in properties)
            {
                try {
                    if (prop.GetValue((object)component).ToString().ToLower().Contains(Search.ToLower()))
                    {
                        return true;
                    }
                }
                catch(System.Exception ex)
                {
                    continue;
                }
            }
            return false;
        }

        #endregion
    }
}