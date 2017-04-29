///////////////////////////////////
///Written By Wesley Haws
/// 
/// Manage switching weapons.
///////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

[RequireComponent(typeof(AudioSource))]
public class WeaponManagerNew : MonoBehaviour {
	[SerializeField] GameObject[] equipedWeapons = new GameObject[2];
	[SerializeField] GameObject[] availableWeapons;
	[SerializeField] GameObject[] dropWeapons;
	[SerializeField] bool mouseWheelScroll = true;
	[SerializeField] private float cycleWeaponWait = 0.25f;
	[SerializeField] private float dropWeaponWait = 2.00f;
	[SerializeField] private Transform dropPosition;

	private int currentWeapon = 0;
	private float cycle_timer = 0;
	private float drop_timer = 0;
	private bool canDrop = false;

	void Start() {
		SwitchWeapon (0);
	}

	void Update () {
		drop_timer = (canDrop == true) ? drop_timer + Time.deltaTime : 0;
		cycle_timer = (cycle_timer > 0) ? cycle_timer - Time.deltaTime : 0;
		if (InputManager.GetAxis ("Scroll") > 0f) {
			SwitchWeapon (1);
		} else if (InputManager.GetAxis ("Scroll") < 0f) {
			SwitchWeapon (-1);
		} 
		if (InputManager.GetButtonDown ("Action")) {
			canDrop = true;
		} else if (InputManager.GetButtonUp ("Action")) {
			canDrop = false;
		}
		if (drop_timer > dropWeaponWait) {
			canDrop = false;
			DropWeapon (currentWeapon);
		}
	}

	public GameObject GetCurrentEquippedWeapon() {
		return equipedWeapons[currentWeapon];
	}

	public GameObject HasWeapon(W_Type type) {
		foreach (GameObject weapon in equipedWeapons) {
			if (!weapon || !weapon.GetComponent<WeaponNew> ())
				continue;
			else if (weapon.GetComponent<WeaponNew> ().weaponType == type) 
				return weapon;
		}
		return null;
	}

	public void DropWeapon(int index) {
		int drop_index = FindDropWeaponIndex (index);
		if (drop_index == 9999 || equipedWeapons [index] == null) {
			return;
		}
		equipedWeapons [index].gameObject.SetActive (false);
		equipedWeapons [index] = null;
		GameObject droppedWeapon = Instantiate (dropWeapons [drop_index].gameObject, dropPosition.position, dropPosition.rotation) as GameObject;
		droppedWeapon.GetComponent<Rigidbody> ().AddRelativeForce (dropPosition.forward * 150f);
	}

	public void DropWeapon(string name) {
		for (int i=0; i < dropWeapons.Length; i++) {
			if (dropWeapons[i].gameObject.name == name) {
				DropWeapon (i);
				break;
			}
		}
	}

	public void UnequipWeapon(int index) {
		int drop_index = FindDropWeaponIndex (index);
		if (drop_index == 9999 || equipedWeapons [index] == null) {
			return;
		}
		equipedWeapons [index].gameObject.SetActive (false);
		equipedWeapons [index] = null;
	}

	public void UnequipWeapon(string name) {
		for (int i=0; i < equipedWeapons.Length; i++) {
			if (equipedWeapons[i].gameObject.name == name) {
				UnequipWeapon (i);
				break;
			}
		}
	}

	public void EquipWeapon(int index) {
		int slot = FindOpenSlot ();
		if (slot == 9999) {
			slot = currentWeapon;
		}
		if (equipedWeapons [currentWeapon] != null) {
			equipedWeapons [currentWeapon].gameObject.SetActive (false);
		}
		equipedWeapons [slot] = availableWeapons [index];
		equipedWeapons [slot].gameObject.SetActive (true);
		currentWeapon = slot;
	}

	int FindOpenSlot() {
		for (int i = 0; i < equipedWeapons.Length; i++) {
			if (equipedWeapons [i] == null) {
				return i;
			}
		}
		return 9999;
	}

	int FindDropWeaponIndex(int equipIndex) {
		if (equipedWeapons [equipIndex] == null) {
			return 9999;
		}
		GameObject target_go = equipedWeapons [equipIndex].gameObject;
		int target_index = 9999;
		for (int i=0; i < availableWeapons.Length; i++) {
			if (availableWeapons [i].gameObject == target_go) {
				target_index = i;
				break;
			}
		}
		if (dropWeapons [target_index] != null) {
			return target_index;
		} else {
			return 9999;
		}
	}

	void SwitchWeapon(int direction) {
		if (cycle_timer != 0)
			return;
		cycle_timer = cycleWeaponWait;
		if (equipedWeapons [currentWeapon] != null) {
			equipedWeapons [currentWeapon].gameObject.SetActive (false);
		}
		currentWeapon += direction;
		if (currentWeapon > equipedWeapons.Length-1) {
			currentWeapon = 0;
		} else if (currentWeapon < 0){
			currentWeapon = equipedWeapons.Length-1;
		}
		if (equipedWeapons [currentWeapon] != null) {
			equipedWeapons [currentWeapon].gameObject.SetActive (true);
		}
	}
}
