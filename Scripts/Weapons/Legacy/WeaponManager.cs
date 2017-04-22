using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine.UI;

[System.Serializable]
class WeaponSelections {
	public string name = "";
	public GameObject weapon = null;
	public enum weaponType { Shotgun, Machinegun, Pistol, Launcher, NULL };
	public weaponType typeOfGun = weaponType.Machinegun;
}	
[System.Serializable]
class AnimatorControllerGroup {
	public string controllerName = "";
	public enum type { Rifle, Pistol, Launcher, NoWeapon };
	public type typeOfGun = type.Pistol;
}	

public class WeaponManager: MonoBehaviour {
	[SerializeField] private Animator anim = null;
	[SerializeField] private AnimatorControllerGroup[] animControllers = null;
	public MouseLook kickPoint = null;
	public bool equippedWeapon = false;
	[HideInInspector] public GameObject currentWeapon = null;
	[Tooltip("These weapons must have the 'Weapon' Script attached to the root")]
	[SerializeField] private WeaponSelections[] weaponSelection = null;
	[SerializeField] private HitVisuals[] hitDictionary;
	[SerializeField] private float scrollSelectSpeed = 0.3f;
	//This script cache variables only
	private float aimSpeed = 2.0f;
	//lerp cam
	private Transform original = null;
	private float startTime;
	private float journeyLength;
	private Transform startMarker = null;
	private Transform endMarker = null;
	private GameObject playerCam = null;
	private bool resetPoints = false;
	private float distCovered;
	private float fracJourney;
	public GameObject lastWeapon = null;
	[HideInInspector] public bool isFiring = false;
	private GUIControl GUI = null;
	private List<GameObject> collection = new List<GameObject>();
	[HideInInspector] public bool canAim = true;
	private float scrollDir = 0.0f;
	private bool canChangeWeapon = true;

	void Start() {
		original = GameObject.FindGameObjectWithTag ("CameraHolder").transform;
		playerCam = GameObject.FindGameObjectWithTag ("PlayerCamera");
		GUI = GameObject.FindGameObjectWithTag ("GUIParent").GetComponent<GUIControl>();
	}
	void Update() {
		if (equippedWeapon == true && InputManager.GetButtonDown ("Equip")) {
			UnequipWeapon ();
		} else if (equippedWeapon == false && InputManager.GetButtonDown ("Equip")) {
			EquipWeapon (lastWeapon.GetComponent<Weapon> ().weaponName, lastWeapon.GetComponent<Weapon> ());
		}
		scrollDir = InputManager.GetAxis ("Scroll");
		if (scrollDir != 0) {
			if (scrollDir > 0)
				StartCoroutine (SelectWeapon (true));
			else
				StartCoroutine(SelectWeapon (false));
		}
		if (equippedWeapon == true) {
			//lerp cam to aim position
			if (InputManager.GetButton ("Block") == true && canAim == true) { 
				if (currentWeapon.GetComponent<Weapon> ().isReloading == false) {
					anim.SetBool ("aim", true);
					if (resetPoints == true) {
						resetPoints = false;
						SetAimCamPoints ();
					}
					if (Vector3.Distance (playerCam.transform.position, endMarker.position) > 0.01f) {
						distCovered = (Time.time - startTime) * aimSpeed;
						fracJourney = distCovered / journeyLength;
						playerCam.transform.position = Vector3.Lerp (startMarker.position, endMarker.position, fracJourney);
						if (Vector3.Distance (playerCam.transform.position, endMarker.position) < 0.01f) {
							playerCam.transform.position = endMarker.position;
							playerCam.transform.parent = endMarker;
						}
					}
				}
			} else {
				if (resetPoints == false) {
					resetPoints = true;
					anim.SetBool ("aim", false);
					ResetAimCamPoints ();
				}
				if (Vector3.Distance (playerCam.transform.position, endMarker.position) < 0.01f) {
					distCovered = (Time.time - startTime) * aimSpeed;
					fracJourney = distCovered / journeyLength;
					playerCam.transform.position = Vector3.Lerp (startMarker.position, endMarker.position, fracJourney);
					if (Vector3.Distance (playerCam.transform.position, endMarker.position) < 0.01f) {
						playerCam.transform.position = endMarker.position;
					}
				}
			}
		} 
		//Attempt to Fire/Attack with the weapoin
		if (InputManager.GetButton ("Attack") && equippedWeapon==true && currentWeapon != null) {
			isFiring = true;
			if (currentWeapon.GetComponent<Weapon>().isReloading == false) {
				if (currentWeapon.GetComponent<Weapon>().Fire (InputManager.GetButton ("Block"))) {
					if (currentWeapon.GetComponent<Weapon>().bulletsLeft > -1) {
						currentWeapon.GetComponent<Weapon>().Kick ();
						currentWeapon.GetComponent<Weapon>().EjectShell ();
					}
				}
			}
		}
		if (InputManager.GetButtonUp ("Attack") && equippedWeapon == true && currentWeapon != null) {
			isFiring = false;
		}
		if (InputManager.GetButtonDown ("Reload")) {
			StartCoroutine(currentWeapon.GetComponent<Weapon> ().Reload ());
		}
	}
		
	IEnumerator SelectWeapon(bool up) {
		//true = +1
		//false = -1
		if (currentWeapon == null || canChangeWeapon == false || collection.Count < 2) {
			yield return false;
		}
		canChangeWeapon = false;
		for (int i=0; i<collection.Count; i++) {
			if (collection [i].GetComponent<Weapon> ().weaponName == currentWeapon.GetComponent<Weapon>().weaponName) {
				i = (up == true) ? i + 1 : i - 1;
				i = (i < 0) ? collection.Count - 1 : i;
				i = (i > collection.Count - 1) ? 0 : i;
				EquipWeapon (collection [i].GetComponent<Weapon> ().weaponName, 
							collection [i].GetComponent<Weapon> ());
				break;
			}
		}
		yield return new WaitForSeconds (scrollSelectSpeed);
		canChangeWeapon = true;
	}

	private void SetAimCamPoints() {
		startMarker = GameObject.FindGameObjectWithTag ("CameraHolder").transform;
		endMarker = (GameObject.FindGameObjectWithTag ("WeaponCamera")) ? GameObject.FindGameObjectWithTag ("WeaponCamera").transform : original;
		journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
		playerCam.transform.rotation = endMarker.rotation;
		startTime = Time.time;
	}
	private void ResetAimCamPoints() {
		startMarker = (GameObject.FindGameObjectWithTag ("WeaponCamera")) ? GameObject.FindGameObjectWithTag ("WeaponCamera").transform : original;
		endMarker = original;
		playerCam.transform.parent = endMarker;
		journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
		playerCam.transform.rotation = endMarker.rotation;
		startTime = Time.time;
	}

	public void InvEquipUnequipWeapon(string name) {
		if (equippedWeapon == true && currentWeapon.GetComponent<Weapon>().weaponName == name) {
			UnequipWeapon ();
		} else {
			foreach (GameObject weapon in collection) {
				if (weapon.GetComponent<Weapon>().weaponName == name) {
					EquipWeapon (name, weapon.GetComponent<Weapon>());
					break;
				}
			}
		}
	}
	public void EquipWeapon(string name, Weapon stats) {
		GUI.AmmoGUI.SetActive (true);
		foreach(WeaponSelections weaponS in weaponSelection) {
			if (weaponS.name == name) {
				if (isInCollections (name) == false) {
					collection.Add (weaponS.weapon);
				}
				aimSpeed = weaponS.weapon.GetComponent<Weapon> ().aimSpeed;
				if(currentWeapon != null)
					currentWeapon.SetActive(false);
				weaponS.weapon.SetActive(true);
				lastWeapon = currentWeapon;
				currentWeapon = weaponS.weapon;
				currentWeapon.GetComponent<Weapon> ().SetStats(stats);
				GUI.AmmoClips.GetComponent<Text>().text = currentWeapon.GetComponent<Weapon> ().numberOfClips.ToString();
				GUI.AmmoBulletsLeft.GetComponent<Text>().text = currentWeapon.GetComponent<Weapon> ().bulletsLeft.ToString();
				anim.runtimeAnimatorController = Resources.Load (GetControllerType (weaponS.typeOfGun)) as RuntimeAnimatorController;
				equippedWeapon = true;
			}
		}
	}

	//check if this object already owns this weapon
	private bool isInCollections(string name) {
		foreach (GameObject weapon in collection) {
			if (name == weapon.GetComponent<Weapon> ().weaponName) {
				return true;
			}
		}
		return false;
	}

	//drop the named weapon and remove it from collections
	public void DropTargetWeapon(string name) {
		if (currentWeapon != null) {
			if (name == currentWeapon.GetComponent<Weapon> ().weaponName) {
				UnequipWeapon ();
//				DropTargetWeapon (name);
			} 
			foreach (GameObject weapon in collection) {
				if (name == weapon.GetComponent<Weapon> ().weaponName) {
					collection.Remove (weapon);
					break;
				}
			}
		}
	}

	//Get all the stats for the named weapon
	public Weapon GetWeaponStats(string name) {
		foreach (GameObject weapon in collection) {
			if (name == weapon.GetComponent<Weapon> ().weaponName) {
				return weapon.GetComponent<Weapon> ();
			}
		}
		return null;
	}

	//Go from equipped weapon state to unequipped state and remove visual for weapon
	public void UnequipWeapon() {
		GUI.AmmoGUI.SetActive (false);
		lastWeapon = currentWeapon;
		currentWeapon.SetActive (false);
		anim.runtimeAnimatorController = Resources.Load (GetControllerType (WeaponSelections.weaponType.NULL)) as RuntimeAnimatorController;
		currentWeapon = null;
		equippedWeapon = false;
	}

	//Pickup the weapon. Either add it to collections and equip or add the ammo to matching weapon.
	public void PickupWeapon(Weapon pickup) {
		bool hasWeapon = false;
		foreach(WeaponSelections weaponS in weaponSelection) {
			if (weaponS.name == pickup.weaponName) {
				if (currentWeapon == null || isInCollections(pickup.weaponName) == false) {
					EquipWeapon (pickup.weaponName, pickup);
					GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager> ().AddToInventory (pickup.weaponName);
				} else {
					PickupAmmo (pickup.weaponName, pickup.numberOfClips);
				}
				hasWeapon = true;
				if(pickup.destroyOnPickup == true)
					Destroy (pickup.transform.gameObject);
				break;
			}
		}
		if (hasWeapon == false) {
			EquipWeapon (pickup.weaponName, pickup);
			if(pickup.destroyOnPickup == true)
				Destroy (pickup.transform.gameObject);
		}
	}

	//Add ammo to currently held weapon
	public void PickupAmmo(string weaponName, int clips) {
		foreach (GameObject weapon in collection) {
			if (weapon.GetComponent<Weapon> ().weaponName == weaponName) {
				if (weapon.GetComponent<Weapon> ().pickupAmmoSounds.Length > 0) {
					AudioClip[] sounds = weapon.GetComponent<Weapon> ().pickupAmmoSounds;
					AudioSource source = weapon.GetComponent<Weapon> ().soundSource;
					source.clip = sounds [Random.Range (0, sounds.Length)];
					source.volume = weapon.GetComponent<Weapon> ().pickupAmmoVolume;
					source.Play ();
				}
				weapon.GetComponent<Weapon> ().numberOfClips += clips;
				if (currentWeapon.GetComponent<Weapon> ().weaponName == weaponName) {
					GUI.AmmoClips.GetComponent<Text> ().text = weapon.GetComponent<Weapon> ().numberOfClips.ToString ();
				}
				break;
			}
		}
	}

	//Get the animator controller used for a certain type of weapon
	private string GetControllerType(WeaponSelections.weaponType weaponType) {
		AnimatorControllerGroup.type controllerType = AnimatorControllerGroup.type.Pistol;
		switch (weaponType) {
			case WeaponSelections.weaponType.NULL:
				controllerType = AnimatorControllerGroup.type.NoWeapon;
				break;
			case WeaponSelections.weaponType.Pistol:
				controllerType = AnimatorControllerGroup.type.Pistol;
				break;
			case WeaponSelections.weaponType.Launcher:
				controllerType = AnimatorControllerGroup.type.Launcher;
				break;
			case WeaponSelections.weaponType.Machinegun:
				controllerType = AnimatorControllerGroup.type.Rifle;
				break;
			case WeaponSelections.weaponType.Shotgun:
				controllerType = AnimatorControllerGroup.type.Rifle;
				break;
		}
		foreach (AnimatorControllerGroup groupItem in animControllers) {
			if (groupItem.typeOfGun == controllerType) {
				return groupItem.controllerName;
			}
		}
		return "null";
	}

	//Check if hit object is found within the hit dictionary
	//if it is call "hitObject"
	public void RunHitDictionaryCheck(GameObject hitObj, RaycastHit hit) {
		//hit a terrain
		if (hitObj.tag == "Terrain") {
			string hitSurfaceName = TerrainSurface.GetMainTexture (transform.position);
			foreach (HitVisuals hitDict in hitDictionary) {
				if (hitSurfaceName == hitDict.materialName || hitSurfaceName == hitDict.materialName+" (Instance)") {
					hitObject (hitDict, hit);
					return;
				}
			}
		}
		//hit something other than a terrain
		else if (hitObj.GetComponent<MeshRenderer> ()) {
			string hitSurfaceName = hitObj.GetComponent<MeshRenderer> ().material.name;
			foreach (HitVisuals hitDict in hitDictionary) {
				if (hitSurfaceName == hitDict.materialName || hitSurfaceName == hitDict.materialName+" (Instance)") {
					hitObject (hitDict, hit);
					return;
				}
			}
		} 
	}

	//This will place objects based on the hit dictionary item and location passed in
	private void hitObject(HitVisuals hitDict, RaycastHit hit) {
		if (hitDict.visual != null) {
			GameObject hitVisual = (GameObject)Instantiate (hitDict.visual, hit.point + (hit.normal * 0.00001f), Quaternion.LookRotation (hit.normal));
			hitVisual.name = "GunManager - HitVisualObject";
			StartCoroutine (destroyObject (hitVisual, hitDict.visualDestroyDelay));
		}
		GameObject empty = new GameObject ("Placeholder Object");
		GameObject hitDecal = (hitDict.decal != null) ? 
			(GameObject)Instantiate (hitDict.decal, hit.point + (hit.normal * 0.00001f), Quaternion.LookRotation (hit.normal)) : 
			(GameObject)Instantiate (empty, hit.point + (hit.normal * 0.00001f), Quaternion.LookRotation (hit.normal)) as GameObject;
		hitDecal.name = "GunManager - HitDecal";
		StartCoroutine (destroyObject (empty, hitDict.decalDestroyDelay));

		if (hitDict.soundToPlay.Length > 0) {
			hitDecal.AddComponent<AudioSource> ();
			AudioSource audioSource = hitDecal.GetComponent<AudioSource> ();
			audioSource.volume = hitDict.volume;
			audioSource.maxDistance = 30;
			audioSource.spread = 360;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.clip = hitDict.soundToPlay [UnityEngine.Random.Range (0, hitDict.soundToPlay.Length)];
			audioSource.loop = false;
			audioSource.Play ();
		}
		StartCoroutine (destroyObject (hitDecal, hitDict.decalDestroyDelay));
	}

	//Use to destroy the decals that were generated
	private IEnumerator destroyObject(GameObject obj, float delay) {
		Destroy(obj, delay);
		yield return true;
	}
}
