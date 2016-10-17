using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LoopSounds : MonoBehaviour {

	[SerializeField] AudioClip[] sounds;	
	private AudioSource audioSource;

	void Start() {
		audioSource = this.GetComponent<AudioSource> ();
	}
	// Update is called once per frame
	void Update () {
		if (!audioSource.isPlaying) {
			audioSource.clip = sounds [Random.Range (0, sounds.Length)];
			audioSource.Play ();
		}
	}
}
