///////////////////////////////////////////////////////////////
///															///
/// 		Written By Wesley Haws April 2016				///
/// 				Tested with Unity 5						///
/// 	Anyone can use this for any reason. No limitations. ///
/// 														///
/// This script is to be used in conjunction with			///
/// "AIBehavior.cs" script. This simply controls the 		///
///	animator to play certain animations based on this 		///
/// objects speed and rotation. 							///
/// 														///
///////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using TeamUtility.IO;												//Custom Input Manager

namespace Pandora {
    namespace Controllers {
        //[RequireComponent (typeof (NavMeshAgent))]
        //[RequireComponent (typeof (Rigidbody))]
        [RequireComponent (typeof (Animator))]
        public class AnimController : MonoBehaviour {
        	public bool NPC = false;
        	private string lastState = "Idle";								//default state
        	[SerializeField] private float calmSpeed = 1.0f;
        	[SerializeField] private float suspiciousSpeed = 3.0f;
        	[SerializeField] private float hostileSpeed = 5.0f;
        	[SerializeField] private bool debugSpeed;						//for logging speed
        	[SerializeField] private bool debugDirection;					//for logging direction
        	[SerializeField] private bool debugStates;						//for logging character state
        	[SerializeField] private bool debugAnimationSpeed;				//for logging character state
            [SerializeField] private MovementController mc = null;
            [HideInInspector] public bool enableCutscene = false;
        	[SerializeField] private Animator[] animators;										//mechanim animation attached to character
        	private float lastRotation = 90;								//for left right direction detection
        	private float normalizedRotation = 0.0f;
        	private float curDir = 0.0f;

        	//for speed calculation
        	private float speed;						//how fast this character is moving
        	private Vector3 lastPos = Vector3.zero;		//for speed calculation
        	private float animSpeed;					//speed var to apply to animations

        	void Start () {
                mc = (mc == null) ? this.transform.root.GetComponent<MovementController>() : mc;
        		if (animators.Length < 1 || animators [0] == null) {
        			animators[0] = this.GetComponent<Animator> ();
        		}
        	}
        	void Update () 
        	{

    			if (enableCutscene == false) {
    				foreach (Animator anim in animators) {
                        if (anim.gameObject.activeInHierarchy == true) {
    						anim.SetFloat ("speed", InputManager.GetAxis ("Vertical"));
    						anim.SetFloat ("direction", InputManager.GetAxis ("Horizontal"));
                            anim.SetBool ("sprinting", (mc.aimWalk == true) ? false : InputManager.GetButton ("Run"));
    					}
    				}
    			}
        		//for debugging
        		if (debugAnimationSpeed == true) {
        			foreach (Animator anim in animators) {
        				Debug.Log (anim.GetFloat ("speed"));
        			}
        		}
        		if (debugSpeed == true) {
        			Debug.Log(speed);
        		}
        		if (debugDirection) {
        			foreach (Animator anim in animators) {
        				Debug.Log (anim.GetFloat ("direction"));
        			}
        		}
        		if (debugStates) {
        			Debug.Log(lastState);
        		}
        	}
        	public void updateState(string State) {
        		string chosenState = "";
        		switch (State) {
        			case "Idle":
        			case "Wander":
        			case "Walking":
        			case "Patrol":
        				chosenState = "calm";
        				break;
        				//Loop: See Player->Suspicious->Investigate->Idle or Hostile->Attack
        			case "Suspicious":
        			case "Investigate":
        			case "Searching":
        				chosenState = "suspicious";
        				break;
        			case "Attacking":
        			case "Hostile":
        			case "FindCover":
        			case "RunningToCover":
        				chosenState = "hostile";
        				break;
        			default:
        				chosenState = "calm";
        				break;
        		}

        		if (NPC == true) {
        			lastState = chosenState;
        			foreach (Animator anim in animators) {
        				anim.SetBool ("calm", false);
        				anim.SetBool ("hostile", false);
        				anim.SetBool ("suspicious", false);
        				anim.SetBool (chosenState, true);
        			}
        		}
        	}
        }
    }
}