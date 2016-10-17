using UnityEngine;
using System.Collections;

public class HideableWall : MonoBehaviour {
	[SerializeField] private bool crouchWhenHiding = false;
	[Header("Recommended You Use A Box Collider (As Trigger)")]
	[SerializeField] private Collider actionTrigger;
	[SerializeField] private Transform[] locToHide;

	private GameObject player;
	private GameObject closestWall;
	private float buttonPress = 0.0f;
	void Start() {
		if (actionTrigger == null) {
			actionTrigger = this.GetComponent<BoxCollider> ();
		}
		this.gameObject.layer = 2;	//ignore raycast
	}
	void Update() {
		if (Input.GetButton ("Action")) {
			buttonPress += Time.deltaTime;
		} else {
			buttonPress = 0;
		}
	}
	void OnTriggerStay(Collider col) {
		if (Input.GetButtonUp ("Action") && closestWall != null && buttonPress < 0.5f) {
			GetOffWall ();
			return;
		}
		if (buttonPress > 0.5f && col.transform.gameObject.tag == "Player" && closestWall == null) {
			player = col.transform.gameObject;
			Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, 3.0f);
			closestWall = null;
			RaycastHit hit;
			foreach (Collider obj in hitColliders) {
				if ( (obj.name == "Wall" || obj.name == "wall") ){
					if (closestWall == null) {
						if (Physics.Raycast (player.transform.position, (obj.transform.position - player.transform.position).normalized, out hit, 4.0f)) {
							closestWall = hit.transform.gameObject;
						}
					} else {
						if (Physics.Raycast (player.transform.position, (obj.transform.position - player.transform.position), out hit, 3.0f)) {
							if (Vector3.Distance (player.transform.position, hit.point) < Vector3.Distance (player.transform.position, closestWall.transform.position)) {
								closestWall = hit.transform.gameObject;
							}
						}
					}
				}
			}
			if (closestWall != null) {
				GetOnWall ();
			}
		}
	}
	void OnTriggerExit(Collider col) {
		if (col.transform.gameObject == player) {
			GetOffWall ();
		}
	}
	void GetOnWall() {
		player.transform.LookAt (closestWall.transform);
		int type = (crouchWhenHiding == true) ? 0 : 1;
		player.GetComponent<Animator> ().SetInteger ("wall_type", type);
		player.GetComponent<Animator> ().SetBool ("OnWall", true);
		player.transform.position = FindClosestHideLocation ().position;
		player.transform.rotation = Quaternion.LookRotation (FindClosestHideLocation ().forward);
		player.GetComponent<MovementController> ().moveSideOnly = true;
		player.GetComponent<MovementController> ().canJump = false;
		player.GetComponentInChildren<MouseLook> ().enabled = false;
	}
	void GetOffWall() {
		player.GetComponent<Animator> ().SetBool ("OnWall", false);
		player.GetComponent<MovementController> ().moveSideOnly = false;
		player.GetComponent<MovementController> ().canJump = true;
		player.GetComponentInChildren<MouseLook> ().enabled = true;
		closestWall = null;
	}
	Transform FindClosestHideLocation() {
		Transform closestHide = null;
		foreach (Transform hide in locToHide) {
			if (closestHide == null) {
				closestHide = hide;
			}
			else {
				if(Vector3.Distance(player.transform.position, hide.position) < 
					Vector3.Distance(player.transform.position, closestHide.position) )
				{
					closestHide = hide;
				}
			}
		}
		return closestHide;
	}
}
