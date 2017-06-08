using UnityEngine;
using System.Collections;
using TeamUtility.IO;

public class DisplayFloatingImage : MonoBehaviour {

	[SerializeField] private float distance = 5.0f;
	[SerializeField] private Texture icon;
	[SerializeField] private Vector3 offset = Vector3.zero;
	[SerializeField] Vector2 size = new Vector2(20,20);
	[SerializeField] private bool hideWithActionPress = true;
	private bool showTexture = false;
	private Vector3 objPos = Vector3.zero;
	private bool hiding = false;

	void Update () {
		if (GameObject.FindGameObjectWithTag ("Player") &&
		    Vector3.Distance (transform.position, GameObject.FindGameObjectWithTag ("Player").transform.position) < distance) {
			if (hiding == false) {
				showTexture = true;
			}
			Camera cam = GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ();
			objPos = cam.WorldToScreenPoint (this.transform.position);
		} else {
			showTexture = false;
			hiding = false;
		}
		if (showTexture && hideWithActionPress && InputManager.GetButtonDown ("Action")) {
			showTexture = false;
			hiding = true;
		}
	}
	void OnGUI() {
		if (showTexture) {
			GUI.DrawTexture (new Rect (objPos.x + offset.x,(Screen.height - objPos.y) + offset.y, size.x, size.y), icon);
		}
	}
}
