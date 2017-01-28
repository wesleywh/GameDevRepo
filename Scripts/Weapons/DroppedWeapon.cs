using UnityEngine;
using System.Collections;

public class DroppedWeapon : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Weapon weapon = GetComponent<Weapon> ();
		WeaponManager wManager = GameObject.FindGameObjectWithTag ("Player").GetComponent<WeaponManager> ();
		Weapon stats = wManager.GetWeaponStats (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().lastDroppedItem);
		weapon.SetStats(stats);
		wManager.DropTargetWeapon (weapon.weaponName);
	}

}
