using UnityEngine;
using System.Collections;
using System.Collections.Generic; 	//for list & dictionary
using Pandora.Controllers;
using Pandora.Cameras;

namespace Pandora {
    namespace GameManager {
        [ExecuteInEditMode]
        public class PlayerManager : MonoBehaviour {
        	public float currentPlayerHealth = 100.0f;
        	public float currentPlayerRegen = 0.0f;
        	public List<string> inventory = new List<string>();

        	public void StorePlayerInfo() {
        		currentPlayerHealth = GameObject.FindGameObjectWithTag ("Player").GetComponent<Health> ().GetHealth ();
        		print (currentPlayerHealth);
                currentPlayerRegen = GameObject.FindGameObjectWithTag ("Player").GetComponent<Health> ().GetRegeneration ();
        		inventory = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().GetPlayerInventory ();
        	}
        	public void LoadPlayerInfo() {
        		GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager>().SetPlayerInventory(inventory);
        	}

            public void SetPlayerControllable(bool enabled)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<MouseLook>().enable = enabled;
                player.GetComponent<MovementController>().enabled = enabled;
                GameObject.FindGameObjectWithTag("CameraHolder").GetComponent<MouseLook>().enable = enabled;
            }
        }
    }
}