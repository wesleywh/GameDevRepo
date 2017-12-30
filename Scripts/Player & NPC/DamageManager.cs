using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pandora.Controllers;

namespace Pandora {
    namespace Legacy {
        public class DamageManager : MonoBehaviour {
            public float melee_distance = 2.0f;
            public float max_melee_dmg = 15.0f;
            public float min_melee_dmg = 5.0f;
            public float attack_angle = 130.0f;
            public Transform punch_position;

            public string[] enemyTags;

            public bool debug_angle = false;

            public void MeleeAttack() 
            {
                List<Transform> targets = GetEnemiesInRadius();
                foreach (Transform target in targets)
                {
                    if (target.GetComponent<Health>())
                    {
                        target.transform.root.GetComponent<Health>().ApplyDamage(Random.Range(min_melee_dmg, max_melee_dmg), transform.gameObject);
                    }
                }
            }

            List<Transform> GetEnemiesInRadius()
            {
                Collider[] objs = Physics.OverlapSphere(transform.position, melee_distance); //Check within swing distance
//                int i = 0;
                List<Transform> enemies = new List<Transform>();
                Vector3 direction = Vector3.zero;
                float angle = 0.0f;
//                RaycastHit hit;
                foreach (Collider obj in objs)
                {
                    if (isIn(obj.tag, enemyTags)) //is something that we want to target
                    {
                        direction = obj.transform.position - punch_position.position;
                        angle = Vector3.Angle(direction, punch_position.forward); 
                        if (angle < attack_angle * 0.5f) //Can see target
                        {
                            if (Physics.Linecast(punch_position.position,obj.transform.position) == false) //Target not blocked in anyway
                            {
                                enemies.Add(obj.transform.root);
                            }
                        }
                    }
                }

                return enemies;
            }
            bool isIn(string compair,string[] list)
            {
                foreach (string item in list)
                {
                    if (item == compair)
                    {
                        return true;
                    }
                }
                return false;
            }

            #region Gizmos/Visuals
            void OnDrawGizmosSelected()
            {
                if (debug_angle)
                {
                    if (punch_position == null)
                        punch_position = transform;
                    
                    float halfFOV = attack_angle / 2;
                    Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
                    Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
                    Vector3 leftRayDirection = leftRayRotation * transform.forward;
                    Vector3 rightRayDirection = rightRayRotation * transform.forward;
                    Gizmos.DrawRay(punch_position.position, leftRayDirection * melee_distance);
                    Gizmos.DrawRay(punch_position.position, rightRayDirection * melee_distance);
                }
            }
            #endregion
        }
    }
}