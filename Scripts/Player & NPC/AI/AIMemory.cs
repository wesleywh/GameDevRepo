using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Pandora.Controllers;

namespace Panda.AI {
    public enum WaypointType{ Random, Loop, PingPong, OneWay}
    public enum AIStatus { Hostile, Suspicious, Calm }
    public enum CombatType { Shooter, Melee }
    public class AIMemory : MonoBehaviour {
        public Animator anim;

        #region Combat
        [Header("---- Combat ----")]
        public float delay_attack_start = 0.1f;
        public Vector2 attacks = new Vector2(2,5);
        public float attack_delay = 0.1f;
        public string[] cover_tags;
        public string[] enemy_tags;
        public float melee_range = 1.3f;
        public float[] melee_stand_numbers;
        public float[] melee_crouch_numbers;
        public float[] defend_numbers;
        public float[] range_numbers;
        public Transform fire_point = null;
        public float fire_angle = 30f;
        public float inaccuracy = 0.05f;
        public float damage_per_shot = 5;
        public CombatType combat_type = CombatType.Shooter;
        [HideInInspector] public bool is_attacking = false;
        [HideInInspector] public float attack_timer = 0;
        [HideInInspector] public GameObject enemy;
        [HideInInspector] public Transform last_enemy_loc;
        [HideInInspector] public GameObject cover;
        #endregion

        #region Movement
        [Space(10)]
        [Header("---- Movement ----")]
        public GameObject[] waypoints;
        public WaypointType waypoint_travel = WaypointType.Loop;
        public AIStatus status = AIStatus.Calm;
        public float wander_radius = 10.0f;
        public float walk_speed = 1;
        public float run_speed = 4;
        [HideInInspector] public Vector3 movePoint;
        [HideInInspector] public int waypoint_dir = 1;
        [HideInInspector] public int waypoint_index = -1;
        #endregion

        #region Senses
        [Space(10)]
        [Header("---- Senses ----")]
        [HideInInspector] public float prev_health = 100;
        public float fieldOfView = 130f;
        public Transform eyes = null;
        public float visualRange = 30f;
        [Space(10)]
        [Header("---- Reaction Times ----")]
        public float become_suspicious = 2.0f;
        public float become_hostile = 0.5f;
        public float hostile_lower_status = 10.0f;
        public float suspicious_lower_status = 3.0f;

        [HideInInspector] public AIStatus prev_status = AIStatus.Calm;
        #endregion

        void Start()
        {
            prev_health = GetComponent<Health>().GetHealth();
        }
        public void ClearMemory()
        {
            if (cover)
                cover.tag = cover_tags[0];
        }
        #region Debugging
        [Space(10)]
        [Header("Debugging")]
        public float see_enemy_timer = 0;
        public float missing_enemy_timer = 0;
        [Space(10)]
        public bool showWaypointPath = false;
        public bool showEyesPosition = false;
        public bool showFOV = false;
        public bool showWanderRadius = false;
        public bool showVisuals = false;
        public bool showTargetCover = false;
        public bool showMeleeRange = false;
        public bool showFireAngle = false;

        void OnDrawGizmosSelected()
        {
            if (showWaypointPath == true)
                DrawWaypointPath();
            if (showEyesPosition == true)
                DrawEyesPosition();
            if (showFOV == true)
                DrawFOV();
            if (showWanderRadius == true)
                DrawWanderRadius();
            if (showMeleeRange == true)
                DrawMeleeRange();
            if (showFireAngle == true)
                DrawFireAngle();
        }
        void DrawWaypointPath() 
        {
            if (waypoint_travel == WaypointType.OneWay)
            {
                Gizmos.color = Color.red;
            }
            else if (waypoint_travel == WaypointType.Random)
            {
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.color = Color.green;
            }
            Vector3 direction = Vector3.zero;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoint_travel == WaypointType.Random)
                {
                    for (int j = 0; j < waypoints.Length; j++)
                    {
                        if (j != i)
                        {
                            direction = waypoints[j].transform.position - waypoints[i].transform.position;
                            Gizmos.DrawRay(waypoints[i].transform.position, direction);
                        }
                    }
                }
                else
                {
                    if (i + 1 != waypoints.Length)
                    {
                        direction = waypoints[i + 1].transform.position - waypoints[i].transform.position;
                        Gizmos.DrawRay(waypoints[i].transform.position, direction);
                    }
                    if (i - 1 > -1)
                    {
                        direction = waypoints[i - 1].transform.position - waypoints[i].transform.position;
                        Gizmos.DrawRay(waypoints[i].transform.position, direction);
                    }
                }
            }
            if (waypoint_travel == WaypointType.Loop && waypoints.Length > 2)
            {
                direction = waypoints[waypoints.Length-1].transform.position - waypoints[0].transform.position;
                Gizmos.DrawRay(waypoints[0].transform.position, direction);
            }
        }
        void DrawFOV()
        {
            Gizmos.color = Color.yellow;
            float halfFOV = fieldOfView / 2;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * eyes.forward;
            Vector3 rightRayDirection = rightRayRotation * eyes.forward;
            Gizmos.DrawRay(eyes.position, leftRayDirection * visualRange);
            Gizmos.DrawRay(eyes.position, rightRayDirection * visualRange);
        }
        void DrawEyesPosition()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(eyes.position, new Vector3(0.5f, 0.5f, 0.5f));
        }
        void DrawWanderRadius()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, wander_radius);
        }
        void DrawMeleeRange()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, melee_range);
        }
        void DrawFireAngle()
        {
            Gizmos.color = Color.red;
            float halfFOV = fire_angle / 2;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * fire_point.forward;
            Vector3 rightRayDirection = rightRayRotation * fire_point.forward;
            Gizmos.DrawRay(fire_point.position, leftRayDirection * visualRange);
            Gizmos.DrawRay(fire_point.position, rightRayDirection * visualRange);
        }
        #endregion
    }
}