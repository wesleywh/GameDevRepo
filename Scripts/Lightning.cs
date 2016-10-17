using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {

	[SerializeField] private float maxWaitTime;
	[SerializeField] private float minWaitTime;
	[SerializeField] private AudioClip[] thunderSounds;
	[SerializeField] private Light lightSource;
	[SerializeField] private float intensity = 1.0f;
	[SerializeField] private AudioSource audioSource;

	private float timer = 0.0f;
	private float chosenTime = 0.0f;

	void Start() {
		chosenTime = Random.Range (minWaitTime, maxWaitTime);
		if (audioSource == null) {
			audioSource = this.GetComponent<AudioSource> ();
		}
	}
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= chosenTime) {
			timer = 0;
			chosenTime = Random.Range (minWaitTime, maxWaitTime);
			audioSource.clip = thunderSounds [Random.Range (0, thunderSounds.Length)];
			audioSource.Play ();
			lightSource.intensity = intensity;
		}
		if (lightSource.intensity > 0) {
			lightSource.intensity -= Time.deltaTime;
		}
	}
}
