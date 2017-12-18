 using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	public AudioSource currentTargetSource;
	public AudioSource audioSource2;
	public AudioClip[] clips;
	public float fadeTime;
	public bool triggerCrossFadeToTrack;
	public AudioClip debugTrack;
	private bool originalTrigger;
	private AudioSource current;
	private AudioSource previous;
    [Range(0,1)]
    public float maxVolume = 1.0f;
	void Start() {
		originalTrigger = triggerCrossFadeToTrack;
		current = currentTargetSource;
		previous = audioSource2;
	}
	void Update() {
		if (originalTrigger != triggerCrossFadeToTrack) {
			originalTrigger = triggerCrossFadeToTrack;
			CrossFadeToTrack (debugTrack);
		}
	}
	public void CrossFadeToIndex(int index) {
		AudioSource newAudio = GetSource (false);
		newAudio.volume = 0.0f;
		newAudio.clip = clips[index];
		newAudio.Play ();
		AudioSource original = GetSource (true);
		StartCoroutine (CrossFade (newAudio, original));
	}

	public void CrossFadeToTrack(AudioClip clip) {
		AudioSource newAudio = GetSource (false);
		newAudio.volume = 0.0f;
		newAudio.clip = clip;
		newAudio.Play ();
		AudioSource original = GetSource (true);
		StartCoroutine (CrossFade (newAudio, original));
		previous = original;
		current = newAudio;
	}
	private AudioSource GetSource(bool getCurrentSource) {
		if (getCurrentSource == true) {
			return current;
		} else {
			return previous;
		}
	}

	IEnumerator CrossFade(AudioSource newSource, AudioSource oldSource) {
		float timer = 0.0f;
		while (timer < fadeTime) {
            newSource.volume = Mathf.Lerp (0.0f, maxVolume, timer / fadeTime);
            oldSource.volume = maxVolume - newSource.volume;

			timer += Time.deltaTime;
			yield return null;
		}
		oldSource.clip = null;
		oldSource.Stop ();
        newSource.volume = maxVolume;
	}
}
