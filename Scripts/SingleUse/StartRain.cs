using UnityEngine;
using System.Collections;

public class StartRain : MonoBehaviour {
	[SerializeField] private GameObject rain;
	[SerializeField] private float increaseVolSpeed = 2.0f;
	private bool playRainSounds = false;

	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") {
			rain.SetActive (true);
			playRainSounds = true;
		}
	}
	void Update() {
		if (playRainSounds) {
			rain.GetComponent<AudioSource> ().volume += (rain.GetComponent<AudioSource> ().volume + Time.deltaTime*increaseVolSpeed > 1) ? 1 : Time.deltaTime*increaseVolSpeed;
		}
	}
}
