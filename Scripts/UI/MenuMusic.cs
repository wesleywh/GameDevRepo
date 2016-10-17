using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class MenuMusic : MonoBehaviour {

	private AudioSource audioSource;
	[SerializeField] private AudioClip menuMusic;

	void Start() {
		audioSource = this.GetComponent<AudioSource> ();
		EnableMenuMusic ();
	}
	void EnableMenuMusic() {
		audioSource.clip = menuMusic;
		audioSource.Play ();
	}
	void DisableMenuMusic() {
		audioSource.Stop ();
	}
}
