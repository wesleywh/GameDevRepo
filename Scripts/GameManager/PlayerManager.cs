using UnityEngine;
using System.Collections;
using System.Collections.Generic; 	//for list & dictionary

public class PlayerManager : MonoBehaviour {
	public float currentPlayerHealth = 100.0f;
	public float currentPlayerRegen = 0.0f;
	public List<string> inventory = new List<string>();

	public void StorePlayerInfo() {
		currentPlayerHealth = GameObject.FindGameObjectWithTag ("Player").GetComponent<Health> ().GetHealth ();
		currentPlayerRegen = GameObject.FindGameObjectWithTag ("Player").GetComponent<Health> ().GetRegeneration ();
		inventory = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().GetPlayerInventory ();
	}
	public void LoadPlayerInfo() {
		GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager>().SetPlayerInventory(inventory);
	}
}
