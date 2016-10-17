using UnityEngine;
using System.Collections;

public class hideorshow : MonoBehaviour {

	[SerializeField] private GameObject[] enterShow;
	[SerializeField] private GameObject[] exitShow;
	[SerializeField] private GameObject[] enterHide;
	[SerializeField] private GameObject[] exitHide;

	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") {
			foreach (GameObject obj in enterShow) {
				obj.SetActive (true);
			}
			foreach (GameObject obj in enterHide) {
				obj.SetActive (false);
			}
		}
	}
	void OnTriggerExit(Collider col) {
		if (col.tag == "Player") {
			foreach (GameObject obj in exitShow) {
				obj.SetActive (true);
			}
			foreach (GameObject obj in exitHide) {
				obj.SetActive (false);
			}
		}
	}
}
