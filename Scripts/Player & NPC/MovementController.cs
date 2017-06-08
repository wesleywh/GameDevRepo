using UnityEngine;
using System.Collections;
using TeamUtility.IO;					//Custom Input Manager

[RequireComponent (typeof (CharacterController))]
public class MovementController: MonoBehaviour {
	#region Variables
//	[SerializeField] private bool playLandSound = false;
	[SerializeField] private float crouchHeight = 1.0f;
	[SerializeField] private float standingHeight = 2.0f;
	public float walkSpeed = 6.0f;
	public float runSpeed = 11.0f;
	public bool crouching = false;

	// If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
	public bool limitDiagonalSpeed = true;

	// If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
	// There must be a button set up in the Input Manager called "Run"
	public bool toggleRun = false;
	public bool toggleCrouch = false;
	public bool canJump = true;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

	// Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
	public float fallingDamageThreshold = 10.0f;

	[Header("Sliding")]
	[SerializeField] private string slideOnTagged = "Slide";
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

	[Header("List of Animators To Apply Animations")]
	public Animator[] anim;
	public Health healthScript;

	public bool groundLocked = false;
	public bool moveLocked = false;
	public bool moveUpOnly = false;
	public bool moveSideOnly = false;

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
	private float inputModifyFactor = 0.0f;

	private bool sliding = false;

	//for swimming
	[Header("Swimming Options")]
	public bool underWater = false;
	public bool swimming = false;
	[SerializeField] public RainCameraController enterPool;
	[SerializeField] public RainCameraController exitPool;
	public GameObject enterWaterSplash;
	public GameObject underwaterBubbles;
	public GameObject topOfWaterDisturbance;
	public bool isJumping = false;
	public UnityEngine.Color underwaterColor = new Color (0.0f, 0.4f, 0.7f, 0.6f);
	public float fogAmount = 0.04f;
	private bool org_airControl = false;
	private float water_height = 0.0f;
	[HideInInspector] public float underwater_offset = 0.25f;
	private float swimSlowGravity = -0.10f;
	private bool enteredWater = false;
	private bool defaultFog;
	private Color defaultFogColor;
	private float defaultFogDensity;
	private Material defaultSkybox;
	private Material noSkybox = null;
	private GameObject water_foam = null;

	[Header("Debugging")]
	[SerializeField] private bool playUnderwaterFX = false;
	[SerializeField] private bool playExitUnderwaterFX = false;

	#endregion

	void Start() {
		//for swimming
		org_airControl = airControl;
		defaultFog = RenderSettings.fog;
		defaultFogColor = RenderSettings.fogColor;
		defaultFogDensity = RenderSettings.fogDensity;
		defaultSkybox = RenderSettings.skybox;

		controller = GetComponent<CharacterController>();
		myTransform = transform;
		speed = walkSpeed;
		rayDistance = controller.height * .5f + controller.radius;
		slideLimit = controller.slopeLimit - .1f;
		jumpTimer = antiBunnyHopFactor;
		if (anim.Length < 1 || anim[0] == null) {
			anim[0] = this.GetComponent<Animator>();
		}
		if (healthScript == null) {
			healthScript = this.GetComponent<Health> ();
		}
	}

	void FixedUpdate() {
		float inputX = (moveLocked == false) ? InputManager.GetAxis("Horizontal") : 0;
		float inputY = (moveLocked == false) ? InputManager.GetAxis("Vertical") : 0;

		//crouch managment
		if (moveLocked == false) {
            if (toggleCrouch == true && InputManager.GetButtonDown ("Crouch") == true) {
                Crouch ((crouching == true) ? false : true);
            } else if (toggleCrouch == false) {
				Crouch (InputManager.GetButton ("Crouch"));
			}
		}
			
		// If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
		inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed)? .7071f : 1.0f;
		inputModifyFactor = (crouching == true) ? inputModifyFactor - 0.5f : inputModifyFactor;

		if (grounded) {
			CheckIfSliding ();

			// If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
			if (falling == true) {
				CheckForFallDamage ();
			}

			// If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
			if (!toggleRun)
				speed = (InputManager.GetButton("Run") && crouching == false)? runSpeed : walkSpeed;

			ApplySlidingEffects (inputX, inputY, inputModifyFactor);
			CheckForJump ();
		}
		// If we stepped over a cliff or something, set the height at which we started falling
		if (!falling) {
			falling = true;
			fallStartLevel = myTransform.position.y;
		}
		ApplyNoGroundMovement (inputX, inputY);
			
		ApplyGravity ();
		CheckIfGrounded ();
	}

	#region Universal
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
		
	private void ApplyGravity() {
		if (notEffectedByGravity == true) {
			moveDirection.y = 0;
			return;
		}
		// Apply gravity
		if (swimming == true) {
			moveDirection.y += (gravity / 2) * Time.deltaTime;
		}
		else if (notEffectedByGravity == false) {
			moveDirection.y -= gravity * Time.deltaTime;
		} 
	}
	void Crouch(bool isCrouching) {
		crouching = isCrouching;
		foreach (Animator am in anim) {
            if (am.transform.gameObject.activeSelf == true) {
				am.SetBool ("crouching", isCrouching);
			}
		}
		if (crouching == true) {
			controller.height = crouchHeight;
		} else {
			controller.height = (controller.height >= standingHeight) ? standingHeight : controller.height + 0.15f;
		}
	}
	private void CheckIfSliding() {
		sliding = false;
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
	}
	private void ApplySlidingEffects(float inputX, float inputY, float inputModifyFactor) {
		// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
		if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == slideOnTagged) ) {
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
	}
	private void CheckForFallDamage() {
		falling = false;
		if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
			FallingDamageAlert (fallStartLevel - myTransform.position.y);
		if (fallStartLevel - myTransform.position.y > 0.1f) {
			this.GetComponent<FootStepKeyFrame> ().PlayLandAudio ();
		}
	}
	private void CheckForJump() {
		// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
		isJumping = InputManager.GetButton("Jump");
		if (!InputManager.GetButton ("Jump") && canJump) {
			jumpTimer++;
		}
		else if (jumpTimer >= antiBunnyHopFactor && crouching == false) {
			foreach (Animator an in anim) {
                if (an.transform.gameObject.activeSelf == true) {
					an.SetTrigger ("jump");
				}
			}
			moveDirection.y = jumpSpeed;
			jumpTimer = 0;
            if (GetComponent<GameDevRepo.Controllers.ClimbController> ())
                GetComponent<GameDevRepo.Controllers.ClimbController> ().InitClimbing ();
		}
	}
	private void CheckIfGrounded() {
		// Move the controller, and set grounded true or false depending on whether we're standing on something
		if (swimming == true)
			grounded = false;
		else
			grounded = (controller.Move (moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
		if (groundLocked == true) {
			grounded = false;
			foreach (Animator animatorSelect in anim) {
				if (animatorSelect) {
					animatorSelect.SetBool ("grounded", true);
				}
			}
		} else {
			foreach (Animator animatorSelect in anim) {
                if (animatorSelect.transform.gameObject.activeSelf == true) {
					animatorSelect.SetBool ("grounded", grounded);
				}
			}
		}
	}
	private void ApplyNoGroundMovement(float inputX, float inputY) {
		// If air control is allowed, check movement but don't touch the y component
		if (airControl && playerControl) {
			if (swimming == true) {
				if (inputY > 0 || inputY < 0) {
					this.transform.position = this.transform.position + GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ().transform.forward * inputY * speed * Time.deltaTime;
				}
				if (inputX > 0 || inputX < 0) {
					this.transform.position = this.transform.position + this.transform.right * inputX / 2 * speed * Time.deltaTime;
				}
				if (swimSlowGravity < 0) {
					swimSlowGravity += Time.deltaTime;
					this.transform.position = this.transform.position + Vector3.up * swimSlowGravity/2;
				}
				if (this.transform.position.y > water_height) {
					this.transform.position = new Vector3 (this.transform.position.x, water_height, this.transform.position.z);
				}
			} else {
				moveDirection.x = inputX * speed * inputModifyFactor;
				moveDirection.z = inputY * speed * inputModifyFactor;
				moveDirection = myTransform.TransformDirection(moveDirection);
			}
		}
	}
	#endregion	

	#region Swimming
	public void EnterWater() {
		swimming = true;
		water_height = this.transform.position.y;
		airControl = true;
		swimSlowGravity = moveDirection.y/18;
	}
	public void ExitWater() {
		swimming = false;
		underWater = false;
		airControl = org_airControl;
		moveDirection.y = swimSlowGravity;
	}
	private void PlayEnterUnderWater() {
		if (enteredWater == false) {
			enterPool.Play ();
			enteredWater = true;
			RenderSettings.fog = true;
			RenderSettings.fogColor = underwaterColor;
			RenderSettings.fogDensity = fogAmount;
			RenderSettings.skybox = noSkybox;
			if (!enterWaterSplash)
				return;
			Vector3 waterSpawn = new Vector3 (this.transform.position.x, water_height, this.transform.position.z);
			GameObject splash = Instantiate (enterWaterSplash, waterSpawn, Quaternion.identity) as GameObject;
			Destroy (splash, 1.0f);
		}
	}
	private void PlayExitUnderWater() {
		if (enteredWater == true) {
			exitPool.Play ();
			enteredWater = false;
			RenderSettings.fog = defaultFog;
			RenderSettings.fogColor = defaultFogColor;
			RenderSettings.fogDensity = defaultFogDensity;
			RenderSettings.skybox = defaultSkybox;
			if (!enterWaterSplash)
				return;
			Vector3 waterSpawn = new Vector3 (this.transform.position.x, water_height, this.transform.position.z);
			GameObject splash = Instantiate (enterWaterSplash, waterSpawn, Quaternion.identity) as GameObject;
			Destroy (splash, 1.0f);
		}
	}
	#endregion

	void Update () {
		if (playUnderwaterFX == true) {
			PlayEnterUnderWater ();
			playUnderwaterFX = false;
		}
		if (playExitUnderwaterFX == true) {
			PlayExitUnderWater ();
			playExitUnderwaterFX = false;
		}
		if (toggleRun && grounded && InputManager.GetButtonDown ("Run"))
			speed = (speed == walkSpeed? runSpeed : walkSpeed);
		if (swimming == true){
			if (topOfWaterDisturbance && water_foam == null) {
				Vector3 waterSpawn = new Vector3 (this.transform.position.x, water_height, this.transform.position.z);
				water_foam = Instantiate (topOfWaterDisturbance, waterSpawn, Quaternion.identity);
			}
			water_foam.transform.position = new Vector3(this.transform.position.x, water_height, this.transform.position.z);
			if(this.transform.position.y < (water_height - underwater_offset)) {
				underWater = true;
				PlayEnterUnderWater ();
			} else {
				underWater = false;
				PlayExitUnderWater ();
			}
			if (underWater == true) {
				Destroy (water_foam);
				water_foam = null;
			}
		}
		else if ((underWater == true  || swimming == false) && water_foam != null) {
			Destroy (water_foam, 1.0f);
			water_foam = null;
		}
	}
}