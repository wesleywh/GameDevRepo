using UnityEngine;
using System.Collections;

public class VoiceController : MonoBehaviour {

	[SerializeField] private AudioClip[] hostileSpottedCall = null;
	[SerializeField] private AudioClip[] attackCalls = null;
	[SerializeField] private AudioClip[] calmVoiceSets = null;
	[SerializeField] private AudioClip[] calmToSuspicious= null;
	[SerializeField] private AudioSource enemySpottedAudioSource = null;
	[SerializeField] private AudioSource normalAudioSource = null;

	[SerializeField] private bool overrideAudio = false;

	public  void PlayHostileSpottedCall() {
		if (overrideAudio == false && enemySpottedAudioSource.isPlaying == false) {
			if (hostileSpottedCall.Length > 0) {
				enemySpottedAudioSource.clip = hostileSpottedCall [Random.Range (0, hostileSpottedCall.Length)];
				enemySpottedAudioSource.Play ();
			}
		} else {
			if (hostileSpottedCall.Length > 0) {
				enemySpottedAudioSource.clip = hostileSpottedCall [Random.Range (0, hostileSpottedCall.Length)];
				enemySpottedAudioSource.Play ();
			}
		}
	}

	public  void PlayAttackVoiceCall() {
		if (overrideAudio == false && normalAudioSource.isPlaying == false) {
			if (attackCalls.Length > 0) {
				normalAudioSource.clip = attackCalls [Random.Range (0, attackCalls.Length)];
				normalAudioSource.Play ();
			}
		} else {
			if (attackCalls.Length > 0) {
				normalAudioSource.clip = attackCalls [Random.Range (0, attackCalls.Length)];
				normalAudioSource.Play ();
			}
		}
	}
	public void PlayCalmVoiceSet() {
		if (overrideAudio == false && normalAudioSource.isPlaying == false) {
			if (calmVoiceSets.Length > 0) {
				normalAudioSource.clip = calmVoiceSets [Random.Range (0, calmVoiceSets.Length)];
				normalAudioSource.Play ();
			}
		} else {
			if (calmVoiceSets.Length > 0) {
				normalAudioSource.clip = calmVoiceSets [Random.Range (0, calmVoiceSets.Length)];
				normalAudioSource.Play ();
			}
		}
	}

	public void PlayCalmToSuspiciousVoiceSet() {
		if (overrideAudio == false && normalAudioSource.isPlaying == false) {
			if (calmToSuspicious.Length > 0) {
				normalAudioSource.clip = calmToSuspicious [Random.Range (0, calmToSuspicious.Length)];
				normalAudioSource.Play ();
			}
		} else {
			if (calmToSuspicious.Length > 0) {
				normalAudioSource.clip = calmToSuspicious [Random.Range (0, calmToSuspicious.Length)];
				normalAudioSource.Play ();
			}
		}
	}
}
