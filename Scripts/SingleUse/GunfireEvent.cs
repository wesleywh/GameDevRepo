using UnityEngine;
using System.Collections;

public class GunfireEvent : MonoBehaviour {
	[SerializeField] private Light lightSource;
	[SerializeField] private AudioClip[] GunfireSounds;
	[SerializeField] private float[] waitBetweenSounds;
	[SerializeField] private AudioSource audioSource;
	private bool playSounds = false;
	private float timer = 0.0f;
	private int i =0;
	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") { 
			playSounds = true;
		}
	}
	void Update() {
		if (playSounds) {
			timer += Time.deltaTime;
			lightSource.intensity -= Time.deltaTime * 2;
			if (i < waitBetweenSounds.Length && timer >= waitBetweenSounds[i]) {
				lightSource.intensity = 2;
				audioSource.clip = GunfireSounds [i];
				audioSource.Play();
				i += 1;
				timer = 0;
			}
			if (i >= GunfireSounds.Length && lightSource.intensity <= 0) {
				Destroy (this.gameObject);
			}
		}
	}
}
