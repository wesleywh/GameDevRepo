using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AI_Suspicious))]
[RequireComponent(typeof(AI_Controller))]
[RequireComponent(typeof(AI_Hostile))]
public class AI_Suspicious : MonoBehaviour {
	private Animator anim;
	private float timer = 0.0f;
	private bool drawImage = false;
	private Texture floatingImage = null;
	private bool moving = false;
	private Vector2 objPos = Vector2.zero;
	private float suspicionTimer = 0.0f;
	private Vector3 originalWanderPosition = Vector3.zero;
	private bool wandering = false;
	private float totalSuspicionTime = 0;
	[SerializeField] private float wanderRadius = 10.0f;
	[SerializeField] private float remainSuspicious = 20.0f;
	[SerializeField] private float movementSpeed = 2.0f;
	[SerializeField] private float reactionTime = 1.5f;
	[SerializeField] private Texture[] suspicionIcons;
	[SerializeField] private Vector2 overrideImageSize = Vector2.zero;
	[SerializeField] private Vector2 textureOffset = Vector2.zero;

	// Use this for initialization
	void OnEnable () {
		anim = this.GetComponent<AI_Controller> ().animator;
		anim.runtimeAnimatorController = this.GetComponent<AI_Controller> ().suspiciousController;
		InvokeRepeating ("CheckTarget", 0.0f, 0.05f);
	}
	void CheckTarget() {
		if (this.GetComponent<AI_Controller> ().canSeeTarget == true && this.GetComponent<AI_Controller> ().heardSound == false) {
			totalSuspicionTime = 0;
			timer += Time.deltaTime;
			MoveToTarget ();
			if (suspicionIcons.Length > 0) {
				drawImage = true;
			} 
			if (timer > reactionTime) {
				CancelInvoke ("CheckTarget");
				this.GetComponent<AI_Hostile>().enabled = true;
				this.GetComponent<AI_Suspicious>().enabled = false;
			}
		} else if (this.GetComponent<AI_Controller> ().heardSound == true && this.GetComponent<AI_Controller> ().canSeeTarget == false) {
			MoveToTarget ();
		} else {
			timer -= (timer > 0) ? Time.deltaTime : 0;
			if (timer <= 0) {
				drawImage = false;
			}
		}
	}
	void MoveToTarget() {
		if(moving == false) {
			this.GetComponent<NavMeshAgent> ().speed = movementSpeed;
			this.GetComponent<NavMeshAgent> ().SetDestination (this.GetComponent<AI_Controller>().currentTarget.transform.position);
		}
	}
	Vector3 WanderPoint () {
		Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * wanderRadius;
		randomDirection += originalWanderPosition;
		NavMeshHit navHit;
		NavMesh.SamplePosition (randomDirection, out navHit, wanderRadius, -1);
		return navHit.position;
	}
	void Update() {
		totalSuspicionTime += Time.deltaTime;
		if (totalSuspicionTime > remainSuspicious) {
			this.GetComponent<AI_Calm> ().enabled = true;
			this.GetComponent<AI_Suspicious> ().enabled = false;
		}
		if (drawImage) {
			objPos = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>().WorldToScreenPoint (this.transform.position);
			if (timer < reactionTime/4) {
				floatingImage = suspicionIcons [0];
				return;
			}
			else if (timer >= reactionTime/4 && timer < reactionTime / 3) {
				floatingImage = suspicionIcons [1];
			}
			else if (timer >= reactionTime / 3 && timer < reactionTime / 2) {
				floatingImage = suspicionIcons [2];
			}
			else if (timer >= reactionTime / 2 && timer < reactionTime) {
				floatingImage = suspicionIcons [3];
			} 
			else if (timer >= reactionTime) {
				floatingImage = suspicionIcons [4];
			}
		}
		if (moving == true && this.GetComponent<NavMeshAgent>().pathStatus == NavMeshPathStatus.PathComplete
			&& this.GetComponent<NavMeshAgent>().remainingDistance == 0 && drawImage == false) {
			moving = false;
			suspicionTimer = 0;
			wandering = true;
		}
		if (wandering && suspicionTimer < remainSuspicious) {
			suspicionTimer += Time.deltaTime;
			if (this.GetComponent<NavMeshAgent> ().pathStatus == NavMeshPathStatus.PathComplete
			   && this.GetComponent<NavMeshAgent> ().remainingDistance == 0) {
				this.GetComponent<NavMeshAgent> ().SetDestination (WanderPoint ());
			}
		}
	}
	void OnGUI() {
		if (drawImage) {
			if (overrideImageSize.x > 0 || overrideImageSize.y > 0) {
				GUI.DrawTexture (new Rect (objPos.x + textureOffset.x, (Screen.height - objPos.y) + textureOffset.y, overrideImageSize.x, overrideImageSize.y), floatingImage);
			} else {
				GUI.DrawTexture (new Rect (objPos.x + textureOffset.x, (Screen.height - objPos.y) + textureOffset.y, 20, 20), floatingImage);
			}
		}
	}
}
