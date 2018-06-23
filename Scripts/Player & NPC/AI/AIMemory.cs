using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CyberBullet.Controllers;
using BehaviorDesigner.Runtime;

namespace CyberBullet.AI {
    public class AIMemory : MonoBehaviour {
        #region Classes
        public enum AIActiveState {Hostile,Suspicious,Calm}
        [System.Serializable]
        public class AITraits 
        {
            public float fleeDistance = 5.0f;
            public float predictionTime = 1.0f;
            public float fieldOfView = 130.0f;
            public float lightVisualRange = 15.0f;
            public float darkVisualRange = 3.0f;
            public float hearingDistance = 10.0f;
            public float hearThreshold = 0.05f;
            public float shoutDistance = 20.0f;
            public float meleeRange = 2.0f;
            public float fireRange = 15.0f;
            public float inaccuracy = 1.2f;
            public float rotationSpeed = 1;
            public LayerMask ignoreLayers = 0;
            public LayerMask enemyLayers = 0;
            public LayerMask coverLayers = 0;
            public LayerMask alertSoundLayers = 0;
            public string[] friendTags = null;
            public string[] enemyTags = null;
            public GameObject[] waypoints = null;
            public Vector3 offset = Vector3.zero;
            public float offsetRange = 1.0f;
            public float minRangeDamage = 2.0f;
            public float maxRangeDamage = 5.0f;
            public float minMeleeDamage = 2.0f;
            public float maxMeleeDamage = 5.0f;
            public float minAttackWait = 0.5f;
            public float maxAttackWait = 5.0f;
            public int minNumberOfAttacks = 1;
            public int maxNumberOfAttacks = 5;
            public float runSpeed = 4.0f;
            public float walkSpeed = 1.0f;
            public float arriveDistance = 0.1f;
            public float hostileTime = 20.0f;
            public float suspiciousTime = 5.0f;
            public float calmReactTIme = 2.0f;
            public float minWanderDistance = 5.0f;
            public float maxWanderDistance = 10.0f;
            public float wanderRate = 1.0f;
            public float minPause = 0.0f;
            public float maxPause = 3.0f;
            public float minStrafeDistance = 1.0f;
            public float maxStrafeDistance = 3.0f;
            public float coverOffset = 1.0f;
            public AIActiveState startingState = AIActiveState.Calm;
            public ExternalBehaviorTree[] extraTrees = null;
            public ExternalBehaviorTree calmTree = null;
            public ExternalBehaviorTree suspiciousTree = null;
            public ExternalBehaviorTree hostileTree = null;
        }
        [System.Serializable]
        public class AIState
        {
            public bool alwaysRootGameObject = true;
            public bool lite = true;
            public GameObject last_damager = null;
            public GameObject target = null;
            public GameObject waypoint = null;
            public AIActiveState activeState = AIActiveState.Calm;
            public Vector3 last_target_position = Vector3.zero;
        }
        [System.Serializable]
        public class AIPositions
        {
            public GameObject eyes = null;
            public GameObject groundCast = null;
        }
        #endregion

        #region Variables
        [SerializeField] private AITraits traits = new AITraits();
        [SerializeField] private AIPositions positions = new AIPositions();
        [SerializeField] private Health health;
        [Space(10)]
        [Header("Debugging Purposes Only")]
        [SerializeField]private AIState state = new AIState();
        #endregion

        #region Initializations
        void Start()
        {
            if (positions.eyes == null)
                Debug.LogError("Eyes position is null. You must supply the eyes position");
            health = (health == null) ? GetComponent<Health>() : health;
            SetState(traits.startingState);
            SetActiveTree(traits.startingState);
            SetBehaviorTreeVariables();
        }
        #endregion

        #region Gets
        public float GetCalmReactTime()
        {
            return traits.calmReactTIme;
        }
        public float GetCoverOffset()
        {
            return traits.coverOffset;
        }
        public LayerMask GetCoverLayers()
        {
            return traits.coverLayers;
        }
        public LayerMask GetAlertSoundLayers()
        {
            return traits.alertSoundLayers;
        }
        public float GetStrafeDistance()
        {
            return Random.Range(traits.minStrafeDistance, traits.maxStrafeDistance);
        }
        public int GetMinNumberOfAttacks()
        {
            return traits.minNumberOfAttacks;
        }
        public int GetMaxNumberOfAttacks()
        {
            return traits.maxNumberOfAttacks;
        }
        public int GetNumberOfAttacks()
        {
            return Random.Range(traits.minNumberOfAttacks, traits.maxNumberOfAttacks);
        }
        public float GetFleeDistance()
        {
            return traits.fleeDistance;
        }
        public float GetPredictionTime()
        {
            return traits.predictionTime;
        }
        public Vector3 GetCurrentTargetPosition()
        {
            if (state.target == null)
                return Vector3.zero;
            return state.target.transform.position;
        }
        public AIActiveState GetCurrentState()
        {
            return state.activeState;
        }
        public float GetRotationSpeed()
        {
            return traits.rotationSpeed;
        }
        public string[] GetFriendTags()
        {
            return traits.friendTags;
        }
        public float GetShoutDistance()
        {
            return traits.shoutDistance;
        }
        public Vector3 GetLastTargetPosition()
        {
            return state.last_target_position;
        }
        public float GetPauseTime()
        {
            return Random.Range(traits.minPause, traits.maxPause);
        }
        public float GetMinPause()
        {
            return traits.maxPause;
        }
        public float GetMaxPause()
        {
            return traits.minPause;
        }
        public float GetWanderRate()
        {
            return traits.wanderRate;
        }
        public float GetMinWanderDistance()
        {
            return traits.minWanderDistance;
        }
        public float GetMaxWanderDistance()
        {
            return traits.maxWanderDistance;
        }
        public float GetArriveDistance()
        {
            return traits.arriveDistance;
        }
        public float GetHostileTime()
        {
            return traits.hostileTime;
        }
        public float GetSuspiciousTime()
        {
            return traits.suspiciousTime;
        }
        public float GetRunSpeed()
        {
            return traits.runSpeed;
        }
        public float GetWalkSpeed()
        {
            return traits.walkSpeed;
        }
        public float GetMinAttackWait()
        {
            return traits.minAttackWait;
        }
        public float GetMaxAttackWait()
        {
            return traits.maxAttackWait;
        }
        public float GetAttackWait()
        {
            return Random.Range(traits.minAttackWait, traits.maxAttackWait);
        }
        public float GetRangeDamageAmount()
        {
            return Random.Range(traits.minRangeDamage, traits.maxRangeDamage);
        }
        public float GetMeleeDamageAmount()
        {
            return Random.Range(traits.minMeleeDamage, traits.maxMeleeDamage);
        }
        public float GetOffsetRange()
        {
            return traits.offsetRange;
        }
        public Vector3 GetOffset()
        {
            return traits.offset;
        }
        public float GetInaccuracy()
        {
            return traits.inaccuracy;
        }
        public GameObject GetEyes()
        {
            return positions.eyes;
        }
        public GameObject GetGroundCast()
        {
            return positions.groundCast;
        }
        public float GetFOV()
        {
            return traits.fieldOfView;
        }
        public float GetFireRange()
        {
            return traits.fireRange;
        }
        public float GetMeleeRange()
        {
            return traits.meleeRange;
        }
        public float GetHearingDistance()
        {
            return traits.hearingDistance;
        }
        public float GetHearingThreshold()
        {
            return traits.hearThreshold;
        }
        public GameObject[] GetWaypointArray()
        {
            return traits.waypoints;
        }
        public List<GameObject> GetWaypointList()
        {
            List<GameObject> retVal = new List<GameObject>();
            retVal.AddRange(traits.waypoints);
            return retVal;
        }
        public int GetWaypointLength()
        {
            if (traits.waypoints.Length > 1)
            {
                return traits.waypoints.Length;
            }
            else
            {
                return 0;
            }
        }
        public GameObject GetLastDamagedBy()
        {
            return state.last_damager;
        }
        public GameObject GetTarget()
        {
            if (state.target != null && state.target.GetComponent<Health>())
            {
                if (state.target.GetComponent<Health>().GetHealth() > 0)
                {
                    return state.target;
                }
                else
                {
                    SetTarget(null);
                    return null;
                }
            }
            else
            {
                return state.target;
            }
        }
        public float GetVisualRange()
        {
            return (state.lite == true) ? traits.lightVisualRange : traits.darkVisualRange;
        }
        public bool GetIsFriend(GameObject obj)
        {
            foreach (string tag in traits.friendTags)
            {
                if (obj.tag.ToLower() == tag.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        public GameObject GetNextWaypoint()
        {
            if (state.waypoint == null)
            {
                state.waypoint = GetClosestWaypoint();
            }
            else
            {
                for (int i = 0; i < traits.waypoints.Length; i++)
                {
                    if (traits.waypoints[i] == state.waypoint)
                    {
                        if (i + 1 == traits.waypoints.Length)
                        {
                            state.waypoint = traits.waypoints[0];
                        }
                        else
                        {
                            state.waypoint = traits.waypoints[i];
                        }
                        break;
                    }
                }
            }
            return state.waypoint;
        }
        public GameObject GetClosestWaypoint()
        {
            GameObject closest = null;
            foreach (GameObject waypoint in traits.waypoints)
            {
                if (closest == null)
                {
                    closest = waypoint;
                }
                else if (Vector3.Distance(closest.transform.position, transform.position) >
                    Vector3.Distance(waypoint.transform.position, transform.position))
                {
                    closest = waypoint;
                }
            }
            return closest;
        }
        public GameObject GetCurrentWaypoint()
        {
            return state.waypoint;
        }
        public LayerMask GetIgnoreLayers()
        {
            return traits.ignoreLayers;
        }
        public LayerMask GetEnemyLayers()
        {
            return traits.enemyLayers;
        }
        #endregion

        #region Sets
        public void SetWaypointTarget(GameObject target, int index)
        {
            traits.waypoints[index] = target;
        }
        public void SetActiveExtraTree(int index)
        {
            if (traits.extraTrees[index])
            {
                SetActiveTree(traits.extraTrees[index]);
            }
        }
        public void SetLastTargetPosition(Vector3 position)
        {
            if (position == Vector3.zero)
                return;
            state.last_target_position = position;
            GetComponent<BehaviorTree>().SetVariable("last_target_position",(SharedVector3)position);
        }
        public void SetLastTargetPosition(GameObject target)
        {
            if (target == null)
                return;
            state.last_target_position = target.transform.position;
            GetComponent<BehaviorTree>().SetVariable("last_target_position",(SharedVector3)target.transform.position);
        }
        public void SetBehaviorTreeVariables()
        {
            BehaviorTree tree = GetComponent<BehaviorTree>();
            tree.SetVariable("fov",(SharedFloat)traits.fieldOfView);
            tree.SetVariable("fire_range",(SharedFloat)traits.fireRange);
            tree.SetVariable("melee_range",(SharedFloat)GetMeleeRange());
            if (state.lite == true)
            {
                tree.SetVariable("visual_range", (SharedFloat)traits.lightVisualRange);
            }
            else
            {
                tree.SetVariable("visual_range", (SharedFloat)traits.darkVisualRange);
            }
            tree.SetVariable("this", (SharedGameObject)this.gameObject);
            tree.SetVariable("ignore_layers", (SharedLayerMask)GetIgnoreLayers());
            tree.SetVariable("object_layers", (SharedLayerMask)GetEnemyLayers());
            tree.SetVariable("cover_layers", (SharedLayerMask)GetCoverLayers());
            tree.SetVariable("alert_sound_layers", (SharedLayerMask)GetAlertSoundLayers());
            tree.SetVariable("hearing_distance", (SharedFloat)GetHearingDistance());
            List<GameObject> waypointsList = new List<GameObject>();
            waypointsList.AddRange(traits.waypoints);
            tree.SetVariable("waypoints", (SharedGameObjectList)GetWaypointList());
            tree.SetVariable("target", (SharedGameObject)GetTarget());
            tree.SetVariable("wait", (SharedFloat)GetAttackWait());
            tree.SetVariable("hearing_threshold", (SharedFloat)GetHearingThreshold());
            tree.SetVariable("run_speed", (SharedFloat)GetRunSpeed());
            tree.SetVariable("walk_speed", (SharedFloat)GetWalkSpeed());
            tree.SetVariable("arrive_distance", (SharedFloat)GetArriveDistance());
            tree.SetVariable("hostile_time", (SharedFloat)GetHostileTime());
            tree.SetVariable("suspicious_time", (SharedFloat)GetSuspiciousTime());
            tree.SetVariable("min_wander_distance", (SharedFloat)GetMinWanderDistance());
            tree.SetVariable("max_wander_distance", (SharedFloat)GetMaxWanderDistance());
            tree.SetVariable("wander_rate", (SharedFloat)GetWanderRate());
            tree.SetVariable("min_pause", (SharedFloat)GetMinPause());
            tree.SetVariable("max_pause", (SharedFloat)GetMaxPause());
            tree.SetVariable("rotation_speed", (SharedFloat)GetRotationSpeed());
            tree.SetVariable("local_position", (SharedVector3)transform.position);
            tree.SetVariable("stored_distance", (SharedFloat)0.0f);
            tree.SetVariable("min_attack_wait", (SharedFloat)GetMinAttackWait());
            tree.SetVariable("max_attack_wait", (SharedFloat)GetMaxAttackWait());
            tree.SetVariable("prediction_time", (SharedFloat)GetPredictionTime());
            tree.SetVariable("flee_distance", (SharedFloat)GetFleeDistance());
            tree.SetVariable("visual_offset", (SharedVector3)GetOffset());
            GameObject blank = null;
            tree.SetVariable("flee_target", (SharedGameObject)blank);
            tree.SetVariable("strafe_position", (SharedVector3)Vector3.zero);
            tree.SetVariable("cover_offset", (SharedFloat)GetCoverOffset());
            tree.SetVariable("calm_react_time", (SharedFloat)GetCalmReactTime());
            tree.SetVariable("pause_time", (SharedFloat)0.0f);
            tree.SetVariable("waypoints", (SharedGameObjectList)GetWaypointList());
            tree.SetVariable("waypoints_length", (SharedInt)GetWaypointLength());
        }
        public void SetActiveTree(ExternalBehaviorTree tree)
        {
            if (tree == null) 
            {
                Debug.LogError("Null tree provided to \"SetActiveTree\" function!");
                return;
            }
            ExternalBehaviorTree extree = Object.Instantiate(tree);
            extree.Init();
            GetComponent<BehaviorTree>().DisableBehavior();
            GetComponent<BehaviorTree>().ExternalBehavior = extree;
            GetComponent<BehaviorTree>().EnableBehavior();
        }
        public void SetActiveTree(AIActiveState input)
        {
            ExternalBehaviorTree extree = null;
            switch (input)
            {
                case AIActiveState.Calm:
                    extree = Object.Instantiate(traits.calmTree);
                   break;
                case AIActiveState.Suspicious:
                    extree = Object.Instantiate(traits.suspiciousTree);
                    break;
                case AIActiveState.Hostile:
                    extree = Object.Instantiate(traits.hostileTree);
                    break;
            }
            extree.Init();
            GetComponent<BehaviorTree>().DisableBehavior();
            GetComponent<BehaviorTree>().ExternalBehavior = extree;
            GetComponent<BehaviorTree>().EnableBehavior();
        }
        public void SetFOV(float amount)
        {
            traits.fieldOfView = amount;
        }
        public void SetLastDamagedBy(GameObject obj)
        {
            if (state.alwaysRootGameObject == true)
            {
                obj = obj.transform.root.gameObject;
            }
            state.last_damager = obj;
            if (state.target != null && 
                Vector3.Distance(transform.position, state.last_damager.transform.position) >
                Vector3.Distance(transform.position, state.target.transform.position))
            {
                state.target = state.last_damager;
            }
        }
        public void SetTarget(GameObject obj)
        {
            if (state.alwaysRootGameObject == true && obj != null)
            {
                if (GetEnemyLayers() == (GetEnemyLayers() | 1 << obj.layer)) //if object is in enemy layer
                {   
                    obj = obj.transform.root.gameObject;
                }
                else
                {
                    obj = null;
                }
            }
            state.target = obj;
            GetComponent<BehaviorTree>().SetVariable("target",(SharedGameObject)obj);
        }
        public void SetTargetOverride(GameObject obj)
        {
            if (state.alwaysRootGameObject == true && obj != null)
            {
                obj = obj.transform.root.gameObject;
            }
            state.target = obj;
            GetComponent<BehaviorTree>().SetVariable("target",(SharedGameObject)obj);
        }
        public void SetTargetEqualLastDamager()
        {
            GameObject lastdamager = GetComponent<Health>().GetLastDamager();
            if (state.alwaysRootGameObject == true)
            {
                lastdamager = lastdamager.transform.root.gameObject;
            }
            state.target = lastdamager;
            state.last_damager = lastdamager;
        }
        public void SetLightState(bool isLite)
        {
            state.lite = isLite;
        }
        public void SetWaypoint(GameObject obj)
        {
            state.waypoint = obj;
        }
        public void SetState(AIActiveState currentState)
        {
            state.activeState = currentState;
        }
        public void SetLowerLevel()
        {
            switch (state.activeState)
            {
                case AIActiveState.Calm:
                    break;
                case AIActiveState.Suspicious:
                    SetActiveTree(traits.calmTree);
                    state.activeState = AIActiveState.Calm;
                    break;
                case AIActiveState.Hostile:
                    SetActiveTree(traits.suspiciousTree);
                    state.activeState = AIActiveState.Suspicious;
                    break;
            }
        }
        public void SetIncreasedLevel()
        {
            switch (state.activeState)
            {
                case AIActiveState.Calm:
                    SetActiveTree(traits.suspiciousTree);
                    state.activeState = AIActiveState.Suspicious;
                    break;
                case AIActiveState.Suspicious:
                    SetActiveTree(traits.hostileTree);
                    state.activeState = AIActiveState.Hostile;
                    break;
                case AIActiveState.Hostile:
                    break;
            }
        }
        #endregion
    }
}
