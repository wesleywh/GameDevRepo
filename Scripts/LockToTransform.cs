using UnityEngine;
using System.Collections;

public class LockToTransform : MonoBehaviour {

	public Transform lockTransform = null;
	
	// Update is called once per frame
	void Update () {
		if (lockTransform != null) {
			this.transform.position = lockTransform.position;
			this.transform.rotation = lockTransform.rotation;
		}
	}
}
