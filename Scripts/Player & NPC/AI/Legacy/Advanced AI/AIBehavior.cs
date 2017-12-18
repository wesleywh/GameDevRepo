///////////////////////////////////////////////////////////////////////
/// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;
using Panda.AI;

namespace Pandora {
    namespace AI {
        #region Senses
        [Serializable]
        public class Senses {
            public float fieldOfView = 130f;
            public float visualRange = 15.0f;
            public Transform eyes = null;
            public Transform crouch = null;
            public float coverEndEdgeDistance = 0.25f;
            public float coverDetectDistance = 0.75f;
            public float susiciousReactionTime = 0.2f;
            public float hostileReactionTime = 0.4f;
            public float susiciousResetTime = 2.0f;
            public float hostileResetTime = 4.0f;
            public float searchRadius = 15.0f;
            public float stopSearchingAfter = 5.0f;
            public int visualCheckEveryXFrames = 3;
        }
        #endregion

        #region Movement
        [Serializable]
        public class Movement {
            public string climbLayer = "Climb";
            public float climbResetTime = 1.0f;
            public float walkTurningSpeed = 1.0f;
            public float runTurningSpeed = 2.0f;
            public float runSpeed = 4.0f;
            public float walkSpeed = 2.0f;
            public float vaultSpeed = 0.5f;
            [Space(10)]
            public GameObject[] waypoints;
            public WaypointType waypointType = WaypointType.Loop;
            [Space(10)]
            public float calmToSuspiciousWait = 1.0f;
            public float ccAdjustSpeed = 1.3f;
            public float ccDistance = 1.5f;
            public Transform ccOrigin = null;
            public string[] ccAvoidTags = null; 
            public Vector3 leftCCPoint = new Vector3(-1.5f,0,-1.5f);
            public Vector3 rightCCPoint = new Vector3(1.5f,0,-1.5f);
        }
        #endregion

        #region Combat
        [Serializable]
        public class Combat {
            public float reloadSpeed = 3.0f;
            public float fireSpeed = 1.0f;
            public Vector2 attackWait = new Vector2(0.2f,0.5f);
            public float meleeDistance = 2.0f;
            public float shotDistance = 15.0f;
            public int moveEveryXAttacks = 1;
            public float moveRadius = 3.0f;
            public CombatType type = CombatType.Melee;
            public float[] meleeNumbers = null;
            public float[] shotNumbers = null;
            public float advanceAfterDamageTime = 3.0f;
            public string[] coverTags = null;
        }
        #endregion

        #region Debugging
        [Serializable]
        public class Debugging {
            public bool showFOV = false;
            public bool showDetections = false;
            public bool showAIState = false;
            public AIStates currentState = AIStates.Calm;
            public bool forceAIState = false;
            public AIStates setState = AIStates.Calm;
            public bool showWaypointPath = false;
            public bool showMeleeDistance = false;
            public bool showShotDistance = false;
            public float attackNumber;
            public bool showCCDistance = false;
            public bool showCCPoints = false;
            public bool triggerDamage = false;
            public bool showCurrentEnemyLocation = false;
            public bool showSearchRadius = false;
            public bool showSelectedCover = false;
            public bool showCoverDetectors = false;
            [HideInInspector] public Vector3 coverPosition = Vector3.zero;
        }
        #endregion
        [RequireComponent(typeof(NavMeshAgent))]
        [RequireComponent(typeof(HeadLookController))]
        public class AIBehavior : MonoBehaviour {
            #region Variables
            #region Adjustables
            public GameObject target = null;
            public string[] enemyTags = null;
            public Senses senses;
            public Movement movement;
            public Combat combat;
            public int updateEveryFrame = 10;
            [Space(10)]
            public Debugging debugSettings = null;
            #endregion

            #region Memory
            private List<GameObject> enemies = new List<GameObject>();
            private AIStates prevState = AIStates.Calm;
            private GameObject enem_target = null;
            private GameObject waypoint_target = null;
            private float seenTimer = 0;
            private float senseResetTimer = 0;
            private int curWaypoint = 0;
            private int waypointDir = 1;
            private Vector3 lastSeenLocation = Vector3.zero;
            private float waitTimer = 0;
            private bool at_targetlocation = false;
            private float attackTimer = 0.0f;
            private float orgSpeed = 1.0f;
            private float damageTimer = 0.0f;
            private float searchTimer = 0.0f;
            private bool isSearching = false;
            private Vector3 searchPosition = Vector3.zero;
            private int attacks = 0;
            private bool findCover = false;
            private bool inCover = false;
            private Transform currentCover = null;
            private float vault_height = 0;
            private float vault_start_height = 0;
            private bool running_vault = false;
            private bool prefClimb = false;
            private float climbTime = 0;
            private Transform ladder = null;
            #endregion

            #region Helpers
//            private AIHelpers helpers;
            private NavMeshAgent agent;
            private int frames = 0;
            #endregion

            #region References
            [HideInInspector] public AIStates currentState = AIStates.Calm;
            [HideInInspector] public float _attack_number = 0.0f;
            [HideInInspector] public bool takenDamage = false;
            [HideInInspector] public bool inStandingCover = false;
            [HideInInspector] public  bool inCrouchCover = false;
            [HideInInspector] public bool endLeftCover = false;
            [HideInInspector] public bool endRightCover = false;
            [HideInInspector] public HeadLookController hlController = null;
            [HideInInspector] public GameObject leader = null;              //Patrolling Behavior
            [HideInInspector] public Vector3 followPosition = Vector3.zero; //Patrolling Behavior
            [HideInInspector] public GameObject ccLeft = null;              //Patrolling Behavior
            [HideInInspector] public GameObject ccRight = null;             //Patrolling Behavior
            [HideInInspector] public bool vaulting = false;
            [HideInInspector] public bool climbing = false;
            #endregion
            #endregion

        	void Start () {
                movement.ccOrigin = (movement.ccOrigin == null) ? this.transform : movement.ccOrigin;
//                helpers = new AIHelpers();
                agent = this.GetComponent<NavMeshAgent>();
                agent.speed = movement.walkSpeed;
                senses.eyes = (senses.eyes == null) ? transform : senses.eyes;
                currentState = AIStates.Calm;
                waypoint_target = movement.waypoints[0];
                hlController = (hlController == null) ? GetComponent<HeadLookController>() : hlController;
                FindAllEnemies();
        	}
        	
            #region HeartBeat
            void Update() { //For managing AI States
                if (debugSettings.triggerDamage == true)
                {
                    takenDamage = true;
                    debugSettings.triggerDamage = false;
                }
                frames++;
                if (frames % senses.visualCheckEveryXFrames == 0)
                {
                    CanSeeEnemy();
                    StandDetect();
                    CrouchDetect();
                    EndCoverDetect();
                    CheckForVault();
                    CheckForClimb();
                }
                if (frames % updateEveryFrame == 0)
                {
                    frames = 0;
                    attackTimer += Time.deltaTime;
                    if (enem_target != null)//also is resposible for setting current enemy target
                    {
                        //for going up in hostility
                        if (hlController.lockToPlayer == false)
                            ChangeLookTarget(enem_target.transform);
                        senseResetTimer = 0;
                        seenTimer += Time.deltaTime;
                        if (currentState == AIStates.Suspicious && seenTimer >= senses.hostileReactionTime)
                        {
                            seenTimer = 0;
                            ChangeState(AIStates.Hostile);
                        }
                        else if (currentState == AIStates.Calm && seenTimer >= senses.susiciousReactionTime)
                        {
                            seenTimer = 0;
                            ChangeState(AIStates.Suspicious);
                        }

                    }
                    else
                    {
                        //For going down in hostility
                        if (isSearching == false)
                        {
                            senseResetTimer += Time.deltaTime;
                        }
                        switch (currentState)
                        {
                            case AIStates.Hostile:
                                if (senseResetTimer >= senses.hostileResetTime)
                                {
                                    senseResetTimer = 0;
                                    ChangeState(AIStates.Suspicious);
                                }
                                break;
                            case AIStates.Suspicious:
                                if (senseResetTimer >= senses.susiciousResetTime)
                                {
                                    senseResetTimer = 0;
                                    ChangeState(AIStates.Calm);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    if (debugSettings.showAIState == true)
                    {
                        debugSettings.currentState = currentState;
                    }
                }
            }

        	void FixedUpdate () { //For managing AI Actions
                if (debugSettings.forceAIState == true)
                {
                    currentState = debugSettings.setState;
                }
                if (takenDamage == true)
                {
                    currentState = AIStates.Hostile;
                    damageTimer += Time.deltaTime;
                    if (damageTimer >= combat.advanceAfterDamageTime)
                    {
                        takenDamage = false;
                        damageTimer = 0;
                    }
                }
                switch (currentState)
                {
                    case AIStates.Hostile:
                        Hostile();
                        break;
                    case AIStates.Suspicious:
                        waitTimer += Time.deltaTime;
                        Suspicious();
                        break;
                    case AIStates.Calm:
                        Calm();
                        break;
                }
                CrowdControl();
                PerformClimb();
        	}
            #endregion

            #region State Transitions Controls
            void FindAllEnemies()
            {
                foreach (string enemy in enemyTags)
                {
                    enemies.AddRange(GameObject.FindGameObjectsWithTag(enemy));
                }
            }
            void ChangeState(AIStates setTo)
            {
                waitTimer = 0;
                movement.calmToSuspiciousWait = 0;
                prevState = currentState;
                currentState = setTo;
                PlayStateTransitionVoice(currentState);
                SetStateVariables();
            }
            void SetStateVariables()
            {
                switch (currentState)
                {
                    case AIStates.Hostile:
                        orgSpeed = movement.runSpeed;
                        agent.speed = movement.runSpeed;
                        agent.angularSpeed = movement.runTurningSpeed;
                        break;
                    case AIStates.Suspicious:
                        orgSpeed = movement.walkSpeed;
                        agent.speed = movement.walkSpeed;
                        agent.angularSpeed = movement.walkTurningSpeed;
                        break;
                    case AIStates.Calm:
                        orgSpeed = movement.walkSpeed;
                        agent.speed = movement.walkSpeed;
                        agent.angularSpeed = movement.walkTurningSpeed;
                        break;
                    default:
                        break;
                }
            }
            void PlayStateTransitionVoice(AIStates overrideVoice = AIStates.None)
            {
                overrideVoice = (overrideVoice == AIStates.None) ? currentState : overrideVoice;
                switch(overrideVoice)
                {
                    case AIStates.Hostile:
                        break;
                    case AIStates.Suspicious:
                        break;
                    case AIStates.Calm:
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region Calm State Actions
            void Calm()
            {
                Patrol();
            }
            void Patrol()
            {
                if (enem_target == null && waypoint_target != null)
                {
                    AIHelpers.lookAtTarget(waypoint_target.transform.position, this.transform, movement.walkTurningSpeed);
                    ChangeLookTarget(waypoint_target.transform);
                }
                if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
                {
                    waypoint_target = FindNextWaypoint();
                    if (waypoint_target != null)
                    {
                        AIHelpers.MoveToTarget(waypoint_target.transform.position, this.transform);
                    }
                }
            }
            GameObject FindNextWaypoint()
            {
                GameObject retVal = movement.waypoints[curWaypoint];
                switch (movement.waypointType)
                {
                    case WaypointType.Random:
                        curWaypoint = UnityEngine.Random.Range(0, movement.waypoints.Length);
                        break;
                    case WaypointType.Loop:
                        curWaypoint++;
                        if (curWaypoint > movement.waypoints.Length - 1)
                        {
                            curWaypoint = 0;
                        }
                        break;
                    case WaypointType.PingPong:
                        if (waypointDir > 0)
                        {
                            curWaypoint++;
                            if (curWaypoint > movement.waypoints.Length-1)
                            {
                                curWaypoint -= 2;
                                waypointDir = -1;
                            }
                        }
                        else
                        {
                            curWaypoint--;
                            if (curWaypoint < 0)
                            {
                                curWaypoint += 2;
                                waypointDir = 1;
                            }
                        }
                        break;
                    case WaypointType.OneWay:
                        curWaypoint++;
                        if (curWaypoint > movement.waypoints.Length-1)
                        {
                            curWaypoint = movement.waypoints.Length - 1;
                        }
                        break;
                }
                return retVal;
            }
            #endregion

            #region Suspicious State Actions
            void Suspicious()
            {
//                if (isSearching == true)
                if (enem_target == null)
                {
                    Search();
                    LookAtTarget(searchPosition);
                }
                else if (waitTimer >= movement.calmToSuspiciousWait)
                {
                    LookAtTarget(); 
                    if (at_targetlocation == false)
                        GoToTarget();
                    if (agent.remainingDistance == 0 && agent.pathStatus == NavMeshPathStatus.PathComplete)
                        at_targetlocation = true;
                }
                return;
            }
            void Search()
            {
                if (searchPosition == Vector3.zero)
                {
                    searchPosition = lastSeenLocation;
                }
                searchTimer += Time.deltaTime;
                if (searchTimer >= senses.stopSearchingAfter)
                {
                    searchTimer = 0;
                    isSearching = false;
                    searchPosition = Vector3.zero;
                }
                else if (agent.remainingDistance == 0 && agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
//                    Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * senses.searchRadius;
//                    randomPoint += searchPosition;
//                    searchPosition = randomPoint;
//                    NavMeshHit hit;
//                    NavMesh.SamplePosition(randomPoint, out hit, senses.searchRadius, 1);
//                    Vector3 finalPosition = hit.position;
                    agent.destination = ChooseRandomPoint(searchPosition, senses.searchRadius);
                }
            }
            #endregion

            #region Hostile State Actions
            void Hostile()
            {
                isSearching = false;
                LookAtTarget();
                switch (combat.type)
                {
                    case CombatType.Melee:
                        MeleeLogic();
                        break;
                    case CombatType.Shooter:
                        ShooterLogic();
                        break;
                }
            }
            void MeleeLogic()
            {
                GoToTarget();
                if (Vector3.Distance(this.transform.position, lastSeenLocation) <= combat.meleeDistance)
                {
                    agent.destination = this.transform.position;
                    MeleeAttack();
                }
            }
            void ShooterLogic()
            {
                
                if (takenDamage == true) //Find cover if you have taken damage first
                {
                    findCover = true;
                }
                else if (findCover == true)
                {
                    ValidCoverCheck();
                    if (inCover == true)
                    {
                        ShootAttack();
                        if (attacks >= combat.moveEveryXAttacks)
                        {
                            findCover = false;
                            inCover = false;
                        }
                    }
                    else
                    {
                        GoToCover();
                        if (Vector3.Distance(this.transform.position, lastSeenLocation) <= combat.shotDistance)
                        {
                            ShootAttack();
                        }
                    }
                }
                else
                {
                    GoToTarget();
                    if (enem_target != null && Vector3.Distance(this.transform.position, lastSeenLocation) <= combat.shotDistance)
                    {
                        agent.destination = this.transform.position;
                        ShootAttack();
                        findCover = true;
                    }
                    else if (enem_target != null)
                    {
                        ShootAttack();
                    }
                }
            }
            void MeleeAttack()
            {
                float time = UnityEngine.Random.Range(combat.attackWait.x, combat.attackWait.y);
                if (attackTimer >= time)
                {
                    attackTimer = 0;
                    if (combat.meleeNumbers.Length > 0)
                        _attack_number = combat.meleeNumbers[UnityEngine.Random.Range(0, combat.meleeNumbers.Length)];
                    else
                        _attack_number = 0;
                    debugSettings.attackNumber = _attack_number;
                }
            }
            void ShootAttack()
            {
                float time = UnityEngine.Random.Range(combat.attackWait.x, combat.attackWait.y);
                if (attackTimer >= time)
                {
                    attackTimer = 0;
                    if (combat.shotNumbers.Length > 0)
                    {
                        _attack_number = combat.shotNumbers[UnityEngine.Random.Range(0, combat.shotNumbers.Length)];
                    }
                    else
                    {
                        _attack_number = 0;
                    }
                    attacks++;
                    debugSettings.attackNumber = _attack_number;
                    if (attacks >= combat.moveEveryXAttacks)
                    {
                        attacks = 0;
//                        agent.destination = ChooseRandomPoint(lastSeenLocation, combat.moveRadius);
                    }
                }
            }
            #endregion

            #region Crowd Control
            void CrowdControl()
            {
                Collider[] allHits;
                allHits = Physics.OverlapSphere(movement.ccOrigin.position, movement.ccDistance);
                foreach (Collider hit in allHits)
                {
                    if (movement.ccAvoidTags.Contains(hit.transform.root.tag))
                    {
                        if (hit.transform.root.GetComponent<NavMeshAgent>().destination == agent.destination &&
                            Vector3.Distance(transform.position, agent.destination) >
                            Vector3.Distance(hit.transform.root.position, hit.transform.root.GetComponent<NavMeshAgent>().destination))
                        {
                            if (leader == null && hit.transform.root.GetComponent<AIBehavior>().leader != this.gameObject)
                                leader = hit.transform.root.gameObject;
                            GetFollowPosition(leader);
                            agent.destination = followPosition;
                        }
                        else
                        {
                            leader = null;
                            followPosition = Vector3.zero;
                        }
                        float leftRight = AIHelpers.IsLeftOrRight(hit.transform, transform);        //right = pos, left = neg
                        float forwardOrBack = AIHelpers.IsForwardOrBack(hit.transform, transform);  //forward = positive, back = neg

//                        if (leftRight > 0)
//                        {
//                            transform.position -= (transform.right).normalized * movement.ccAdjustSpeed * Time.deltaTime;
//                        }
//                        else if (leftRight < 0)
//                        {
//                            transform.position += (transform.right).normalized * movement.ccAdjustSpeed * Time.deltaTime;
//                        }
//                        if (forwardOrBack > 0)
//                        {
//                            transform.position -= (transform.forward).normalized * movement.ccAdjustSpeed * Time.deltaTime;
////                            agent.speed = movement.walkSpeed;
//                        }
//                        else
//                        {
//                            transform.position += (transform.forward).normalized * movement.ccAdjustSpeed * Time.deltaTime;
////                            agent.speed = orgSpeed;
//                        }
                    } 
                }

            }
            void GetFollowPosition(GameObject target)
            {
                if (target.GetComponent<AIBehavior>().ccLeft == null)
                {
                    followPosition = target.transform.TransformPoint(target.GetComponent<AIBehavior>().movement.leftCCPoint);
                }
                else if (target.GetComponent<AIBehavior>().ccRight == null)
                {
                    followPosition = target.transform.TransformPoint(target.GetComponent<AIBehavior>().movement.rightCCPoint);
                }
                else 
                {
                    GetFollowPosition(target.GetComponent<AIBehavior>().ccLeft);
                }
                return;
            }
            IEnumerator WaitToProceed(Vector3 point)
            {
                yield return new WaitForSeconds(0.2f);
                agent.destination = point;
            }
            #endregion

            #region Universal Actions
            IEnumerator Parabola (NavMeshAgent agent, float height, float duration) {
                OffMeshLinkData data = agent.currentOffMeshLinkData;
                Vector3 startPos = agent.transform.position;
                Vector3 endPos = data.endPos + Vector3.up*agent.baseOffset;
                float normalizedTime = 0.0f;
                while (normalizedTime < 1.0f) {
                    float yOffset = height * 4.0f*(normalizedTime - normalizedTime*normalizedTime);
                    agent.transform.position = Vector3.Lerp (startPos, endPos, normalizedTime) + yOffset * Vector3.up;
                    normalizedTime += Time.deltaTime / duration;
                    yield return null;
                }
            }
//            IEnumerator Curve (NavMeshAgent agent, float duration) {
//                OffMeshLinkData data = agent.currentOffMeshLinkData;
//                Vector3 startPos = agent.transform.position;
//                Vector3 endPos = data.endPos + Vector3.up*agent.baseOffset;
//                float normalizedTime = 0.0f;
//                while (normalizedTime < 1.0f) {
//                    float yOffset = curve.Evaluate (normalizedTime);
//                    agent.transform.position = Vector3.Lerp (startPos, endPos, normalizedTime) + yOffset * Vector3.up;
//                    normalizedTime += Time.deltaTime / duration;
//                    yield return null;
//                }
//            }
            IEnumerator NormalSpeed (NavMeshAgent agent) {
                OffMeshLinkData data = agent.currentOffMeshLinkData;
                Vector3 endPos = data.endPos + Vector3.up*agent.baseOffset;
                while (agent.transform.position != endPos) {
                    agent.transform.position = Vector3.MoveTowards (agent.transform.position, endPos, agent.speed*Time.deltaTime);
                    yield return null;
                }
            }
            void CheckForVault()
            {
//                if (Vector3.Distance(agent.destination, transform.position) > 0.5f &&
//                    inCrouchCover == true && inStandingCover == false &&)
//                {
//                    vaulting = true;
//                    if (vault_start_height == 0)
//                    {
//                        vault_start_height = transform.position.y;
//                    }
//                }
//                else
//                {
//                    vaulting = false;
//                    running_vault = false;
//                }
//                if (vaulting == true && running_vault==false)
//                {
//                    running_vault = true;
//                }
            }
            void CheckForClimb()
            {
                if (AIHelpers.RaycastHitting(transform, senses.eyes, transform.forward, senses.coverDetectDistance, (1 << LayerMask.NameToLayer(movement.climbLayer))))
                {
                    if (ladder == null)
                    {
                        RaycastHit hit = AIHelpers.ReturnRaycast(transform, senses.crouch, Vector3.forward, 4);
                        ladder = hit.transform.root.transform;
                    }
                    climbing = (inStandingCover == true) ? true : false;
                }
                else
                {
                    climbing = false;
                }
            }
            void PerformClimb()
            {
                if (climbing == true)
                {
                    Vector3 lookPos = ladder.position - transform.position;
                    lookPos.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1000);
                    agent.speed = 0.1f;
                    prefClimb = true;
                    climbTime = 0;
                }
                else if (prefClimb == true)
                {
                    climbTime += Time.deltaTime;
                    agent.speed = 0;
                    if (climbTime >= movement.climbResetTime)
                    {
                        prefClimb = false;
                        agent.speed = orgSpeed;
                    }
                }
            }
            void ChangeLookTarget(Transform target)
            {
                hlController.currentTarget = target;
            }
            void LockLookToPlayer(bool value)
            {
                hlController.lockToPlayer = value;
            }
            bool CanSeeEnemy() 
            {
                foreach (GameObject enemy in enemies)
                {
                    if (AIHelpers.CanSeeObject(enemy, senses.eyes, senses.fieldOfView, senses.visualRange, debugSettings.showDetections))
                    {
                        isSearching = false;
                        enem_target = enemy;
                        lastSeenLocation = enem_target.transform.position;
                        bool value = (enemy.transform.root.tag == "Player");
                        LockLookToPlayer(value);
                        return true;
                    }
                    else
                    {
                        enem_target = null;
                    }
                }
                return false;
            }
            void EndCoverDetect()
            {
                Vector3 startPoint = senses.crouch.position + Vector3.left * senses.coverEndEdgeDistance;
                RaycastHit hit;
                int layerMask = ~(1 << transform.gameObject.layer);
                if (Physics.Raycast(startPoint, transform.forward, out hit, senses.coverDetectDistance, layerMask))
                {
                    endLeftCover = false;
                }
                else
                {
                    endLeftCover = true;
                }
                startPoint = senses.crouch.position - Vector3.left * senses.coverEndEdgeDistance;
                if (Physics.Raycast(startPoint, transform.forward, out hit, senses.coverDetectDistance, layerMask))
                {
                    endRightCover = false;
                }
                else
                {
                    endRightCover = true;
                }
            }
            void CrouchDetect()
            {
                if (AIHelpers.RaycastHitting(transform, senses.crouch, transform.forward,senses.coverDetectDistance) ||
                    AIHelpers.RaycastHitting(transform, senses.crouch, -transform.forward,senses.coverDetectDistance))
                {
                    inCrouchCover = true;
                    RaycastHit wall = AIHelpers.ReturnRaycast(transform, senses.crouch, -transform.forward, senses.coverDetectDistance);
                    vault_height = wall.transform.lossyScale.y;
//                    Quaternion newRot =  Quaternion.FromToRotation(transform.up, wall.normal);
//                    transform.rotation = newRot * transform.rotation;
                }
                else
                {
                    inCrouchCover = false;
                }
            }
            void StandDetect()
            {
                if (AIHelpers.RaycastHitting(transform, senses.eyes, transform.forward, senses.coverDetectDistance) ||
                    AIHelpers.RaycastHitting(transform, senses.eyes, -transform.forward, senses.coverDetectDistance))
                {
                    inStandingCover = true;
//                    RaycastHit wall = AIHelpers.ReturnRaycast(transform, senses.eyes, -transform.forward, senses.coverDetectDistance);
//                    Quaternion newRot =  Quaternion.FromToRotation(transform.up, wall.normal);
//                    transform.rotation = newRot * transform.rotation;
//                    agent.destination = transform.position;
                }
                else
                {
                    inStandingCover = false;
                }
            }
            void GoToTarget()
            {
                float distance = (combat.type == CombatType.Shooter) ? combat.shotDistance : combat.meleeDistance;
                if (Vector3.Distance(transform.position, lastSeenLocation) > distance)
                {
                    Vector3 target_point = lastSeenLocation;
                    AIHelpers.MoveToTarget(target_point, this.transform);
                }
            }
            void LookAtTarget(Vector3 overrideTarget = default(Vector3))
            {
                Vector3 target_point = (overrideTarget == default(Vector3)) ? lastSeenLocation : overrideTarget;
                AIHelpers.lookAtTarget(target_point, this.transform);
            }
            void GoToCover()
            {
                if (inCover == true)
                    return;
                Transform cover = null;
                if (enem_target != null)
                {
                    cover = AIHelpers.ReturnValidCover(combat.coverTags, enem_target.transform, senses.eyes, senses.visualRange, debugSettings.showCoverDetectors); 
                }
                if (cover != null)
                {
                    inCover = (Vector3.Distance(transform.position, cover.position) <= agent.stoppingDistance) ? true : false;
                    if (inCover == false)
                        AIHelpers.MoveToTarget(cover.position, transform);
                    if (debugSettings.showSelectedCover == true)
                    {
                        debugSettings.coverPosition = cover.position;
                    }
                    currentCover = cover;
                }
                
            }
            void ValidCoverCheck()
            {
                if (enem_target == null)
                {
                    inCover = true;
                }
                else if (currentCover == null)
                {
                    inCover = false;
                }
                else if (AIHelpers.InOffSetRaycast(enem_target.transform,currentCover.transform,0.5f,senses.visualRange,this.transform,debugSettings.showDetections))
                {
                    inCover = false;
                }
            }
            Vector3 ChooseRandomPoint(Vector3 origin, float radius)
            {
                Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * radius;
                randomPoint += origin;
//                searchPosition = randomPoint;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomPoint, out hit, radius, 1);
                return hit.position;
            }
            #endregion

            #region Debugging
            void OnDrawGizmosSelected() 
            {
                if (debugSettings.showFOV)
                    DrawFOV();
                if (debugSettings.showWaypointPath == true)
                    DrawWaypointPath();
                if (debugSettings.showMeleeDistance == true)
                    DrawMeleeDistance();
                if (debugSettings.showShotDistance == true)
                    DrawShotDistance();
                if (debugSettings.showCCDistance == true)
                    DrawCCDistance();
                if (debugSettings.showCurrentEnemyLocation == true)
                    DrawEnemyLocation();
                if (debugSettings.showSearchRadius == true)
                    DrawSearchRadius();
                if (debugSettings.showSelectedCover == true)
                    DrawCover();
                if (debugSettings.showCoverDetectors == true)
                    DrawCoverDetections();
                if (debugSettings.showCCPoints == true)
                    DrawCCPoints();
            }
            void DrawFOV()
            {
                Gizmos.color = Color.yellow;
                float halfFOV = senses.fieldOfView / 2;
                Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
                Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
                Vector3 leftRayDirection = leftRayRotation * transform.forward;
                Vector3 rightRayDirection = rightRayRotation * transform.forward;
                Gizmos.DrawRay(senses.eyes.position, leftRayDirection * senses.visualRange);
                Gizmos.DrawRay(senses.eyes.position, rightRayDirection * senses.visualRange);
            }
            void DrawWaypointPath() 
            {
                if (movement.waypointType == WaypointType.OneWay)
                {
                    Gizmos.color = Color.red;
                }
                else if (movement.waypointType == WaypointType.Random)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                Vector3 direction = Vector3.zero;
                for (int i = 0; i < movement.waypoints.Length; i++)
                {
                    if (movement.waypointType == WaypointType.Random)
                    {
                        for (int j = 0; j < movement.waypoints.Length; j++)
                        {
                            if (j != i)
                            {
                                direction = movement.waypoints[j].transform.position - movement.waypoints[i].transform.position;
                                Gizmos.DrawRay(movement.waypoints[i].transform.position, direction);
                            }
                        }
                    }
                    else
                    {
                        if (i + 1 != movement.waypoints.Length)
                        {
                            direction = movement.waypoints[i + 1].transform.position - movement.waypoints[i].transform.position;
                            Gizmos.DrawRay(movement.waypoints[i].transform.position, direction);
                        }
                        if (i - 1 > -1)
                        {
                            direction = movement.waypoints[i - 1].transform.position - movement.waypoints[i].transform.position;
                            Gizmos.DrawRay(movement.waypoints[i].transform.position, direction);
                        }
                    }
                }
                if (movement.waypointType == WaypointType.Loop && movement.waypoints.Length > 2)
                {
                    direction = movement.waypoints[movement.waypoints.Length-1].transform.position - movement.waypoints[0].transform.position;
                    Gizmos.DrawRay(movement.waypoints[0].transform.position, direction);
                }
            }
            void DrawMeleeDistance()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, combat.meleeDistance);
            }
            void DrawShotDistance()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, combat.shotDistance);
            }
            void DrawCCDistance()
            {
                movement.ccOrigin = (movement.ccOrigin == null) ? this.transform : movement.ccOrigin;
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(movement.ccOrigin.position, movement.ccDistance);
            }
            void DrawEnemyLocation()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(lastSeenLocation, new Vector3(1, 1, 1));
            }
            void DrawSearchRadius()
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, senses.searchRadius);
            }
            void DrawCover()
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(debugSettings.coverPosition, new Vector3(1, 1, 1));
            }
            void DrawCoverDetections()
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(senses.eyes.position, transform.forward * senses.coverDetectDistance);
                Gizmos.DrawRay(senses.crouch.position, transform.forward * senses.coverDetectDistance);
                Gizmos.DrawRay(senses.eyes.position, -transform.forward * senses.coverDetectDistance);
                Gizmos.DrawRay(senses.crouch.position, -transform.forward * senses.coverDetectDistance);
                Gizmos.DrawRay(senses.crouch.position+Vector3.left*senses.coverEndEdgeDistance, transform.forward * senses.coverDetectDistance);
                Gizmos.DrawRay(senses.crouch.position-Vector3.left*senses.coverEndEdgeDistance, transform.forward * senses.coverDetectDistance);
                Gizmos.DrawRay(senses.crouch.position+Vector3.left*senses.coverEndEdgeDistance, -transform.forward * senses.coverDetectDistance);
                Gizmos.DrawRay(senses.crouch.position-Vector3.left*senses.coverEndEdgeDistance, -transform.forward * senses.coverDetectDistance);
            }
            void DrawCCPoints()
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(this.transform.TransformPoint(movement.leftCCPoint), new Vector3(0.5f, 0.5f, 0.5f));
                Gizmos.DrawWireCube(this.transform.TransformPoint(movement.rightCCPoint), new Vector3(0.5f, 0.5f, 0.5f));
            }
            #endregion
        }
    }
}