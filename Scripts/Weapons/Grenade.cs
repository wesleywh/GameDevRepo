using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

namespace Pandora {
    namespace Weapons {
        [RequireComponent(typeof(AudioSource))]
        public class Grenade : MonoBehaviour {

        	[SerializeField] GameObject throwObject = null;
        	[SerializeField] Transform throwPosition = null;
        	[SerializeField] Controllers.WeaponManagerNew weaponManager = null;
        	[SerializeField] Trajectory trajectoryScript = null;
        	private bool drawLine = false;

        	// Use this for initialization
        	void OnEnable () {
        		trajectoryScript.sightLine.enabled = false;
        	}

        	// Update is called once per frame
        	void Update () {
        		if (InputManager.GetButtonDown ("Attack")) {
        			drawLine = true;
        		}
        		if (InputManager.GetButtonUp ("Attack")) {
        			trajectoryScript.sightLine.enabled = false;
        			drawLine = false;
        		}
        		if (drawLine == true) {
        			trajectoryScript.sightLine.enabled = true;
        			trajectoryScript.simulatePath ();
        		}
        	}

        	public void ThrowObject() {
        		GameObject grenade = Instantiate (throwObject, throwPosition.position, throwPosition.rotation) as GameObject;
        		if (!grenade.GetComponent<Rigidbody> ()) {
        			grenade.AddComponent<Rigidbody> ();
        		}
        		Vector3 force = trajectoryScript.ForceDirection ();
        		grenade.GetComponent<Rigidbody> ().AddForce (force * trajectoryScript.fireStrength);
                if (this.GetComponent<WeaponNew> ().baseSettings.clips_left < 1 && this.GetComponent<WeaponNew> ().baseSettings.bullet_left < 1) {
        			GameObject equipGrenade = weaponManager.GetCurrentEquippedWeapon ();
        			weaponManager.UnequipWeapon (equipGrenade.name);
        		}
        	}
        }
    }
}