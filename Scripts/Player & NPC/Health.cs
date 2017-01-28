///////////////////////////////////////////////////////////////
///															///
/// 		Written By Wesley Haws April 2016				///
/// 				Tested with Unity 5						///
/// 	Anyone can use this for any reason. No limitations. ///
/// 														///
/// This script is to be used in conjunction with			///
/// "AnimController.cs" and "AIBehavior.cs" script. 		///
///	This script is capable of the following:				///
/// *Detect direction hit (Play dir sepcific animations)	///
/// *Signal object death									///
/// *Play sounds when damaged								///
/// *Display visual when damaged							///
/// *Signal death when health is zero						///
/// 														///
///////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using RAIN.Core;

public class Health : MonoBehaviour {
	[SerializeField] private bool NPC = false;				//Is this a player or an NPC?
	[SerializeField] private float[] NPCDeathNumbers;		//valid mecanim "deathNumber" values
	[SerializeField] private bool ragdollDeath = true;		//Have ragdoll effects
	[SerializeField] private float delayRagdollEffects = 0.0f;//How long to wait until enabling ragdoll effects
	[SerializeField] private bool useCombatController = true;
	[SerializeField] private Animator[] anim;
	[SerializeField] private float health = 100.0f;			//total health of object
	[SerializeField] private float regeneration = 0.0f;		//slowly regenerate health
	[SerializeField] private Texture2D guiOnHit;			//visual display when this object is damaged
	[SerializeField] private float guiFadeSpeed = 2;		//this will divide Time.deltaTime
	[Range(0.0F, 1.0F)]
	[SerializeField] private float hitSoundsVolume = 0.5f;	//how loud do you want to play these sounds?
	[SerializeField] private Camera playerCamera;			//playerCamera to show gui effects on
	[SerializeField] private AudioClip[] hitSounds;			//sounds to play when this object is damaged
	[SerializeField] private AudioClip[] gainHealthSounds;	//sound to play when gaining health
	[SerializeField] private AudioSource audioSource;		//auto filled if none applied(can be dangerous sound wise)
	[SerializeField] private Transform deathCamParent;		//for Different camera angle
	[Space(10)]
	[SerializeField] private bool debugHealth = false;		//for debugging
	[SerializeField] private bool debugDirHit = false;		//for debugging
	[Space(10)]
	[Header("==== Following Requires: Animator")]
	[SerializeField] private bool staggerOnEveryHit = false;
	[SerializeField] private GameObject deathPosition;
	private float damageNumber = 0.0f;
	private bool gotHit = false;
	private float guiAlpha = 1.0f;
	private float originalVolume;
	private bool ragdolled = false;
	private bool rdLastState = false;
	private Camera originalCamera;
	private Transform originalCamParent;
	private bool isDead = false;

	void Start() {
		GameObject[] remaining = GameObject.FindGameObjectsWithTag("Player");
		string myname = this.gameObject.name+"(Clone)";
		foreach (GameObject clone in remaining) {
			if(clone.name == myname){
				GameObject.Destroy(clone);
			}
		}
		SetRagdollState (false);
		if (NPC == false) {
			originalCamera = this.GetComponentInChildren<Camera> ();
			originalCamParent = originalCamera.transform.parent;
		}
		if (deathPosition == null && NPC == false) {
			deathPosition = this.transform.GetChild (6).GetChild (1).GetChild (2).gameObject;
		}
		if ((anim.Length < 1 || anim[0] == null) && NPC == false) {
			if (this.GetComponentInChildren<Animator> ()) {
				anim[0] = this.GetComponentInChildren<Animator> ();
			} 
		}
		if ((playerCamera == null && guiOnHit != null) && NPC == false) {
			if (this.GetComponent<Camera> ()) {
				playerCamera = this.GetComponent<Camera> ();
			} 
			else if(NPC == false){
				playerCamera = this.GetComponentInChildren<Camera> ();
			}
		}
		if (audioSource == null) {
			audioSource = this.GetComponent<AudioSource> ();
		}
		originalVolume = audioSource.volume;
		if (NPC == false) {
			health = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<PlayerManager> ().currentPlayerHealth;
			regeneration = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<PlayerManager> ().currentPlayerRegen;
		}
	}
	// Update is called once per frame
	void Update () {
		if (debugHealth == true) {
			Debug.Log("Health: "+health);
		}
		if (health <= 0) {
			Death ();
		}
		if (regeneration > 0) {
			health += regeneration * Time.deltaTime;
		}
		if (NPC == false) {
			if (gotHit == true) {
				guiAlpha -= Time.deltaTime / guiFadeSpeed;
				if (guiAlpha <= 0) {
					gotHit = false;
				}
			}
			if (ragdolled != rdLastState && health > 0 && anim [0].GetBool ("grounded") == true) {
				StartCoroutine (PlayGetUpAnim ());
			}
		}

	}
	IEnumerator PlayGetUpAnim() {
		originalCamera.transform.parent = deathCamParent.transform;
		//deathCamParent.SetActive (true);
		originalCamera.gameObject.SetActive(false);
		this.GetComponent<MouseLook> ().enabled = false;
		yield return new WaitForSeconds (2);
		if (anim[0].GetBool ("grounded") == true) {
			ragdolled = false;
			Vector3 currentLoc = deathPosition.transform.position;
			foreach (Animator animator in anim) {
				animator.SetTrigger ("GetUpFromBack");
			}
			this.transform.position = currentLoc;
			SetRagdollState (false);

			yield return new WaitForSeconds (5);
			originalCamera.gameObject.SetActive(true);
			this.GetComponent<MouseLook> ().enabled = true;
			//deathCamParent.SetActive (false);
			originalCamera.transform.parent = originalCamParent;
		}
	}
	public void PlayHitVoiceSoundKeyFrame() {
		StartCoroutine (PlayHitSound ());
	}
	IEnumerator PlayHitSound(){
		audioSource.volume = hitSoundsVolume;
		audioSource.clip = hitSounds[UnityEngine.Random.Range(0,hitSounds.Length)];
		audioSource.Play ();
		yield return new WaitForSeconds (audioSource.clip.length);
		audioSource.volume = originalVolume;
	}
	public void ApplyDamage(float damage, GameObject sender = null, bool stagger = false, bool isPunch = false) {
		if (isDead == false) {
			bool isDamaged = true;
			if (isPunch == true && useCombatController == true && GetComponent<CombatController> ().isBlocking == true) {
				if (NPC == true && anim [0].GetCurrentAnimatorStateInfo (0).IsTag ("defend")) {
					isDamaged = false;
				} else if (NPC == false) {
					isDamaged = false;
				}
			} 
			if (isDamaged == true) {
				health -= damage;
				guiAlpha = 1.0f;
				gotHit = true;
				//if was falling play ragdoll
				if (NPC == false) {
					if (anim [0].GetCurrentAnimatorStateInfo (0).IsName ("Falling")) {
						SetRagdollState (true);
					}
				} else {
					this.GetComponentInChildren<AIRig> ().AI.WorkingMemory.SetItem<bool>("damaged", true);
					if (sender != null) {
						this.GetComponentInChildren<AIRig> ().AI.WorkingMemory.SetItem<GameObject>("damage_giver", sender);
					}
				}
				if (hitSounds.Length > 0) {
					StartCoroutine (PlayHitSound ());
				}
				if ((staggerOnEveryHit == true || stagger == true) && anim [0] != null) {
					foreach (Animator animator in anim) {
						animator.SetTrigger ("damaged");
					}
					if (sender == null) {
						damageNumber = 0.0f;
						foreach (Animator animator in anim) {
							animator.SetFloat ("damagedNumber", damageNumber);
							animator.SetTrigger ("damaged");
						}
					} else {
						Vector3 direction = (sender.transform.position - this.transform.position).normalized;
						float angle = Vector3.Angle (direction, this.transform.forward);
						if (angle > 50 && angle < 130) {//side hit
							Vector3 pos = transform.TransformPoint (sender.transform.position);
							if (pos.x < 0) {
								if (debugDirHit == true) {
									Debug.Log ("Left");
								}
								damageNumber = 0.6f;
								foreach (Animator animator in anim) {
									animator.SetFloat ("damagedNumber", damageNumber);
								}
							} else {
								if (debugDirHit == true) {
									Debug.Log ("Right");
								}
								damageNumber = 1.0f;
								foreach (Animator animator in anim) {
									animator.SetFloat ("damagedNumber", damageNumber);
								}
							}
						} else if (angle < 50 && angle > -1) {
							if (debugDirHit == true) {
								Debug.Log ("Front Hit");
							}
							damageNumber = 0.0f;
							foreach (Animator animator in anim) {
								animator.SetFloat ("damagedNumber", damageNumber);
							}
						} else if (angle > 130 && angle < 270) {
							if (debugDirHit == true) {
								Debug.Log ("Back Hit");
							}
							damageNumber = 0.3f;
							foreach (Animator animator in anim) {
								animator.SetFloat ("damagedNumber", damageNumber);
							}
						}
						foreach (Animator animator in anim) {
							animator.SetTrigger ("damaged");
						}
					}

				}
				if (this.GetComponent<AIBehavior> ()) {
					this.GetComponent<AIBehavior> ().memory.currentState = "Hostile";
				}
				if (this.GetComponent<AnimController> ()) {
					this.GetComponent<AnimController> ().updateState ("Hostile");
				}
			}
		}
	}
	public void ApplyHealth(float amount) {
		health += amount;
		if (health > 100) {
			health = 100;
		}
		if (gainHealthSounds.Length > 0) {
			audioSource.clip = gainHealthSounds [Random.Range (0, gainHealthSounds.Length)];
			audioSource.Play ();
		}
	}
	public void CallDeath() {
		Death ();
	}
	void Death(){
		if (isDead == false) {
			this.tag = "Untagged";
			if (deathCamParent != null && NPC == false) {
				originalCamera.transform.parent = deathCamParent;
			}
			if (NPC == true) {
				transform.Find ("AI").GetComponent<AIRig> ().enabled = false;
			}
			foreach (Animator animator in anim) {
				if (NPC == true) {
					animator.SetFloat ("deadNumber", NPCDeathNumbers [Random.Range (0, NPCDeathNumbers.Length)]);
				} 
			}
			if (NPC == false) {
				LockLook (true);
				GameObject.FindGameObjectWithTag ("GUIParent").GetComponent<PlayerDeath> ().playerDead = true;
			}
			if (ragdollDeath == true) {
				if (delayRagdollEffects == 0) {
					SetRagdollState (true);
				} else {
					StartCoroutine (DelayRagdollEffects ());
				}
			}
			isDead = true;
		}
	}
	void OnGUI(){
		if (NPC == false) {
			Color color = GUI.color;
			if (gotHit && guiOnHit != null) {
				color.a = guiAlpha;
				GUI.color = color;
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), guiOnHit, ScaleMode.StretchToFill);
			}
		}
	}
	public float GetHealth() {
		return health;
	}
	public float GetRegeneration() {
		return regeneration;
	}
	private IEnumerator DelayRagdollEffects() {
		yield return new WaitForSeconds (delayRagdollEffects);
		SetRagdollState (true);
	}
	public void SetRagdollState(bool newValue) {
		ragdolled = newValue;
		newValue = !newValue;
		//Get an array of components that are of type Rigidbody
		Rigidbody[] bodies=GetComponentsInChildren<Rigidbody>();

		//For each of the components in the array, treat the component as a Rigidbody and set its isKinematic property
		foreach (Rigidbody rb in bodies)
		{
//			rb.useGravity = ragdolled; 
			rb.isKinematic=newValue;
		}
			
		if (newValue == false && NPC == false) {
			GetComponent<Animator> ().enabled = false;
			this.GetComponent<MovementController> ().moveLocked = true;
		} else if(newValue == true && NPC == false){
			GetComponent<Animator> ().enabled = true;
			this.GetComponent<MovementController> ().moveLocked = false;
		}
		if (NPC == true) {
			foreach (Animator mecanim in anim) {
				mecanim.enabled = newValue;
			}
		}
	}
	private void LockLook(bool value) {
		MouseLook[] looks = GameObject.FindObjectsOfType (typeof(MouseLook)) as MouseLook[];
		foreach (MouseLook look in looks) {
			look.enable = value;
		}
		if (this.GetComponent<MouseLook> () && this.GetComponent<MouseLook> ().enable != value) {
			this.GetComponent<MouseLook> ().enable = value;
		}
	}
}
