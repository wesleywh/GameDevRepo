using UnityEngine;
using System.Collections;
using UnityEngine.UI;			//for UI elements
using TeamUtility.IO;			//for recognizing buttons
using CyberBullet.Controllers;
using CyberBullet.Weapons;
using CyberBullet.GameManager;

namespace CyberBullet.Items {
    [RequireComponent(typeof(AudioSource))]
    public class ItemScripts : MonoBehaviour {
    	private bool showGUI = false;
    	[SerializeField] private GUIStyle textStyling;
    	[SerializeField] private AudioClip keyUnlockSound;
    	[SerializeField] private AudioClip keyFailSound;
    	[SerializeField] private AudioClip scrollOpenSound;
    	[SerializeField] private AudioClip scrollCloseSound;
    	private float guiAlpha = 0.0f;
    	private string message = "";
    	private bool openScroll = false;
    	private bool closeScroll = false;
    	private bool scrollOpen = false;
        private int itemId = 9999999;

    	void Update() {
    		if (guiAlpha > 0.0f) {
    			guiAlpha -= Time.deltaTime;
    			if (guiAlpha <= 0) {
    				showGUI = false;
    			}
    		}
    		if (scrollOpen && InputManager.GetButton ("Cancel")) {
    			CloseScroll ();
    		}
    		if (openScroll) {
    			GameObject.Find ("ScrollUI").transform.Find("Scroll").GetComponent<Image>().fillAmount += Time.deltaTime * 2;
    			if (GameObject.Find ("ScrollUI").transform.Find("Scroll").GetComponent<Image> ().fillAmount >= 1) {
    				Color color = GameObject.Find ("ScrollUI").transform.Find("Text").GetComponent<Text> ().color;
    				color.a += Time.deltaTime * 2;
    				GameObject.Find ("ScrollUI").transform.Find("Text").GetComponent<Text> ().color = color;
    				if (color.a >= 1) {
    					openScroll = false;
    				}
    			}
    		}
    		if (closeScroll) {
    			GameObject.Find ("ScrollUI").transform.Find("Scroll").GetComponent<Image>().fillAmount -= Time.deltaTime * 2;
    			if (GameObject.Find ("ScrollUI").transform.Find("Scroll").GetComponent<Image> ().fillAmount <= 0) {
    				Color color = GameObject.Find ("ScrollUI").transform.Find("Text").GetComponent<Text> ().color;
    				color.a -= Time.deltaTime * 2;
    				GameObject.Find ("ScrollUI").transform.Find("Text").GetComponent<Text> ().color = color;
    				if (color.a <= 0) {
    					closeScroll = false;
    				}
    			}
    		}
    	}
        public void ActivateWeapon(int weapon_id)
        {
            GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<InvWeaponManager>().SelectWeapon(weapon_id);
        }
        public void OpenScroll(string text) {
            text = text.Replace("\\n", "\n").Replace("\\t","\t");
            GameObject.Find("ScrollUI").transform.Find("Scroll").GetComponent<Image>().fillAmount = 0;
            GameObject.Find("ScrollUI").transform.Find("Text").GetComponent<Text>().text = text;
            Color color = GameObject.Find("ScrollUI").transform.Find("Text").GetComponent<Text>().color;
            color.a = 0;
            GameObject.Find("ScrollUI").transform.Find("Text").GetComponent<Text>().color = color;
            openScroll = true;
            scrollOpen = true;
            this.GetComponent<AudioSource>().clip = scrollOpenSound;
            this.GetComponent<AudioSource>().Play();
    	}

    	public void CloseScroll() {
    		GameObject.Find ("ScrollUI").transform.Find("Scroll").GetComponent<Image>().fillAmount = 1;
    		Color color = GameObject.Find ("ScrollUI").transform.Find("Text").GetComponent<Text> ().color;
    		color.a = 1;
    		GameObject.Find ("ScrollUI").transform.Find("Text").GetComponent<Text> ().color = color;
    		closeScroll = true;
    		scrollOpen = false;
    		this.GetComponent<AudioSource> ().clip = scrollCloseSound;
    		this.GetComponent<AudioSource> ().Play ();
    	}
    	
    	GameObject closestObject(string name) {
    		GameObject player = GameObject.FindGameObjectWithTag ("Player");
    		GameObject[] allObjects = GameObject.FindObjectsOfType (typeof(GameObject)) as GameObject[];
    		GameObject closest = null;
    		foreach (GameObject obj in allObjects) {
    			if (obj.name == name) {
    				if (closest == null) {
    					closest = obj;
    				}
    				if (Vector3.Distance (player.transform.position, closest.transform.position) >
    				   Vector3.Distance (player.transform.position, obj.transform.position)) {
    					closest = obj;
    				}
    			} 
    		}
    		return closest;
    	}
        public void Heal(float amount)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().ApplyHealth(amount);
        }
        public void SetWeaponId(int id)
        {
            itemId = id;
        }
        public void AddAmmo(int amount)
        {
            //NOTE: the "SetWeaponId" must be called prior to this
            if (itemId == 9999999)
                return;
            InvWeaponManager invMg = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<InvWeaponManager>();
            invMg.AddAmmo(itemId, amount);
            itemId = 9999999;
        }
        public void UseFlashlight()
        {
            GameObject.FindGameObjectWithTag("PlayerFlashlight").GetComponent<FlashLight>().ActivateFlashlight();
        }
        void OnGUI() {
    		if (showGUI) {
    			GUI.Label (new Rect (50, Screen.height - 150, 200, 150), message, textStyling);
    			Color color = textStyling.normal.textColor;
    			color.a = guiAlpha;
    			GUI.color = color;
    			textStyling.normal.textColor = GUI.color;
    		}
    	}
    }
}