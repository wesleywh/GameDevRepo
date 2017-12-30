using UnityEngine;
using System.Collections;

public class hideorshow : MonoBehaviour {

	[SerializeField] private GameObject[] enterShow;
	[SerializeField] private GameObject[] exitShow;
	[SerializeField] private GameObject[] enterHide;
	[SerializeField] private GameObject[] exitHide;
//    [SerializeField] private float duration = 1.0f;
//    [SerializeField] private float alpha = 0;

//    private bool perform_fade = false;

//    private int completed = 0;
//    private int total = 0;

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
