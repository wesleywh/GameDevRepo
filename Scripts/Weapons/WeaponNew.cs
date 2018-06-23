/// <summary>
/// Add this to a weapon that will be attached to a weapon manager. 
/// Settings specific to a particular weapon.
/// </summary>
//////////////////
/// Written by: Wesley Haws
/// 
/// Liscense: May use this for any reason. Even Commercially
/// ///////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;					//Custom Input Manager
using UnityEngine.UI;
using UnityEngine.Events;
using CyberBullet.Controllers;
using CyberBullet.GameManager;

namespace CyberBullet.Weapons {
    public enum W_Type {Sniper, Shotgun, Revolver, Pistol, AK47, M4A1, Skorpin, UMP45, AssaultRifle, Ammo, Grenade, Melee, Rifle }

    #region Base Weapon Settings
    [Serializable]
    public class WeaponBaseSettings {
        public LayerMask ignoreLayersOnShot = new LayerMask();
        public float shotDistance = 400;
        public Transform shotPoint = null;
        public int addedHipInaccuracy = 0;
        public int aimInaccuracy = 0;
        public float waitBetweenShots = 0.05f;
        public float aimSpeed = 3f;
        public float recoilAmount = 5f;
        public float damagePerShot = 20f;
        public bool canReload = true;
        public bool timedReload = true;
        public float reloadTime = 2.0f;
        public bool infiniteAmmo = false;
        public int loaded_ammo = 7;
        public int bullets_left = 10;
        public int ammo_per_reload = 7;
        public int max_ammo = 7;
        public int raysPerShot = 1;
        public bool auto_reload = false;
        public Grenade grenadeScript = null;
        public bool fireWeaponOnDownPress = true;
        public bool canAim = true;
        public bool walkWhenAiming = true;
    }
    #endregion

    #region Weapon Sounds
    [Serializable]
    public class WeaponSounds {
        public AudioSource weaponSoundSource = null;
        public AudioClip[] equip;
        public AudioClip[] unequip;
        public AudioClip[] fire;
        public AudioClip[] reload;
        public AudioClip[] empty;
        [Range(0,1)]
        public float fire_volume = 1.0f;
        [Range(0,1)]
        public float equip_volume = 1.0f;
        [Range(0,1)]
        public float unequip_volume = 1.0f;
        [Range(0,1)]
        public float reload_volume = 1.0f;
        [Range(0,1)]
        public float empty_volume = 1.0f;
    }
    #endregion

    #region Weapon Effects
    [Serializable]
    public class WeaponEffects {
        public Texture2D sniperAim = null;
        public float zoomSpeed = 70f;
        public float aimZoom = 15;
        public Camera zoomEffect = null;
        public Camera recoilEffect = null;
        public ParticleSystem muzzleFlash = null;
        public Light muzzleLight = null;
        public Vector2 fireTracerBetweenXShots = new Vector2(2,4);
        public ParticleSystem tracer = null;
        public GameObject ejectShell = null;
        public Transform ejectPoint = null;
        public W_Eject ejectDirection = W_Eject.Left;
        [Range(0,1)]
        public float blurEdgeAmount = 0.7f;
    }
    #endregion

    #region Animation Settings
    [Serializable]
    public class WeaponAnimationSettings {
        public Animator anim = null;
        public float recoilSpeed = 160.0f;
        [Header("Camera Effects")]
        public Animation cameraEffects = null;
        public string calmEffect = "camera_sway";
        public string shotEffect = "weapon_shot";
        [Header("Run Settings")]
        public bool disableOverrides = false;
        public Vector3 leftPos = Vector3.zero;
        public Vector3 leftRot = Vector3.zero;
        public Vector3 rightPos = Vector3.zero;
        public Vector3 rightRot = Vector3.zero;
        public float posSpeed = 0.15f;
        public float rotSpeed = 10.0f;
        public float errorRange = 0.0001f;
        [Header("Hip Sway Settings")]
        public bool hipSway = true;
        public float hipswayMaxAmount = 0.03f;
        public float hipswayForwardOffset = 1.0f;
        public float hipswayPosSmoothing = 0.5f;
        public float hipswayRotSmoothing = 0.5f;
        public float hipswayTiltAngle = 1.0f;
        [Header("Aim Sway Settings")]
        public bool aimSway = false;
        public float aimswayMaxAmount = 0.03f;
        public float aimswayForwardOffset = 1.0f;
        public float aimswayPosSmoothing = 0.5f;
        public float aimswayRotSmoothing = 0.5f;
        public float aimswayTiltAngle = 0.5f;
        [Space(10)]
        public bool hipBob = true;
        public bool aimBob = false;
        public Vector3 idleBobSpeed = Vector3.zero;
        public Vector3 walkBobSpeed = new Vector3(0,0.13f,-0.1f);
        public Vector3 runBobSpeed = new Vector3(0,0.18f,-0.1f);
        public float walkBobAmount = 0.01f;
        public float runBobAmount = 0.03f;
        public float bobMidpoint = 0;
        [Header("Animation Params")]
        public string reloadTrigger = "reload";
        public string reloadBool = "reloading";
        public string attackTrigger = "attack";
        public string aimBool = "block";
    }
    #endregion
   
    #region Override Positioning
    [Serializable]
    public class WeaponPositioning {
        [Header("Override Aim shot position")]
        public Vector3 shot_offset = Vector3.zero;
        [Header("Overrides - Hip")]
        public Vector3 hipPosition = Vector3.zero;
        public Vector3 hipRotation = Vector3.zero;
        [HideInInspector] public Vector3 hipMoveVelocity = Vector3.zero;
        public float hipMoveSpeed = 0.3f;
        public float hipRotationSpeed = 60f;
        [Header("Overrides - Aim")]
        public Vector3 aimPosition = Vector3.zero;
        public Vector3 aimRotation = Vector3.zero;
        [HideInInspector] public Vector3 aimMoveVelocity = Vector3.zero;
        public float aimMoveSpeed = 0.05f;
        public float aimRotationSpeed = 60f;
        [Header("Overrides - Reload Position")]
        public bool moveForReload = true;
        public Vector3 reloadPosition = Vector3.zero;
        public Vector3 reloadRotation = Vector3.zero;
        [HideInInspector] public Vector3 reloadMoveVelocity = Vector3.zero;
        public float reloadMoveSpeed = 0.05f;
        public float reloadRotationSpeed = 60f;
    }
    #endregion

    #region Weapon Particle Effects
    [Serializable]
    public class WeaponParticleEffects {
        public string name = "Untagged Section";
        public string[] tags = null;
        public ParticleSystem[] particles = null;
        public WeaponPEDecal[] decals = null;
    }
    [Serializable]
    public class WeaponPEDecal {
        public GameObject bulletDecal = null;
        public bool invert = false;
    }
    #endregion

    #region Weapon Aimer Visuals
    [Serializable]
    public class WeaponAimerVisuals {
        public Vector3 aimPosition = Vector3.zero;
        public Vector3 aimRotation = Vector3.zero;
        public Texture2D aimerHorizontal = null;
        public Texture2D aimerVertical = null;
        public Texture2D aimerHit = null;
        public float increaseAimerEveryShot = 10f;
        public Vector2 aimerHorizontalSize = new Vector2(20,20);
        public Vector2 aimerVerticalSize = new Vector2(20,20);
        public Vector2 aimerCenter = new Vector2(10,10);
        public Vector2 aimerEdge = new Vector2 (30, 30);
        public float aimerMoveOutSpeed = 50.0f;
        public float aimerMoveInSpeed = 30.0f;
        public bool hideAimerWhenAiming = false;
        public GameObject[] disableWhenAiming = null;
    }
    #endregion

    #region Weapon UI 
    [Serializable]
    public class WeaponUI {
        public bool displayUI = true;
        public Sprite clipImage = null;
        public Sprite bulletImage = null;
        public Vector2 UIBulletImageSize = new Vector2(10,50);
        public Vector2 UIClipImageSize = new Vector2(50,50);
    }
    #endregion

    #region Events
    [Serializable]
    public class WeaponEvents {
        public UnityEvent OnReload;
        public UnityEvent FinishedReloading;
        public UnityEvent OnFire;
    }
    #endregion

    #region Debugging 
    [Serializable]
    public class WeaponDebugging {
        public bool debug_raycast = false;
        public bool aiming = false;
        public bool manuallySetPosition = false;
        public bool autoCalculateRotation = false;
        public bool reloadPosition = false;
        public bool show_screen_hitpoint = false;
        [HideInInspector] public Vector2 hitpoint = new Vector2(1,1);
    }
    #endregion
    [RequireComponent(typeof(AudioSource))]
    public class WeaponNew : MonoBehaviour {
        
        #region Types
    	enum W_FireMode {Auto, Semi }
    	enum W_ZoomType {Simple, Sniper }
    	[SerializeField] W_FireMode fireMode = W_FireMode.Auto;
    	[SerializeField] W_ZoomType zoomType = W_ZoomType.Simple;
    	public W_Type weaponType = W_Type.Pistol;
        public MovementController mc = null;
        public InvWeaponManager wm = null;
        public WeaponEvents events = null;
        #endregion

        #region Base Classes
        [Space(10)]
        public WeaponSounds weaponSounds;
        public WeaponBaseSettings baseSettings;
        public WeaponEffects weaponEffects;
        public WeaponAnimationSettings animationSettings;
        public WeaponPositioning overridePosition;
        public WeaponParticleEffects[] particleEffects;
        public WeaponAimerVisuals aimerVisuals;
        public WeaponUI weaponUI;
        public int id = 9999999;
        [Space(10)]
        public WeaponDebugging debugging;
        #endregion

        #region Internal Use Only
        #region Animation Settings
        private Vector3 def; //for weapon sway
        private Vector3 bob_timer = Vector3.zero;
        private Vector3 bobbingSpeed = new Vector3(0,0.36f,0);
        #endregion

        #region Debugging
        private float shot_timer = 0;

    	//for base weapon settings
    	private Vector2 hit_point;

    	//For original values
    	private bool prev_aim = false;
    	private float original_zoom;
    	private float original_recoil;
    	private float aim_zoom;
    	private float aim_recoil;
        #endregion

        #region Lerps
    	[Space(10)]
    	//-----For Lerps-----//
    	//For Aiming
    	private float distCovered;								//for aim movement positioning
    	private float fracJourney;								//for aim positioning
    	private float targetZoom;
    	private float orgBlurAmt;
    	//For Recoil
    	private float targetRecoil;
    	private bool recoil_backward = false;
    	private bool initial_equip = true;
        //For Running
        private bool sprinting = false;
        private bool lerpLeft = false;
        private Vector3 diff = Vector3.zero;
    	//------------------//
        #endregion

        #region Visuals
    	//for visuals
    	private bool show_snip_texture;
    	private int tracer_count = 0;
    	private int tracer_shot = 0;
    	private float inputX = 0f;
    	private float inputY = 0f;
    	private float aim_pos_horz = 0;
    	private float aim_pos_vert = 0;
        #endregion

        private bool reloading = false;
    	private bool can_reload = true;
    	private bool can_aim = true;
        private bool canUseWeapon = true;
        private bool weaponActive = false;
        GUIManager gui_manager = null;

        #endregion
    	
        #region Initialization
        void Start() {
            gui_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GUIManager>();
            mc = (mc == null) ? this.transform.root.GetComponent<MovementController>() : mc;
            overridePosition.hipPosition = (overridePosition.hipPosition == Vector3.zero) ? transform.localPosition : overridePosition.hipPosition;
    		
            if (weaponSounds.weaponSoundSource == null)
                weaponSounds.weaponSoundSource = this.GetComponent<AudioSource> ();
            if (animationSettings.anim == null)
                animationSettings.anim = this.GetComponent<Animator> ();
    	}
        #endregion

        #region Actions
        void Update () {
            if (debugging.reloadPosition == true)
                reloading = true;
            if (canUseWeapon == false)
                return;
            inputX = InputManager.GetAxis ("Horizontal");
            inputY = InputManager.GetAxis("Vertical");
            sprinting = InputManager.GetButton("Run");

            SetAimerPosition (inputX, inputY);
            SetMovement(sprinting,inputX, inputY);

            shot_timer = (shot_timer > 0) ? shot_timer - Time.deltaTime : 0;
            if (InputManager.GetButtonDown ("Reload")) {
                Reload ();
            }
            if (InputManager.GetButton ("Attack") && shot_timer == 0 && fireMode == W_FireMode.Auto) {
                FireShot ();
            } else if (InputManager.GetButtonDown ("Attack") && shot_timer <= 0 && baseSettings.fireWeaponOnDownPress == true) {
                FireShot ();
            } else if (InputManager.GetButtonUp ("Attack") && shot_timer <= 0 && baseSettings.fireWeaponOnDownPress == false) {
                FireShot ();
            }
            if (can_aim == true && InputManager.GetButton("Block"))
            {
                mc.SetForceWalk(true);
                debugging.aiming = true;
            }
            else if (baseSettings.walkWhenAiming == true)
            {
                mc.SetForceWalk(false);
            }
            if (InputManager.GetButtonUp ("Block")) debugging.aiming = false;
            if (prev_aim != debugging.aiming) {
                SetZoomValues ();
                prev_aim = debugging.aiming;
            }
            Aim();
            SetWeaponPosition();
            Zoom (debugging.aiming);
            Recoil ();
        }

        #endregion

        #region Animation
        void SetMovement(bool sprinting, float x, float y)
        {
            animationSettings.anim.SetBool("sprinting",sprinting);
            animationSettings.anim.SetFloat("direction", x);
            animationSettings.anim.SetFloat("speed", y);
        }
        #endregion

        #region Enable/Disables
        public void EnableWeaponUse(bool state)
        {
            canUseWeapon = state;
        }
    	// When Weapon Is Equiped
    	void OnEnable () {
            weaponActive = true;
            reloading = false;
            can_reload = true;
            can_aim = true;
            original_zoom = weaponEffects.zoomEffect.fieldOfView;
            original_recoil = weaponEffects.recoilEffect.fieldOfView;
            if(weaponEffects.muzzleLight)
                weaponEffects.muzzleLight.enabled = false;
            tracer_shot = UnityEngine.Random.Range ((int)weaponEffects.fireTracerBetweenXShots.x, (int)weaponEffects.fireTracerBetweenXShots.y+1);
    		PlayEquipSound ();
            SetUIVisuals(true);
    		if (this.GetComponent<Grenade> ()) {
    			this.GetComponent<Grenade> ().enabled = true;
    		}
            if (animationSettings.anim)
            {
                animationSettings.anim.SetBool("grounded",true);
            }
            EnableWeaponUse(true);
    	}

        void SetUIVisuals(bool enable) {
            if (weaponUI.displayUI == false)
            {
                return;
            }
            if (gui_manager == null)
                gui_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GUIManager>();
            if (gui_manager.gui_ammo.background)
                gui_manager.gui_ammo.background.enabled = enable;

            if (gui_manager.gui_ammo.bullet)
            {
                gui_manager.gui_ammo.bullet.enabled = (weaponUI.bulletImage == null || weaponUI.clipImage == null) ? false : enable;
                gui_manager.gui_ammo.bullet.sprite = weaponUI.bulletImage;
                gui_manager.gui_ammo.bullet.gameObject.GetComponent<RectTransform>().sizeDelta = weaponUI.UIBulletImageSize;
            }
            if (gui_manager.gui_ammo.clip)
            {
                gui_manager.gui_ammo.clip.enabled = enable;
                gui_manager.gui_ammo.clip.sprite = weaponUI.clipImage;
                gui_manager.gui_ammo.clip.GetComponent<RectTransform>().sizeDelta = weaponUI.UIClipImageSize;
            }
            if (gui_manager.gui_ammo.bullets_left)
            {
                gui_manager.gui_ammo.bullets_left.enabled = enable;
                gui_manager.gui_ammo.bullets_left.text = baseSettings.loaded_ammo.ToString();
            }
            if (gui_manager.gui_ammo.clip_number)
            {
                gui_manager.gui_ammo.clip_number.enabled = enable;
                gui_manager.gui_ammo.clip_number.text = baseSettings.bullets_left.ToString();
            }
    	}
    	
        //When weapon is dropped or put away
    	void OnDisable() {
            SetUIVisuals(false);
            weaponActive = false;
            weaponEffects.zoomEffect.fieldOfView = original_zoom;
            debugging.aiming = false;
    		show_snip_texture = false;
    		PlayUnequipSound ();
    		if (this.GetComponent<Grenade> ()) {
    			this.GetComponent<Grenade> ().enabled = false;
    		}
            EnableWeaponUse(false);
    	}             
        #endregion

        #region Weapon Positioning
        void SetWeaponPosition()
        {
            if (reloading == true && overridePosition.moveForReload == true)
            {
                MoveToReload();
            }
            else if (debugging.aiming == true)
            {
                MoveToAim();
            }
            else if (sprinting == true && ((inputX > 0.1 || inputX < -0.1) || (inputY > 0.1 || inputY < -0.1)) && 
                !InputManager.GetButton ("Attack"))
            {
                MoveToSprintPositions();
            }
            else
            {
                MoveToHip();
            }
        }
        void MoveToAim()
        {
            Vector3 swayPos; 
            Quaternion swayRot;
            SetSways(out swayPos, out swayRot, true);
            if (animationSettings.aimBob == true)
                swayPos = swayPos + GetWeaponBobPoint();
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition,swayPos, ref overridePosition.aimMoveVelocity,overridePosition.aimMoveSpeed);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation,swayRot,overridePosition.aimRotationSpeed * Time.deltaTime);
        }
        void MoveToHip()
        {
            Vector3 swayPos; 
            Quaternion swayRot; 
            SetSways(out swayPos,out swayRot, false);
            if (animationSettings.hipBob == true)
                swayPos = swayPos + GetWeaponBobPoint();
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition,swayPos, ref overridePosition.hipMoveVelocity,overridePosition.hipMoveSpeed);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation,swayRot,overridePosition.hipRotationSpeed * Time.deltaTime);
        }
        void MoveToSprintPositions()
        {
            Vector3 swayPos; 
            Quaternion swayRot; 
            SetSways(out swayPos,out swayRot, false);
            if (animationSettings.hipBob == true)
                swayPos = swayPos + GetWeaponBobPoint();
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition,swayPos, ref overridePosition.hipMoveVelocity,animationSettings.posSpeed);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation,swayRot,animationSettings.rotSpeed * Time.deltaTime);
            diff = transform.localPosition - swayPos;
            if (diff.sqrMagnitude < animationSettings.errorRange)
            {
                lerpLeft = !lerpLeft;
            }
        }
        void MoveToReload()
        {
            Vector3 swayPos; 
            Quaternion swayRot;
            SetSways(out swayPos,out swayRot, false);
            if (animationSettings.hipBob == true)
                swayPos = swayPos + GetWeaponBobPoint();
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition,swayPos, ref overridePosition.reloadMoveVelocity,overridePosition.reloadMoveSpeed);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation,swayRot,overridePosition.reloadRotationSpeed * Time.deltaTime);
        }
        void SetSways(out Vector3 swayPos, out Quaternion swayRot, bool aiming)
        {
            if (reloading == true)
            {
                swayPos = GetWeaponSwayPosition(false) + overridePosition.reloadPosition;
                swayRot = GetWeaponSwayRotation(false) * Quaternion.Euler(overridePosition.reloadRotation);
                return;
            }
            else if (sprinting == true && ((inputX > 0.1 || inputX < -0.1) || (inputY > 0.1 || inputY < -0.1)) && 
                aiming == false && !InputManager.GetButton ("Attack") && animationSettings.disableOverrides == false)
            {
                if (lerpLeft)
                {
                    swayPos = GetWeaponSwayPosition(false) + animationSettings.leftPos;
                    swayRot = GetWeaponSwayRotation(false) * Quaternion.Euler(animationSettings.leftRot);
                }
                else
                {
                    swayPos = animationSettings.rightPos;
                    swayRot = Quaternion.Euler(animationSettings.rightRot);
                }
            }
            else if (animationSettings.hipSway == true && aiming == false)
            {
                swayPos = GetWeaponSwayPosition(aiming) + overridePosition.hipPosition;
                swayRot = GetWeaponSwayRotation(aiming) * Quaternion.Euler(overridePosition.hipRotation);
            }
            else if (animationSettings.aimSway == true && aiming == true)
            {
                swayPos = GetWeaponSwayPosition(aiming) + overridePosition.aimPosition;
                swayRot = GetWeaponSwayRotation(aiming) * Quaternion.Euler(overridePosition.aimRotation);
            }
            else if (aiming == true)
            {
                swayPos = overridePosition.aimPosition;
                swayRot = Quaternion.Euler(overridePosition.aimRotation);
            }
            else
            {
                swayPos = overridePosition.hipPosition;
                swayRot = Quaternion.Euler(overridePosition.hipRotation);
            }
        }
        Vector3 GetWeaponSwayPosition(bool aiming)
        {
            float rotationX = -InputManager.GetAxis("Mouse X") * 0.02f;
            float rotationY = -InputManager.GetAxis("Mouse Y") * 0.02f;
            if (aiming == true)
            {
                rotationX = (rotationX > animationSettings.aimswayMaxAmount) ? animationSettings.aimswayMaxAmount : rotationX;
                rotationX = (rotationX < -animationSettings.aimswayMaxAmount) ? -animationSettings.aimswayMaxAmount : rotationX;
                rotationY = (rotationY > animationSettings.aimswayMaxAmount) ? animationSettings.aimswayMaxAmount : rotationY;
                rotationY = (rotationY < -animationSettings.aimswayMaxAmount) ? -animationSettings.aimswayMaxAmount : rotationY;
            }
            else
            {
                rotationX = (rotationX > animationSettings.hipswayMaxAmount) ? animationSettings.hipswayMaxAmount : rotationX;
                rotationX = (rotationX < -animationSettings.hipswayMaxAmount) ? -animationSettings.hipswayMaxAmount : rotationX;
                rotationY = (rotationY > animationSettings.hipswayMaxAmount) ? animationSettings.hipswayMaxAmount : rotationY;
                rotationY = (rotationY < -animationSettings.hipswayMaxAmount) ? -animationSettings.hipswayMaxAmount : rotationY;
            }

            return new Vector3(def.x + rotationX, def.y + rotationY, def.z);
        }
        Quaternion GetWeaponSwayRotation(bool aiming)
        {
            float tiltZ = 0.0f;
            float tiltX = 0.0f;
            Quaternion target;
            if (aiming == true)
            {
                tiltZ = InputManager.GetAxis("Mouse X") * animationSettings.aimswayTiltAngle;
                tiltX = InputManager.GetAxis("Mouse Y") * animationSettings.aimswayTiltAngle;
                target = Quaternion.Euler(tiltX, 0, tiltZ);
            }
            else
            {
                tiltZ = InputManager.GetAxis("Mouse X") * animationSettings.hipswayTiltAngle;
                tiltX = InputManager.GetAxis("Mouse Y") * animationSettings.hipswayTiltAngle;
                target = Quaternion.Euler(tiltX, 0, tiltZ);
            }
            return target;
        }
        Vector3 GetWeaponBobPoint()
        {
            float wavesliceX = 0.0f;
            float wavesliceY = 0.0f;
            float wavesliceZ = 0.0f;
            float horizontal = InputManager.GetAxis("Horizontal");
            float vertical = InputManager.GetAxis("Vertical");
            bool running = (baseSettings.walkWhenAiming == true) ? false : InputManager.GetButton ("Run");
            if (running == true)
            {
                bobbingSpeed = animationSettings.runBobSpeed;
            }
            else if (horizontal != 0 || vertical != 0)
            {
                bobbingSpeed = animationSettings.walkBobSpeed;
            }
            else 
            {
                bobbingSpeed = animationSettings.idleBobSpeed;
            }
            Vector3 cSharpConversion = Vector3.zero;//transform.localPosition; 

            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0 
                && animationSettings.idleBobSpeed == Vector3.zero) {
                bob_timer = Vector3.zero;
            }
            else {
                wavesliceX = Mathf.Sin(bob_timer.x);
                wavesliceY = Mathf.Sin(bob_timer.y);
                wavesliceZ = Mathf.Sin(bob_timer.z);
                bob_timer.x += bobbingSpeed.x;
                bob_timer.y += bobbingSpeed.y;
                bob_timer.x += bobbingSpeed.z;
                if (bob_timer.x > Mathf.PI * 2) {
                    bob_timer.x -= (Mathf.PI * 2);
                }
                if (bob_timer.y > Mathf.PI * 2) {
                    bob_timer.y -= (Mathf.PI * 2);
                }
                if (bob_timer.z > Mathf.PI * 2) {
                    bob_timer.z -= (Mathf.PI * 2);
                }
            }
            cSharpConversion.x = GetWaveSlice(wavesliceX, horizontal, vertical, running);
            cSharpConversion.y = GetWaveSlice(wavesliceY, horizontal, vertical, running);
            cSharpConversion.z = GetWaveSlice(wavesliceZ, horizontal, vertical, running);
            return cSharpConversion;
        }
        float GetWaveSlice(float waveslice, float horizontal, float vertical, bool running)
        {
            float bobAmount = (running == true) ? animationSettings.runBobAmount : animationSettings.walkBobAmount;
            if (waveslice != 0) {
                float translateChange = waveslice * bobAmount;
                float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;
                return animationSettings.bobMidpoint + translateChange;
            }
            return animationSettings.bobMidpoint;
        }
        #endregion

        #region Logic
    	void Aim() {
            animationSettings.anim.SetBool(animationSettings.aimBool, debugging.aiming);
            if (zoomType != W_ZoomType.Sniper)
                return;
            if (debugging.aiming == true && Vector3.Distance(transform.localPosition, overridePosition.aimPosition) < 0.1f)
            {
                SetSniperTexture(true);
            }
            else 
            {
                SetSniperTexture(false);
            }
        }
        void Zoom(bool isAiming) {
    		if (targetZoom == 0) {
    			return;
    		}
            if (isAiming == true && weaponEffects.zoomEffect.fieldOfView != targetZoom) {
                weaponEffects.zoomEffect.fieldOfView = (weaponEffects.zoomEffect.fieldOfView > targetZoom) ? weaponEffects.zoomEffect.fieldOfView - (Time.deltaTime * weaponEffects.zoomSpeed) : targetZoom;
    		} 
    		else if (isAiming == false) {
                weaponEffects.zoomEffect.fieldOfView = (weaponEffects.zoomEffect.fieldOfView < targetZoom) ? weaponEffects.zoomEffect.fieldOfView + (Time.deltaTime * weaponEffects.zoomSpeed) : targetZoom;
    		}
    	}
    	void SetSniperTexture(bool enabled) {
    		if (enabled == true) {
    			show_snip_texture = true;
                foreach (GameObject obj in aimerVisuals.disableWhenAiming) {
    				obj.SetActive (false);
    			}
    		} else {
    			show_snip_texture = false;
                foreach (GameObject obj in aimerVisuals.disableWhenAiming) {
    				obj.SetActive (true);
    			}
    		}
    	}        		
    	void Recoil() {
    		if (initial_equip == true)
    			return;
    		if (recoil_backward == true) {
                weaponEffects.recoilEffect.fieldOfView = (weaponEffects.recoilEffect.fieldOfView > targetRecoil) ? weaponEffects.recoilEffect.fieldOfView - (Time.deltaTime * animationSettings.recoilSpeed) : targetRecoil;
                if (weaponEffects.recoilEffect.fieldOfView == targetRecoil) {
    				SetRecoilValues (false);
    			}
    		} 
    		else if (recoil_backward == false) {
                weaponEffects.recoilEffect.fieldOfView = (weaponEffects.recoilEffect.fieldOfView < targetRecoil) ? weaponEffects.recoilEffect.fieldOfView + (Time.deltaTime * animationSettings.recoilSpeed) : targetRecoil;
    		}
    	}
        public void LoadAmmo(int amount)
        {
            if (CanReload() == false)
            {
                SetStateReload(false);
                return;
            }
            debugging.aiming = false;
            can_aim = false;
            can_reload = false;
            PlayReloadSound ();
            amount = ((baseSettings.bullets_left - amount) >= baseSettings.ammo_per_reload) ? amount : baseSettings.bullets_left;
            if ((amount + baseSettings.loaded_ammo) > baseSettings.max_ammo)
            {
                amount -= baseSettings.loaded_ammo; //prevents overloading on ammo
            }
           
            baseSettings.bullets_left -= amount;
            baseSettings.loaded_ammo += amount;
            StartCoroutine (UpdateReloadUI ());
        }
        void Reload() {
            if (CanReload() == false || baseSettings.canReload == false)
            {
                return;
            }
            if (baseSettings.timedReload == true)
            {
                LoadAmmo(baseSettings.ammo_per_reload);
                shot_timer = baseSettings.reloadTime;
                StartCoroutine(SetReload());
            }
            else
            {
                SetStateReload(true);
            }
            animationSettings.anim.SetTrigger(animationSettings.reloadTrigger);
            events.OnReload.Invoke();


        }
        public void AddAmmo(int amount) {
            baseSettings.bullets_left += amount;
            if (gui_manager.gui_ammo.clip_number && weaponActive == true)
                gui_manager.gui_ammo.clip_number.text = baseSettings.bullets_left.ToString();
        }
        public bool CanReload()
        {
            if (baseSettings.bullets_left == 0 || can_reload == false || baseSettings.loaded_ammo >= baseSettings.max_ammo)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Effects On Fire
    	//Effects On fire
    	void FireShot() {
    		if (shot_timer != 0) return;
            if (reloading == true)
                return;
            shot_timer = baseSettings.waitBetweenShots;
            if (baseSettings.loaded_ammo == 0 && baseSettings.infiniteAmmo == false) {
    			PlayEmptySound ();
    			return;
    		}
            if (baseSettings.infiniteAmmo == false)
            {
                baseSettings.loaded_ammo -= (baseSettings.loaded_ammo > 0) ? 1 : 0;
//                gui_manager.gui_ammo.bullets_left.text = baseSettings.loaded_ammo.ToString();
            }
            events.OnFire.Invoke();
            SetUIVisuals(true);
            StartCoroutine(PlayWeaponShotAnim());
            SetRecoilValues(true);
    		StartCoroutine(MuzzleFlash ());
    		Tracer ();
    		PlayFireSound ();
            if (weaponEffects.ejectPoint == null || weaponEffects.ejectShell == null)
                WeaponHelpers.EjectShell(weaponEffects.ejectShell,weaponEffects.ejectPoint,weaponEffects.ejectDirection);
            AddAimerPosition (aimerVisuals.increaseAimerEveryShot);
            for (int i = 0; i < baseSettings.raysPerShot; i++) {
    			RaycastDamage ();
    		}
    		ThrowGrenade ();
            if (baseSettings.auto_reload == true && baseSettings.loaded_ammo < 1 && can_reload == true) {
    			Reload ();
    		}
    	}        		
    	void ThrowGrenade() {
            if (!baseSettings.grenadeScript)
    			return;
            baseSettings.grenadeScript.ThrowObject ();
    	}
        IEnumerator SetReload()
        {
            SetStateReload(true);
            yield return new WaitForSeconds(baseSettings.reloadTime);
            if (baseSettings.timedReload == true)
            {
                SetStateReload(false);
            }
            else
            {
                yield return null;
            }
        }
        public void SetStateReload(bool state)
        {
            reloading = state;
            animationSettings.anim.SetBool(animationSettings.reloadBool,state);
            if (state == false)
            {
                events.FinishedReloading.Invoke();
            }
        }
        public bool IsFull()
        {
            return baseSettings.loaded_ammo >= baseSettings.max_ammo;
        }
        IEnumerator UpdateReloadUI() {
            yield return new WaitForSeconds (baseSettings.reloadTime-0.1f);
            if (weaponActive == true)
            {
                SetUIVisuals(true);
//                gui_manager.gui_ammo.bullets_left.text = baseSettings.loaded_ammo.ToString();
//                gui_manager.gui_ammo.clip_number.text = baseSettings.bullets_left.ToString();
            }
    		can_reload = true;
    		can_aim = true;
    	}
    	void RaycastDamage() {
            RaycastHit hit;
            Vector3 hitpoint = GetHitPoint(debugging.aiming);
            Ray ray = weaponEffects.zoomEffect.ScreenPointToRay(hitpoint);

            if (debugging.debug_raycast == true)
                Debug.DrawRay (ray.origin, ray.direction * baseSettings.shotDistance, new Color (1f, 0.922f, 0.016f, 1f),5.0f);  
            if (Physics.Raycast(ray.origin, ray.direction, out hit, baseSettings.shotDistance, ~baseSettings.ignoreLayersOnShot)) {
                if (debugging.debug_raycast == true) {
                    Debug.Log ("NAME: "+hit.transform.name+", DISTANCE: "+hit.distance+", TAG: "+hit.transform.tag+", POINT: "+hit.point+", Layer:"+LayerMask.LayerToName(hit.transform.gameObject.layer));
    			}
    			SpawnParticle (hit);
                SpawnDecal (hit);
    			if (hit.transform.root.GetComponent<Health> ()) {
                    hit.transform.root.GetComponent<Health> ().ApplyDamage (baseSettings.damagePerShot, this.transform.root.gameObject);
    			} 
    		}
    	}
    	Vector3 GetHitPoint(bool isAiming) {
    		Vector3 hitpoint;
    		float x;
    		float y;
    		if (isAiming == false) {
                x = UnityEngine.Random.Range (Screen.width/2 - aim_pos_horz - baseSettings.addedHipInaccuracy, Screen.width/2 + aim_pos_horz + baseSettings.addedHipInaccuracy);
                y = UnityEngine.Random.Range (Screen.height/2 - aim_pos_vert - baseSettings.addedHipInaccuracy, Screen.height/2 + aim_pos_vert + baseSettings.addedHipInaccuracy);
                hitpoint = new Vector3(x,y,0);
                debugging.hitpoint = hitpoint;
            } 
            else 
            {
//                x = (baseSettings.shotPoint == null) ? Screen.width/2 : weaponEffects.zoomEffect.WorldToViewportPoint(baseSettings.shotPoint.position).x;
//                y = (baseSettings.shotPoint == null) ? Screen.height/2 : weaponEffects.zoomEffect.WorldToViewportPoint(baseSettings.shotPoint.position).y;
//                hitpoint = weaponEffects.recoilEffect.ViewportToWorldPoint(baseSettings.shotPoint.position);
//                hitpoint = weaponEffects.recoilEffect.ViewportToScreenPoint(hitpoint);
//                hitpoint = weaponEffects.zoomEffect.ScreenToWorldPoint(hitpoint);
       
                hitpoint.x = (Screen.width / 2f) + overridePosition.shot_offset.x;
                hitpoint.y = (Screen.height / 2f) + overridePosition.shot_offset.y;
                hitpoint.z = 0;
                if (baseSettings.aimInaccuracy > 0)
                {
                    hitpoint.x = UnityEngine.Random.Range(hitpoint.x - baseSettings.aimInaccuracy, hitpoint.x + baseSettings.aimInaccuracy);
                    hitpoint.y = UnityEngine.Random.Range(hitpoint.y - baseSettings.aimInaccuracy, hitpoint.y + baseSettings.aimInaccuracy);
                }
                debugging.hitpoint = GUIUtility.ScreenToGUIPoint( new Vector2(hitpoint.x,hitpoint.y));
            }
    		return hitpoint;
    	}
    	ParticleSystem GetParticle(string tag) {
    		ParticleSystem particle = null;
            foreach (WeaponParticleEffects effects in particleEffects)
            {
                if (System.Array.IndexOf(effects.tags, tag) != -1)
                {
                    if (effects.particles.Length > 0) {
                        particle = effects.particles[UnityEngine.Random.Range(0, effects.particles.Length - 1)];
                    }
                    break;
                }
            }

    		return particle;
    	}
    	IEnumerator MuzzleFlash() {
            if (!weaponEffects.muzzleFlash)
    			yield break;
            weaponEffects.muzzleFlash.Play ();
            if (weaponEffects.muzzleLight == null)
    			yield break;
            weaponEffects.muzzleLight.enabled = true;
    		yield return new WaitForSeconds (0.05f);
            weaponEffects.muzzleLight.enabled = false;
    	}
    	void Tracer() {
            if (!weaponEffects.tracer)
    			return;
    		tracer_count += 1;
    		if (tracer_count >= tracer_shot) {
                weaponEffects.tracer.Play ();
    			tracer_count = 0;
                tracer_shot = UnityEngine.Random.Range ((int)weaponEffects.fireTracerBetweenXShots.x, (int)weaponEffects.fireTracerBetweenXShots.y+1);
    		}
    	}
        void SpawnDecal(RaycastHit hit) {
            foreach (WeaponParticleEffects effects in particleEffects)
            {
                if (System.Array.IndexOf(effects.tags, hit.transform.tag) != -1)
                {
                    if (effects.decals.Length > 0) {
                        GameObject bullet_hit;
                        int decal_int = UnityEngine.Random.Range(0, effects.decals.Length - 1);
                        if (effects.decals[decal_int].invert == true)
                        {
                            bullet_hit = Instantiate (effects.decals[UnityEngine.Random.Range(0, effects.decals.Length - 1)].bulletDecal, hit.point, Quaternion.LookRotation (-hit.normal)) as GameObject;                            
                        }
                        else
                        {
                            bullet_hit = Instantiate (effects.decals[UnityEngine.Random.Range(0, effects.decals.Length - 1)].bulletDecal, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
                        }
                        Destroy(bullet_hit, 20.0f);
                    }
                    break;
                }
            }
    	}
    	void SpawnParticle(RaycastHit hit) {
            ParticleSystem particle = GetParticle (hit.transform.tag);
    		if (particle == null)
    			return;
    		ParticleSystem spawned = Instantiate (particle, hit.point, Quaternion.LookRotation(hit.normal)) as ParticleSystem;
    		spawned.Play ();
    		StartCoroutine (WeaponHelpers.DestroyGameObject (spawned.transform.gameObject, 2.0f));
    	}
        IEnumerator PlayWeaponShotAnim()
        {
            if (animationSettings.anim && !string.IsNullOrEmpty(animationSettings.attackTrigger))
            {
                animationSettings.anim.SetTrigger(animationSettings.attackTrigger);
            }
            if (animationSettings.cameraEffects)
            {
                animationSettings.cameraEffects.Play(animationSettings.shotEffect);
                yield return new WaitForSeconds(animationSettings.cameraEffects.clip.length);
                animationSettings.cameraEffects.Play(animationSettings.calmEffect);
            }

            yield return null;
        }
        #endregion

        #region Sounds
    	void PlayFireSound() {
            if (weaponSounds.fire.Length <= 0)
    			return;
            weaponSounds.weaponSoundSource.clip = weaponSounds.fire [UnityEngine.Random.Range (0, weaponSounds.fire.Length)];
            weaponSounds.weaponSoundSource.volume = weaponSounds.fire_volume;
            weaponSounds.weaponSoundSource.Play ();
    	}
    	public void PlayEquipSound() {
            if (weaponSounds.equip.Length <= 0)
    			return;
            weaponSounds.weaponSoundSource.clip = weaponSounds.equip [UnityEngine.Random.Range (0, weaponSounds.equip.Length)];
            weaponSounds.weaponSoundSource.volume = weaponSounds.equip_volume;
            weaponSounds.weaponSoundSource.Play ();
    	}
    	void PlayUnequipSound() {
            if (weaponSounds.unequip.Length <= 0)
    			return;
            weaponSounds.weaponSoundSource.clip = weaponSounds.unequip [UnityEngine.Random.Range (0, weaponSounds.unequip.Length)];
            weaponSounds.weaponSoundSource.volume = weaponSounds.unequip_volume;
            weaponSounds.weaponSoundSource.Play ();
    	}
    	void PlayEmptySound() {
            if (weaponSounds.empty.Length <= 0)
    			return;
            weaponSounds.weaponSoundSource.clip = weaponSounds.empty [UnityEngine.Random.Range (0, weaponSounds.empty.Length)];
            weaponSounds.weaponSoundSource.volume = weaponSounds.empty_volume;
            weaponSounds.weaponSoundSource.Play ();
    	}
    	void PlayReloadSound() {
            if (weaponSounds.reload.Length <= 0)
    			return;
            weaponSounds.weaponSoundSource.clip = weaponSounds.reload [UnityEngine.Random.Range (0, weaponSounds.reload.Length)];
            weaponSounds.weaponSoundSource.volume = weaponSounds.reload_volume;
            weaponSounds.weaponSoundSource.Play ();
    	}
        #endregion

        #region Set Values
    	//Set Values
    	void SetZoomValues() {
            aim_zoom = original_zoom - weaponEffects.aimZoom;
            targetZoom = (debugging.aiming == true) ? aim_zoom : original_zoom;
    	}
    	void SetRecoilValues(bool perform_recoil) {
    		recoil_backward = perform_recoil;
            aim_recoil = original_recoil - baseSettings.recoilAmount;
    		targetRecoil = (perform_recoil == true) ? aim_recoil : original_recoil;
    		initial_equip = false;
    	}
    	void SetAimerPosition(float x, float y) {
    		if (x > 0 || y > 0 || x < 0 || y < 0) {
                aim_pos_horz = (aim_pos_horz < aimerVisuals.aimerEdge.x) ? aim_pos_horz + (Time.deltaTime * aimerVisuals.aimerMoveOutSpeed) : aimerVisuals.aimerEdge.x;
                aim_pos_vert = (aim_pos_vert < aimerVisuals.aimerEdge.x) ? aim_pos_vert + (Time.deltaTime * aimerVisuals.aimerMoveOutSpeed) : aimerVisuals.aimerEdge.x;
    		} 
    		else {
                aim_pos_horz = (aim_pos_horz > aimerVisuals.aimerCenter.x) ? aim_pos_horz - (Time.deltaTime * aimerVisuals.aimerMoveInSpeed) : aimerVisuals.aimerCenter.x;
                aim_pos_vert = (aim_pos_vert > aimerVisuals.aimerCenter.x) ? aim_pos_vert - (Time.deltaTime * aimerVisuals.aimerMoveInSpeed) : aimerVisuals.aimerCenter.x;
    		}
    	}
    	void AddAimerPosition(float amount) {
            if (aim_pos_horz < aimerVisuals.aimerEdge.x || aim_pos_vert < aimerVisuals.aimerEdge.y) {
    			aim_pos_horz += amount;
    			aim_pos_vert += amount;
    		} else {
                aim_pos_horz = aimerVisuals.aimerEdge.x;
                aim_pos_vert = aimerVisuals.aimerEdge.y;
    		}
    	}
    	public AudioClip[] GetEquipSounds() {
            return weaponSounds.equip;
    	}
        #endregion
  		
        #region GUI Visuals
        void DrawColoredBox(Rect position, Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            GUI.Box(position, GUIContent.none);
        }
    	void OnGUI() {
            if (debugging.show_screen_hitpoint == true)
            {
                DrawColoredBox(new Rect(debugging.hitpoint.x, debugging.hitpoint.y, 5, 5), Color.white);
            }
            if (canUseWeapon == false)
                return;
    		if (zoomType == W_ZoomType.Sniper && show_snip_texture == true) {
                GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), weaponEffects.sniperAim);
    		} else {
                if (aimerVisuals.hideAimerWhenAiming == true && debugging.aiming == true) {
    				return;
    			} else {
                    if (aimerVisuals.aimerHorizontal != null) {
                        GUI.DrawTexture (new Rect ((Screen.width / 2) - aim_pos_horz, Screen.height / 2, aimerVisuals.aimerHorizontalSize.x, aimerVisuals.aimerHorizontalSize.y), aimerVisuals.aimerHorizontal);
                        GUI.DrawTexture (new Rect ((Screen.width / 2) + aim_pos_horz, Screen.height / 2, aimerVisuals.aimerHorizontalSize.x, aimerVisuals.aimerHorizontalSize.y), aimerVisuals.aimerHorizontal);
    				}
                    if (aimerVisuals.aimerVertical != null) {
                        GUI.DrawTexture (new Rect (Screen.width / 2, (Screen.height / 2) - aim_pos_vert, aimerVisuals.aimerVerticalSize.x, aimerVisuals.aimerVerticalSize.y), aimerVisuals.aimerVertical);
                        GUI.DrawTexture (new Rect (Screen.width / 2, (Screen.height / 2) + aim_pos_vert, aimerVisuals.aimerVerticalSize.x, aimerVisuals.aimerVerticalSize.y), aimerVisuals.aimerVertical);
    				}
    			}
    		}
    	}
        #endregion
    }
}