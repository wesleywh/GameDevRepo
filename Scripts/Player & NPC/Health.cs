using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using CyberBullet.Cameras;
using UnityEngine.Events;
using CyberBullet.Interactions;

namespace CyberBullet.Controllers {
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
        public bool changeTagInChildren = false;
        public bool changeLayerInChildren = true;
        public string changeTagOnDeath = "Untagged";
        public string changeLayerOnDeath = "Dead";
        public bool destroyOnDeath = false;
        public float destroyWait = 120.0f;
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
        private bool gotHit = false;
        private float guiAlpha = 1.0f;
        private Camera originalCamera;
        private Transform originalCamParent;
        #endregion

        void Start() {
            if (GetHealth() > 0)
            {
                StartCoroutine(SetRagdollState(false));
            }
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

        #region Sounds
        public void PlayHitVoiceSoundKeyFrame() {
            StartCoroutine (PlaySound (SoundType.loss));
        }
        IEnumerator PlaySound(SoundType type){
            bool play = false;
            if (sounds.audioSource == null || sounds.audioSource.isPlaying == true)
            {
                yield return null;
            }
            else
            {
                sounds.audioSource.volume = sounds.volume;
                switch (type)
                {
                    case SoundType.loss:
                        if (sounds.hitSounds.Length > 0)
                        {
                            sounds.audioSource.clip = sounds.hitSounds[UnityEngine.Random.Range(0, sounds.hitSounds.Length - 1)];
                            play = true;
                        }
                        break;
                    case SoundType.gain:
                        if (sounds.gainHealthSounds.Length > 0)
                        {
                            sounds.audioSource.clip = sounds.gainHealthSounds[UnityEngine.Random.Range(0, sounds.gainHealthSounds.Length - 1)];
                            play = true;
                        }
                        break;
                }
                if (play == true)
                {
                    sounds.audioSource.Play();
                    yield return new WaitForSeconds(sounds.audioSource.clip.length);
                }
                else
                {
                    yield return 0.0f;
                }
            }
        }
        #endregion

        #region Events
        public void ApplyDamage(float damage, GameObject sender = null, bool stagger = false)
        {
            baseSettings.health -= damage;
            EnableHitGUI(true);
            if (sender != null)
            {
                memory.lastDamager = sender;
                if (GetComponent<BehaviorDesigner.Runtime.BehaviorTree>())
                {
                    BehaviorDesigner.Runtime.SharedGameObject lastSender = (BehaviorDesigner.Runtime.SharedGameObject)sender;
                    BehaviorDesigner.Runtime.SharedVector3 lastSenderPosition = (BehaviorDesigner.Runtime.SharedVector3)sender.transform.position;
                    GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().SetVariable("target",lastSender);
                    GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().SetVariable("last_target_position",lastSenderPosition);
                    GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().SendEvent("Damaged");
                }
//                float dir = GetDirection(sender);
            }
            StartCoroutine(PlaySound(SoundType.loss));
            if (baseSettings.health <= 0)
            {
                baseSettings.health = 0;
                Death();
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
        public void Death() 
        {
            this.tag = baseSettings.changeTagOnDeath;
            if (baseSettings.changeTagInChildren == true)
            {
                foreach (Transform child in transform)
                {
                    child.tag = baseSettings.changeTagOnDeath;
                }
            }
            gameObject.layer = LayerMask.NameToLayer(baseSettings.changeLayerOnDeath);
            if (baseSettings.changeLayerInChildren == true)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.layer = LayerMask.NameToLayer(baseSettings.changeLayerOnDeath);
                }
            }
            if (sounds.deathSounds.Length > 0)
            {
                sounds.audioSource.clip = sounds.deathSounds[Random.Range(0, sounds.deathSounds.Length - 1)];
                sounds.audioSource.Play();
            }
            if (animations.ragdollDeath == true)
            {
                StartCoroutine(SetRagdollState(true));
            }

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
            if (baseSettings.destroyOnDeath == true)
            {
                Destroy(this.gameObject, baseSettings.destroyWait);
            }
        }
        #endregion

        #region GUI
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
        #endregion

        #region Gets
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
        public float GetHealth() {
        	return baseSettings.health;
        }
        public float GetRegeneration() {
            return baseSettings.regeneration;
        }
        public float GetMaxHealth()
        {
            return baseSettings.maxHealth;
        }
        public GameObject GetLastDamager()
        {
            return memory.lastDamager;
        }
        #endregion

        #region Sets
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
        public void SetHealth(float health)
        {
            baseSettings.health = health;
        }
        public void SetRegeneration(float regen)
        {
            baseSettings.regeneration = regen;
        }
        public void SetMaxHealth(float max)
        {
            baseSettings.maxHealth = max;
        }
        #endregion
    }
}