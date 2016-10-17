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
//[RequireComponent (typeof (NavMeshAgent))]
//[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Animator))]

public class AnimController : MonoBehaviour {
	public bool playerController = false;
	private string lastState = "Idle";								//default state
	[SerializeField] private float calmSpeed = 1.0f;
	[SerializeField] private float suspiciousSpeed = 3.0f;
	[SerializeField] private float hostileSpeed = 5.0f;
	[SerializeField] private bool debugSpeed;						//for logging speed
	[SerializeField] private bool debugDirection;					//for logging direction
	[SerializeField] private bool debugStates;						//for logging character state
	[SerializeField] private bool debugAnimationSpeed;				//for logging character state
	[HideInInspector] public bool enableCutscene = false;
	private Animator animator;										//mechanim animation attached to character
	private float lastRotation = 90;								//for left right direction detection
	private float normalizedRotation = 0.0f;
	private float curDir = 0.0f;

	//for speed calculation
	private float speed;						//how fast this character is moving
	private Vector3 lastPos = Vector3.zero;		//for speed calculation
	private float animSpeed;					//speed var to apply to animations

	void Start () {
		animator = this.GetComponent<Animator> ();
	}
	void Update () 
	{
		if (playerController == false) 
		{
			//is turning left or right?
			normalizedRotation = (transform.rotation.eulerAngles.y - lastRotation > 1) ? 1 : transform.rotation.eulerAngles.y - lastRotation;
			if (curDir != normalizedRotation) 
			{
				curDir += (normalizedRotation > curDir) ? Time.deltaTime * 0.1f : 0;
				curDir -= (normalizedRotation < curDir) ? Time.deltaTime * 0.1f : 0;
			}
			animator.SetFloat ("direction", curDir);
			lastRotation = transform.rotation.eulerAngles.y;

			//for speed calculation
			speed = Vector3.Distance (lastPos, this.transform.position) / Time.deltaTime;
			lastPos = this.transform.position;
			switch (lastState) 
			{
				case "calm":
					animSpeed = speed / calmSpeed;
					break;
				case "suspicious":
					animSpeed = speed / suspiciousSpeed;
					break;
				case "hostile":
					animSpeed = speed / hostileSpeed;
					break;
			}
			if (animSpeed > 1000 || animSpeed < -1000 || animSpeed == 0 ||
			    animSpeed == Mathf.Infinity || animSpeed == Mathf.NegativeInfinity) 
			{
				animSpeed = 0;
				animator.SetFloat ("speed", animSpeed);
			} 
			else 
			{
				animator.SetFloat ("speed", animSpeed);
			}

			//for state updates
			updateState (this.GetComponent<AIBehavior> ().memory.currentState);
		} 
		else //this is a player (easier to animated btw)
		{
			if (enableCutscene == false) {
				animator.SetFloat ("speed", InputManager.GetAxis ("Vertical"));
				animator.SetFloat ("direction", InputManager.GetAxis ("Horizontal"));
				animator.SetBool ("sprinting", InputManager.GetButton ("Run"));
			}
		}
		//for debugging
		if (debugAnimationSpeed == true) {
			Debug.Log(animator.GetFloat("speed"));
		}
		if (debugSpeed == true) {
			Debug.Log(speed);
		}
		if (debugDirection) {
			Debug.Log(animator.GetFloat("direction"));
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

		lastState = chosenState;
		animator.SetBool ("calm", false);
		animator.SetBool ("hostile", false);
		animator.SetBool ("suspicious", false);
		animator.SetBool (chosenState, true);
	}
}
