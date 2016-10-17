using UnityEngine;
using System.Collections;
using TeamUtility.IO;					//Custom Input Manager

[RequireComponent (typeof (CharacterController))]
public class MovementController: MonoBehaviour {
	private float playerHeight = 0;
	public string parkourObjectTag = "Parkour";
	public float parkourDistance = 2.0f;
	public float walkSpeed = 6.0f;

	public float runSpeed = 11.0f;

	// If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
	public bool limitDiagonalSpeed = true;

	// If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
	// There must be a button set up in the Input Manager called "Run"
	public bool toggleRun = false;

	public bool canJump = true;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

	// Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
	public float fallingDamageThreshold = 10.0f;

	// If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
	public bool slideWhenOverSlopeLimit = false;

	// If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
	public bool slideOnTaggedObjects = false;

	public float slideSpeed = 12.0f;

	// If checked, then the player can change direction while in the air
	public bool airControl = false;

	// Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
	public float antiBumpFactor = .75f;

	// Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
	public int antiBunnyHopFactor = 1;

	public Animator anim;
	public Health healthScript;

	public bool groundLocked = false;
	public bool moveLocked = false;
	public bool moveUpOnly = false;
	public bool moveSideOnly = false;
	public bool onLadder = false;

	public Vector3 moveDirection = Vector3.zero;
	private bool grounded = false;
	private CharacterController controller;
	private Transform myTransform;
	private float speed;
	private RaycastHit hit;
	private float fallStartLevel;
	private bool falling;
	private float slideLimit;
	private float rayDistance;
	private Vector3 contactPoint;
	private bool playerControl = false;
	private int jumpTimer;
	public bool notEffectedByGravity = false;

	//for parkour
	private bool parkouring = false;
	private bool animate = true;
	private bool canLook = true;
	private float setSpeed = 0.0f;
	private Vector3 parkourDirection = Vector3.zero;
	private bool parkoursliding = false;
	private bool wallclimb = false;
	private bool climbover = false;
	private GameObject parkouringObject;
	private bool parkPosSet = false;
	private bool rotateDone = false;
	private float startHeight = 0f;
	private float forwardDistance = 0f;
	private Vector3 startLocation = Vector3.zero;
	private bool setStartLoc = false;
	private float travelDistance = 0f;

	void Start() {
		controller = GetComponent<CharacterController>();
		playerHeight = controller.height;
		myTransform = transform;
		speed = walkSpeed;
		rayDistance = controller.height * .5f + controller.radius;
		slideLimit = controller.slopeLimit - .1f;
		jumpTimer = antiBunnyHopFactor;
		if (anim == null) {
			anim = this.GetComponent<Animator>();
		}
		if (healthScript == null) {
			healthScript = this.GetComponent<Health> ();
		}
	}

	void FixedUpdate() {
		float inputX = (moveLocked == false) ? InputManager.GetAxis("Horizontal") : 0;
		float inputY = (moveLocked == false) ? InputManager.GetAxis("Vertical") : 0;
		if (anim.GetBool ("OnWall") == true) {
			inputX = -inputX;
			inputY = -inputY;
		}
		if (onLadder == false) {
			if (moveSideOnly == true) {
				inputY = 0;
			}
			if (moveUpOnly == true) {
				inputX = 0;
			}
		} else {
			moveDirection.y = inputY;
			inputY = 0;
			inputX = 0;
		}
		// If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
		float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed)? .7071f : 1.0f;

		if (grounded) {
			bool sliding = false;
			// See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
			// because that interferes with step climbing amongst other annoyances
			if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance)) {
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}
			// However, just raycasting straight down from the center can fail when on steep slopes
			// So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
			else {
				Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}

			// If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
			if (falling) {
				falling = false;
				if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
					FallingDamageAlert (fallStartLevel - myTransform.position.y);
			}

			// If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
			if (!toggleRun)
				speed = InputManager.GetButton("Run")? runSpeed : walkSpeed;

			// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
			if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
				Vector3 hitNormal = hit.normal;
				moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
				Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
				moveDirection *= slideSpeed;
				playerControl = false;
			}
			// Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
			else {
				moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
				moveDirection = myTransform.TransformDirection(moveDirection) * speed;
				playerControl = true;
			}

			// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
			if (!InputManager.GetButton ("Jump") && canJump) {
				jumpTimer++;
			}
			else if (jumpTimer >= antiBunnyHopFactor) {
				anim.SetTrigger ("jump");
				moveDirection.y = jumpSpeed;
				jumpTimer = 0;
			}
		}
		else {
			// If we stepped over a cliff or something, set the height at which we started falling
			if (!falling) {
				falling = true;
				fallStartLevel = myTransform.position.y;
			}

			// If air control is allowed, check movement but don't touch the y component
			if (airControl && playerControl) {
				moveDirection.x = inputX * speed * inputModifyFactor;
				moveDirection.z = inputY * speed * inputModifyFactor;
				moveDirection = myTransform.TransformDirection(moveDirection);
			}
		}

		// Apply gravity
		if (onLadder == false || notEffectedByGravity == true) {
			moveDirection.y -= gravity * Time.deltaTime;
		} 

		// Move the controller, and set grounded true or false depending on whether we're standing on something
		if (parkouring == false) {
			grounded = (controller.Move (moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
			if (groundLocked == true) {
				grounded = false;
				anim.SetBool ("grounded", true);
			} else {
				anim.SetBool ("grounded", grounded);
			}
		} 
	}

	void SlideOverObject(float x, float y, float setSpeed, float objHeight) {
		moveDirection = new Vector3(x, 0, y);
		moveDirection *= setSpeed;
		parkourDirection = transform.TransformDirection(moveDirection);
		if (parkPosSet == false) {
			this.transform.position = new Vector3 (this.transform.position.x, objHeight + 0.05f, this.transform.position.z);
			parkPosSet = true;
		}
		float rotationAmt = (Mathf.Abs (transform.localRotation.z) > Mathf.Abs (transform.localRotation.x))? Mathf.Abs (transform.localRotation.z) : Mathf.Abs (transform.localRotation.x);
		if (rotateDone == false) {
			transform.Rotate (Vector3.forward * Time.deltaTime * 160);
			if (rotationAmt >= 0.3f) {
				rotateDone = true;
			}
		} else {
			transform.Rotate (-Vector3.forward * Time.deltaTime * 160);
			if (rotationAmt <= 0.05f) {
				EndParkour ();
			}
		}
		controller.Move (parkourDirection * Time.deltaTime);
	}
	void WallClimb(GameObject parkouringObject) {
		if (Mathf.Abs (startHeight - transform.position.y) <= parkouringObject.transform.localScale.y / 2) {
			transform.Translate (Vector3.up * Time.deltaTime * 5);
		} 
		else if (Mathf.Abs (startHeight - transform.position.y) >= parkouringObject.transform.localScale.y) {
			if (setStartLoc == false) {
				setStartLoc = true;
				startLocation = transform.position;
				travelDistance = Vector3.Distance (transform.position, parkouringObject.transform.position)/2;
				rotateDone = true;
			}
			forwardDistance = Vector3.Distance (startLocation, transform.position);
			transform.Translate (Vector3.forward * Time.deltaTime*5);
			if(forwardDistance > travelDistance) {
				EndParkour ();
			}
		}
		else {
			transform.Translate (Vector3.up * Time.deltaTime*2);
		}
	}

	//Everything inside Update() is for parkouring only
	void Update () {
		this.GetComponent<AnimController> ().enabled = animate;
		this.GetComponent<MouseLook> ().enabled = canLook;
		if (toggleRun && grounded && InputManager.GetButtonDown ("Run"))
			speed = (speed == walkSpeed? runSpeed : walkSpeed);
		if(parkoursliding == true) {
			SlideOverObject (1,setSpeed, 1, parkouringObject.transform.localScale.y);
		}
		else if(climbover == true) {
			SlideOverObject (0.6f,setSpeed, 0.6f, parkouringObject.transform.localScale.y);
		}
		else if(wallclimb == true){
			WallClimb (parkouringObject);
		}
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast(transform.position, fwd, out hit, parkourDistance)) {
			if (hit.transform.gameObject.tag == parkourObjectTag && parkouring == false) {
				float objHeight = hit.transform.localScale.y;
				if (objHeight < playerHeight / 1.2) {
					if (InputManager.GetButton ("Run") == true && grounded == true) {
						GetComponent<BreathingController> ().PlayEffortVoice ();
						GetComponent<Animator> ().SetFloat ("parkourNumber", 1);
						StartParkour (this.transform, runSpeed,"sliding");
					} else {
						if (InputManager.GetButton ("Action") && grounded == true) {
							GetComponent<BreathingController> ().PlayEffortVoice ();
							GetComponent<Animator> ().SetFloat ("parkourNumber", 1);
							StartParkour (this.transform, walkSpeed,"climbover");
						}
					}
				} 
				else if(hit.distance < 0.5f){
					if (InputManager.GetButton ("Jump")) {
						GetComponent<BreathingController> ().PlayEffortVoice ();
						GetComponent<Animator> ().SetFloat ("parkourNumber", 0);
						StartParkour (this.transform, walkSpeed,"wallclimb");
					}
				}
				parkouringObject = hit.transform.gameObject;
			}
		}
	}
	void EndParkour() {
		GetComponent<Animator> ().SetBool ("parkour", false);
		moveLocked = false;
		animate = true;
		canJump = true;
		canLook = true;
		parkouring = false;
		parkoursliding = false;
		wallclimb = false;
		climbover = false;
		//GameObject.FindGameObjectWithTag ("CameraHolder").GetComponent<MouseLook> ().enabled = true;
		this.GetComponent<MouseLook> ().enabled = true;
	}
	void StartParkour(Transform start, float speed, string type) {
		GetComponent<Animator> ().SetBool ("parkour", true);
		rotateDone = false;
		parkPosSet = false;
		setSpeed = speed;
		moveLocked = true;
		animate = false;
		canJump = false;
		canLook = false;
		parkouring = true;
		setStartLoc = false;
		startHeight = transform.position.y;
		forwardDistance = 0;
		//GameObject.FindGameObjectWithTag ("CameraHolder").GetComponent<MouseLook> ().enabled = false;
		this.GetComponent<MouseLook> ().enabled = false;
		switch (type) {
		case "sliding":
			parkoursliding = true;	
			break;
		case "wallclimb":
			wallclimb = true;	
			break;
		case "climbover":
			climbover = true;	
			break;
		}
	}
	IEnumerator SetFallDelayAnim() {
		anim.SetBool ("fall_delay", true);
		yield return new WaitForSeconds (0.5f);
		anim.SetBool ("fall_delay", false);
	}
	// Store point that we're in contact with for use in FixedUpdate if needed
	void OnControllerColliderHit (ControllerColliderHit hit) {
		contactPoint = hit.point;
	}

	// If falling damage occured, this is the place to do something about it. You can make the player
	// have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
	void FallingDamageAlert (float fallDistance) {   
//		healthScript.SetRagdollState (true);
		healthScript.ApplyDamage (fallDistance);
	}
}