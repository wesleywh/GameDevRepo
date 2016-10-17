using UnityEngine;
using System.Collections;

public class LOD : MonoBehaviour {

	private GameObject playerCamera;
	[SerializeField] private float[] distances;
	[SerializeField] private GameObject[] lodModels;
	private int current = -2;

	void Start() {
		playerCamera = GameObject.FindGameObjectWithTag ("PlayerCamera");
		for (int i = 0; i < lodModels.Length; i++) {
			lodModels [i].SetActive (false);
		}
	}
	void FixedUpdate() {
		playerCamera = GameObject.FindGameObjectWithTag ("PlayerCamera");
	}
	void Update () {
		if (playerCamera != null) {
			float curDis = Vector3.Distance (playerCamera.transform.position, transform.position);
			int level = -1;
			for (int i = 0; i < distances.Length; i++) {
				if (curDis < distances [i]) {
					level = i;
					i = distances.Length;
				}
			}
			if (level == -1) {
				level = distances.Length;
			}
			if (current != level) {
				ChangeLOD (level);
			}
		}
	}
	void ChangeLOD(int level){
		lodModels[level].SetActive(true);
		if( current >= 0) {
			lodModels[current].SetActive(false);
		}
		current = level;
	}
}
