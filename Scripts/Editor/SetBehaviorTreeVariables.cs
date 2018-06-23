using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;              //for making this an editor
using BehaviorDesigner.Runtime;

namespace CyberBullet.Editors {
    public class SetBehaviorTreeVariables : EditorWindow {
        #region Inputs
        GameObject reference = null;
        BehaviorTree tree = null;
        #endregion

        #region Internal Use Only
        string helpBox = "Select your target objects.";
        GenericEditor editor = null;
        #endregion

        [MenuItem("Cyber Bullet/Helpers/Set Variables On Behavior Tree")]
        private static void GDR_SetBTVariables()
        {
            EditorWindow window = EditorWindow.GetWindow (typeof(SetBehaviorTreeVariables));//create an new windows
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
            editor.DrawTitle (" - CyberBullet - ","Set Behavior Tree Variables");
            if (tree == null) {
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
            reference = EditorGUILayout.ObjectField("Reference GameObject:", reference, typeof(GameObject), true, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300)) as GameObject;
            GUILayout.EndArea ();
            GUILayout.BeginArea(new Rect(20, 150, Screen.width-40, 100));
            tree = EditorGUILayout.ObjectField("Behavior Tree:", tree, typeof(BehaviorTree), true, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300)) as BehaviorTree;
            GUILayout.EndArea ();
        }
        void Options() 
        {
            if (tree == null)
                return;
            helpBox = "With you tree selected you can now set all of your values.";
            GUILayout.BeginArea(new Rect(30, 300, Screen.width - 70, 30));
            if (GUILayout.Button("Set Behavior Tree Values", GUILayout.Height(30)))
            {
                SetValues();
            }
            GUILayout.EndArea();
        }
        void SetValues()
        {
            if (reference != null && reference.GetComponent<CyberBullet.AI.AIMemory>())
            {
                CyberBullet.AI.AIMemory memory = reference.GetComponent<CyberBullet.AI.AIMemory>();
                tree.SetVariable("fov", (SharedFloat)memory.GetFOV());
                tree.SetVariable("fire_range", (SharedFloat)memory.GetFireRange());
                tree.SetVariable("melee_range", (SharedFloat)memory.GetMeleeRange());
                tree.SetVariable("visual_range", (SharedFloat)memory.GetVisualRange());
                tree.SetVariable("this", (SharedGameObject)reference);
                tree.SetVariable("ignore_layers", (SharedLayerMask)memory.GetIgnoreLayers());
                tree.SetVariable("object_layers", (SharedLayerMask)memory.GetEnemyLayers());
                tree.SetVariable("cover_layers", (SharedLayerMask)memory.GetCoverLayers());
                tree.SetVariable("hearing_distance", (SharedFloat)memory.GetHearingDistance());
                List<GameObject> waypointsList = new List<GameObject>();
                waypointsList.AddRange(memory.GetWaypointArray());
                tree.SetVariable("waypoints", (SharedGameObjectList)waypointsList);
                tree.SetVariable("target", (SharedGameObject)memory.GetTarget());
                tree.SetVariable("wait", (SharedFloat)memory.GetAttackWait());
                tree.SetVariable("hearing_threshold", (SharedFloat)memory.GetHearingThreshold());
                tree.SetVariable("run_speed", (SharedFloat)memory.GetRunSpeed());
                tree.SetVariable("walk_speed", (SharedFloat)memory.GetWalkSpeed());
                tree.SetVariable("arrive_distance", (SharedFloat)memory.GetArriveDistance());
                tree.SetVariable("hostile_time", (SharedFloat)memory.GetHostileTime());
                tree.SetVariable("suspicious_time", (SharedFloat)memory.GetSuspiciousTime());
                tree.SetVariable("min_wander_distance", (SharedFloat)memory.GetMinWanderDistance());
                tree.SetVariable("max_wander_distance", (SharedFloat)memory.GetMaxWanderDistance());
                tree.SetVariable("wander_rate", (SharedFloat)memory.GetWanderRate());
                tree.SetVariable("min_pause", (SharedFloat)memory.GetMinPause());
                tree.SetVariable("max_pause", (SharedFloat)memory.GetMaxPause());
                tree.SetVariable("rotation_speed", (SharedFloat)memory.GetRotationSpeed());
                tree.SetVariable("last_target_position", (SharedVector3)memory.GetLastTargetPosition());
                tree.SetVariable("min_attack_wait", (SharedFloat)memory.GetMinAttackWait());
                tree.SetVariable("max_attack_wait", (SharedFloat)memory.GetMaxAttackWait());
                tree.SetVariable("prediction_time", (SharedFloat)memory.GetPredictionTime());
                tree.SetVariable("flee_distance", (SharedFloat)memory.GetFleeDistance());
                tree.SetVariable("cover_offset", (SharedFloat)memory.GetCoverOffset());
                GameObject blank = null;
                tree.SetVariable("flee_target", (SharedGameObject)blank);
                tree.SetVariable("strafe_position", (SharedVector3)Vector3.zero);
                tree.SetVariable("calm_react_time", (SharedFloat)memory.GetCalmReactTime());
                tree.SetVariable("pause_time", (SharedFloat)0.0f);
                tree.SetVariable("waypoints", (SharedGameObjectList)memory.GetWaypointList());
                tree.SetVariable("waypoints_length", (SharedInt)memory.GetWaypointLength());
                tree.SetVariable("visual_offset", (SharedVector3)memory.GetOffset());
            }
            else
            {
                tree.SetVariable("fov", (SharedFloat)0);
                tree.SetVariable("fire_range", (SharedFloat)0);
                tree.SetVariable("melee_range", (SharedFloat)0);
                tree.SetVariable("visual_range", (SharedFloat)0);
                GameObject blank = null;
                tree.SetVariable("this", (SharedGameObject)blank);
                LayerMask nothing = LayerMask.GetMask("Nothing");
                tree.SetVariable("ignore_layers", (SharedLayerMask)nothing);
                tree.SetVariable("object_layers", (SharedLayerMask)nothing);
                tree.SetVariable("cover_layers", (SharedLayerMask)nothing);
                tree.SetVariable("hearing_distance", (SharedFloat)0);
                List<GameObject> waypointsList = new List<GameObject>();
                tree.SetVariable("waypoints", (SharedGameObjectList)waypointsList);
                GameObject newObject = null;
                tree.SetVariable("target", (SharedGameObject)newObject);
                tree.SetVariable("wait", (SharedFloat)0.5f);
                tree.SetVariable("hearing_threshold", (SharedFloat)0.05f);
                tree.SetVariable("run_speed", (SharedFloat)4.0f);
                tree.SetVariable("walk_speed", (SharedFloat)1.0f);
                tree.SetVariable("arrive_distance", (SharedFloat)0.1f);
                tree.SetVariable("hostile_time", (SharedFloat)20.0f);
                tree.SetVariable("suspicious_time", (SharedFloat)5.0f);
                tree.SetVariable("min_wander_distance", (SharedFloat)5.0f);
                tree.SetVariable("max_wander_distance", (SharedFloat)10.0f);
                tree.SetVariable("wander_rate", (SharedFloat)1.0f);
                tree.SetVariable("min_pause", (SharedFloat)0.0f);
                tree.SetVariable("max_pause", (SharedFloat)1.0f);
                tree.SetVariable("rotation_speed", (SharedFloat)1.0f);
                tree.SetVariable("last_target_position", (SharedVector3)Vector3.zero);
                tree.SetVariable("min_attack_wait", (SharedFloat)1.0f);
                tree.SetVariable("max_attack_wait", (SharedFloat)4.0f);
                tree.SetVariable("prediction_time", (SharedFloat)1.0f);
                tree.SetVariable("flee_distance", (SharedFloat)5.0f);
                tree.SetVariable("flee_target", (SharedGameObject)blank);
                tree.SetVariable("strafe_position", (SharedVector3)Vector3.zero);
                tree.SetVariable("cover_offset", (SharedFloat)1.0f);
                tree.SetVariable("calm_react_time", (SharedFloat)2.0f);
                tree.SetVariable("pause_time", (SharedFloat)0.0f);
                List<GameObject> none = new List<GameObject>();
                tree.SetVariable("waypoints", (SharedGameObjectList)none);
                tree.SetVariable("waypoints_length", (SharedInt)0);
                tree.SetVariable("visual_offset", (SharedVector3)Vector3.zero);
            }
            tree.SaveResetValues();
        }
    }
}