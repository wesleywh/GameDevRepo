using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour {

	[SerializeField] private AudioSource overrideAudioSource = null;
	[SerializeField] private AudioClip flashlightOnSound = null;
	[SerializeField] private AudioClip flashlightOffSound = null;

	// Use this for initialization
	void Start () {
	
	}
	public void ActivateFlashlight() {
		this.gameObject.GetComponent<Light> ().enabled = !this.gameObject.GetComponent<Light> ().enabled;
		AudioClip soundToPlay = (this.gameObject.GetComponent<Light> ().enabled == true) ? flashlightOnSound : flashlightOffSound;
		this.GetComponent<AudioSource> ().clip = soundToPlay;
		if (overrideAudioSource == null) {
			this.GetComponent<AudioSource> ().Play ();
		}
		else {
			overrideAudioSource.Play ();
		}
		StartCoroutine (CheckIfHoldingFlashlight ());
	}
	IEnumerator CheckIfHoldingFlashlight() {
		yield return new WaitForSeconds(0.1f);
		if(GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>().HasItem("flashlight") == false) {
			this.gameObject.GetComponent<Light> ().enabled = false;
			this.GetComponent<AudioSource> ().clip = flashlightOffSound;
			if (overrideAudioSource == null) {
				this.GetComponent<AudioSource> ().Play ();
			}
			else {
				overrideAudioSource.Play ();
			}
		}
	}
}
