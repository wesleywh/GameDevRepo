using UnityEngine;
using System.Collections;

public class LockView : MonoBehaviour {
	public Transform LookAtTarget;
	// Update is called once per frame
	void Update () {
		this.transform.LookAt(LookAtTarget);
	}
}
