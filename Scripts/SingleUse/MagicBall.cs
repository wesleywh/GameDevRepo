using UnityEngine;
using System.Collections;

public class MagicBall : MonoBehaviour {
	[SerializeField] private float destroyDelay = 2.0f;
	[SerializeField] private bool disableOtherCameras = true;
	[SerializeField] private GameObject attachedCamera;
	[SerializeField] private GameObject visualAtEnd;
	[SerializeField] private string[] locationsToTravel;
	[SerializeField] private float[] movePercentages;
	[SerializeField] private float[] moveSpeeds;
	[SerializeField] private float stopNumber = 0;
	[SerializeField] private bool useOnEnable = true;
	[SerializeField] private bool useOnStart = false;
	private int i = 0;
	private float startTime = 0;
	private float journeyLength = 0;
	float fracJourney = 0;
	float distCovered = 0;
	private bool final = false;

	void OnEnable () {
		if (useOnEnable) {
			if (disableOtherCameras) {
				GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ().enabled = false;
			}
			startTime = Time.time;
			journeyLength = Vector3.Distance (this.transform.position, GameObject.Find(locationsToTravel [i]).transform.position);
		}
	}
	void OnStart() {
		if (useOnStart) {
			startTime = Time.time;
			journeyLength = Vector3.Distance (this.transform.position, GameObject.Find(locationsToTravel [i]).transform.position);
		}
	}
	void Update() {
		distCovered = (Time.time - startTime) * moveSpeeds[i];
		fracJourney =(float.IsNaN(distCovered / journeyLength)) ? 0 : distCovered / journeyLength;
		transform.position = Vector3.Lerp (transform.position, GameObject.Find(locationsToTravel [i]).transform.position, fracJourney);
		if (i == stopNumber && attachedCamera) {
			attachedCamera.transform.parent = null;
		}
		if (fracJourney >= 1.00f || fracJourney >= movePercentages [i]) {
			if (i + 1 < locationsToTravel.Length) {
				i = i + 1;
			} else {
				final = true;
			}
			startTime = Time.time;
			journeyLength = Vector3.Distance (this.transform.position, GameObject.Find(locationsToTravel [i]).transform.position);
		}
		if(final && (fracJourney >= 1.00f || fracJourney >= movePercentages [i]) ) {
			if (visualAtEnd) {
				Instantiate (visualAtEnd, transform.position, transform.rotation);
			}
			if (attachedCamera) {
				attachedCamera.transform.parent = null;
			}
			StartCoroutine (DelayDestroy ());
		}
	}
	IEnumerator DelayDestroy() {
		yield return new WaitForSeconds (destroyDelay);
		if (attachedCamera) {
			attachedCamera.GetComponent<Camera> ().enabled = false;
		}
		if (GameObject.FindGameObjectWithTag ("PlayerCamera")) {
			GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ().enabled = true;
		}
		Destroy (attachedCamera);
		Destroy (this.gameObject);
	}
}
