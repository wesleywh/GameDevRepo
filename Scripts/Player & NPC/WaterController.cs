using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevRepo {
    namespace Controllers {
        [RequireComponent(typeof(Collider))]
        public class WaterController : MonoBehaviour {
        	[SerializeField] private float underwater_height = 2.07f;
        	[SerializeField] private bool enter = false;
        	[SerializeField] private bool exit = false;

        	void OnTriggerEnter(Collider col) {
        		if (col.GetComponent<MovementController> ()) {
        			if (enter == true) {
                        col.GetComponent<SwimController> ().underwater_offset = underwater_height;
                        col.GetComponent<SwimController> ().SetSwimming (true, col.GetComponent<MovementController>().moveDirection.y);
        			} else if (exit == true) {
                        col.GetComponent<SwimController> ().SetSwimming (false);
        			}
        		}
        	}
        }
    }
}