using UnityEngine;
using System;
using System.Collections;

[Serializable]
class MovePointList {
	public string movePointTag = null;
	public string movePointName = null;
	public Transform movePoint = null;
	public float speed = 1.0F;
	public float closeEnough = 0.1f;
}

public class MoveToPoints : MonoBehaviour {
	[SerializeField] private MovePointList[] movePoints = null;
	private Transform startMarker;
	private Transform endMarker;
	private float startTime;
	private float journeyLength;
	[SerializeField] private bool pingpong = true;
	private int index = 0;
	private bool forward = true;

	void Start() {
		MoveStart ();
	}
	private void MoveStart() {
		startTime = Time.time;
		startMarker = this.transform;
		if (string.IsNullOrEmpty (movePoints [index].movePointTag) == false) {
			endMarker = GameObject.FindGameObjectWithTag (movePoints [index].movePointTag).transform;
		} else if (string.IsNullOrEmpty (movePoints [index].movePointName) == false) {
			endMarker = GameObject.Find (movePoints [index].movePointName).transform;
		} else {
			endMarker = movePoints [index].movePoint;
		}
		journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
	}
	// Update is called once per frame
	void Update () {
		float distCovered = (Time.time - startTime) * movePoints[index].speed;
		float fracJourney = distCovered / journeyLength;
		transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
		if (Vector3.Distance (startMarker.position, endMarker.position) <= movePoints [index].closeEnough) {
			index = (forward == true) ? index + 1 : index - 1;
			if (index <= 0 || index >= movePoints.Length) {
				if (pingpong == true) {
					forward = !forward;
					index = (forward == true) ? index + 1 : index - 1;
					index = (forward == true) ? index + 1 : index - 1;
					MoveStart ();
				}
				else
					GetComponent<MoveToPoints> ().enabled = false;
			} else {
				MoveStart ();
			}
		}
	}
}
