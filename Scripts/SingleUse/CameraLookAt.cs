using UnityEngine;
using System.Collections;

public class CameraLookAt : MonoBehaviour {

	[SerializeField] private GameObject target;

	void Update () {
		if (target) {
			this.transform.LookAt (target.transform);
		}
	}
}
