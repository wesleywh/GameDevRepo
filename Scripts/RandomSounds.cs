using UnityEngine;
using System.Collections;

public class RandomSounds : MonoBehaviour {


	[SerializeField] private float maxWaitTime;
	[SerializeField] private float minWaitTime;
	[SerializeField] private AudioClip[] soundsToPlay;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private bool playSingleShot = false;

	private float timer = 0.0f;
	private float chosenTime = 0.0f;

	void Start() {
		chosenTime = Random.Range (minWaitTime, maxWaitTime);
		if (audioSource == null) {
			audioSource = this.GetComponent<AudioSource> ();
		}
		if (playSingleShot == true) {
			audioSource.clip = soundsToPlay [Random.Range (0, soundsToPlay.Length)];
			audioSource.Play ();
		}
	}
	// Update is called once per frame
	void Update () {
		if (playSingleShot == false) {
			timer += Time.deltaTime;
			if (timer >= chosenTime) {
				timer = 0;
				chosenTime = Random.Range (minWaitTime, maxWaitTime);
				audioSource.clip = soundsToPlay [Random.Range (0, soundsToPlay.Length)];
				audioSource.Play ();
			}
		}
	}

	public void PlaySoundsOnce() {
		audioSource.clip = soundsToPlay [Random.Range (0, soundsToPlay.Length)];
		audioSource.Play ();
	}
}
