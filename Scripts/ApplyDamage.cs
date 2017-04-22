using UnityEngine;
using System.Collections;

public class ApplyDamage : MonoBehaviour {

	private GameObject target = null;
	public bool targetPlayerOnly = true;
	public float damagePerTime = 1;
	public float timeInSeconds = 1;
	private float timer = 0;

	//requires rigidbody
	void OnTriggerEnter(Collider col) {
		if (targetPlayerOnly == true) {
			if (col.gameObject.tag == "Player") {
				target = col.transform.gameObject;	
			}
		} else {
			target = col.transform.gameObject;
		}
	}

	//requires rigidbody
	void OnTriggerExit(Collider col) {
		if (col.transform.gameObject == target) {
			target = null;
		}
	}
	void Update() {
		timer += Time.deltaTime;
		if (target != null && target.GetComponent<Health> () && timer >= timeInSeconds) {
			timer = 0;
			target.GetComponent<Health> ().ApplyDamage (damagePerTime);
		}
	}
}
