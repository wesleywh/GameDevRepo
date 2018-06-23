using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using CyberBullet.Controllers;
using CyberBullet.Helpers;

namespace CyberBullet.Cameras {
    [System.Serializable]
    public class CamAnims {
        public Animation anim;
        public string calm = "camera_sway";
        public string sprint = "camera_run";
    }
    [System.Serializable]
    public class CamSoundOptions {
        [Range(0,1)]
        public float volume = 1.0f;
        public bool loop = false;
        public AudioClip sound = null;
    }
    [System.Serializable]
    public class CamSwimming {
        [Header("Detections")]
        public string enterWaterTag = "Water";
        public string exitWaterTag = "ExitWater";
        [Header("Animator")]
        public string underwater_bool = "underwater";
        [Header("Sounds")]
        public AudioSource source = null;
        public CamSoundOptions[] enterWaterSounds;
        public CamSoundOptions[] exitWaterSounds;
        public AudioSource loopSource = null;
        public CamSoundOptions[] underwaterLoops;
        public AudioSource swimSource = null;
        public AudioClip[] aboveWaterSwimming;
        public AudioClip[] underWaterSwimming;
        public float above_fastSoundDelay = 0.5f;
        public float above_slowSoundDelay = 1.0f;
        public float under_fastSoundDelay = 1.0f;
        public float under_slowSoundDelay = 2.0f;
        [Header("Camera Effects")]
        public RainCameraController enterPool;
        public RainCameraController exitPool;
        public UnityEngine.Color underwaterColor = new Color (0.0f, 0.4f, 0.7f, 0.6f);
        public float fogAmount = 0.04f;
        [Header("GUI")]
        public GUIStyle style = null;
        public string exitText = "Press <ACTION> to exit";
        public ButtonOptions replaceActionWith = ButtonOptions.Action;
        public Vector3 offset = new Vector3(0, 1, 0);
        public float climbOutDistance = 2f;
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class CameraEffects : MonoBehaviour {
        public MovementController mc;
        [SerializeField] private CamAnims animations;
        [SerializeField] private CamSwimming swimming;

        private bool queued = false;
        private bool defaultFog = false;
        private Color defaultFogColor;
        private float defaultFogDensity = 0.0f;
        private Material defaultSkybox;
        private float timer = 0.0f;
        private bool underwater = false;
        private Ray ray;
        private RaycastHit hit;
        private bool showExitGUI = false;

    	void Start () {
            defaultFog = RenderSettings.fog;
            defaultFogColor = RenderSettings.fogColor;
            defaultFogDensity = RenderSettings.fogDensity;
            defaultSkybox = RenderSettings.skybox;

            animations.anim = (animations.anim == null) ? GetComponent<Animation>() : animations.anim;
            mc = (mc == null) ? transform.root.GetComponent<MovementController>() : mc;
    	}
    	
        void Update()
        {
            if (mc.GetSwimState() == true)
            {
                timer += (timer > 20) ? timer : Time.deltaTime;
                if (InputManager.GetAxis("Horizontal") != 0 || InputManager.GetAxis("Vertical") != 0)
                {
                    PlaySwimMoveSounds();
                }
                ray = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.Raycast(ray, out hit, swimming.climbOutDistance))
                {
                    if (hit.transform.tag == swimming.exitWaterTag)
                    {
                        showExitGUI = true;
                    }
                }
                else
                {
                    showExitGUI = false;
                }
                if (showExitGUI == true && InputManager.GetButtonDown("Action"))
                {
                    InitExitWater();
                }
            }
        }
    	void FixedUpdate () {
            float inputX = (mc.GetLockedMovement() == false) ? InputManager.GetAxis("Horizontal") : 0;
            float inputY = (mc.GetLockedMovement() == false) ? InputManager.GetAxis("Vertical") : 0;
            if (mc.GetSwimState() == false && InputManager.GetButton("Run") && ((inputX > 0.1f || inputX < -0.1f) || (inputY > 0.1f || inputY < -0.1f)))
            {
                StartCoroutine(PlayRunAnimation());
            }
    	}

        IEnumerator PlayRunAnimation()
        {
            if (string.IsNullOrEmpty(animations.sprint) == false && mc.GetSwimState() == false && 
                animations.anim != null && animations.anim.clip.name == animations.calm)
            {
                animations.anim.Play(animations.sprint);
                yield return new WaitForSeconds(animations.sprint.Length);
                animations.anim.Play(animations.calm);
            }
            else
            {
                yield return null;
            }
        }
    
        #region Swimming
        void InitExitWater()
        {
            WaterState(false);
            Vector3 destination = hit.collider.bounds.max;
            destination.x = hit.point.x;
            destination.z = hit.point.z;
            destination.y += 0.25f;
            Vector3 start = hit.collider.bounds.min;
            start.x = hit.point.x;
            start.z = hit.point.z;
            start += hit.normal * 1.5f;
            float height = hit.collider.bounds.max.y + 0.5f;
            GameObject.FindGameObjectWithTag("CameraHolder").transform.rotation = Quaternion.LookRotation(-hit.normal);
            mc.ExitWater(start, destination - hit.normal*1.5f, height);
        }
        void OnTriggerEnter(Collider col)
        {
            if (col.tag == swimming.enterWaterTag)
            {
                WaterState(true);
            }
        }
        void OnTriggerExit(Collider col)
        {
            if (col.tag == swimming.enterWaterTag)
            {
                WaterState(false);
            }
        }
        void WaterState(bool state)
        {
            underwater = state;
            mc.SetAnimValue(swimming.underwater_bool, state);
            if (state == true)
            {
                swimming.enterPool.Play();
                PlayRandomSound(swimming.enterWaterSounds,swimming.source);
                PlayRandomSound(swimming.underwaterLoops,swimming.loopSource);
                EnableFogState(true);
            }
            else
            {
                showExitGUI = false;
                swimming.exitPool.Play();
                swimming.loopSource.Stop();
                PlayRandomSound(swimming.exitWaterSounds,swimming.source);
                EnableFogState(false);
            }
        }
        void PlayRandomSound(CamSoundOptions[] inputSounds, AudioSource overrideSource = null, bool WaitToFinish = false) 
        {
            if (inputSounds.Length < 1)
                return;
            CamSoundOptions selected = inputSounds[UnityEngine.Random.Range(0, inputSounds.Length)];
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
                    StartCoroutine(WaitForClip(selected, swimming.source));
                }
                else
                {
                    swimming.source.volume = selected.volume;
                    swimming.source.loop = selected.loop;
                    swimming.source.clip = selected.sound;
                    swimming.source.Play();
                }
            }
        }
        void PlayRandomSound(AudioClip[] inputSounds, AudioSource sSource, bool WaitToFinish = false) 
        {
            if (inputSounds.Length < 1)
                return;
            CamSoundOptions selected = new CamSoundOptions();
            AudioClip clip = inputSounds[UnityEngine.Random.Range(0, inputSounds.Length)];
            selected.sound = clip;
            selected.loop = sSource.loop;
            selected.volume = sSource.volume;
            if (WaitToFinish == true)
            {
                StartCoroutine(WaitForClip(selected, sSource));
            }
            else
            {
                sSource.clip = clip;
                sSource.Play();
            }
        }
        IEnumerator WaitForClip(CamSoundOptions clip, AudioSource source) 
        {
            if (queued == false)
            {
                queued = true;
                yield return new WaitWhile(() => source.isPlaying);
                if (mc.GetSwimState() == true)
                {
                    source.volume = clip.volume;
                    source.loop = clip.loop;
                    source.clip = clip.sound;
                    source.Play();
                }
                queued = false;
            }
        }
        float GetSoundDelay() {
            if (underwater == true)
            {
                if (InputManager.GetButton("Run"))
                {
                    return swimming.under_fastSoundDelay;
                }
                else
                {
                    return swimming.under_slowSoundDelay;
                }
            }
            else
            {
                if (InputManager.GetButton("Run"))
                {
                    return swimming.above_fastSoundDelay;
                }
                else
                {
                    return swimming.above_slowSoundDelay;
                }
            }
        }
        void EnableFogState(bool state)
        {
            if (state == true)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = swimming.underwaterColor;
                RenderSettings.fogDensity = swimming.fogAmount;
                RenderSettings.skybox = null;
            }
            else
            {
                RenderSettings.fog = defaultFog;
                RenderSettings.fogColor = defaultFogColor;
                RenderSettings.fogDensity = defaultFogDensity;
                RenderSettings.skybox = defaultSkybox;
            }
        }
        void PlaySwimMoveSounds()
        {
            if (mc.GetSwimState() == false)
                return;
            if (timer >= GetSoundDelay())
            {
                if (underwater == true)
                {
                    PlayRandomSound(swimming.underWaterSwimming, swimming.swimSource, true);
                }
                else
                {
                    PlayRandomSound(swimming.aboveWaterSwimming, swimming.swimSource, true);
                }
                timer = 0;
            }
        }
        void OnGUI()
        {
            if (showExitGUI == true)
            {
                Vector3 screenPos = GetComponent<Camera>().WorldToScreenPoint(hit.point);
                GUI.TextArea(new Rect(screenPos.x + swimming.offset.x, (Screen.height - screenPos.y) + swimming.offset.y, 0, 0), CyberBullet.Helpers.Helpers.ModifiedText(swimming.replaceActionWith, swimming.exitText), 1000, swimming.style);
            }
        }
        #endregion
    }
}