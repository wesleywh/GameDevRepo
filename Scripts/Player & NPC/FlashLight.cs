using UnityEngine;
using System.Collections;
using Pandora.GameManager;

public class FlashLight : MonoBehaviour {

	[SerializeField] private AudioSource overrideAudioSource = null;
	[SerializeField] private AudioClip flashlightOnSound = null;
	[SerializeField] private AudioClip flashlightOffSound = null;
    [SerializeField] private int flashlightId = 9999999;

    private InventoryManagerNew inventoryManager = null;

    void Start()
    {
        inventoryManager = dontDestroy.currentGameManager.GetComponent<InventoryManagerNew>();
    }
	public void ActivateFlashlight() {
		GetComponent<Light> ().enabled = !GetComponent<Light> ().enabled;
		AudioClip soundToPlay = (GetComponent<Light> ().enabled == true) ? flashlightOnSound : flashlightOffSound;
		GetComponent<AudioSource> ().clip = soundToPlay;
		if (overrideAudioSource == null) {
			GetComponent<AudioSource> ().Play ();
		}
		else {
			overrideAudioSource.Play ();
		}
		StartCoroutine (CheckIfHoldingFlashlight ());
	}
	IEnumerator CheckIfHoldingFlashlight() {
		yield return new WaitForSeconds(0.1f);
        if(inventoryManager.HasItem(flashlightId) == false) {
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
