using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

	[SerializeField] private float destroyAfterSeconds = 2.0f;

	// Use this for initialization
	void Start () {
		StartCoroutine (DestroyInSeconds ());
	}
	IEnumerator DestroyInSeconds() {
		yield return new WaitForSeconds(destroyAfterSeconds);
		Destroy(this.gameObject);
	}
}
