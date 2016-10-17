using UnityEngine;
using System.Collections;

public class WormAI : MonoBehaviour {
	[SerializeField] private float delayAIStart = 0;
	[SerializeField] private float meleeAttackDistance = 4.0f;
	[SerializeField] private float fieldOfView = 145f;
	[SerializeField] private float sightDistance = 50f;
	[SerializeField] private string wanderPointTag = "";
	[SerializeField] private float minScreamTime = 5.0f;
	[SerializeField] private float maxScreamTime = 15.0f;
	[SerializeField] private float screamCamShakeDist = 10.0f;
	[SerializeField] private float shakeAmount = 5.0f;
	[SerializeField] private float shakeTime = 2.0f;
	[SerializeField] private AudioClip[] screamClip;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip[] attackSounds;
	[SerializeField] private float maxDamage = 25;
	[SerializeField] private float minDamage = 15;
	[SerializeField] private bool alwaysMaxDamage = false;
	[SerializeField] private GameObject groundHitVFX;

	private float screamAt = 0;
	private float screamTimer = 0;
	private bool begin = false;
	private float timer = 0;
	private GameObject player;
	private Vector3 targetPosition = Vector3.zero;
	private float speed = 0;
	private float lastRotation = 90;								//for left right direction detection
	private float normalizedRotation = 0.0f;
	private float curDir = 0.0f;
	private Vector3 lastPos = Vector3.zero;
	private GameObject[] wanderPoints;

	void Start() {
		wanderPoints = GameObject.FindGameObjectsWithTag (wanderPointTag);
		screamAt = Random.Range (minScreamTime, maxScreamTime);
	}
	void Update () {
		if (begin == false) {
			timer += Time.deltaTime;
		}
		screamTimer += Time.deltaTime;
		if (screamTimer >= screamAt) {
			if (audioSource) {
				audioSource.clip = screamClip[Random.Range(0, screamClip.Length)];
				audioSource.Play ();
			}
			if (GameObject.FindGameObjectWithTag ("Player") && Vector3.Distance (transform.position, GameObject.FindGameObjectWithTag ("Player").transform.position) < screamCamShakeDist) {
				if (GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<CameraShake> ().isShaking () == false) {
					GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<CameraShake> ().ShakeCamera (shakeAmount, shakeTime);
				}
			}
			screamTimer = 0;
		}
		if (timer > delayAIStart) {
			begin = true;
		}
		if (begin) {
			if (SeePlayer ()) {
				GetComponent<NavMeshAgent> ().destination = targetPosition;
			} else {
				float dist=GetComponent<NavMeshAgent>().remainingDistance;
				if (dist!=Mathf.Infinity && GetComponent<NavMeshAgent>().pathStatus==NavMeshPathStatus.PathComplete && 
					GetComponent<NavMeshAgent>().remainingDistance==0) {
					GetComponent<NavMeshAgent> ().destination =  wanderPoints[Random.Range(0, wanderPoints.Length)].transform.position;
				}
			}
			if (GameObject.FindGameObjectWithTag ("Player") && Vector3.Distance (GameObject.FindGameObjectWithTag ("Player").transform.position,
				    transform.position) < meleeAttackDistance) {
				GetComponent<Animator> ().SetBool ("attack", true);
			} else {
				GetComponent<Animator> ().SetBool ("attack", false);
			}

			normalizedRotation = (transform.rotation.eulerAngles.y - lastRotation > 1) ? 1 : transform.rotation.eulerAngles.y - lastRotation;
			if (curDir != normalizedRotation) 
			{
				curDir += (normalizedRotation > curDir) ? Time.deltaTime * 0.1f : 0;
				curDir -= (normalizedRotation < curDir) ? Time.deltaTime * 0.1f : 0;
			}
			GetComponent<Animator> ().SetFloat ("direction", curDir);
			lastRotation = transform.rotation.eulerAngles.y;
			speed = Vector3.Distance (lastPos, this.transform.position) / Time.deltaTime;
			lastPos = this.transform.position;
			GetComponent<Animator> ().SetFloat ("speed", speed);
		}
	}

	bool SeePlayer()
	{
		bool targetInSight = false;
		// Create a vector from the enemy to the player and store the angle between it and forward.
		if (!GameObject.FindGameObjectWithTag ("Player")) {
			return false;
		}
		Vector3 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
		float angle = Vector3.Angle (direction, transform.forward);	

		// If the angle between forward and where the player is, is less than half the angle of view...
		if (angle < fieldOfView * 0.5f) {
			RaycastHit hit;

			// ... and if a raycast towards the player hits something...
			if (Physics.Raycast (this.transform.position + this.transform.up, direction.normalized, out hit, sightDistance)) {
				// ... and if the raycast hits the player...
				if (hit.collider.gameObject.tag == "Player") {
					// ... the player is in sight.
					targetInSight = true;

					// Set the last sighting is the players current position.
					targetPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
				}
			}
		}
		return targetInSight;
	}
	void AttackPlayer() {
		if (audioSource) {
			audioSource.clip = attackSounds [Random.Range (0, attackSounds.Length)];
			audioSource.Play ();
		}
		if (GameObject.FindGameObjectWithTag ("Player") && Vector3.Distance (transform.position, GameObject.FindGameObjectWithTag ("Player").transform.position) < meleeAttackDistance) {
			if (alwaysMaxDamage) {
				GameObject.FindGameObjectWithTag ("Player").GetComponent<Health> ().ApplyDamage (maxDamage);
			} else {
				GameObject.FindGameObjectWithTag ("Player").GetComponent<Health> ().ApplyDamage (Random.Range(minDamage, maxDamage));
			}
			if (groundHitVFX) {
				Instantiate (groundHitVFX, GameObject.FindGameObjectWithTag ("Player").transform.position, Quaternion.identity);
			}
		}
	}
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position, meleeAttackDistance);

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (transform.position, screamCamShakeDist);
	}
}
