using UnityEngine;
using System.Collections;
using TeamUtility.IO;                   //Custom Input Manager
using CyberBullet.GameManager;

namespace CyberBullet.Controllers {
    [System.Serializable]
    public class MvSwimming {
        public string WaterTag = "Water";
        public float slowSpeed = 0.10f;
        public float fastSpeed = 0.30f;
        public float climbOutSpeed = 2.0f;
        public AudioSource soundSource = null;
        public float gravity = 1.0f;
        public mvSwimSound[] enter;
        public mvSwimSound[] exit;
    }
    [System.Serializable]
    public class mvSwimSound {
        [Range(0,1)]
        public float volume = 1.0f;
        public bool loop = false;
        public AudioClip sound = null;
    }
    [System.Serializable]
    public class MvGround {
        public float walkSpeed = 4.0f;
        public float runSpeed = 8.0f;
        public bool toggleRun = false;
        public bool limitDiagonalSpeed = true;
        [Space(10)]
        public float crouchHeight = 0.50f;
        public float standingHeight = 2.0f;
        public bool toggleCrouch = false;
    }
    [System.Serializable]
    public class MvJumping {
        public float jumpSpeed = 8.0f;
        public float gravity = 20.0f;
        public float fallingDamageThreshold = 10.0f;
        public bool airControl = false;                 // If checked, then the player can change direction while in the air
        public float antiBumpFactor = .75f;             // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
        public int antiBunnyHopFactor = 1;              // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
        public bool notEffectedByGravity = false;
        public bool canJump = true;
    }
    [System.Serializable]
    public class MvSliding {
        public string slideOnTagged = "Slide";
        public bool slideWhenOverSlopeLimit = false;    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
        public bool slideOnTaggedObjects = false;       // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
        public float slideSpeed = 12.0f;
    }
    [System.Serializable]
    public class MvAnimations {
        public Animator[] animators;
        public string swimming_bool = "swimming";
    }
    [System.Serializable]
    public class MvDebugging {
        public  bool swimming_water_hit = false;
        public bool swimming_show_exit_point = false;
    }
    [RequireComponent (typeof (CharacterController))]
    public class MovementController: MonoBehaviour {
        #region Variables
        [SerializeField] private InvWeaponManager wm;
        [SerializeField] private Health healthScript;
        [SerializeField] private Camera playerCam = null;
        [SerializeField] private FootStepKeyFrame fsController = null;
        private CharacterController controller;
        #region Swimming
        [SerializeField] private MvSwimming swimming;
        private bool isSwimming = false;
        #endregion

        #region Basic Ground Movement
        [SerializeField] private MvGround groundMovement;
        [HideInInspector] public bool crouching = false;
        private bool moveLocked = false;
        private bool groundLocked = false;
        private bool moveUpOnly = false;
        private bool moveSideOnly = false;
        private Vector3 moveDirection = Vector3.zero;
        private float speed;
        private bool aimWalk = false;
        #endregion

        #region Jumping/AirMovement
        [SerializeField] private MvJumping jumping;
        private int jumpTimer;
        private float inputModifyFactor = 0.0f;
        private float fallStartLevel;
        private bool falling;
        private bool grounded = false;
        #endregion

        #region Sliding
        [SerializeField] private MvSliding sliding;
        private bool isSliding = false;
        private float slideLimit = 0.0f;
        private Vector3 contactPoint;
        private float rayDistance;
        #endregion

        #region Animations
        [SerializeField] private MvAnimations anims;
        #endregion

        [SerializeField] private MvDebugging debugging;

        private Transform myTransform;
        private RaycastHit hit;
        private bool playerControl = false;
        private float parabola_timer = 0;
        private bool parabola_move = false;
        private Vector3 parabola_start = Vector3.zero;
        private Vector3 parabola_end = Vector3.zero;
        private bool lerp_move = false;
        private float lerp_timer = 0.0f;
        private Vector3 lerp_end = Vector3.zero;
        private GameObject camHolder = null;
        private Vector3 look_dir = Vector3.zero;
        #endregion

        void Start() 
        {
            camHolder = GameObject.FindGameObjectWithTag("CameraHolder");
            if (playerCam == null)
                Debug.LogError("You must supply a player Camera to MovementController!");
            wm = GetComponentInChildren<InvWeaponManager>();
            controller = GetComponent<CharacterController>();
            myTransform = transform;
            speed = groundMovement.walkSpeed;
            rayDistance = controller.height * .5f + controller.radius;
            slideLimit = controller.slopeLimit - .1f;
            jumpTimer = jumping.antiBunnyHopFactor;
            if (anims.animators.Length < 1 || anims.animators[0] == null) {
                anims.animators[0] = this.GetComponent<Animator>();
            }
            if (healthScript == null) {
                healthScript = this.GetComponent<Health> ();
            }
        }

        void FixedUpdate() {
            float inputX = (moveLocked == false) ? InputManager.GetAxis("Horizontal") : 0;
            float inputY = (moveLocked == false) ? InputManager.GetAxis("Vertical") : 0;

            inputX = (moveUpOnly == true) ? 0 : inputX;
            inputY = (moveSideOnly == true) ? 0 : inputY;

            //crouch managment
            if (moveLocked == false) {
                if (groundMovement.toggleCrouch == true && InputManager.GetButtonDown ("Crouch") == true) {
                    Crouch ((crouching == true) ? false : true);
                } else if (groundMovement.toggleCrouch == false) {
                    Crouch (InputManager.GetButton ("Crouch"));
                }
            }

            // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
            inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && groundMovement.limitDiagonalSpeed)? .7071f : 1.0f;
            inputModifyFactor = (crouching == true || aimWalk == true) ? inputModifyFactor - 0.5f : inputModifyFactor;

            if (grounded) {
                CheckIfSliding ();

                // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
                if (falling == true) {
                    CheckForFallDamage ();
                }

                // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
                if (!groundMovement.toggleRun)
                    speed = (InputManager.GetButton("Run") && crouching == false && aimWalk == false)? groundMovement.runSpeed : groundMovement.walkSpeed;

                ApplySlidingEffects (inputX, inputY, inputModifyFactor); 
                CheckForJump ();
            }
            // If we stepped over a cliff or something, set the height at which we started falling
            if (!grounded) {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }

            ApplyNoGroundMovement (inputX, inputY);
            ApplyGravity ();
            CheckIfGrounded ();
        }

        #region Callable
        public bool GetForceWalk()
        {
            return aimWalk;
        }
        public float GetCurrentMoveSpeed()
        {
            return speed;
        }
        public float GetWalkSpeed()
        {
            return groundMovement.walkSpeed;
        }
        public float GetRunSpeed()
        {
            return groundMovement.runSpeed;
        }
        public bool GetLockedMovement()
        {
            return moveLocked;
        }
        public float GetGravity()
        {
            if (isSwimming == true)
            {
                return swimming.gravity;
            }
            else
            {
                return jumping.gravity;
            }
        }
        public bool GetSwimState()
        {
            return isSwimming;
        }
        public void SetMoveSpeed(float walk=-1.0f, float run=1.0f)
        {
            if (walk != -1.0f)
            {
                groundMovement.walkSpeed = walk;
            }
            if (run != -1.0f)
            {
                groundMovement.runSpeed = run;
            }
        }
        public void SetLockMovement(bool state)
        {
            moveLocked = state;
        }
        public void SetLockToGround(bool state)
        {
            groundLocked = state;
        }
        public void SetUpDownMovementOnly(bool state)
        {
            moveUpOnly = state;
        }
        public void SetSideMovementOnly(bool state)
        {
            moveSideOnly = state;
        }
        public void SetForceWalk(bool state)
        {
            aimWalk = state;
        }
        public void SetLockJump(bool state)
        {
            jumping.canJump = !state;
        }
        #endregion

        #region Sliding
        private void CheckIfSliding() {
            isSliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance)) {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    isSliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    isSliding = true;
            }
        }
        private void ApplySlidingEffects(float inputX, float inputY, float inputModifyFactor) {
            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if ( (isSliding == true && sliding.slideWhenOverSlopeLimit) || (sliding.slideOnTaggedObjects && hit.collider.tag == sliding.slideOnTagged) ) {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
                moveDirection *= sliding.slideSpeed;
                playerControl = false;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else {
                moveDirection = new Vector3(inputX * inputModifyFactor, -jumping.antiBumpFactor, inputY * inputModifyFactor);
                moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                playerControl = true;
            }
        }
        #endregion

        #region Jumping 
        private void CheckForJump() {
            if (jumping.canJump == false)
            {
                jumpTimer = 0;
                return;
            }
            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            if (!InputManager.GetButton ("Jump")) {
                jumpTimer++;
            }
            else if (jumpTimer >= jumping.antiBunnyHopFactor && crouching == false) {
                foreach (Animator an in anims.animators) {
                    if (an.transform.gameObject.activeInHierarchy == true) {
                        an.SetTrigger ("jump");
                    }
                }
                moveDirection.y = jumping.jumpSpeed;
                jumpTimer = 0;
                if (GetComponent<ClimbController> ())
                    GetComponent<ClimbController> ().InitClimbing ();
            }
        }
        private void CheckIfGrounded() {
            if (isSwimming == true)
            {
                grounded = true;
            }
            // Move the controller, and set grounded true or false depending on whether we're standing on something
            grounded = (controller.Move (moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
            if (groundLocked == true) {
                grounded = false;
                foreach (Animator animatorSelect in anims.animators) {
                    if (animatorSelect) {
                        animatorSelect.SetBool ("grounded", true);
                    }
                }
            } else {
                foreach (Animator animatorSelect in anims.animators) {
                    if (animatorSelect.transform.gameObject.activeInHierarchy == true) {
                        animatorSelect.SetBool ("grounded", grounded);
                    }
                }
            }
        }
        #endregion

        #region Gravity
        // If falling damage occured, this is the place to do something about it. You can make the player
        // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
        void FallingDamageAlert (float fallDistance) {   
            //      healthScript.SetRagdollState (true);
            healthScript.ApplyDamage (fallDistance);
        }
        private void CheckForFallDamage() {
            falling = false;
            if (isSwimming == true)
                return;
            if (myTransform.position.y < fallStartLevel - jumping.fallingDamageThreshold)
                FallingDamageAlert (fallStartLevel - myTransform.position.y);
            if (fallStartLevel - myTransform.position.y > 0.1f && fsController != null) {
                fsController.PlayLandAudio ();
            }
        }
        // Store point that we're in contact with for use in FixedUpdate if needed
        void OnControllerColliderHit (ControllerColliderHit hit) {
            contactPoint = hit.point;
        }
        private void ApplyGravity() {
            if (jumping.notEffectedByGravity == true)
            {
                moveDirection.y = 0;
                return;
            }
            else if (isSwimming == false)
            {
                moveDirection.y -= GetGravity() * Time.deltaTime;
            }
//            else
//            {
//                controller.Move(new Vector3(0, moveDirection.y - GetGravity(), 0));
//            }

        }
        #endregion

        #region Apply Movement
        private void ApplyNoGroundMovement(float inputX, float inputY) {
            if (isSwimming == true)
            {
                moveDirection = new Vector3(inputX * speed, 0, inputY * speed);
                moveDirection = playerCam.transform.TransformDirection(moveDirection);
            }
            // If air control is allowed, check movement but don't touch the y component
            else if (jumping.airControl == true && playerControl) {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }
        void Crouch(bool isCrouching) {
            crouching = isCrouching;
            foreach (Animator am in anims.animators) {
                if (am.transform.gameObject.activeInHierarchy == true) {
                    am.SetBool ("crouching", isCrouching);
                }
            }
            if (crouching == true) {
                controller.height = groundMovement.crouchHeight;
            } else {
                controller.height = (controller.height >= groundMovement.standingHeight) ? groundMovement.standingHeight : controller.height + 0.15f;
            }
        }
        #endregion

        #region For Animations
        public void SetAnimValue(string parameter, bool value)
        {
            foreach (Animator anim in anims.animators)
            {
                if (anim.transform.gameObject.activeSelf == false)
                    continue;
                anim.SetBool(parameter, value);
            }
        }
        public void SetAnimValue(string parameter, float value)
        {
            foreach (Animator anim in anims.animators)
            {
                if (anim.transform.gameObject.activeSelf == false)
                    continue;
                anim.SetFloat(parameter, value);
            }
        }
        #endregion  

        #region Swimming
        void OnTriggerEnter(Collider trigCol)
        {
            if (trigCol.tag == swimming.WaterTag)
            {
                if (debugging.swimming_water_hit == true)
                {
                    Debug.Log(trigCol.transform.name);
                }
                InWaterState(true);
            }
        }
        void OnTriggerStay(Collider trigCol)
        {
            if (trigCol.tag == swimming.WaterTag && isSwimming == false)
            {
                InWaterState(true);
            }
        }
        void OnTriggerExit(Collider trigCol)
        {
            if (trigCol.tag == swimming.WaterTag)
            {
                InWaterState(false);
            }
        }
        public void InWaterState(bool state)
        {
            if (state == true)
            {
                grounded = false;
                wm.DisableAllWeapons();
                PlaySound(swimming.soundSource, swimming.enter);
            }
            else
            {
                PlaySound(swimming.soundSource, swimming.exit);
            }
            if (GetComponentInChildren<HeadBobber>())
            {
                GetComponentInChildren<HeadBobber>().enabled = !state;
            }
            SetLockJump(state);
            wm.CanEquipWeapons(!state);
            wm.EnableHands();
            SetAnimValue(anims.swimming_bool, state);
            isSwimming = state;
            falling = false;
        }
        void PlaySound(AudioSource sSource, mvSwimSound[] sounds)
        {
            if (sounds.Length < 1 || sSource == null)
                return;
            mvSwimSound tsound = sounds[Random.Range(0, sounds.Length)];
            sSource.clip = tsound.sound;
            sSource.volume = tsound.volume;
            sSource.loop = tsound.loop;
            sSource.Play();
        }
        public void ExitWater(Vector3 start,Vector3 destination, float height)
        {
            InWaterState(false);
            controller.enabled = false;
            parabola_timer = 0;
            parabola_start = start;
            parabola_end = destination;
            PlayerManager pm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
            pm.EnableCameraControl(false);
            SetLockMovement(true);
            if (Vector3.Distance(transform.position, parabola_start) < 0.1f)
            {
                parabola_move = true;
            }
            else
            {
                lerp_timer = 0.0f;
                lerp_end = parabola_start;
                lerp_move = true;
            }
            if (debugging.swimming_show_exit_point == true)
            {
                GameObject end_cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                end_cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                Destroy(end_cube.GetComponent<BoxCollider>());
                end_cube.transform.position = destination;

                GameObject start_cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(start_cube.GetComponent<BoxCollider>());
                start_cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                start_cube.transform.position = start;
            }
        }
        public void EndExitWater()
        {
            parabola_move = false;
            SetLockMovement(false);
            PlayerManager pm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
            pm.EnableCameraControl(true);
            parabola_timer = 0;
            this.transform.position = parabola_end;
            controller.enabled = true;
        }
        #endregion

        void Update () {
            if (isSwimming == true)
            {
                speed = (InputManager.GetButton("Run"))? swimming.fastSpeed : swimming.slowSpeed;
            }
            else if (isSwimming == false && groundMovement.toggleRun && grounded == true && InputManager.GetButtonDown ("Run"))
                speed = (speed == groundMovement.walkSpeed && aimWalk == false)? groundMovement.runSpeed : groundMovement.walkSpeed;
            //Exit water - parabolic movement to land position
            if (parabola_move == true)
            {
                parabola_timer += Time.deltaTime;
                if ((parabola_timer / swimming.climbOutSpeed) <= 0.35f)
                {
                    camHolder.transform.LookAt(parabola_end);
                    look_dir = new Vector3(parabola_end.x, parabola_end.y + camHolder.transform.position.y, parabola_end.z) - parabola_end;
                }
                else
                {
                    Quaternion toRotation = Quaternion.FromToRotation(camHolder.transform.forward, look_dir);
                    camHolder.transform.rotation = Quaternion.Lerp(camHolder.transform.rotation, toRotation, 0.65f * Time.deltaTime);
                }

                transform.position = Vector3.Lerp(transform.position, parabola_end, parabola_timer / swimming.climbOutSpeed);
                if (Vector3.Distance(transform.position,parabola_end) < 0.1f)
                {
                    EndExitWater();
                }
            }
            //Exit Water - Move to start position before parabolic movement
            if (lerp_move == true)
            {
                lerp_timer += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, lerp_end, lerp_timer);
                camHolder.transform.LookAt(parabola_end);
                if (Vector3.Distance(transform.position, lerp_end) < 0.1f)
                {
                    lerp_move = false;
                    parabola_move = true;
                }
            }
        }
    }
}