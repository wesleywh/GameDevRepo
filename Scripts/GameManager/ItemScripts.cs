using UnityEngine;
using System.Collections;
using UnityEngine.UI;			//for UI elements
using TeamUtility.IO;			//for recognizing buttons
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

	void Update() {
		if (guiAlpha > 0.0f) {
			guiAlpha -= Time.deltaTime;
			if (guiAlpha <= 0) {
				showGUI = false;
			}
		}
		if (scrollOpen && InputManager.GetButton ("Esc")) {
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
	public void OpenScroll(string text) {
		GameObject.Find ("ScrollUI").transform.Find("Scroll").GetComponent<Image>().fillAmount = 0;
		GameObject.Find ("ScrollUI").transform.Find("Text").GetComponent<Text> ().text = text;
		Color color = GameObject.Find ("ScrollUI").transform.Find("Text").GetComponent<Text> ().color;
		color.a = 0;
		GameObject.Find ("ScrollUI").transform.Find("Text").GetComponent<Text> ().color = color;
		openScroll = true;
		scrollOpen = true;
		this.GetComponent<AudioSource> ().clip = scrollOpenSound;
		this.GetComponent<AudioSource> ().Play ();
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
	public void UseKey(string type) {
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		GameObject closest = closestObject (type);
		if (Vector3.Distance (player.transform.position, closest.transform.position) < 2.0f) {
			message = "Successfully used key";
			if (keyUnlockSound) {
				this.GetComponent<AudioSource> ().clip = keyUnlockSound;
				this.GetComponent<AudioSource> ().Play ();
			}
			if (closest.GetComponent<Item> ()) {
				closest.GetComponent<Item> ().isLocked = false;
			}
			showGUI = true;
			guiAlpha = 1.0f;
		} else {
			message = "Unable to use key";
			showGUI = true;
			guiAlpha = 1.0f;
			if (keyFailSound) {
				this.GetComponent<AudioSource> ().clip = keyFailSound;
				this.GetComponent<AudioSource> ().Play ();
			}
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager> ().AddToInventory (type);
		}
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
	public void ActiveRuneStatue() {
		if (this.GetComponent<InventoryManager> ().HasItem ("qi_rune1")) {
			if (Vector3.Distance (GameObject.FindGameObjectWithTag ("Player").transform.position, GameObject.Find ("KnightStatueCutsceneLookAtPoint").transform.position) < 3.0f) {
				this.GetComponent<InventoryManager> ().RemoveItem ("qi_rune1");
				GameObject.Find ("Rune_Knight_Statue").GetComponent<CutScene> ().StartCutScene ();
				GameObject.Find ("KnightStatueCutsceneLookAtPoint").GetComponent<DisplayFloatingImage> ().enabled = false;
			}
		}
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
