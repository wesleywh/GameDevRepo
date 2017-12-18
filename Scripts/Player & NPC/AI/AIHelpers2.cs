using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevRepo {
    namespace AI {
        
        public class AIHelpers2 : MonoBehaviour {
            public GameObject FindClosestEnemy(string[] enemyList, Transform check, float maxDistance=999999) 
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
            public Transform FindClosestCover(string[] coverTags, Transform check, float maxDistance=99999, Transform eyesPoint=null)
            {
                Transform retVal = null;
                List<GameObject> covers = new List<GameObject>();
                foreach (string tag in coverTags)
                {
                    covers.AddRange(GameObject.FindGameObjectsWithTag(tag));
                }
                float distance = 99999.0f;
                Transform eyes = null;
                foreach (GameObject cover in covers)
                {
                    if (Vector3.Distance(check.position, cover.transform.position) > maxDistance)
                        continue;
                    eyes = (eyesPoint == null) ? cover.transform : eyesPoint;

                    if (retVal == null)
                    {
                        retVal = cover.transform;
                        distance = Vector3.Distance(check.position, cover.transform.position);
                    }
                    else if (!CanSeeObject(cover, eyes.transform, 360) && 
                        Vector3.Distance(check.position, cover.transform.position) < distance)
                    {
                        retVal = cover.transform;
                        distance = Vector3.Distance(check.position, cover.transform.position);
                    }
                }
                return retVal;
            }
            public bool CanSeeObject(GameObject target, Transform check, float fieldOfView, bool debugging=false)
            {
                Vector3 direction = (target.transform.position - check.position)+Vector3.up;
                RaycastHit hit;
                // If the angle between forward and where the player is, is less than half the angle of view...
                if (Vector3.Angle(direction, check.forward) <= fieldOfView / 2 ) // within field of view
                {
                    if (Physics.Raycast(check.position, direction, out hit)) //draw raycast straight to object from eyes
                    {
                        if (hit.collider.transform.gameObject.Equals(target)) //Hit the object?
                        {
                            if (debugging == true)
                            {
                                Debug.Log(hit.collider.transform.root.gameObject.name);
                                Debug.DrawRay(check.position, direction, Color.red, 1.0f);
                            }
                            return true;
                        }
                        else if (debugging == true)
                        {
                            Debug.DrawRay(check.position, direction, Color.blue, 1.0f);
                        }
                    }
                }
                return false;
            }
            public GameObject ReturnCanSeeObject(GameObject target, Transform check, float fieldOfView)
            {
                Vector3 direction = target.transform.position - check.position;
                float angle = Vector3.Angle(direction, check.forward); 
                RaycastHit hit;
                // If the angle between forward and where the player is, is less than half the angle of view...
                if (angle < fieldOfView * 0.5f)
                {
                    if (Physics.Raycast(check.position, direction, out hit))
                    {
                        if (hit.collider.gameObject.Equals(target))
                            return hit.transform.gameObject;
                    }
                }
                return null;
            }
            public List<AudioSource> GetTargetsListenList(string[] enemyTags)
            {
                List<AudioSource> sources = new List<AudioSource>();
                sources.AddRange(GameObject.FindObjectsOfType(typeof(AudioSource)) as AudioSource[]);
                List<AudioSource> retVal = new List<AudioSource>();
                foreach (AudioSource source in sources)
                {
                    if (isIn(source.transform.tag, enemyTags))
                    {
                        retVal.Add(source);
                    }
                }
                return retVal;
            }
            public bool isIn(string compair,string[] list)
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
        }
    }
}
