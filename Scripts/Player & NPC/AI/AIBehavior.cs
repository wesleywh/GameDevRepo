///////////////////////////////////////////////////////////////
///															///
/// 		Written By Wesley Haws April 2016				///
/// 				Tested with Unity 5						///
/// 	Anyone can use this for any reason. No limitations. ///
/// 														///
/// This is the main script to give life to your gameobject.///
/// This is capable of the following:						///
/// *Patrolling waypoints									///
/// *Hide and Shooting										///
/// *Listening for tagged sounds							///
/// *Visually detecting tags in a cone of vision			///
/// *Pathfinding											///
/// *Sending Damage											///
/// *Audio Warning	(Calling For Help)						///
/// *Melee or Distance Attacking AI							///
/// *Random Investigation Based On Suspicion				///
/// *Shoot with level of accuracy specified					///
/// *Particle effects at hit locations based on tags		///
/// 														///
///////////////////////////////////////////////////////////////		

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;			//for adding to arrays
//using System.Threading;						//possibly un-used?

//For AI Memory -- Descision Making
public class AIMemory {
	public Vector3 lastTargetPosition = Vector3.zero;
	public bool seeTarget = false;
	public string currentState = "Idle";
	public bool wasInCover = false;
	public Vector3 cover = Vector3.zero;
	public bool respondingToHelp = false;
}
[Serializable]
class Particle {
	public string tag = "";
	public GameObject particle = null;
	public float timeToLive = 0.2f;
}

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (NavMeshAgent))]	//require navmesh component
[RequireComponent (typeof (AudioSource))]	//require audio source
public class AIBehavior : MonoBehaviour {
	enum Dropdown // your custom enumeration
	{
		Good, //will not attack players, good, or neutral but will -- attack evil and zombie
		Evil, //will not attack evil or Neutral, but will -- attack players, good, and zombies
		Zombie, // attack everyone that isn't a zombie
		Neutral, //will only attack zombies
		NoneReactive //will not do anything and not react any any other class
	};
	enum States
	{
		Idle,
		Patrol,
		Suspicious,
		Hostile
	};

	[SerializeField] private Dropdown classGroup = Dropdown.Neutral; //default AI (Neutral)
	[SerializeField] private States startingState = States.Idle;	//Where to start in Behavior Tree

	[Header("Identifying other NPCs or Players", order=0)]
	[Space(-10, order=1)]
	[Header("based on custom tags you made",order=2)]
	[Space(10,order=3)]
	[SerializeField] private string playerTag = "Player";			//Tag to ID Players
	[SerializeField] private string goodTag = "Good";				//Tag to ID good AI
	[SerializeField] private string evilTag = "Evil";				//Tag to ID evil AI
	[SerializeField] private string neutralTag = "Neutral";			//Tag to ID neutral AI
	[SerializeField] private string zombieTag = "Zombie";			//Tag to ID zombie AI

	[Header("Set the AI visual Senses", order=0)]
	[Space(10,order=1)]
	[SerializeField] private float fieldOfView = 110;				//controls AI cone of sight
	[SerializeField] private float sightDistance = 20;				//How far the AI can see

	[Header("Set how well the AI will react", order=0)]
	[Space(-10,order=1)]
	[Header("and move based on settings here.", order=2)]
	[Space(10, order=3)]
	[SerializeField] private float idleWaitTime = 5.0f;				//The Amount of time the AI will site in one place.
	[SerializeField] private bool randomlyWalk = true;				//whether or not the AI will randomly walk around or not.
	[SerializeField] private float maxRandomWalkRadius = 15.0f;		//How far from current position to wander
	[SerializeField] private float reactionTime = 2.0f;				//How long it will stare at the target before it reacts.
	[SerializeField] private float searchTime = 20.0f;				//How long for the AI to search for a target after it lost sight of it
	[SerializeField] private float runSpeed = 5.0f;				//how fast the AI moves when running
	[SerializeField] private float walkSpeed = 1.0f;				//how fast the AI moves when walking
	[SerializeField] private float stuckTime = 3.0f;				//How long to wait if the AI gets stuck before reseting it

	[Header("Set how accurate this AI shoots,", order=0)]
	[Space(-10,order=1)]
	[Header("damage it does, etc. Also mark", order=2)]
	[Space(-10,order=3)]
	[Header("all possible cover locations", order=4)]
	[Space(-10,order=5)]
	[Header("and dictates whether or not this", order=6)]
	[Space(-10,order=7)]
	[Header("AI is melee and will call for help.", order=8)]
	[Space(10,order=9)]
	[SerializeField] private float bulletMissRate = 0.05f;
	[SerializeField] private float attackDistance = 2.0f;			//How far away AI can be for it to attack target
	[SerializeField] private float maxDamage = 5.0f;				//How much damage each attack does
	[SerializeField] private bool alwaysDoMaxDamage = false;		//Choose random damage or always do the max?
	[SerializeField] private float attackInterval = 2f;				//how fast the AI will attack
	[SerializeField] private GameObject[] optionalTargets;			//Additional Attack Target That you can put in manually
	[SerializeField] private bool isMelee = true;					//Decides if the AI will run for cover after attacking or not
	[SerializeField] private float swingDistance = 2.0f;			//Melee hit range
	[SerializeField] private float maxHideInCoverTime = 10.0f;		//max amount of time AI will spend behind cover
	[SerializeField] private string[] coverTags;					//Tag to ID all objects that can be used as cover
	[SerializeField] private float toCoverDistance = 2.0f;			//how close to cover objects the ai should get
	[SerializeField] private bool neverCallForHelp = false;			//Whether or not this AI will call for help 
	[SerializeField] private float delayCallForHelp = 30.0f;		//how long to wait before calling for help
	[SerializeField] private float timeBetweenHelpCalls = 10.0f;	//how long to wait until the next help call should be made
	private float helpCallTimer = 0.0f;								//for keeping track of how long it has been since the last call
	private bool calledForHelp = false;

	[Header("Muzzle flashes and particle effects settings.", order=0)]
	[Space(-10, order=1)]
	[Header("muzzle flash = displays from weapon source", order=2)]
	[Space(-10, order=3)]
	[Header("particle effects= displays from hit source", order=4)]
	[Space(10, order=5)]
	[SerializeField] private GameObject muzzleFlashObject;			//the object that will display particles when attacking
	[SerializeField] private Particle[] particleEffects;								//this connects a hit tag with a particle effect to show
	private List<int> particleList = new List<int>(); 

	[Header("Set waypoints to travel to or use", order=0)]
	[Space(-10, order=1)]
	[Header("a tag to randomly walk between.", order=2)]
	[Space(-10,order=3)]
	[Header("Waypoint = Specific objects", order=4)]
	[Space(-10,order=5)]
	[Header("Patrol Tag = Group of Objects By Tag", order=6)]
	[Space(10,order=7)]
	[SerializeField] private GameObject[] waypoints;				//Specific Points to Partol
	[SerializeField] private string patrolTag = "";					//GameObjects with Tag to mark as a patrol point

	private float stuckCounter = 0.0f;								//If object gets stuck for anything timer
	private float currentCheck;
	private float previousCheck;

	[Header("Weapon and Voice Sound Options", order=0)]
	[Space(10, order=1)]
	[SerializeField] private AudioClip[] attackVoiceSounds;			//Random list of sounds the AI makes when attacking
	[SerializeField] private AudioClip[] attackWeaponSounds;		//Random list of sounds the weapon makes when attacking(not hit sounds)
	[SerializeField] private float hearingDistance = 25.0f;			//distance the AI can hear suspicious sounds
	[SerializeField] private string[] suspiciousSoundTags;			//list of suspicious sound tags to listen for
	[SerializeField] private AudioSource audioSource;				//this audio source should be different from the footstep audio source
	private float audioVolume = 0.0f;
	private List<GameObject> sounds = new List<GameObject>();

	//the following is for debugging
	[Header("The following is for debugging only!", order=0)]
	[Space(-10, order=1)]
	[Header("Debugging information logged to the console",order=2)]
	[Space(10, order=3)]
	[SerializeField] private bool DebugLineOfSight = false;			//for debugging (Draws a red line to nearest target -- logs what it sees)
	[SerializeField] private bool DebugStates = false;				//for debugging (Logs what state the AI is currently In)
	[SerializeField] private bool DebugLastTarget = false;			//for debugging (place red cube at AI's current target)
	private bool debugCreated = false;								//for debugging
	private GameObject debugCube;									//for debugging
	[SerializeField] private bool DebugHearingDistance = false;		//for debugging (shows bubble of hearing distance);
	[SerializeField] private bool DebugSuspciousSounds = false;		//for debugging (logs all suspicious gameobjects name's heard)
	[SerializeField] private bool DebugVisualDistance = false;		//for debugging (shows -- sphere format -- how far the AI can see)
	[SerializeField] private bool DebugAttacks = false;				//will show direction of shot and what was hit

	//general settings for scripts
	private float searchTimer =0.0f;								//keep track of how long AI has been searching
	private float attackTimer = 0.0f;								//Keeps track of the last time the AI attacked the target
	public Vector3 closestCover = Vector3.zero;						//the closest found cover object
	private float sawAmount = 0.0f;									//How long the AI has seen a target
	private List<GameObject> targets = new List<GameObject>();		//All Attackable Targets for This AI Only
	private List<GameObject> coverObjects = new List<GameObject>();	//All objects that can be used as cover
	private List<GameObject> patrolPoints = new List<GameObject>(); //List of points for the AI to randomly choose and walk
	public AIMemory memory = new AIMemory();						//Helps to Remember States for the AI
	private float timer = 0;

	void Start()
	{
		AssignAttackTargets ();
		AssignPatrolPoints ();
		memory.currentState = startingState.ToString();
		this.GetComponent<NavMeshAgent> ().stoppingDistance = 1.0f;
		InvokeRepeating("UpdateMethod",0.05f,0.05f);
		audioVolume = this.GetComponent<AudioSource> ().volume;
	}

	// Update is called once per frame
	void UpdateMethod () 
	{
		checkForTargets ();
		checkForSounds ();
		timer += Time.deltaTime;
		if (DebugStates == true) {
			Debug.Log (memory.currentState);
		}
		if (DebugLastTarget == true) {
			if(debugCreated == false){
				debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);				//create a cube
				debugCube.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);//change cube color to blue
				debugCreated = true;
			}
			debugCube.transform.position = memory.lastTargetPosition;		//move cube according to last position of target
		}
		//Loop: Idle->Wander->Walking->Idle or Patrol...
		switch(memory.currentState) {
			case "Idle":
				Idle ();
				break;
			case "Wander":
				Wander ();
				break;
			case "Walking":
				Walking ();
				break;
			//Loop: See Player->Suspicious->Investigate->Idle or Hostile->Attack
			case "Suspicious":
				Suspicious ();
				break;
			case "Investigate":
				Investigate ();
				break;
			case "Attacking":
				Attack ();
				break;
			case "Hostile":
				Hostile ();
				break;
			case "Searching":
				Searching ();
				break;
			case "FindCover":
				FindCover ();
				break;
			case "RunningToCover":
				RunToCover();
				break;
			case "Patrol":
				Patrol ();
				break;
			default:
				Idle ();
				break;
		}
		if (memory.seeTarget == true) {
			sawAmount += Time.deltaTime;
		} else {
			sawAmount -= (sawAmount > 0) ? (Time.deltaTime / 2) : 0;	//ternary statment. Look it up for more info
		}
		if (memory.currentState != "FindCover" && memory.currentState != "RunningToCover") {
			if (sawAmount > (reactionTime / 2) && sawAmount <= reactionTime) {
				memory.currentState = "Suspicious";
				this.GetComponent<NavMeshAgent> ().speed = walkSpeed;
				this.GetComponent<NavMeshAgent> ().destination = this.transform.position;
			}
			if (sawAmount > reactionTime) {
				memory.currentState = "Hostile";
			} 
		}
	}
//-------------------------------- NORMAL SITUATIONS
	void Idle()
	{
		if (timer > idleWaitTime && randomlyWalk == true) {
			//set random wander position
			Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * maxRandomWalkRadius;
			randomDirection += this.gameObject.transform.position;
			NavMeshHit hit;
			NavMesh.SamplePosition(randomDirection, out hit, maxRandomWalkRadius, 1);
			memory.lastTargetPosition = hit.position;	//save random wander position to travel to
			if (patrolTag == "" && patrolPoints.Count <= 0) {
				memory.currentState = "Wander";
			} else {
				memory.currentState = "Patrol";
			}
			timer = 0;
		} 
	}
	void Wander(){
		this.GetComponent<NavMeshAgent> ().speed = walkSpeed;
		this.GetComponent<NavMeshAgent> ().destination = memory.lastTargetPosition;
		memory.currentState = "Walking";
	}
	void Walking() {
		this.GetComponent<NavMeshAgent> ().speed = walkSpeed;
		currentCheck = Vector3.Distance (memory.lastTargetPosition, this.gameObject.transform.position);
		if (currentCheck != previousCheck) {
			previousCheck = currentCheck;
		} else {
			if (stuckCounter > stuckTime) {
				memory.currentState = "Patrol";
				stuckCounter = 0;
			} else {
				stuckCounter += Time.deltaTime;
			}
		}
		if (Vector3.Distance (this.gameObject.transform.position, memory.lastTargetPosition) < 2) {
			memory.currentState = "Idle";
		}
	}
	void Patrol () {
		memory.lastTargetPosition = patrolPoints[UnityEngine.Random.Range (0, patrolPoints.Count)].transform.position;
		this.GetComponent<NavMeshAgent> ().SetDestination(memory.lastTargetPosition);
		memory.currentState = "Walking";
	}
	void LookInRandomDirection() {
		Vector3 randomDirection = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
		this.transform.Rotate(randomDirection);
	}
//-------------------------------- SUSPICIOUS SITUATIONS
	void Suspicious()
	{
		if (memory.seeTarget == true && (sawAmount > reactionTime)) {
			memory.currentState = "Hostile";
		} 
		else if (memory.seeTarget == true && (Vector3.Distance (this.gameObject.transform.position, memory.lastTargetPosition) < attackDistance)) {
			Attack ();
		}
		else {
			this.GetComponent<NavMeshAgent> ().destination = memory.lastTargetPosition;
			memory.currentState = "Investigate";
		}
	}
	void Investigate ()
	{
		if (Vector3.Distance (this.transform.position, memory.lastTargetPosition) < 2) {
			memory.currentState = "Searching";
			searchTimer = 0.0f;
			timer = 0;
		}
	}
	void Searching ()
	{
		if (timer > searchTime) {
			memory.currentState = "Idle";
		} 
		else {
			searchTimer += Time.deltaTime;

			if (searchTimer >= 5) {
				Vector3 newPos = RandomNavSphere(transform.position, maxRandomWalkRadius, -1);
				this.GetComponent<NavMeshAgent>().SetDestination(newPos);
				searchTimer = 0;
			}
		}
	}
	public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
		Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
		randDirection += origin;
		NavMeshHit navHit;
		NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
		return navHit.position;
	}
//-----------------------------------COMBAT SITUATIONS
	void Hostile() {
		this.GetComponent<NavMeshAgent> ().speed = runSpeed;
		//checkForTargets ();
		if (memory.currentState != "Hostile") {
			return;
		}
		helpCallTimer += Time.deltaTime;
		if(helpCallTimer >= delayCallForHelp && neverCallForHelp == false) {
			if(calledForHelp == false){
				CallForHelp();
				calledForHelp = true;
			}
			else if(calledForHelp == true && helpCallTimer >= timeBetweenHelpCalls) {
				helpCallTimer = 0;
			}
		}
		if (memory.seeTarget == true && Vector3.Distance (this.transform.position, memory.lastTargetPosition) <= attackDistance) {
			if (isMelee == true) {
				this.GetComponent<NavMeshAgent> ().SetDestination(this.transform.position);
				Attack ();
			} 
			else {
				GameObject curTarget = closestTarget ();
				Attack ();
				Quaternion targetRotation = Quaternion.LookRotation (curTarget.transform.position - transform.position);
				transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, 0.001f * Time.deltaTime);
				//this.transform.LookAt (curTarget.transform.position);
			}
		} 
		else if (memory.seeTarget == true && Vector3.Distance (this.transform.position, memory.lastTargetPosition) > attackDistance) {
			if (isMelee == true) {
				this.GetComponent<NavMeshAgent> ().destination = memory.lastTargetPosition;
			} 
			else {
				Attack ();
			}
		} 
		else {
			if(memory.wasInCover == true){
				memory.wasInCover = false;
				memory.currentState = "Not Assigned";
				StartCoroutine(LeaveCover());
			}
			else {
				if(memory.respondingToHelp == true) {
					this.GetComponent<NavMeshAgent>().SetDestination(memory.lastTargetPosition);
					memory.respondingToHelp = false;
				}
				if (timer > idleWaitTime) {
					LookInRandomDirection ();
					memory.currentState = "Searching";
					timer = 0;
				}
			}
		}
	}
	IEnumerator LeaveCover() {
		if (DebugStates == true) {
			Debug.Log ("Hiding In Cover");
		}
		float waitTime = UnityEngine.Random.Range(1, maxHideInCoverTime);
		yield return new WaitForSeconds (waitTime);
		this.GetComponent<NavMeshAgent> ().SetDestination(memory.lastTargetPosition);
		sawAmount = reactionTime + 10;
		memory.currentState = "Hostile";
		if (DebugStates == true) {
			Debug.Log ("Leaving Cover");
		}
	}
	void Attack ()
	{
		if (DebugStates == true) {
			Debug.Log ("Attacking");
		}
		memory.currentState = "Attacking";
		this.transform.LookAt (memory.lastTargetPosition);
		attackTimer += Time.deltaTime;
		if (memory.seeTarget == true && Vector3.Distance (this.transform.position, memory.lastTargetPosition) > attackDistance) {
			this.GetComponent<NavMeshAgent> ().destination = memory.lastTargetPosition;
		} 
		else if (memory.seeTarget == true && Vector3.Distance (this.transform.position, memory.lastTargetPosition) <= attackDistance) {
			this.GetComponent<NavMeshAgent> ().destination = this.transform.position;
			//only attack every X seconds
			if (attackTimer >= attackInterval) {
				if (DebugStates == true) {
					Debug.Log ("Sent Damage");
				}
				attackTimer = 0;
				float damage;
				if (alwaysDoMaxDamage == true) {
					damage = maxDamage;
				} else {
					damage = UnityEngine.Random.Range (maxDamage / 2, maxDamage);
				}
				//display a muzzle flash if you have one
				if(muzzleFlashObject) {
					StartCoroutine(muzzleFlash(true));
				}

				//play your weapon sound
				if(attackWeaponSounds.Length > 0) {
					AudioClip weaponSound;
					if(attackWeaponSounds.Length-1 > 0 ){
						weaponSound = attackWeaponSounds[UnityEngine.Random.Range(0, attackWeaponSounds.Length-1)];
					}
					else {
						weaponSound = attackWeaponSounds[0];
					}
					audioSource.volume = audioVolume;
					audioSource.clip = weaponSound;
					audioSource.Play();
					StartCoroutine(ProduceSound(this.tag));				//play the "suspicious" sound
				}
				//Hit with certain accuracy
				Vector3 direction;
				float distance = 0.0f;
				if (isMelee == false) {
					direction = transform.forward;
					direction.x += UnityEngine.Random.Range(-bulletMissRate, bulletMissRate);
					direction.y += UnityEngine.Random.Range(-bulletMissRate, bulletMissRate);
					direction.z += UnityEngine.Random.Range(-bulletMissRate, bulletMissRate);
					distance = Mathf.Infinity;
				}
				else {
					direction = transform.forward;
					distance = swingDistance;
				}
				if(DebugAttacks == true) {
					StartCoroutine(DebugDrawLine(transform.position,direction.normalized));
				}
				RaycastHit hit;
				if (Physics.Raycast (this.transform.position + this.transform.up, direction.normalized, out hit, distance)){ 
					//display particle effects
					particleList.Clear();
					if(particleEffects.Length > 0) {
						for(int i=0; i < particleEffects.Length; i++){			
							if(particleEffects[i].tag == hit.collider.gameObject.tag) {
								particleList.Add (i);					//first generate list of index's for appropriate particles that you hit
							}
						}
						if(particleList.Count > 0){
							GameObject particle = particleEffects[particleList[UnityEngine.Random.Range(0, particleList.Count-1)]].particle;//chose random particle
							float destroyTime = particleEffects[particleList[UnityEngine.Random.Range(0, particleList.Count-1)]].timeToLive;//get that particles time to live
							StartCoroutine(SpawnParticle(particle, hit.point, destroyTime));//show particle at hit point then destroy it after time to live expires
						}
					}
					//Send the damage to the target
					if(hit.collider.gameObject.GetComponent<Health>()) {
						hit.collider.gameObject.GetComponent<Health>().ApplyDamage(damage, this.transform.gameObject);
					}

					//the following is for debugging
					if(hit.collider.gameObject.tag == playerTag) {
						if(DebugAttacks == true) {
							Debug.Log ("Hit Player: "+hit.collider.gameObject.name);
						}
					}
					else {
						if(DebugAttacks == true) {
							Debug.Log("Missed Player and Hit: "+hit.collider.gameObject.name);
						}
					}

					//only find cover if they are not a melee character
					if (isMelee == false) {
						//memory.currentState = "FindCover";
						FindCover ();
					}
				}
			}
		} 
		else {
			memory.currentState = "Hostile";
		}
	}
	IEnumerator SpawnParticle(GameObject particle, Vector3 position, float destroyTime) {
		GameObject spawnedParticle = Instantiate (particle, position, Quaternion.identity) as GameObject;
		yield return new WaitForSeconds (destroyTime);
		Destroy (spawnedParticle);
	}
	IEnumerator muzzleFlash(bool state){
		GameObject muzzleFlash = muzzleFlashObject;
		muzzleFlash.SetActive(state);
		yield return new WaitForSeconds (0.1f);
		muzzleFlash.SetActive(false);
	}
	IEnumerator DebugDrawLine(Vector3 start, Vector3 direction){
		Debug.DrawRay(start, direction, Color.green);
		yield return new WaitForSeconds (2.0f);
	}
	IEnumerator ProduceSound(string originalTag){
		this.tag = "Sound";
		yield return new WaitForSeconds (0.5f);
		this.tag = originalTag;
	}
//----------------------------------------- TEST -----------------------------------------------------------
	//A function that finds all available covers
	void FindCover()
	{
		if (DebugStates == true) {
			Debug.Log ("Find Cover");
		}
		//We find all the colliders near us
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightDistance);
		if (hitColliders.Length > 0) {
			for (var i = 0; i < hitColliders.Length; i++) {
				Transform possible = hitColliders [i].transform;
				foreach (Transform child in possible) 
				{
					if (child.tag == "Cover") 
					{
						RaycastHit hit;
						if (Physics.Raycast (child.transform.position + child.transform.up, (memory.lastTargetPosition - child.transform.position), out hit)) 
						{
							if (hit.transform.tag != playerTag) 
							{
								memory.cover = GetClosestPosition (memory.cover, child.transform.position);
							}
						} 
						else 
						{
							memory.cover = GetClosestPosition (memory.cover, child.transform.position);
						}
					}
				}
			}
			this.GetComponent<NavMeshAgent> ().SetDestination (memory.cover);
			RunToCover ();
		} 
		else {
			Attack ();
		}

	}
	Vector3 GetClosestPosition(Vector3 position1, Vector3 position2)
	{
		Vector3 closest;
		if (Vector3.Distance (this.transform.position, position1) <=
			Vector3.Distance (this.transform.position, position2)) {
			closest = position1;
		}
		else {
			closest = position2;
		}
		return closest;
	}
	void RunToCover() {
		if (DebugStates == true) {
			Debug.Log ("Running To Cover");
		}
		memory.currentState = "RunningToCover";
		this.transform.LookAt (memory.lastTargetPosition);
		this.GetComponent<NavMeshAgent> ().speed = runSpeed;
		if (Vector3.Distance (this.transform.position, memory.cover) < toCoverDistance) {
			while(DetectWallFacing() == false){
				DetectWallFacing();
			}
			memory.wasInCover = true;
			memory.currentState = "Hostile";
		}
	}
	bool DetectWallFacing() {
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast (transform.position, fwd, 3)) {
			this.transform.Rotate (this.transform.rotation.x + 45, 
			                       this.transform.rotation.y + 45, 
			                       this.transform.rotation.z + 45);
			return false;
		} 
		else {
			return true;
		}
	}
	void CallForHelp() {
		if (DebugStates == true) {
			Debug.Log ("Called For Help");
		}
		string tagToSearch = "";
		switch (classGroup) {
			case Dropdown.Evil:
				tagToSearch = evilTag;
				break;
			case Dropdown.Good:
				tagToSearch = goodTag;
				break;
			case Dropdown.Neutral:
				tagToSearch = neutralTag;
				break;
			case Dropdown.Zombie:
				tagToSearch = zombieTag;
				break;
		}
		GameObject[] allAllies = GameObject.FindGameObjectsWithTag (tagToSearch);
		float memberDistance = 0.0f;
		foreach (GameObject member in allAllies) {
			memberDistance = Vector3.Distance(this.transform.position, member.transform.position);
			if(memberDistance <= member.GetComponent<AIBehavior>().hearingDistance) {	//is the ally close enough to hear the call?
				member.GetComponent<AIBehavior>().RecieveHelpCall(memory.lastTargetPosition);
			}
		}
	}
	public void RecieveHelpCall(Vector3 targetFoundLocation) {
		if (DebugStates == true) {
			Debug.Log ("Recieve Help Call");
		}
		memory.lastTargetPosition = targetFoundLocation;
		memory.respondingToHelp = true;
		this.GetComponent<NavMeshAgent> ().speed = runSpeed;
		memory.currentState = "Hostile";
	}
//-----------------------------Visual & Hearing Functions
	void checkForTargets()
	{
		bool targetInSight = false;
		Vector3 targetPosition = Vector3.zero;
		foreach (GameObject target in targets) {
			// Create a vector from the enemy to the player and store the angle between it and forward.
			Vector3 direction = target.transform.position - this.transform.position;
			float angle = Vector3.Angle (direction, transform.forward);	
	
			// If the angle between forward and where the player is, is less than half the angle of view...
			if (angle < fieldOfView * 0.5f) {
				RaycastHit hit;

				// ... and if a raycast towards the player hits something...
				if (Physics.Raycast (this.transform.position + this.transform.up, direction.normalized, out hit, sightDistance)) {
					if (DebugLineOfSight == true) {
						Debug.DrawLine(this.transform.position, hit.collider.gameObject.transform.position, Color.red);
						Debug.Log (hit.collider.gameObject.tag + " vs " + target.tag);
					}
					// ... and if the raycast hits the player...
					if (hit.collider.gameObject.tag == target.tag) {
						// ... the player is in sight.
						targetInSight = true;

						// Set the last sighting is the players current position.
						targetPosition = target.transform.position;
						break;
					}
				}
			}
		}
		memory.seeTarget = targetInSight;
		if (memory.seeTarget == true) {
			memory.lastTargetPosition = targetPosition;
		}
	}
	void checkForSounds() {
		sounds.Clear();
		Vector3 closestSoundTarget = Vector3.zero;
		bool foundSound = false;
		float currTarget = 100000000.0f;
		float lastTarget = 10000000.0f;
		foreach (string soundTag in suspiciousSoundTags) {
			sounds.AddRange(GameObject.FindGameObjectsWithTag (soundTag));
		}
		if(DebugSuspciousSounds == true){
			int totalSounds = 0;
			foreach(GameObject sound in sounds){
				if(Vector3.Distance(this.transform.position, sound.transform.position) <= hearingDistance) {
					totalSounds += 1;
				}
			}
			Debug.Log("Hearing "+totalSounds+" Sounds");
		}
		foreach (GameObject sound in sounds) {
			if(Vector3.Distance(sound.transform.position, this.transform.position) <= hearingDistance) {
				currTarget = Vector3.Distance(sound.transform.position, this.transform.position);
			}
			if(currTarget < lastTarget) {
				foundSound = true;
				closestSoundTarget = sound.transform.position;
			}
			lastTarget = currTarget;
		}
		if (foundSound == true && memory.currentState != "Hostile" && 
		    memory.currentState != "FindCover" && memory.currentState != "RunningToCover") {
			memory.lastTargetPosition = closestSoundTarget;
			memory.currentState = "Suspicious";
		}
	}
//----------------------- STARTUP FUNCTIONS
	void AssignAttackTargets(){
		//Assign Attackable Targets To Choose From
		if (classGroup == Dropdown.Evil) { //AI Marked As Evil (Attack Player & Other Good)
			targets.AddRange (GameObject.FindGameObjectsWithTag (playerTag));
			targets.AddRange (GameObject.FindGameObjectsWithTag (goodTag));
			targets.AddRange (GameObject.FindGameObjectsWithTag (zombieTag));
		} else if (classGroup == Dropdown.Good) {
			targets.AddRange (GameObject.FindGameObjectsWithTag (evilTag));
			targets.AddRange (GameObject.FindGameObjectsWithTag (zombieTag));
		} else if (classGroup == Dropdown.Zombie) {
			targets.AddRange (GameObject.FindGameObjectsWithTag (playerTag));
			targets.AddRange (GameObject.FindGameObjectsWithTag (goodTag));
			targets.AddRange (GameObject.FindGameObjectsWithTag (evilTag));
			targets.AddRange (GameObject.FindGameObjectsWithTag (neutralTag));
		}

		//Assign Optional Targets -- if any
		if (optionalTargets.Length > 0) {
			foreach (GameObject target in optionalTargets) {
				targets.Add (target);
			}
		}
	}
	void AssignCoverObjects()
	{
		coverObjects.Clear ();
		foreach (string tag in coverTags) {
			coverObjects.AddRange (GameObject.FindGameObjectsWithTag (tag));
		}
	}
	void AssignPatrolPoints()
	{
		if (patrolTag != "") {
			patrolPoints.AddRange (GameObject.FindGameObjectsWithTag (patrolTag));
		}
		foreach (GameObject point in waypoints) {
			patrolPoints.Add (point);
		}
	}
	GameObject closestTarget(){
		float prevDistance = 0.0f;
		GameObject nearestTarget = targets[0];
		foreach (GameObject target in targets){
			if(prevDistance > Vector3.Distance(transform.position, target.transform.position)) {
				prevDistance = Vector3.Distance(transform.position, target.transform.position);
				nearestTarget = target;
			}
		}
		return nearestTarget;
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