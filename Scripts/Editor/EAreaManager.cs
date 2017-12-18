using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.IO;
using System.Text;

[CustomEditor(typeof(AreaManager))]
public class EAreaManager : Editor {
//    bool removeVariable = false;
//    string removeVariableButton = "Remove Variable";
//    bool addVariable = false;
//    string addVariableButton = "Add Variable";
//    bool show = false;
//    string showButton = "Show All Variables";
//    string varName = "";
//    Vector2 scrollPos;
//
//    public override void OnInspectorGUI()
//	{
//        #region Adding
//        if (GUILayout.Button (addVariableButton)) {
//            addVariable = !addVariable;
//            addVariableButton = (addVariable == true) ? "Cancel" : "Add Variable";
//            varName = "";
//        }
//        if (addVariable)
//        {
//            GUILayout.Space(20);
//            varName = GUILayout.TextField(varName);
//            if (GUILayout.Button("Confirm Adding New Variable"))
//            {
//                addVariable = false;
//                addVariableButton = "Add Variable";
//                System.IO.File.WriteAllText("/Assets/Scripts/GameManager/AreaManager.cs",varName+" = false;");
//            }
//            GUILayout.Space(20);
//        }
//        #endregion
//        #region Show
//        if (GUILayout.Button (showButton)) {
//            show = !show;
//            showButton = (show == true) ? "Hide All Variables" : "Show All Variables";
//        }
//       
//        if (show)
//        {
//            DrawDefaultInspector();
//        }
//        #endregion
//	}
}
