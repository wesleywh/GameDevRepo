using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using System;

namespace CyberBullet {
    namespace Controllers {
        [Serializable]
        public class SoundOptions {
            [Range(0,1)]
            public float volume = 1.0f;
            public bool loop = false;
            public AudioClip sound = null;
        }

        public class SwimController : MonoBehaviour {

            #region Variables
            [SerializeField] public float underwater_offset = 0.25f;
            [HideInInspector] public bool swimming = false;
            private float swimSlowGravity = 0.0f;
            [SerializeField] private float water_height = 0.0f;
            private bool underwater = false;
            private bool enteredWater = false;

            #region Movement
            [Header("Movement")]
            [SerializeField] private float swimSpeed = 0.1f;
            [SerializeField] private float swimFastSpeed = 0.2f;
            [SerializeField] private string swimTowardCamLook = "PlayerCamera";
            private Vector3 moveDir;
            private Camera targetCam = null;
            public ClimbController climbController;
            public MovementController moveController;
            public InvWeaponManager weaponManager;
            private Vector3 lastPosition;
            private bool isMoving = false;
            #endregion
            #region Visual
            [Header("Visuals")]
            public float fogAmount = 0.04f;
            public GameObject enterWaterSplash;
            public GameObject underwaterBubbles;
            public GameObject topOfWaterDisturbance;
            public RainCameraController enterPool;
            public RainCameraController exitPool;
            public UnityEngine.Color underwaterColor = new Color (0.0f, 0.4f, 0.7f, 0.6f);

            private bool defaultFog;
            private Color defaultFogColor;
            private float defaultFogDensity;
            private Material defaultSkybox;
            private GameObject water_foam = null;
            #endregion
            #region Animation
            [Header("Animation")]
            public Animator[] anims;
            public float slowSpeed = 0.5f;
            public float fastSpeed = 1.0f;
            public string swimClipName = "swim";
            public string orgClipName = "camera_sway";
            private bool clipPlaying = false;
            public Animation camAnimHolder;
            public HeadBobber headBobScript;
            #endregion
            #region Sounds
            [Header("Sounds")]
            public float noMoveSoundDelay = 3f;
            public float moveSlowSoundDelay = 1f;
            public float moveFastSoundDelay = 0.5f;
            private float timer = 1000000.0f;
            public SoundOptions[] aboveWaterMovement;
            public SoundOptions[] underWaterMovement;
            public SoundOptions[] enterWaterSounds;
            public SoundOptions[] exitWaterSounds;
            public SoundOptions[] enterUnderWaterSounds;
            public SoundOptions[] exitUnderWaterSounds;
            public AudioSource soundSource = null;
            public AudioSource moveSource = null;
            private bool queued = false;
            #endregion
            #endregion

            #region Universal
            void Start () {
                lastPosition = gameObject.transform.position;
                defaultFog = RenderSettings.fog;
                defaultFogColor = RenderSettings.fogColor;
                defaultFogDensity = RenderSettings.fogDensity;
                defaultSkybox = RenderSettings.skybox;
                if (soundSource == null)
                    soundSource = GetComponent<AudioSource>();
                targetCam = GameObject.FindGameObjectWithTag(swimTowardCamLook).GetComponent<Camera>();
                climbController = (climbController == null) ? GetComponent<ClimbController>() : climbController;
                moveController = (moveController == null) ? GetComponent<MovementController>() : moveController;
                weaponManager = (weaponManager == null) ? GetComponentInChildren<InvWeaponManager>() : weaponManager;
        	}
        	void Update () {
                timer += Time.deltaTime;
                ClimbCheck();
                ApplySwimMovement(InputManager.GetAxis("Horizontal"), InputManager.GetAxis("Vertical"));
                CheckIfMoving();
                CheckUnderWaterVisuals();
                CheckTopWaterVisuals();
                PlaySwimMoveSounds();
                PlayAnim();
                isMoving = false;
        	}     
            public void SetSwimming(bool value, float ySpeed = 0.0f)
            {
                swimming = value;
                if (swimming == true)
                {
                    timer = 1000;
                    weaponManager.SelectWeapon(0);
                    enteredWater = false;
                    water_height = this.transform.position.y;
                    swimSlowGravity = ySpeed/18;
                    PlayRandomSound(enterWaterSounds);
                }
                else
                {
                    queued = false;
                    underwater = false;
                    enteredWater = false;
                    PlayRandomSound(exitWaterSounds);
                }
                SetAnimState("swimming", swimming);
                moveController.enabled = !swimming;
                weaponManager.CanEquipWeapons(!swimming);
                headBobScript.enabled = !swimming;
                this.enabled = swimming;
            }
            #endregion

            #region Visuals
            void CheckTopWaterVisuals()
            {
                if (swimming == true){
                    if (topOfWaterDisturbance && water_foam == null) {
                        Vector3 waterSpawn = new Vector3 (this.transform.position.x, water_height, this.transform.position.z);
                        water_foam = Instantiate (topOfWaterDisturbance, waterSpawn, Quaternion.identity);
                    }
                    water_foam.transform.position = new Vector3(this.transform.position.x, water_height, this.transform.position.z);
                    if(this.transform.position.y < (water_height - underwater_offset)) {
                        underwater = true;
                        PlayEnterUnderWater ();
                    } else {
                        underwater = false;
                        PlayExitUnderWater ();
                    }
                    if (underwater == true) {
                        Destroy (water_foam);
                        water_foam = null;
                    }
                }
                else if ((underwater == true  || swimming == false) && water_foam != null) {
                    Destroy (water_foam, 1.0f);
                    water_foam = null;
                }
            }
            void CheckUnderWaterVisuals()
            {
                if(this.transform.position.y < (water_height - underwater_offset)) {
                    underwater = true;
                    PlayEnterUnderWater ();
                } else {
                    underwater = false;
                    PlayExitUnderWater ();
                }
                if (underwater == true) {
                    Destroy (water_foam);
                    water_foam = null;
                }
            }
            void PlayEnterUnderWater() {
                if (enteredWater == false) {
                    enterPool.Play ();
                    enteredWater = true;
                    RenderSettings.fog = true;
                    RenderSettings.fogColor = underwaterColor;
                    RenderSettings.fogDensity = fogAmount;
                    RenderSettings.skybox = null;
                    if (!enterWaterSplash)
                        return;
                    Vector3 waterSpawn = new Vector3 (this.transform.position.x, water_height, this.transform.position.z);
                    GameObject splash = Instantiate (enterWaterSplash, waterSpawn, Quaternion.identity) as GameObject;
                    Destroy (splash, 1.0f);
                    PlayRandomSound(enterUnderWaterSounds);
                }
            }
            void PlayExitUnderWater() {
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
                    PlayRandomSound(exitUnderWaterSounds);
                }
            }
            #endregion

            #region Movement
            void CheckIfMoving() {
                if (lastPosition != gameObject.transform.position) // only attempt to play these sounds if you're moving
                {
                    lastPosition = gameObject.transform.position;
                    isMoving = true;
                }
            }
            float SwimSpeed() {
                if (InputManager.GetButton("Run"))
                    return swimFastSpeed;
                else
                    return swimSpeed;
            }
            void ApplySwimMovement(float inputX, float inputY) {
                // If air control is allowed, check movement but don't touch the y component
                if (swimming == true) {
                    moveDir = new Vector3(inputX, 0 , inputY);
                    moveDir = GameObject.FindGameObjectWithTag(swimTowardCamLook).transform.TransformDirection(moveDir);
                    moveDir *= SwimSpeed();
                    GetComponent<CharacterController>().Move(moveDir);
                    if (swimSlowGravity < 0) {
                        swimSlowGravity += Time.deltaTime;
                        this.transform.position = this.transform.position + Vector3.up * swimSlowGravity/2;
                    }
                    if (this.transform.position.y > water_height) {
                        this.transform.position = new Vector3 (this.transform.position.x, water_height, this.transform.position.z);
                    }
                } 
            }
            void ClimbCheck() {//check if something climbable is within reach distance
                if (InputManager.GetButton("Jump")) //climb it if the jump button is pressed.
                {
                    Ray ray = targetCam.ScreenPointToRay(InputManager.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, climbController.reachDistance, climbController.climbLayer))
                    {
                        SetSwimming(false);
                        climbController.InitClimbing();
                    }
                }
            }
            #endregion

            #region Animation
            void SetAnimState(string parameter, bool value)
            {
                foreach (Animator anim in anims)
                {
                    if (anim.transform.gameObject.activeSelf == false)
                        continue;
                    anim.SetBool(parameter, value);
                }
            }
            float AnimSpeed() {
                if (InputManager.GetButton("Run"))
                    return fastSpeed;
                else
                    return slowSpeed;
            }
            void PlayAnim()
            {
                if (swimming == false)
                    return;
                if (isMoving) // only attempt to play these sounds if you're moving
                {
                    StartCoroutine(PlayLegacyAnimation());
                }
            }
            IEnumerator PlayLegacyAnimation() {
                if (camAnimHolder == null || clipPlaying == true)
                    yield return null;
                camAnimHolder[swimClipName].speed = AnimSpeed();
                camAnimHolder.Play(swimClipName);
                clipPlaying = true;
                yield return new WaitWhile(() => camAnimHolder.isPlaying);
                clipPlaying = false;
            }
            #endregion

            #region Sounds
            void PlaySwimMoveSounds()
            {
                if (swimming == false)
                    return;
                if (isMoving) // only attempt to play these sounds if you're moving
                {
                    if (timer >= GetSoundDelay())
                    {
                        if (underwater == true)
                        {
                            PlayRandomSound(underWaterMovement, moveSource, true);
                        }
                        else
                        {
                            PlayRandomSound(aboveWaterMovement, moveSource, true);
                        }
                        timer = 0;
                    }
                }
                else if (timer >= noMoveSoundDelay)
                {
                    if (underwater == true)
                    {
                        PlayRandomSound(underWaterMovement, moveSource, true);
                    }
                    else
                    {
                        PlayRandomSound(aboveWaterMovement, moveSource, true);
                    }
                    timer = 0;
                }
            }
            float GetSoundDelay() {
                if (InputManager.GetButton("Run"))
                {
                    return moveFastSoundDelay;
                }
                else
                {
                    return moveSlowSoundDelay;
                }
            }
            void PlayRandomSound(SoundOptions[] inputSounds, AudioSource overrideSource = null, bool WaitToFinish = false) 
            {
                if (inputSounds.Length <= 0)
                    return;
                SoundOptions selected = inputSounds[UnityEngine.Random.Range(0, inputSounds.Length)];
                if (overrideSource != null)
                {
                    if (WaitToFinish == true)
                    {
                        StartCoroutine(WaitForClip(selected, overrideSource));
                    }
                    else
                    {
                        overrideSource.volume = selected.volume;
                        overrideSource.loop = selected.loop;
                        overrideSource.clip = selected.sound;
                        overrideSource.Play();
                    }
                }
                else
                {
                    if (WaitToFinish == true)
                    {
                        StartCoroutine(WaitForClip(selected, soundSource));
                    }
                    else
                    {
                        soundSource.volume = selected.volume;
                        soundSource.loop = selected.loop;
                        soundSource.clip = selected.sound;
                        soundSource.Play();
                    }
                }
            }
            IEnumerator WaitForClip(SoundOptions clip, AudioSource source) 
            {
                if (queued == false)
                {
                    queued = true;
                    yield return new WaitWhile(() => source.isPlaying);
                    if (swimming == true)
                    {
                        source.volume = clip.volume;
                        source.loop = clip.loop;
                        source.clip = clip.sound;
                        source.Play();
                        queued = false;
                    }
                }
            }
            #endregion
        }
    }
}
