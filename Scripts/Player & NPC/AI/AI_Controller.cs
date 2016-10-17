using UnityEngine;
using System.Collections;
using System.Collections.Generic;				//for lists

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AI_Suspicious))]
[RequireComponent(typeof(AI_Controller))]
[RequireComponent(typeof(AI_Hostile))]
public class AI_Controller : MonoBehaviour {
	private enum StartingState { 
		Calm,Suspicious,Sneak,Hostile
	}
	[SerializeField] StartingState startingState = StartingState.Calm;
	public Animator animator = null;
	public RuntimeAnimatorController calmController = null;
	public RuntimeAnimatorController sneakController = null;
	public RuntimeAnimatorController suspiciousController = null;
	public RuntimeAnimatorController hostileController = null;
	[HideInInspector] public GameObject lastSeenTarget = null;
	[HideInInspector] public GameObject currentTarget = null;
	[HideInInspector] public GameObject hearTarget = null;
	[HideInInspector] public bool canSeeTarget = false;
	[HideInInspector] public bool heardSound = false;
	[Space(10)]
	public string[] enemyTags;
	public string[] soundTags;
	[SerializeField] private float hearingDistance = 15.0f;
	[SerializeField] private float sightDistance = 30.0f;
	[SerializeField] private float fieldOfView = 130.0f;
	[Space(10)]
	[SerializeField] private bool DebugHearingDistance = false;
	[SerializeField] private bool DebugVisualDistance = false;
	[SerializeField] private bool DebugLineOfSight = false;

	private List<GameObject> targetList = new List<GameObject> ();

	void Start () {
		this.GetComponent<AI_Calm> ().enabled = false;
		this.GetComponent<AI_Suspicious> ().enabled = false;
		this.GetComponent<AI_Hostile> ().enabled = false;
		switch (startingState) {
			case StartingState.Calm:
			this.GetComponent<AI_Calm> ().enabled = true;
				break;
			case StartingState.Hostile:
			this.GetComponent<AI_Hostile> ().enabled = true;
				break;
			case StartingState.Sneak:
				break;
			case StartingState.Suspicious:
				this.GetComponent<AI_Suspicious> ().enabled = true;
				break;
		}
		InvokeRepeating ("checkForSounds", 0.04f, 0.05f);
		InvokeRepeating ("checkForTargets", 0.05f, 0.05f);
		InvokeRepeating ("UpdateTargetList", 0.01f, 60.0f);
	}
	void UpdateTargetList() 
	{
		targetList.Clear ();
		for(int i =0; i<enemyTags.Length; i++) {
			targetList.AddRange(GameObject.FindGameObjectsWithTag(enemyTags[i]));
		}
	}

	void checkForTargets()
	{
		canSeeTarget = false;
		foreach (GameObject target in targetList) {
			Vector3 direction = target.transform.position - this.transform.position;
			float angle = Vector3.Angle (direction, transform.forward);	
			if (angle < fieldOfView * 0.5f) {
				RaycastHit hit;
				if (Physics.Raycast (this.transform.position + this.transform.up, direction.normalized, out hit, sightDistance)) {
					if (DebugLineOfSight == true) {
						Debug.DrawLine(this.transform.position, hit.collider.gameObject.transform.position, Color.red);
						Debug.Log (hit.collider.gameObject.tag + " vs " + target.tag);
					}
					if (hit.collider.gameObject.tag == target.tag) {
						canSeeTarget = true;
						currentTarget = hit.collider.gameObject;
						return;
					}
				}
			}
		}
		currentTarget = null;
	}

	void checkForSounds()
	{
		bool soundExists = false;
		for (int i = 0; i < soundTags.Length; i++) {
			if (GameObject.FindGameObjectWithTag (soundTags [i])) {
				soundExists = true;
				break;
			}
		}
		if (soundExists == false) {
			hearTarget = null;
			return;
		}
		else {
			for (int i = 0; i < soundTags.Length; i++) {
				GameObject[] sounds = GameObject.FindGameObjectsWithTag (soundTags [i]);
				foreach (GameObject sound in sounds) {
					float dist = Vector3.Distance (sound.transform.position, this.transform.position);
					if (dist < hearingDistance) {
						hearTarget = sound;
						return;
					}
				}
			}
		} 
	}

	void OnDrawGizmosSelected() {
		if (DebugHearingDistance == true) {	//draws sphere of hearing
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (transform.position, hearingDistance);
		}
		if (DebugVisualDistance == true) { //draws cone of vision
			float totalFOV = fieldOfView;
			float rayRange = sightDistance;
			float halfFOV = totalFOV / 2.0f;
			Gizmos.color = Color.blue;
			Quaternion leftRayRotation = Quaternion.AngleAxis( -halfFOV, Vector3.up );
			Quaternion rightRayRotation = Quaternion.AngleAxis( halfFOV, Vector3.up );
			Vector3 leftRayDirection = leftRayRotation * transform.forward;
			Vector3 rightRayDirection = rightRayRotation * transform.forward;
			Gizmos.DrawRay( transform.position, leftRayDirection * rayRange );
			Gizmos.DrawRay( transform.position, rightRayDirection * rayRange );
		}
	}
}
