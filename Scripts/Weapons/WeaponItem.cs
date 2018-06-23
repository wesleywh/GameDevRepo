using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;
using CyberBullet.Helpers;

namespace CyberBullet {
    namespace Weapons {
        public class WeaponItem : MonoBehaviour {

        	[SerializeField] AudioClip overridePickupSound = null;
        	[SerializeField] W_Type pickupType = W_Type.Ammo;
        	[SerializeField] int amount = 2;
        	public int weaponIndex = 0;
        	public string pickupTag = "Player";
        	public string pickupString = "Press <ACTION> to pickup Pistol.";
            [SerializeField] private GUIStyle style = null;
            [SerializeField] private Vector3 offset = Vector3.zero;
//        	public string UITextTag = "PopUpText"; //legacy
        	[SerializeField] ButtonOptions replaceActionWith = ButtonOptions.Action;

        	private bool canPickup = false;
        	private GameObject wm;
            [SerializeField] private float distance = 5.0f;
            [SerializeField] private bool debugDistance = false;
            private GameObject player = null;
            private Vector3 screenPos;
            private Camera cam;

            void Start() {
                cam = GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ();
                player = GameObject.FindGameObjectWithTag("Player");
            }
           
            void IsWithinDistance () {
                if (Vector3.Distance(transform.position, player.transform.position) <= distance)
                {
                    canPickup = true;
                }
                else
                {
                    canPickup = false;
                }
            }

            void FixedUpdate() {
                if (cam == null)
                {
                    if (GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ())
                        cam = GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ();
                }
                if (player == null)
                {
                    if (GameObject.FindGameObjectWithTag("Player"))
                        player = GameObject.FindGameObjectWithTag("Player");
                    else
                        return;
                }
                IsWithinDistance();
            }

        	void Update() {
        		if (canPickup == true) {
        			if (InputManager.GetButtonDown ("Action")) {
        				DecideAction ();
//        				ClearUIText ();
        			}
        		}
        	}

        	void DecideAction() {
        		wm = GameObject.FindGameObjectWithTag ("WeaponManager");
        		//If this is simply ammo and you have a weapon equipped simply add the clips
        		if (pickupType == W_Type.Ammo) {
        			GameObject curWeap = wm.GetComponent<Controllers.WeaponManagerNew> ().GetCurrentEquippedWeapon ();
        			if (curWeap.GetComponent<WeaponNew> ()) {
        				curWeap.GetComponent<WeaponNew> ().AddAmmo (amount);
        				PlayPickup ();
        				Destroy (this.gameObject);
        			}
        		} else {
        			//Not just ammo. Decide whether or not to give a new weapon or ammo.
        			GameObject weapon = wm.GetComponent<Controllers.WeaponManagerNew> ().HasWeapon (pickupType);
        			if (weapon == null) {
        				//give a new weapon
        				if(wm.GetComponent<Controllers.WeaponManagerNew> ().EquipWeapon (weaponIndex))
        					Destroy (this.gameObject);
        			} else {
        				//give ammo
        				weapon.GetComponent<WeaponNew> ().AddAmmo (amount);
        				PlayPickup ();
        				Destroy (this.gameObject);
        			}
        		}
        	}

        	void PlayPickup() {
        		wm = GameObject.FindGameObjectWithTag ("WeaponManager");
        		if (overridePickupSound != null) {
        			wm.GetComponent<AudioSource> ().clip = overridePickupSound;
        			wm.GetComponent<AudioSource> ().Play ();
        		} else {
        			GameObject weapon = wm.GetComponent<Controllers.WeaponManagerNew> ().GetCurrentEquippedWeapon ();
        			if (weapon != null) {
        				AudioClip[] sounds = weapon.GetComponent<WeaponNew> ().GetEquipSounds ();
        				if (sounds.Length < 1)
        					return;
        				wm.GetComponent<AudioSource> ().clip = sounds [Random.Range (0, sounds.Length)];
        				wm.GetComponent<AudioSource> ().Play ();
        			}
        		}
        	}
               
            void OnGUI() {
                if (canPickup == true && cam != null)
                {
                    screenPos = cam.WorldToScreenPoint(transform.position);
                    GUI.TextArea(new Rect(screenPos.x +  offset.x, (Screen.height - screenPos.y) + offset.y, 0, 0), Helpers.Helpers.ModifiedText(replaceActionWith, pickupString),1000,style);
                }
            }

            void OnDrawGizmosSelected() {
                if (debugDistance)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere (transform.position, distance);
                }
            }
        }
    }
}