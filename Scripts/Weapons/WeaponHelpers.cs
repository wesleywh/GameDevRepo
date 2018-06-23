using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberBullet {
    namespace Weapons {
        public enum W_Eject {Left, Right };

        public static class WeaponHelpers {

            public static void EjectShell(GameObject casing, Transform ejectPoint, W_Eject direction) {
                if (ejectPoint == null || casing == null)
                    return; 
                GameObject shell = GameObject.Instantiate (casing, ejectPoint.position, ejectPoint.rotation) as GameObject;
                shell.transform.parent = ejectPoint.transform;
                shell.transform.localPosition = Vector3.zero;
                if (direction == W_Eject.Right) {
                    shell.GetComponent<Rigidbody> ().velocity = ejectPoint.transform.TransformDirection (Vector3.right * Random.Range (1, 3));
                } else {
                    shell.GetComponent<Rigidbody> ().velocity = ejectPoint.transform.TransformDirection (Vector3.left * Random.Range (1, 3));
                }
                new Task(DestroyGameObject(shell, 3)); //Custom Coroutine that can be called without MonoBehavior reference.
            }

            public static IEnumerator DestroyGameObject(GameObject target, float delay) {
                yield return new WaitForSeconds (delay);
                UnityEngine.Object.Destroy (target);
            }  
        
        }
    }
}