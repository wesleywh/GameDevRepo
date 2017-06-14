using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;

namespace GameDevRepo {
    namespace Weapons {
        [RequireComponent(typeof(SphereCollider))]
        public class WeaponItem : MonoBehaviour {

        	[SerializeField] AudioClip overridePickupSound = null;
        	[SerializeField] W_Type pickupType = W_Type.Ammo;
        	[SerializeField] int amount = 2;
        	public int weaponIndex = 0;
        	public string pickupTag = "Player";
        	public string pickupString = "Press <ACTION> to pickup Pistol.";
        	public string UITextTag = "PopUpText";
        	enum ButtonOptions {
        		Left, 
        		Right,
        		Forwards, 
        		Backwards, 
        		Submit,
        		Jump, 
        		Run, 
        		Close, 
        		Action, 
        		Inventory, 
        		InventorySlot1,
        		InventorySlot2,
        		InventorySlot3,
        		InventorySlot4,
        		InventorySlot5,
        		Objectives
        	}
        	[SerializeField] ButtonOptions replaceActionWith = ButtonOptions.Action;

        	private bool canPickup = false;
        	private GameObject wm;

        	void OnTriggerEnter(Collider col) {
        		if (col.tag == pickupTag) {
        			Text UIText = GameObject.FindGameObjectWithTag (UITextTag).GetComponent<Text>();
        			UIText.text = ModifiedText();
        			canPickup = true;
        		}
        	}

        	void OnTriggerExit(Collider col) {
        		if (col.tag == pickupTag) {
        			Text UIText = GameObject.FindGameObjectWithTag (UITextTag).GetComponent<Text>();
        			if (UIText.text == ModifiedText())
        				UIText.text = "";
        			canPickup = false;
        		}
        	}

        	void Update() {
        		if (canPickup == true) {
        			if (InputManager.GetButtonDown ("Action")) {
        				DecideAction ();
        				ClearUIText ();
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

        	void ClearUIText() {
        		Text UIText = GameObject.FindGameObjectWithTag (UITextTag).GetComponent<Text>();
        		if (UIText.text == ModifiedText())
        			UIText.text = "";
        	}

        	string ModifiedText() {
        		string button = "";
        		switch (replaceActionWith) {
        			case ButtonOptions.Backwards:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [1].negative.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.Forwards:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [1].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.Left:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [0].negative.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.Right:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [0].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.Submit:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [7].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.Jump:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [6].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.Run:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [9].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.Close:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [17].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.Action:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [10].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.Inventory:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [11].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.InventorySlot1:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [12].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.InventorySlot2:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [13].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.InventorySlot3:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [14].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.InventorySlot4:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [15].positive.ToString ().Replace ("Alpha", "");
        				break;
        			case ButtonOptions.InventorySlot5:
        				button = InputManager.GetInputConfiguration (PlayerID.One).axes [16].positive.ToString ().Replace ("Alpha", "");
        				break;
        		}

        		return pickupString.Replace ("<ACTION>", button);
        	}
        }
    }
}