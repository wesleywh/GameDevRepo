using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CyberBullet.Controllers;

namespace CyberBullet.AI {
    public enum AIStates {Calm, Suspicious, Hostile, None}
    public enum CombatType {Melee, Shooter}

    public static class AIHelpers {
        public static GameObject[] GetAllGameObjectsInRange(string[] tags, float distance)
        {
            List<GameObject> targets = new List<GameObject>();
            foreach (string tag in tags)
            {
                targets.AddRange(GameObject.FindGameObjectsWithTag(tag));
            }
            return targets.ToArray();
        }
        public static GameObject FindClosestEnemy(LayerMask mask, Transform check, bool isVisible = false, float maxDistance=999999, float angle=360) 
        {
//            GameObject retVal = null;
//            GameObject[] gos = GameObject.FindObjectOfType(typeof(GameObject)) as GameObject[];
            Collider[] colliders = Physics.OverlapSphere(check.position,maxDistance);
            GameObject enemy = null;
            foreach (Collider collider in colliders)
            {
                if (mask.value != 1<<collider.gameObject.layer)
                {
                    if (collider.transform.root.gameObject == check.transform.root.gameObject)
                    {
                        continue;
                    }
                    if (enemy == null)
                    {
                        enemy = collider.gameObject;
                    }
                    else if(Vector3.Distance(check.transform.position, enemy.transform.position) >
                        Vector3.Distance(check.transform.position, collider.gameObject.transform.position))
                    {
                        enemy = collider.gameObject;
                    }
                }
            }
            return enemy;
        }
        public static GameObject FindClosestEnemy(string[] enemyList, Transform check, bool isVisible = false, float maxDistance=999999, float angle=360) 
        {
            GameObject retVal = null;
            List<GameObject> enemies = new List<GameObject>();
            foreach (string enemyTag in enemyList) 
            {
                enemies.AddRange(GameObject.FindGameObjectsWithTag(enemyTag));
            }
            float closest = 99999.0f;
            foreach (GameObject enemy in enemies)
            {
                if (Vector3.Distance(check.position, enemy.transform.position) > maxDistance)
                    continue;
                if (isVisible== true && !CanSeeObject(enemy, check, angle, maxDistance))
                    continue;
                if (retVal == null)
                {
                    retVal = enemy;
                    closest = Vector3.Distance(check.position, enemy.transform.position);
                }
                else if (Vector3.Distance(check.position, enemy.transform.position) < closest)
                {
                    retVal = enemy;
                    closest = Vector3.Distance(check.position, enemy.transform.position);
                }
            }
            return retVal;
        }     
        public static Transform ReturnValidCover(string[] coverTags, Transform enemy, Transform eyes, float maxDistance=9999999f, bool debugging = false)
        {
            Transform retVal = null;
            List<GameObject> covers = new List<GameObject>();
//            int layerMask = (1 << eyes.gameObject.layer); //Make layermask to only cast on this layer
//            Collider[] hits = null;
            foreach (string item in coverTags)
            {
                covers.AddRange(GameObject.FindGameObjectsWithTag(item));
            }
            foreach (GameObject cover in covers)
            {
                if (Vector3.Distance(cover.transform.position, eyes.position) > maxDistance)
                {
                    continue;
                }
                else if (!InOffSetRaycast(enemy.transform, cover.transform, 0.5f, maxDistance, eyes, debugging))
                {
                    if (retVal == null)
                    {
                        retVal = cover.transform;
                    }
                    else if (Vector3.Distance(retVal.position, eyes.position) >
                             Vector3.Distance(cover.transform.position, eyes.position))
                    {
                        retVal = cover.transform;
                    }
                    continue;
                }
                else
                {
                    continue;
                }
            }
            return retVal;
        }
        public static GameObject FindClosest(GameObject[] items, Transform check)
        {
            GameObject retVal = null;
            float distance = 99999999999;
            foreach (GameObject item in items)
            {
                if (retVal == null)
                {
                    retVal = item;
                    distance = Vector3.Distance(check.position, item.transform.position);
                }
                else if (Vector3.Distance(check.position, item.transform.position) < distance)
                {
                    retVal = item;
                    distance = Vector3.Distance(check.position, item.transform.position);
                }
            }
            return retVal;
        }
        public static bool RaycastHitting(Transform check, Transform start, Vector3 direction, float distance, int layerMask=0)
        {
            int ignoreLayer = 0;
            if (layerMask != 0)
            {
                ignoreLayer = (~(1 << check.gameObject.layer) | layerMask);
            }
            else 
            {
                ignoreLayer = ~(1 << check.gameObject.layer);
            }
            if (Physics.Raycast(start.position, direction, distance, ignoreLayer))
            {
                return true;
            }

            return false;
        }
        public static RaycastHit ReturnRaycast(Transform check, Transform start, Vector3 direction, float distance)
        {
            int ignoreLayer = ~(1 << check.gameObject.layer);
            RaycastHit hit;
            Physics.Raycast(start.position, check.forward, out hit, distance, ignoreLayer);
            return hit;
        }
        public static bool CanSeeObject(GameObject enemy, Transform eyes, float fieldOfView, float range, bool debugging=false)
        {
            if (!enemy || !eyes)
                return false;
            Vector3 direction = (enemy.transform.position - eyes.position);
            // If the angle between forward and where the player is, is less than half the angle of view...
            if (Vector3.Angle(direction, eyes.forward) <= (fieldOfView / 2) ) // within field of view
            {
                if (Vector3.Distance(eyes.position, enemy.transform.position) <= range)
                {
                    if (InOffSetRaycast(enemy.transform, eyes, 0f, range, null, debugging) ||
                        InOffSetRaycast(enemy.transform, eyes, -0.5f, range, null, debugging) ||
                        InOffSetRaycast(enemy.transform, eyes, 0.5f, range, null, debugging) ||
                        InOffSetRaycast(enemy.transform, eyes, 1f, range, null, debugging) ||
                        InOffSetRaycast(enemy.transform, eyes, -1f, range, null, debugging) ||
                        InOffSetRaycast(enemy.transform, eyes, -1.5f, range, null, debugging) ||
                        InOffSetRaycast(enemy.transform, eyes, 1.5f, range, null, debugging) ||
                        InOffSetRaycast(enemy.transform, eyes, 2.0f, range, null, debugging) ||
                        InOffSetRaycast(enemy.transform, eyes, -2.0f, range, null, debugging))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool InOffSetRaycast(Transform enemy, Transform eyes, float offset, float range, Transform ignoreLayer = null, bool debugging=false)
        {
            Vector3 direction = (enemy.position - eyes.position);
            RaycastHit hit;
            int layerMask = ~(1 << eyes.gameObject.layer);
            if (ignoreLayer != null)
            {
                layerMask |= ~(1 << ignoreLayer.gameObject.layer);
            }
            direction = (offset == 0) ? direction : direction + (Vector3.up * offset);
            if (Physics.Raycast(eyes.position, direction, out hit, range, layerMask)) //draw raycast straight to object from eyes
            {
                if (debugging == true)
                {
                    Debug.DrawRay(eyes.position, direction, Color.red, 0.5f);
                }
                if (hit.transform.root.gameObject == enemy.gameObject) //Hit the object?
                {
                    return true;
                }
            }
            else if (debugging == true)
            {
                Debug.DrawRay(eyes.position, direction, Color.blue, 1.0f);
            }
            return false;
        }
        public static GameObject InaccurateRaycast(Transform enemy, Transform eyes, LayerMask ignoreLayers, float offset, float inaccuracy, float range, bool debugging=false)
        {
            Vector3 direction = (enemy.position - eyes.position);
            RaycastHit hit;
            direction = (offset == 0) ? direction : direction + (Vector3.up * offset);
            direction = new Vector3(direction.x+Random.Range(-inaccuracy,inaccuracy),
                                    direction.y+Random.Range(-inaccuracy,inaccuracy), 
                                    direction.z+Random.Range(-inaccuracy,inaccuracy));
            if (Physics.Raycast(eyes.position, direction, out hit, range, ~ignoreLayers)) //draw raycast straight to object from eyes
            {
                if (debugging == true)
                {
                    Debug.DrawRay(eyes.position, direction, Color.red, 0.5f);
                }
                return hit.transform.gameObject;
            }
            else if (debugging == true)
            {
                Debug.DrawRay(eyes.position, direction, Color.blue, 1.0f);
            }
            return null;
        }
        public static RaycastHit GetRaycast(Transform enemy, Transform eyes, float inaccuracy, float range, Transform ignoreLayer = null, bool debugging=false)
        {
            Vector3 direction = (enemy.position - eyes.position);
            RaycastHit hit;
            int layerMask = ~(1 << eyes.gameObject.layer);
            if (ignoreLayer != null)
            {
                layerMask |= ~(1 << ignoreLayer.gameObject.layer);
            }
            if (Physics.Raycast(eyes.position, direction, out hit, range, layerMask)) //draw raycast straight to object from eyes
            {
                return hit;
            }
            return hit;
        }
        public static bool TargetInRaycast(GameObject target, Transform check, Vector3 start_pos, float offset, bool positive = true, bool debug = false)
        {
            Vector3 direction;
            if (positive == true)
            {
                direction = (target.transform.position + Vector3.up * offset) - check.position;
            }
            else
            {
                direction = (target.transform.position - Vector3.up * offset) - check.position;
            }
            RaycastHit[] allHits;
            allHits = Physics.RaycastAll(start_pos, direction);
            if (allHits.Length == 0)
            {
                return false;
            }
            if (allHits[0].transform.root.gameObject == target || 
                (allHits[0].transform.root == check && allHits.Length > 1 && allHits[1].transform.root.gameObject == target))
            {
                if (debug == true)
                {
                    Debug.DrawRay(start_pos, direction, Color.red, 1.0f);
                }
                return true;
            }
            else
            {
                if (debug == true)
                {
                    Debug.DrawRay(start_pos, direction, Color.blue, 1.0f);
                }
                return false;
            }
        }
        public static bool isIn(string compair,string[] list)
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
        public static void lookAtTarget(Vector3 target, Transform owner, float damping = 1.0f) 
        {
            Quaternion rotation = Quaternion.LookRotation(target - owner.position);
            owner.rotation = Quaternion.Slerp(owner.rotation, rotation, Time.deltaTime * damping);
        }
        public static void MoveToTarget(Vector3 position, Transform owner)
        {
            owner.GetComponent<NavMeshAgent>().destination = position;
        }
        public static float IsLeftOrRight(Transform target, Transform check)
        {
            Vector3 heading = target.position - check.position;
            Vector3 perp = Vector3.Cross(check.forward, heading);
            float dir = Vector3.Dot(perp, check.up);

            if (dir > 0f) 
            {
                return 1f;
            } 
            else if (dir < 0f) 
            {
                return -1f;
            } 
            else 
            {
                return 0f;
            }
        }
        public static float IsForwardOrBack(Transform target, Transform check) 
        {
            Vector3 heading = target.position - check.position;
            float dir = Vector3.Dot(heading, check.forward);
            if (dir > 0f)
            {
                return 1;
            }
            else if (dir < 0f)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        public static Vector3 ChooseRandomPoint(Vector3 origin, float radius)
        {
            Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * radius;
            randomPoint += origin;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPoint, out hit, radius, 1);
            return hit.position;
        }
        //legacy
//        public static GameObject FindNextWaypoint(ref int index, GameObject[] waypoints, WaypointType type, int dir=0)
//        {
//            GameObject retVal = null;
//
//            switch (type)
//            {
//                case WaypointType.Random:
//                    index = UnityEngine.Random.Range(0, waypoints.Length);
//                    break;
//                case WaypointType.Loop:
//                    index++;
//                    if (index > waypoints.Length - 1)
//                    {
//                        index = 0;
//                    }
//                    break;
//                case WaypointType.PingPong:
//                    if (dir > 0)
//                    {
//                        index++;
//                        if (index > waypoints.Length - 1)
//                        {
//                            index -= 2;
//                            dir = -1;
//                        }
//                    }
//                    else
//                    {
//                        index--;
//                        if (index < 0)
//                        {
//                            index += 2;
//                            dir = 1;
//                        }
//                    }
//                    break;
//                case WaypointType.OneWay:
//                    index++;
//                    if (index > waypoints.Length - 1)
//                    {
//                        index = waypoints.Length - 1;
//                    }
//                    break;
//            }
//            retVal = waypoints[index];
//
//            return retVal;
//        }
    }
}