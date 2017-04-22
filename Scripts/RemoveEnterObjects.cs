using UnityEngine;
using System.Collections;

public class RemoveEnterObjects : MonoBehaviour {

	//requires rigidbody
	void OnTriggerEnter(Collider col) {
		Destroy (col.transform.gameObject);
	}
}
