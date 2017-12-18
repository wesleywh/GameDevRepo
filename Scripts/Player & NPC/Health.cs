using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Pandora.Cameras;
using UnityEngine.Events;
using Panda.AI;

namespace Pandora.Controllers {
    #region Animations
    [System.Serializable]
    public class HealthAnimators {
        public Animator[] anims = null;
        public float[] deathNumbers = null;
        public bool ragdollDeath = true;
        public float delayRagdollEffects = 0.0f;
        public Transform parentCameraOnDeath = null;
    }
    #endregion

    #region UI
    [System.Serializable]
    public class HealthUI {
        public bool displayEffects = true;
        public RainCameraController SplatterEffects = null;
        public RainCameraController BloodCorners = null;
        public Texture2D guiOnHit = null;
        public float guiFadeSpeed = 2.0f;
    }
    #endregion

    #region Memory
    [System.Serializable]
    public class HealthMemory {
        public GameObject lastDamager = null;
    }
    #endregion

    #region Base Settings
    [System.Serializable]
    public class HealthSettings {
        public float health = 100.0f;
        public float maxHealth = 100.0f;
        public float regeneration = 0.0f;
        public Camera playerCamera = null;
        public string changeTagOnDeath = "Untagged";
        public UnityEvent eventsOnDeath;
        public UnityEvent OnDamaged;
    }
    #endregion

    #region Sounds
    [System.Serializable]
    public class HealthSounds {
        [Range(0,1)]
        public float volume = 0.5f;
        public AudioSource audioSource = null; 
        public AudioClip[] hitSounds = null;         //sounds to play when this object is damaged
        public AudioClip[] gainHealthSounds = null;  //sound to play when gaining health
        public AudioClip[] deathSounds = null;
    }
    #endregion

    #region Debugging
    [System.Serializable]
    public class HealthDebugging {
        public bool showHealth = false;      //for debugging
        public bool detectDirHit = false;      //for debugging
        public bool applyDamage = false;    //for debugging
        public bool applyHealth = false;
    }
    #endregion

    public class Health : MonoBehaviour {
        enum SoundType {gain, loss}
        private SoundType type = SoundType.gain;

        [SerializeField] private bool isPlayer = false;
        #region BaseClasses
        [SerializeField] private HealthSettings baseSettings;
        [SerializeField] private HealthUI ui;
        [SerializeField] private HealthAnimators animations;
        [SerializeField] private HealthSounds sounds;
        [SerializeField] private HealthMemory memory;
        [SerializeField] private HealthDebugging debugging;
        #endregion

        #region Internal Use Only
        private float damageNumber = 0.0f;
        private bool gotHit = false;
        private float guiAlpha = 1.0f;
        private bool ragdolled = false;
        private bool rdLastState = false;
        private Camera originalCamera;
        private Transform originalCamParent;
        private bool isDead = false;
        #endregion

        void Start() {
            EnableHitGUI(false);
            baseSettings.playerCamera = (baseSettings.playerCamera == null) ? GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>() : baseSettings.playerCamera; 
        }
        void Update () {
            if (debugging.detectDirHit == true)
            {
                Debug.Log(GetDirection(memory.lastDamager));
            }
            if (debugging.applyDamage == true) {
                debugging.applyDamage = false;
        		ApplyDamage (10, this.gameObject, false);
        	}
            if (debugging.applyHealth == true) {
                debugging.applyHealth = false;
                ApplyHealth (10);
            }
            if (debugging.showHealth == true) {
        		Debug.Log("Health: "+baseSettings.health);
        	}
            if (baseSettings.regeneration > 0 && baseSettings.health < baseSettings.maxHealth) {
        		baseSettings.health += baseSettings.regeneration * Time.deltaTime;
        	}
            ResetGUI();
        }
        public void PlayHitVoiceSoundKeyFrame() {
            StartCoroutine (PlaySound (SoundType.loss));
        }
        IEnumerator PlaySound(SoundType type){
            bool play = false;
            if (sounds.audioSource.isPlaying == true)
            {
                yield return null;
            }
            else
            {
                sounds.audioSource.volume = sounds.volume;
                switch (type)
                {
                    case SoundType.loss:
                        if (sounds.hitSounds.Length < 1)
                            yield return null;
                        sounds.audioSource.clip = sounds.hitSounds[UnityEngine.Random.Range(0, sounds.hitSounds.Length)];
                        play = true;
                        break;
                    case SoundType.gain:
                        if (sounds.gainHealthSounds.Length < 1)
                            yield return null;
                        sounds.audioSource.clip = sounds.gainHealthSounds[UnityEngine.Random.Range(0, sounds.gainHealthSounds.Length)];
                        play = true;
                        break;
                }
                if (play == true)
                    sounds.audioSource.Play();
                yield return new WaitForSeconds(sounds.audioSource.clip.length);
            }
        }
        private void EnableHitGUI(bool state)
        {
            if (state == true)
            {
                if (ui.BloodCorners)
                {
                    ui.BloodCorners.Alpha = 1;
                    ui.BloodCorners.Play();
                }
                if (ui.SplatterEffects != null) ui.SplatterEffects.Play ();
                gotHit = true;
                guiAlpha = 1;
            }
            else
            {
                if (ui.BloodCorners) ui.BloodCorners.Alpha = 0;
                gotHit = false;
                guiAlpha = 0;
            }
        }
        private void ResetGUI()
        {
            if (guiAlpha > 0)
            {
                guiAlpha -= Time.deltaTime * ui.guiFadeSpeed;
                if (guiAlpha < 0)
                {
                    gotHit = false;
                    guiAlpha = 0;
                }
            }
            if (ui.BloodCorners != null) {
                ui.BloodCorners.Alpha = (ui.BloodCorners.Alpha > 0) ? ui.BloodCorners.Alpha - Time.deltaTime * ui.guiFadeSpeed : 0;
            }

        }
        public void ApplyDamage(float damage, GameObject sender = null, bool stagger = false)
        {
            baseSettings.health -= damage;
            EnableHitGUI(true);
            if (sender != null)
            {
                memory.lastDamager = sender;
                float dir = GetDirection(sender);
            }
            StartCoroutine(PlaySound(SoundType.loss));
            if (baseSettings.health <= 0)
            {
                baseSettings.health = 0;
                Death();
            }
            if (GetComponent<AIMemory>() && sender != null)
            {
                GetComponent<AIMemory>().enemy = sender;
                GetComponent<AIMemory>().last_enemy_loc = sender.transform;
                GetComponent<AIMemory>().status = AIStatus.Hostile;
            }
            baseSettings.OnDamaged.Invoke();
        }
        public void ApplyHealth(float amount) {
        	baseSettings.health += amount;
            if (baseSettings.health > baseSettings.maxHealth) {
                baseSettings.health = baseSettings.maxHealth;
        	}
            StartCoroutine(PlaySound(SoundType.gain));
        }
        public float GetDirection(GameObject damager)
        {
            // 1 = forward
            //0.75 = right
            //0.50 = left
            //0 = back
            float ret_val = 0;
            if (damager == null)
                return 99999;
            Vector3 heading = damager.transform.position - this.transform.position;
            float angle = Vector3.Angle(this.transform.forward, heading);
            //forward
            if (angle == 0 || angle < 40.1)
            {
                ret_val = 1;
            }
            //side
            else if (angle >= 40.1 && angle < 111)
            {
                //right
                if (IsLeftOrRight(damager.transform) > 0)
                {
                    ret_val = 0.75f;
                }
                //left
                else
                {
                    ret_val = 0.5f;
                }
            }
            //back
            else if (angle >= 111)
            {
                ret_val = 0;
            }

            return ret_val;
        }
        float IsLeftOrRight(Transform target)
        {
            Vector3 heading = target.position - this.transform.position;
            Vector3 perp = Vector3.Cross(this.transform.forward, heading);
            float dir = Vector3.Dot(perp, this.transform.up);

            if (dir > 0f) 
            {
                return 1f;
            } 
            else if (dir < 0f) 
            {
                return -1f;
            } 
            else 
            {
                return 0f;
            }
        }
        public void Death() 
        {
            if (sounds.deathSounds.Length > 0)
            {
                sounds.audioSource.clip = sounds.deathSounds[Random.Range(0, sounds.deathSounds.Length - 1)];
                sounds.audioSource.Play();
            }
            if (animations.ragdollDeath == true)
            {
                StartCoroutine(SetRagdollState(true));
            }
            this.tag = baseSettings.changeTagOnDeath;
            if (animations.parentCameraOnDeath != null)
            {
                baseSettings.playerCamera.transform.parent = animations.parentCameraOnDeath;
            }
            if (isPlayer == true)
            {
                GameObject.FindGameObjectWithTag("GUIParent").GetComponent<PlayerDeath>().playerDead = true;
            }
            else if (GetComponent<NavMeshAgent>() && GetComponent<NavMeshAgent>().enabled == true)
            {
                GetComponent<NavMeshAgent>().destination = transform.position;
                GetComponent<NavMeshAgent>().enabled = false;
            }

            baseSettings.eventsOnDeath.Invoke();
        }
        void OnGUI(){
            if (ui.displayEffects == true) {
                if (gotHit == true && ui.guiOnHit != null) {
                    Color color = GUI.color;
        			color.a = guiAlpha;
        			GUI.color = color;
                    GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), ui.guiOnHit, ScaleMode.StretchToFill);
        		}
        	}
        }
        public float GetHealth() {
        	return baseSettings.health;
        }
        public float GetRegeneration() {
            return baseSettings.regeneration;
        }
        public void SetRegeneration(float amount)
        {
            baseSettings.regeneration = amount;
        }
        IEnumerator SetRagdollState(bool state) {
            yield return new WaitForSeconds(animations.delayRagdollEffects);
        	//Get an array of components that are of type Rigidbody
        	Rigidbody[] bodies=GetComponentsInChildren<Rigidbody>();

        	//For each of the components in the array, treat the component as a Rigidbody and set its isKinematic property
        	foreach (Rigidbody rb in bodies)
        	{
                rb.isKinematic=!state;
        	}
            for (int i = 0; i < animations.anims.Length; i++)
            {
                animations.anims[i].enabled = !state;
            }
            if (GetComponent<Animator>()) GetComponent<Animator>().enabled = !state;
            if (GetComponent<MovementController>()) GetComponent<MovementController>().enabled = !state;
        }
    }
}