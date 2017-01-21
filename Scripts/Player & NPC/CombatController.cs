using UnityEngine;
using System.Collections;
using TeamUtility.IO;					//Custom Input Manager
using RAIN.Core;

public class CombatController : MonoBehaviour {
	[SerializeField] private bool NPC = false;
	[Tooltip("Arms will mimic this rotation")]
	[SerializeField] private GameObject punchTarget = null;
	[SerializeField] private Animator overrideAnimator = null;
	[Header("Animation Blend Tree Numbers")]
	[SerializeField] private float[] crouchAttackNumbers;
	[SerializeField] private float[] standAttackNumbers;
	[SerializeField] private float[] crouchBlockNumbers;
	[SerializeField] private float[] standBlockNumbers;
	[SerializeField] private MovementController movementController;
	[Space(10)]
	[Tooltip("Object to apply adjustments to")]
	[SerializeField] private GameObject arms;
	[SerializeField] private float unarmed_damage = 10.0f;
	[SerializeField] private float meleeDistance = 1.5f;
	[Header("Audio Actions")]
	[SerializeField] private AudioClip[] voiceSounds;
	[SerializeField] private AudioClip[] punchHitSounds;
	[SerializeField] private AudioSource overrideAudioSource;
	[Range(0,1)]
	[SerializeField] private float voiceVolume = 0.25f;
	[SerializeField] private AudioSource overrideHitAudioSource;
	[Range(0,1)]
	[SerializeField] private float hitVolume = 0.25f;
	[Header("Punch Distance")]
	[SerializeField] private float rayCastStartHeight = 1.0f;
	[Header("Take down Actions")]
	[SerializeField] private float takedownArmAdjustmentUp = 0.1f;
	[SerializeField] private float takeDownDistance = 2.0f;
	[SerializeField] private float[] takeDownNumbers;
	[Header("Other Script Access Vars")]
	public bool takedownAble = true;
	public bool isBlocking = false;
	[Space(10)]
	[Header("Debugging Actions")]
	[SerializeField] private bool debugRayDistance = false;
	[SerializeField] private bool debugRayHit = false;


	private Animator anim;
	private bool isCrouched = false;
	private Vector3 adjustmentPos;
	private AudioSource audioSource;
	private AudioSource hitAudioSource;
	private Vector3 raycastAdjustment;
	private RaycastHit hitObj;
	private bool inTakedown = false;

	void Start() {
		raycastAdjustment = new Vector3 (0, rayCastStartHeight, 0);
		audioSource = (overrideAudioSource != null) ? overrideAudioSource : GetComponent<AudioSource> ();
		arms = (arms == null) ? this.gameObject : arms;
		anim = (overrideAnimator != null) ? overrideAnimator : this.GetComponent<Animator> ();
		hitAudioSource = (overrideHitAudioSource != null) ? overrideHitAudioSource : GetComponent<AudioSource> ();
	}
	// Update is called once per frame
	void Update () {
		if (NPC == false) {
			//Adjust arms according to punch rotation
			if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("Combat") || anim.GetCurrentAnimatorStateInfo (0).IsTag ("Defense")
			    || anim.GetCurrentAnimatorStateInfo (0).IsTag ("takedown")) {
				arms.transform.rotation = punchTarget.transform.rotation;
			} else {
				arms.transform.rotation = arms.transform.root.transform.rotation;
			}
			//Punch
			if (InputManager.GetButtonDown ("Attack")) {
				anim.SetFloat ("attackNumber", GetAttackNumber ());
				anim.SetTrigger ("attack");
			} 
			//Block
			else if (InputManager.GetButtonDown ("Block")) {
				isBlocking = !isBlocking;
				anim.SetFloat ("blockNumber", GetBlockNumber ());
				anim.SetBool ("block", isBlocking);
			} 
			if (InputManager.GetButtonDown ("Action")) {
				if (inTakedown == false) {
					inTakedown = true;
					GameObject takedownTarget = CheckTakedownTarget ();
					if (takedownTarget != null) {
						StartCoroutine(PerformTakedown (takedownTarget, takeDownNumbers [Random.Range (0, takeDownNumbers.Length)]));
					} else {
						inTakedown = false;
					}
				}
			}
		}
	}
	private GameObject CheckTakedownTarget() {
		Vector3 origin = (this.transform.position + raycastAdjustment);
		RaycastHit[] hits;
		GameObject target = null;
		hits = Physics.RaycastAll (origin, transform.forward, takeDownDistance);
		foreach (RaycastHit hit in hits) {
			if (hit.collider.GetComponent<CombatController> () || hit.collider.transform.root.GetComponent<CombatController> ()) {
				CombatController combat = (hit.collider.GetComponent<CombatController> ()) ? hit.collider.GetComponent<CombatController> () : hit.collider.transform.root.GetComponent<CombatController> ();
				if (combat.takedownAble == true) {
					target = hit.collider.gameObject;
					break;
				} 
			} 
		}
		return target;
	}
	public void CallTakedown(float number, Quaternion rotation, Vector3 position) {
		anim.SetFloat ("takedownNumber", number);
		anim.SetTrigger ("takedown");
		this.GetComponent<Collider> ().enabled = false;
		this.transform.rotation = rotation;
		this.transform.position = position;
		this.transform.root.GetComponentInChildren<AIRig> ().enabled = false;
	}
	IEnumerator PerformTakedown(GameObject victim, float takedownNumber) {
		this.GetComponent<MovementController> ().moveLocked = true;
		this.GetComponent<MovementController> ().crouching = false;
		if (this.gameObject == victim) {
			anim.SetFloat ("takedownNumber", takedownNumber);
			anim.SetTrigger ("takedown");
		} else {
//			this.GetComponent<Collider> ().enabled = false;
			Vector3 positioning = this.transform.position + (transform.forward * 0.5f);
			positioning = new Vector3 (positioning.x, positioning.y+takedownArmAdjustmentUp, positioning.z);
			victim.transform.root.gameObject.GetComponent<CombatController> ().CallTakedown (takedownNumber, this.transform.rotation, positioning);
			anim.SetFloat ("takedownNumber", takedownNumber);
			anim.SetTrigger ("takedown");
		}
		bool inTakedownState = true;
		while (inTakedownState == true) {
			yield return new WaitForSeconds (0.2f);
			inTakedownState = anim.GetCurrentAnimatorStateInfo (0).IsTag ("takedown");
		}
		inTakedown = false;
		this.GetComponent<MovementController> ().moveLocked = false;
		this.GetComponent<Collider> ().enabled = true;
	}
	private GameObject FindClosestTagged(string tag) {
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag(tag);
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos) {
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}
	private float GetAttackNumber() {
		float value = 0.0f;
		if (isCrouched == true) 
			value = crouchAttackNumbers [UnityEngine.Random.Range (0, crouchAttackNumbers.Length)];
		else 
			value = standAttackNumbers[UnityEngine.Random.Range (0, standAttackNumbers.Length)];
		return value;
	}
	private float GetBlockNumber() {
		float value = 0.0f;
		if (isCrouched == true) 
			value = crouchBlockNumbers [UnityEngine.Random.Range (0, crouchBlockNumbers.Length - 1)];
		else 
			value = standBlockNumbers[UnityEngine.Random.Range (0, standBlockNumbers.Length - 1)];
		return value;
	}
	public void Punch() {
		Vector3 origin = (this.transform.position + raycastAdjustment);
		RaycastHit hit;
		if (debugRayDistance == true) {
			Debug.DrawRay (origin, transform.forward, Color.red, 10.0f);
		}
		if (Physics.Raycast (origin, transform.forward, out hit, meleeDistance)) {
			if (debugRayHit == true) {
				Debug.Log ("Hit: " + hit.collider.gameObject.name);
			}
			if (hit.collider.transform.GetComponent<Health> () && hit.collider.transform.gameObject != this.gameObject) {
				PlayHitSound ();
				hit.collider.transform.GetComponent<Health> ().ApplyDamage (unarmed_damage, this.gameObject, true, true);
			}
		}
	}
	public void PlayVoiceSound() {
		if (voiceSounds.Length > 0) {
			audioSource.clip = voiceSounds [Random.Range (0, voiceSounds.Length)];
			audioSource.volume = voiceVolume;
			audioSource.Play ();
		}
	}
	public void PlayHitSound() {
		if (punchHitSounds.Length > 0) {
			hitAudioSource.clip = punchHitSounds [Random.Range (0, punchHitSounds.Length)];
			hitAudioSource.volume = hitVolume;
			hitAudioSource.Play ();
		}
	}
}
