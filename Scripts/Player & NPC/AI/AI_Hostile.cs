using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AI_Suspicious))]
[RequireComponent(typeof(AI_Controller))]
[RequireComponent(typeof(AI_Hostile))]
public class AI_Hostile : MonoBehaviour {
	private Animator anim = null;
	private float timer = 0;
	private bool attacking = false;
//	private bool drawImage = false;
	private float attackTimer = 0;
	private float lostTimer = 0;
	[Header("This is for your animator")]
	[SerializeField] private float[] attackNumbers;
	[SerializeField] private float remainHostile = 20.0f;
	[SerializeField] private float attackDistance = 1.2f;
	[SerializeField] private float timeBetweenAttacks = 1.5f;
	[SerializeField] private float moveSpeed = 4.0f;
	void OnEnable() {
		anim = this.GetComponent<AI_Controller> ().animator;
		anim.runtimeAnimatorController = this.GetComponent<AI_Controller> ().hostileController;
		this.transform.LookAt (this.GetComponent<AI_Controller> ().currentTarget.transform);
		InvokeRepeating ("CheckTarget", 0.05f, 0.05f);
	}
	void CheckTarget() {
		if (this.GetComponent<AI_Controller> ().canSeeTarget == true && this.GetComponent<AI_Controller> ().heardSound == false) {
			RunToTarget (this.GetComponent<AI_Controller> ().currentTarget.transform.position);
		} else if (this.GetComponent<AI_Controller> ().heardSound == true && this.GetComponent<AI_Controller> ().canSeeTarget == false) {
			this.transform.LookAt (this.GetComponent<AI_Controller> ().currentTarget.transform);
			RunToTarget (this.GetComponent<AI_Controller> ().hearTarget.transform.position);
			CancelInvoke ("CheckTarget");
		} else {
			timer -= (timer > 0) ? Time.deltaTime : 0;
			if (timer <= 0) {
//				drawImage = false;
			}
		}
	}
	void RunToTarget(Vector3 target) {
		if (attacking == false) {
			this.transform.LookAt (target);
			this.GetComponent<NavMeshAgent> ().speed = moveSpeed;
			this.GetComponent<NavMeshAgent> ().SetDestination (target);
		}
	}
	void AttackTarget() {
		if (attackTimer <= 0) {
			this.transform.LookAt (this.GetComponent<AI_Controller> ().currentTarget.transform);
			attackTimer = timeBetweenAttacks;
			attacking = true;
			anim.SetFloat ("attackNumber", attackNumbers[Random.Range(0,attackNumbers.Length)]);
			anim.SetTrigger ("attack");
		}
	}
	float TargetDistance() {
		return Vector3.Distance (this.transform.position, this.GetComponent<AI_Controller> ().currentTarget.transform.position);
	}
	void Update() {
		if (attackTimer > 0) {
			attackTimer = (attackTimer - Time.deltaTime > 0) ? attackTimer - Time.deltaTime : 0;
		}
		if (this.GetComponent<AI_Controller> ().canSeeTarget) {
			this.transform.LookAt (this.GetComponent<AI_Controller> ().currentTarget.transform);
			if (TargetDistance () < attackDistance) {
				attacking = true;
				lostTimer = 0;
				AttackTarget ();
			} else { 
				attacking = false;
				lostTimer += Time.deltaTime;
				if (lostTimer >= remainHostile) {
					this.GetComponent<AI_Suspicious> ().enabled = true;
					this.GetComponent<AI_Hostile> ().enabled = false;
				}
			}
		}
	}
}