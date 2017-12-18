using UnityEngine;
using System.Collections;

public class OnCollisionPlay : MonoBehaviour {
	public AudioClip[] soundClips;
	public float timeBetweenSounds = 0.1f;
	private float timer = 0;
	void OnCollisionEnter() {
		if (timer >= timeBetweenSounds) {
			timer = 0;
			this.GetComponent<AudioSource> ().clip = soundClips [Random.Range (0, soundClips.Length)];
			this.GetComponent<AudioSource> ().Play ();
		}
	}
	void Update() {
		timer += Time.deltaTime;
	}
}
