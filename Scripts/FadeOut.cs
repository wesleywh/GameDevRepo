using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {

	public bool start = false;
	public float life = 1.0f;
	public float fadeSpeed = 0.3f;
	private bool turnOff = false;
	private Color startC;
	private Color endC;
	private float alpha = 1.0f;

	void Start() {
		startC = Color.black;
		endC = Color.white;
	}
	// Update is called once per frame
	void Update () {
		if (start == true) {
			start = false;
			StartCoroutine(StartFade ());
		}
		if (turnOff == true) {
			alpha -= Time.deltaTime * fadeSpeed;
			startC.a = alpha;
			endC.a = alpha;
//			this.GetComponent<LineRenderer> ().SetColors (startC, endC);
            this.GetComponent<LineRenderer> ().startColor = startC;
            this.GetComponent<LineRenderer> ().endColor = endC;
			if (alpha <= 0) {
				Destroy (this.gameObject);
			}
		}
	}

	IEnumerator StartFade() {
		yield return new WaitForSeconds(life);
		turnOff = true;
	}
}
