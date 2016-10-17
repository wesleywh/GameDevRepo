using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AI_Suspicious))]
[RequireComponent(typeof(AI_Controller))]
[RequireComponent(typeof(AI_Hostile))]
public class AI_Calm : MonoBehaviour {
	
	private Animator anim;
	[SerializeField] private float timer = 0;
	private bool drawImage = false;
	private Vector3 objPos = Vector3.zero;
	private Texture floatingImage = null;
	[SerializeField] private float reactionTime = 4.0f;
	[SerializeField] private float moveSpeed = 1.0f;
	[SerializeField] private Texture[] noticeIcons;
	[SerializeField] private Vector2 overrideImageSize = Vector2.zero;
	[SerializeField] private Vector2 textureOffset = Vector2.zero;
	[SerializeField] private bool moveInOrder = false;
	[SerializeField] private GameObject[] movePoints;
	[SerializeField] private float waitBetweenPoints = 1.0f;
	private int moveInt = 0;
	private bool moving = false;
	private float waitTimer = 0.0f;

	void Start () {
		timer = 0;
		if (anim == null) {
			anim = this.GetComponent<AI_Controller> ().animator;
		}
	}
	void OnEnable() {
		timer = 0;
		anim = this.GetComponent<AI_Controller> ().animator;
		anim.runtimeAnimatorController = this.GetComponent<AI_Controller>().calmController;
		InvokeRepeating ("CheckTarget", 0.05f, 0.05f);
		MoveToTarget ();
	}
	void OnDisable() {
		CancelInvoke ("CheckTarget");
	}
	void CheckTarget() {
		if (this.GetComponent<AI_Controller> ().canSeeTarget == true && this.GetComponent<AI_Controller> ().heardSound == false) {
			timer += Time.deltaTime;
			if (noticeIcons.Length > 0) {
				drawImage = true;
			} 
			if (timer > reactionTime) {
				CancelInvoke ("CheckTarget");
				this.GetComponent<AI_Suspicious> ().enabled = true;
				this.GetComponent<AI_Calm>().enabled = false;
			}
		} else if (this.GetComponent<AI_Controller> ().heardSound == true && this.GetComponent<AI_Controller> ().canSeeTarget == false) {
			timer = 0;
			CancelInvoke ("CheckTarget");
			//enable suspicious state!
			this.GetComponent<AI_Calm>().enabled = false;
		} else {
			timer -= (timer > 0) ? Time.deltaTime : 0;
			if (timer <= 0) {
				drawImage = false;
			}
		}
	}
	void MoveToTarget() {
		if(moving == false) {
			this.GetComponent<NavMeshAgent> ().speed = moveSpeed;
			this.GetComponent<NavMeshAgent> ().SetDestination (movePoints[moveInt].transform.position);
			moving = true;
			if (moveInOrder) {
				moveInt = (moveInt < movePoints.Length - 1) ? moveInt + 1 : 0;
			} else {
				moveInt = Random.Range (0, movePoints.Length);
			}
		}
	}
	void Update() {
		if (drawImage) {
			objPos = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>().WorldToScreenPoint (this.transform.position);
			if (timer < reactionTime/4) {
				floatingImage = noticeIcons [0];
				return;
			}
			else if (timer >= reactionTime/4 && timer < reactionTime / 3) {
				floatingImage = noticeIcons [1];
			}
			else if (timer >= reactionTime / 3 && timer < reactionTime / 2) {
				floatingImage = noticeIcons [2];
			}
			else if (timer >= reactionTime / 2 && timer < reactionTime) {
				floatingImage = noticeIcons [3];
			} 
			else if (timer >= reactionTime) {
				floatingImage = noticeIcons [4];
			}
		}
		if (moving == true && this.GetComponent<NavMeshAgent>().pathStatus == NavMeshPathStatus.PathComplete
			&& this.GetComponent<NavMeshAgent>().remainingDistance == 0 && drawImage == false) {
			waitTimer += Time.deltaTime;
			if (waitTimer >= waitBetweenPoints) {
				waitTimer = 0;
				moving = false;
				MoveToTarget ();
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
