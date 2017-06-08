using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameDevRepo {
	namespace Editors {
        public class CreateClimbPrefab : EditorWindow {

            #region Variables
            GenericEditor editor = new GenericEditor();
            #endregion

			#region Visuals
			[MenuItem("GameDevRepo/Climbing System/Generate ClimbPoint Prefab")]
			private static void E_ClimbPrefab() {
				EditorWindow window = EditorWindow.GetWindow (typeof(CreateClimbPrefab));//create an new windows
				window.minSize = new Vector2(400,500);
				window.Show ();
			}
			void OnEnable() 
            {
				editor.InitTextures ();
			}
			void OnGUI() 
            {
				editor.DrawBox ();
				editor.DrawTitle ("GameDevRepo","Generate ClimbPoint Prefab");
				editor.DrawHelpBox ("This will generate an empty prefab that will work with the climb system. The object name doesn't matter. Be sure to adjust the hands, feet, elbows, and knees to where you want them to be. Then best to make a prefab of this.");
				Options ();
    		}
			void Options() 
            {
				GUILayout.BeginArea (new Rect (30, 200, Screen.width - 70, 30));
				if (GUILayout.Button ("Generate ClimbPoint Prefab", GUILayout.Height (30))) 
                {
                    GeneratePrefab();
				}
			}
			#endregion
			#region Logic
			void GeneratePrefab() 
			{
                //Generate Object Hierarchy
				GameObject climbPrefab = new GameObject ("ClimbPoint");
                GameObject rootPosition = new GameObject("RootPosition");
                rootPosition.transform.position = new Vector3(climbPrefab.transform.position.x, climbPrefab.transform.position.y-1.5f, climbPrefab.transform.position.z);
                rootPosition.transform.parent = climbPrefab.transform;

                GameObject hands = new GameObject("Hands");
                GameObject feet = new GameObject("Feet");
                hands.transform.position = new Vector3(climbPrefab.transform.position.x, climbPrefab.transform.position.y, climbPrefab.transform.position.z);
                feet.transform.position = new Vector3(rootPosition.transform.position.x, rootPosition.transform.position.y+0.5f, rootPosition.transform.position.z);
                hands.transform.parent = rootPosition.transform;
                feet.transform.parent = rootPosition.transform;

                GameObject lh = new GameObject("LeftHand");
                GameObject rh = new GameObject("RightHand");
                GameObject le = new GameObject("LeftElbow");
                GameObject re = new GameObject("RightElbow");
                lh.transform.position = new Vector3(hands.transform.position.x-0.25f, hands.transform.position.y, hands.transform.position.z);
                rh.transform.position = new Vector3(hands.transform.position.x+0.25f, hands.transform.position.y, hands.transform.position.z);
                le.transform.position = new Vector3(hands.transform.position.x-2.0f, hands.transform.position.y-0.5f, hands.transform.position.z-1.0f);
                re.transform.position = new Vector3(hands.transform.position.x+2.0f, hands.transform.position.y-0.5f, hands.transform.position.z-1.0f);
                GameObject lf = new GameObject("LeftFoot");
                GameObject rf = new GameObject("RightFoot");
                GameObject lk = new GameObject("LeftKnee");
                GameObject rk = new GameObject("RightKnee");
                lf.transform.position = new Vector3(feet.transform.position.x-0.25f, feet.transform.position.y, feet.transform.position.z);
                rf.transform.position = new Vector3(feet.transform.position.x+0.25f, feet.transform.position.y, feet.transform.position.z);
                lk.transform.position = new Vector3(feet.transform.position.x-0.25f, feet.transform.position.y, feet.transform.position.z+2.0f);
                rk.transform.position = new Vector3(feet.transform.position.x+0.25f, feet.transform.position.y, feet.transform.position.z+2.0f);

                lh.transform.parent = hands.transform;
                rh.transform.parent = hands.transform;
                le.transform.parent = hands.transform;
                re.transform.parent = hands.transform;
                lf.transform.parent = feet.transform;
                rf.transform.parent = feet.transform;
                lk.transform.parent = feet.transform;
                rk.transform.parent = feet.transform;

                //Add and populate script
                rootPosition.AddComponent<Climbing.ClimbPoint>();
                Climbing.ClimbPoint climbPoint = rootPosition.GetComponent<Climbing.ClimbPoint>();

                List<Climbing.ClimbIKPositions> ik = new List<Climbing.ClimbIKPositions>();
                Climbing.ClimbIKPositions ikLH = new Climbing.ClimbIKPositions();
                ikLH.ikType = AvatarIKGoal.LeftHand;
                ikLH.target = lh.transform;
                ik.Add(ikLH);
                Climbing.ClimbIKPositions ikRH = new Climbing.ClimbIKPositions();
                ikRH.ikType = AvatarIKGoal.RightHand;
                ikRH.target = rh.transform;
                ik.Add(ikRH);
                Climbing.ClimbIKPositions ikLF = new Climbing.ClimbIKPositions();
                ikLF.ikType = AvatarIKGoal.LeftFoot;
                ikLF.target = lf.transform;
                ik.Add(ikLF);
                Climbing.ClimbIKPositions ikRF = new Climbing.ClimbIKPositions();
                ikRF.ikType = AvatarIKGoal.RightHand;
                ikRF.target = rf.transform;
                ik.Add(ikRF);
                climbPoint.iks = ik.ToArray();

                List<Climbing.ClimbHintIKPositions> hint = new List<Climbing.ClimbHintIKPositions>();
                Climbing.ClimbHintIKPositions hintLE = new Climbing.ClimbHintIKPositions();
                hintLE.hintType = AvatarIKHint.LeftElbow;
                hintLE.target = le.transform;
                hint.Add(hintLE);
                Climbing.ClimbHintIKPositions hintRE = new Climbing.ClimbHintIKPositions();
                hintRE.hintType = AvatarIKHint.RightElbow;
                hintRE.target = re.transform;
                hint.Add(hintRE);
                Climbing.ClimbHintIKPositions hintLK = new Climbing.ClimbHintIKPositions();
                hintLK.hintType = AvatarIKHint.LeftKnee;
                hintLK.target = lk.transform;
                hint.Add(hintLK);
                Climbing.ClimbHintIKPositions hintRK = new Climbing.ClimbHintIKPositions();
                hintRK.hintType = AvatarIKHint.RightKnee;
                hintRK.target = rk.transform;
                hint.Add(hintRK);
                climbPoint.hints = hint.ToArray();
			}
			#endregion
		}
	}
}