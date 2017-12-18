using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using Pandora.Climbing;

namespace Pandora {
	namespace Editors {
		public class CreateClimbPoints : EditorWindow {

			#region Variables
			#region Inputs
			GameObject targetObject = null;
			GameObject climbPoint = null;
			float distance = 2.0f;
			#endregion
			#region Helpers
			GameObject leftEdge = null;
			GameObject rightEdge = null;
			GenericEditor editor = new GenericEditor ();
			string helpBox = "Add the object you want to make climbable to continue...";
			bool edges = false;
			bool points = false;
			bool connections = false;
			string axis = "x";
			#endregion
			#endregion

			#region Visuals
			[MenuItem("GameDevRepo/Climbing System/Generate Climb Points")]
			private static void GDR_GeneratePlayer() {
				EditorWindow window = EditorWindow.GetWindow (typeof(CreateClimbPoints));//create an new windows
				window.minSize = new Vector2(400,500);
				window.Show ();
			}
			void OnEnable() {
				editor.InitTextures ();
			}
			void OnGUI() {
				editor.DrawBox ();
				editor.DrawTitle (" - GameDevRepo - ","Generate Climb Points");
				if (targetObject == null) {
					editor.DrawHelpBox ("Add the object you want to make climbable to continue...");
				} else {
					editor.DrawHelpBox (helpBox);
				}
				ObjectInput ();
				Options ();
                GUILayout.BeginArea (new Rect (30, Screen.height - 150, Screen.width - 70, 30));
                if (GUILayout.Button ("Update Connections Only", GUILayout.Height (30))) {
                    UpdateConnections();
                }
                helpBox = "All points have updated.";
                GUILayout.EndArea ();
			}
			void ObjectInput() {
				GUILayout.BeginArea(new Rect(20, 130, Screen.width-40, 100));
				targetObject = EditorGUILayout.ObjectField("Object:", targetObject, typeof(GameObject), true, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300)) as GameObject;
				climbPoint = EditorGUILayout.ObjectField("Climb Point Prefab:", climbPoint, typeof(GameObject), true, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300)) as GameObject;
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("  Distance: ", editor.SettingsSytle, GUILayout.Width (150));
				distance = EditorGUILayout.FloatField (distance, GUILayout.Width (50), GUILayout.MaxHeight (15));
				GUILayout.EndHorizontal ();
				GUILayout.EndArea ();
			}
			void Options() {
				if (targetObject == null)
					return;
				if (edges == false) {
					helpBox = "Generate edge objects...";
					GUILayout.BeginArea (new Rect (30, 200, Screen.width - 70, 30));
					if (GUILayout.Button ("Generate Edges", GUILayout.Height (30))) {
						edges = true;
						GenerateEdges ();
					}
					GUILayout.EndArea ();
				} else if (points == false) {
					GUILayout.BeginArea (new Rect (30, 200, Screen.width - 70, 30));
					helpBox = "Move these objects to the edges of your object to indicate the furthest left and right move points. Edit the \"Distance\" variable for how far you want your points to be apart.";
					if (GUILayout.Button ("Generate Points", GUILayout.Height (30))) {
						points = true;
						GeneratePoints ();
					}
					GUILayout.EndArea ();
				} else if (connections == false) {
					GUILayout.BeginArea (new Rect (30, 200, Screen.width - 70, 30));
					helpBox = "Make sure your points position and rotation are correct. Generate the connections between the points...";
					if (GUILayout.Button ("Generate Connections", GUILayout.Height (30))) {
						connections = true;
						GenerateConnections ();
					}
					GUILayout.EndArea ();
				} else {
					GUILayout.BeginArea (new Rect (30, 200, Screen.width - 70, 30));
					helpBox = "Your done! If you added more points and want to update your connections grid, click refresh...";
					if (GUILayout.Button ("Refresh Connections", GUILayout.Height (30))) {
						GenerateConnections();
					}
					GUILayout.EndArea ();
				}
				GUILayout.BeginArea (new Rect (30, Screen.height - 100, Screen.width - 70, 30));
				if (GUILayout.Button ("Delete All Points", GUILayout.Height (30))) {
					edges = points = connections = false;
					DeleteAll ();
				}
				GUILayout.EndArea ();
			}
			#endregion

			#region Logic
			#region Point Generation
			void GenerateEdges() {
				rightEdge = Instantiate (climbPoint, targetObject.transform.position, targetObject.transform.rotation) as GameObject;
				rightEdge.name = "ClimbPoint - RightEdge";
				leftEdge = Instantiate (climbPoint, targetObject.transform.position, targetObject.transform.rotation) as GameObject;
				leftEdge.name = "ClimbPoint - LeftEdge";
				rightEdge.transform.position = new Vector3(rightEdge.transform.position.x - 1, rightEdge.transform.position.y, rightEdge.transform.position.z);
				rightEdge.transform.position = new Vector3(rightEdge.transform.position.x + 1, rightEdge.transform.position.y, rightEdge.transform.position.z);
				GameObject climbPointChild;
				if (!targetObject.transform.Find ("ClimbPoints")) {
					climbPointChild = new GameObject ("ClimbPoints");
					climbPointChild.transform.position = targetObject.transform.position;
					climbPointChild.transform.parent = targetObject.transform;
				} else {
					climbPointChild = (GameObject)targetObject.transform.Find ("ClimbPoints").gameObject;
				}
				rightEdge.transform.parent = climbPointChild.transform;
				leftEdge.transform.parent = climbPointChild.transform;
			}
			void GeneratePoints() {
				axis = GetAxis ();
				int amount = GetInstantiateAmount ();
				GameObject climbParent = (GameObject)targetObject.transform.Find ("ClimbPoints").gameObject;
				for (int i = 1; i <= amount; i++) {
                    GameObject generated = Instantiate (leftEdge, GetTargetPosition(leftEdge, axis, distance * i), targetObject.transform.rotation) as GameObject;
					generated.transform.parent = climbParent.transform;
				}
			}
			Vector3 GetTargetPosition(GameObject target, string direction, float amount) {
				Vector3 retVal = Vector3.zero;
				if (direction == "x") {
					retVal = new Vector3 (target.transform.position.x + amount, target.transform.position.y, target.transform.position.z);
				} else if (direction == "z") {
					retVal = new Vector3 (target.transform.position.x, target.transform.position.y, target.transform.position.z + amount);
				} else if (direction == "y") {
					retVal = new Vector3 (target.transform.position.x, target.transform.position.y + amount, target.transform.position.z);
				}
				return retVal;
			}
			int GetInstantiateAmount() {
				float furthest = Vector3.Distance (leftEdge.transform.position, rightEdge.transform.position);
				return Mathf.RoundToInt(furthest / distance); 
			}
			string GetAxis() {
				string axis = "x";
				float val = 0;
				if (Mathf.Abs (rightEdge.transform.position.x - leftEdge.transform.position.x) > val) {
					val = Mathf.Abs (rightEdge.transform.position.x - leftEdge.transform.position.x);
					axis = "x";
				}
				if (Mathf.Abs (rightEdge.transform.position.z - leftEdge.transform.position.z) > val) {
					val = Mathf.Abs (rightEdge.transform.position.z - leftEdge.transform.position.z);
					axis = "z";
				}
				if (Mathf.Abs (rightEdge.transform.position.y - leftEdge.transform.position.y) > val) {
					val = Mathf.Abs (rightEdge.transform.position.y - leftEdge.transform.position.y);
					axis = "y";
				}
				return axis;
			}
			#endregion
			#region Make Connections
			void GenerateConnections() {
				foreach (Transform curObj in targetObject.transform.Find("ClimbPoints").transform) {
					Debug.Log (FindAllNeighbors (curObj));
					curObj.GetComponentInChildren<ClimbPoint>().neighbors = FindAllNeighbors (curObj);
				}
			}
            void UpdateConnections() {
                foreach (Transform childObj in targetObject.transform) {
                    foreach (Transform climbObj in childObj.Find("ClimbPoints").transform) {
                        climbObj.GetComponentInChildren<ClimbPoint>().neighbors = UpdateAllNeighbors (climbObj);
                    }
                }
            }
			ClimbNeighbor[] FindAllNeighbors(Transform curTarget) {
				List<ClimbNeighbor> retVal = new List<ClimbNeighbor>();
				foreach(Transform child in targetObject.transform.Find("ClimbPoints").transform) {
					if (curTarget == child)
						continue;
					if (Vector3.Distance (curTarget.position, child.position) <= distance) {
						ClimbNeighbor foundPoint = new ClimbNeighbor ();
						foundPoint.target = child.GetComponentInChildren<ClimbPoint>();
						foundPoint.type = ClimbTransitionType.step;
                        foundPoint.direction = GetClimbDirection(child, curTarget);
						retVal.Add (foundPoint);
					} 
				}
				return retVal.ToArray();
			}
            ClimbNeighbor[] UpdateAllNeighbors(Transform curTarget) {
                List<ClimbNeighbor> retVal = new List<ClimbNeighbor>();
                foreach (Transform childObj in targetObject.transform)
                {
                    foreach (Transform climbObj in childObj.Find("ClimbPoints").transform)
                    {
                        if (curTarget == climbObj)
                            continue;
                        else if (Vector3.Distance (curTarget.position, climbObj.position) <= distance) {
                            ClimbNeighbor foundPoint = new ClimbNeighbor ();
                            foundPoint.target = climbObj.GetComponentInChildren<ClimbPoint>();
                            foundPoint.type = ClimbTransitionType.step;
                            foundPoint.direction = GetClimbDirection(climbObj, curTarget);
                            retVal.Add (foundPoint);
                        }
                    }

                }
//                foreach(Transform child in curTarget) {
//                    if (curTarget == child)
//                        continue;
//                    if (Vector3.Distance (curTarget.position, child.position) <= distance) {
//                        ClimbNeighbor foundPoint = new ClimbNeighbor ();
//                        foundPoint.target = child.GetComponentInChildren<ClimbPoint>();
//                        foundPoint.type = ClimbTransitionType.step;
//                        foundPoint.direction = GetClimbDirection(child, curTarget);
//                        retVal.Add (foundPoint);
//                    } 
//                }
                return retVal.ToArray();
            }

            Climbing.ClimbDirection GetClimbDirection(Transform childPoint, Transform curTarget) {
                Climbing.ClimbDirection retVal =  Climbing.ClimbDirection.Right;
                bool assigned = false;
                if (childPoint.position.y < curTarget.position.y)
                {
                    retVal = Climbing.ClimbDirection.Down;
                    assigned = true;
                }
                else if (childPoint.position.y > curTarget.position.y)
                {
                    retVal = Climbing.ClimbDirection.Up;
                    assigned = true;
                }
                if (assigned == false)
                {
                    if (childPoint.position.x < curTarget.position.x)
                    {
                        retVal = Climbing.ClimbDirection.Left;
                        assigned = true;
                    }
                    else if (childPoint.position.x > curTarget.position.x)
                    {
                        retVal = Climbing.ClimbDirection.Right;
                        assigned = true;
                    }
                    else if (childPoint.position.z < curTarget.position.z)
                    {
                        retVal = Climbing.ClimbDirection.Left;
                        assigned = true;
                    }
                    else
                    {
                        retVal = Climbing.ClimbDirection.Right;
                        assigned = true;
                    }
                }
                return retVal;
            }
			#endregion
			void DeleteAll() {
				edges = points = connections = false;
				if (targetObject.transform.Find ("ClimbPoints"))
					DestroyImmediate (targetObject.transform.Find ("ClimbPoints").gameObject);
			}
			#endregion
		}
	}
}