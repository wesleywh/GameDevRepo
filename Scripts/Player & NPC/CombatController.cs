using UnityEngine;
using System.Collections;
using TeamUtility.IO;					//Custom Input Manager

public class CombatController : MonoBehaviour {
	[SerializeField] private Animator overrideAnimator = null;
	[SerializeField] private float[] crouchAttackNumbers;
	[SerializeField] private float[] standAttackNumbers;
	[SerializeField] private float[] crouchBlockNumbers;
	[SerializeField] private float[] standBlockNumbers;
	[SerializeField] private Vector3 armAdjustment = Vector3.zero;
	[SerializeField] private GameObject arms;
	[SerializeField] private float unarmed_damage = 10.0f;
	[SerializeField] private float meleeDistance = 1.5f;
	[SerializeField] private AudioClip[] voiceSounds;
	[SerializeField] private AudioClip[] punchHitSounds;
	[SerializeField] private AudioSource overrideAudioSource;
	[Range(0,1)]
	[SerializeField] private float voiceVolume = 0.25f;
	[SerializeField] private AudioSource overrideHitAudioSource;
	[Range(0,1)]
	[SerializeField] private float hitVolume = 0.25f;
	[SerializeField] private float rayCastStartHeight = 1.0f;
	[SerializeField] private bool debugRayDistance = false;
	[SerializeField] private bool debugRayHit = false;

	private Animator anim;
	private bool isCrouched = false;
	private bool isBlocking = false;
	private Vector3 adjustmentPos;
	private AudioSource audioSource;
	private AudioSource hitAudioSource;
	private Vector3 raycastAdjustment;
	void Start() {
		raycastAdjustment = new Vector3 (0, rayCastStartHeight, 0);
		audioSource = (overrideAudioSource != null) ? overrideAudioSource : GetComponent<AudioSource> ();
		arms = (arms == null) ? this.gameObject : arms;
		adjustmentPos = arms.transform.position + armAdjustment;
		anim = (overrideAnimator != null) ? overrideAnimator : GetComponent<Animator> ();
		hitAudioSource = (overrideHitAudioSource != null) ? overrideHitAudioSource : GetComponent<AudioSource> ();
	}
	// Update is called once per frame
	void Update () {
		if (anim.GetCurrentAnimatorStateInfo (0).IsTag ("Combat") || anim.GetCurrentAnimatorStateInfo (0).IsTag ("Defense")) {
			arms.transform.position = new Vector3 (arms.transform.root.position.x, adjustmentPos.y, arms.transform.root.position.z);
		} else {
			arms.transform.position = new Vector3 (arms.transform.root.position.x, arms.transform.root.position.y,arms.transform.root.position.z);
		}
		if (InputManager.GetButtonDown ("Attack")) {
			anim.SetFloat ("attackNumber", GetAttackNumber ());
			anim.SetTrigger ("attack");
		} else if (InputManager.GetButtonDown ("Block")) {
			isBlocking = !isBlocking;
			anim.SetFloat ("blockNumber", GetBlockNumber ());
			anim.SetBool ("block", isBlocking);
		} 
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
				hit.collider.transform.GetComponent<Health> ().ApplyDamage (unarmed_damage);
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
