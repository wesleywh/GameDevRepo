using UnityEngine;
using System.Collections;

public class CoverSpot : MonoBehaviour {

	public string gizmoName = "position-marker.png";
	public Vector3 gizmoOffset = Vector3.zero;

	[Space(10)]
	public bool isTaken = false;
	public bool isDestroyed = false;
	public GameObject occupant = null;

	public void TakeSpot(GameObject person) {
		occupant = person;
		isTaken = true;
	}
	public void LeaveSpot(GameObject person){
		if (person == occupant) {
			occupant = null;
			isTaken = false;
		}
	}

	public void OnDrawGizmos(){
		Gizmos.DrawIcon (transform.position + gizmoOffset, gizmoName);
	}
}
