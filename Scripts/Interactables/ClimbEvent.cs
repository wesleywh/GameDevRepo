///////////////////////////////////////////////////////////////
///															///
/// 		Written By Wesley Haws April 2016				///
/// 				Tested with Unity 5						///
/// 	Anyone can use this for any reason. No limitations. ///
/// 														///
/// 		Used for Climbing Up and Down Ladders			///
/// 		Climbing Up Large walls							///
/// 		Stepping Up over Large ledges					///
///////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class ClimbEvent : MonoBehaviour {
	enum Type { 
		Ladder, Edge, Step
	}
	enum Direction {
		MoreDown, MoreUp, NoRestriction
	}
	[SerializeField] private Type climbType = Type.Step;
	[SerializeField] private float eventRadius = 2.0f;
	[SerializeField] private float climbSpeed = 3.0f;
	[SerializeField] private GameObject start;
	[SerializeField] private GameObject end;
	[SerializeField] private float resetTimer = 2.0f;
	[SerializeField] private bool cannotJump = true;
	[SerializeField] private bool cannotMoveVertical = true;
	[SerializeField] private bool cannotMoveHorizontal = true;
	[SerializeField] private Direction dontAllowDirectionAtBegin = Direction.NoRestriction;
	[SerializeField] private Direction dontAllowDirectionAtEnd = Direction.NoRestriction;
	[SerializeField] private bool addShakeEffect = true;
	[SerializeField] private float shakeAmount = 4.0f;
	[SerializeField] private float shakeDuration = 3.0f;
	[SerializeField] private int[] childIndexForShake;

	//for ledges and edges
	private float timer = 0.0f;
	private GameObject target;
	private bool moving = false;
	private float journey = 0.0f;
	private float startTime = 0.0f;
	private bool exited = false;
	private bool startTimer = false;

	//for Ladders
	private float letGoTimer = 0.0f;
	private bool canGetOnLadder = true;
	private Vector3 highestClimbPoint = Vector3.zero;
	private bool exitLadder = false;

	void Start()
	{
		InvokeRepeating ("InRadius", 0.05f, 0.05f);
	}
	void OnTriggerStay() {
		if (exitLadder == false) {
			if (climbType == Type.Ladder && Input.GetButton ("Action") && canGetOnLadder) {
				canGetOnLadder = false;
				target.GetComponent<Animator> ().SetInteger ("climb_number", 2);
				target.GetComponent<MovementController> ().moveUpOnly = true;
				target.GetComponent<MovementController> ().onLadder = true;
				target.GetComponent<MovementController> ().moveDirection = Vector3.zero;
				EnterClimbEvent ();
			}
			if (Input.GetButton ("Action") && canGetOnLadder == false) {
				Debug.Log ("holding exit button");
				letGoTimer += Time.deltaTime;
				if (letGoTimer > 3.0f) {
					StartCoroutine (StartEnterTimer ());
					if (Mathf.Abs (this.transform.lossyScale.y - target.transform.position.y) < 1.5f) {
						startTime = Time.time;
						journey = Vector3.Distance (target.transform.position, new Vector3(this.transform.position.x, 0, this.transform.position.z));
						exitLadder = true;
					} else {
						ExitClimbEvent ();
					}
				}
			} else {
				letGoTimer = 0;
			}
			if (Mathf.Abs (this.transform.lossyScale.y - target.transform.position.y) < 0.5f && highestClimbPoint == Vector3.zero) {
				highestClimbPoint = target.transform.position;
			} else if (Mathf.Abs (this.transform.lossyScale.y - target.transform.position.y) < 0.5f && highestClimbPoint != Vector3.zero) {
				target.transform.position = highestClimbPoint;
			}
		}
	}
	void OnTriggerEnter() {
		if (climbType == Type.Ladder && Input.GetButton("Action") && canGetOnLadder && exitLadder == false) {
			canGetOnLadder = false;
			target.GetComponent<Animator> ().SetInteger ("climb_number", 2);
			target.GetComponent<MovementController> ().moveUpOnly = true;
			target.GetComponent<MovementController> ().onLadder = true;
			target.GetComponent<MovementController> ().moveDirection = Vector3.zero;
			EnterClimbEvent ();
		}
	}
	IEnumerator StartEnterTimer() {
		canGetOnLadder = false;
		yield return new WaitForSeconds (0.5f);
		canGetOnLadder = true;
	}
	void Update()
	{
		if (exited == true) 
		{
			timer += Time.deltaTime;
			if (timer > 1.0f) 
			{
				timer = 0;
				exited = false;
			}
		}
		if (startTimer) {
			resetTimer += Time.deltaTime;
			if (resetTimer > 2.0f) {
				resetTimer = 0;
			}
		}
		if (InRadius () == true && Input.GetButton("Action") && exited == false && climbType != Type.Ladder) 
		{
			if (cannotMoveHorizontal == true && cannotMoveVertical == true) 
			{
				target.GetComponent<MovementController> ().moveLocked = true;
			}
			else if (cannotMoveHorizontal == false && cannotMoveVertical == true) 
			{
				target.GetComponent<MovementController> ().moveUpOnly = true;
			}
			else if (cannotMoveHorizontal == true && cannotMoveVertical == false) 
			{
				target.GetComponent<MovementController> ().moveSideOnly = true;
			}
			if (cannotJump == true) 
			{
				target.GetComponent<MovementController> ().canJump = false;
			}
			if (addShakeEffect == true) 
			{
				StartCoroutine (AddShake ());
			}
			if (dontAllowDirectionAtBegin == Direction.MoreDown &&
				target.transform.position.y < start.transform.position.y) {
				target.transform.position = start.transform.position;
			}
			else if (dontAllowDirectionAtBegin == Direction.MoreUp &&
				target.transform.position.y > start.transform.position.y) {
				target.transform.position = start.transform.position;
			}
			if (dontAllowDirectionAtEnd == Direction.MoreDown &&
				target.transform.position.y < end.transform.position.y) {
				target.transform.position = end.transform.position;
			}
			else if (dontAllowDirectionAtEnd == Direction.MoreUp &&
				target.transform.position.y > end.transform.position.y) {
				target.transform.position = end.transform.position;
			}
			target.GetComponent<MovementController> ().groundLocked = true;
			target.transform.position = start.transform.position;
			journey = Vector3.Distance (start.transform.position, end.transform.position);
			switch(climbType)
			{
				case Type.Edge:
					startTime = Time.time;
					target.GetComponent<Animator> ().SetInteger ("climb_number", 3);
					target.GetComponent<MovementController> ().moveLocked = false;
					moving = true;
					break;
				case Type.Step:
					startTime = Time.time;
					target.GetComponent<Animator> ().SetInteger ("climb_number", 1);
					target.GetComponent<MovementController> ().moveLocked = true;
					moving = true;
					break;
				default:
					target.GetComponent<Animator> ().SetInteger ("climb_number", 0);
					break;
			}
			EnterClimbEvent ();
		}
		//---------------------- for Ledges and steps -----------------------
		if (moving) 
		{
			float distCovered = (Time.time - startTime) * climbSpeed;
			float fracJourney = distCovered / journey;
			if (fracJourney < 0.8f) {
				target.GetComponentInChildren<Camera> ().transform.LookAt (end.transform);
			}
			target.transform.position = Vector3.Lerp (start.transform.position, end.transform.position, fracJourney);
			if (target.transform.position == end.transform.position) 
			{
				ExitClimbEvent ();
			}
		}
		//-------------------------------------------------------------------------
		//------------------------- for exiting ladder ----------------------------
		if (exitLadder) {
			float distCovered = (Time.time - startTime) * climbSpeed;
			float fracJourney = distCovered / journey;
			target.transform.position = Vector3.Lerp(target.transform.position, new Vector3(this.transform.position.x, this.transform.lossyScale.y , this.transform.position.z), fracJourney);
			if(Mathf.Abs(target.transform.position.x-this.transform.position.x) < 0.13f &&
				Mathf.Abs(target.transform.position.y-this.transform.lossyScale.y) < 0.13f &&
				Mathf.Abs(target.transform.position.z-this.transform.position.z) < 0.13f) 
			{
				Debug.Log ("exit climb event");
				ExitClimbEvent ();
			}
		}
	}
	bool InRadius()
	{
		GameObject[] targets = GameObject.FindGameObjectsWithTag ("Player");
		foreach(GameObject obj in targets)
		{
			target = obj;
			Transform pos = obj.transform;
			float distanceSqr = (transform.position - pos.position).sqrMagnitude;
			if (distanceSqr < eventRadius)
				return true;
		}
		return false;
	}
	IEnumerator AddShake()
	{
		GameObject current = target;
		for(int i=0; i < childIndexForShake.Length; i++) {
			current = current.transform.GetChild (childIndexForShake [i]).gameObject;
		}
		current.GetComponent<CameraShake> ().smooth = true;
		current.GetComponent<CameraShake> ().ShakeCamera (shakeAmount, shakeDuration);
		yield return new WaitForSeconds (4.0f);
	}
	void OnDrawGizmosSelected()
	{
		if (climbType != Type.Ladder) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (this.transform.position, eventRadius);
		}
	}
	void ExitClimbEvent() {
		startTimer = true;
		exited = true;
		timer = 0;
		moving = false;
		exitLadder = false;
		target.GetComponent<Animator> ().SetBool ("climb", false);
		target.GetComponent<MovementController> ().groundLocked = false;
		target.GetComponent<MovementController> ().canJump = true;
		target.GetComponent<MovementController> ().moveLocked = false;
		target.GetComponent<MovementController> ().moveSideOnly = false;
		target.GetComponent<MovementController> ().moveUpOnly = false;
		target.GetComponent<MovementController> ().onLadder = false;
		target.GetComponent<MovementController> ().notEffectedByGravity = false;
	}
	void EnterClimbEvent() {
		target.GetComponent<Animator> ().SetBool ("climb", true);
		target.GetComponent<MovementController> ().groundLocked = true;
		target.GetComponent<MovementController> ().canJump = false;
	}
}
