using UnityEngine;
using System.Collections;
using System.Collections.Generic; 	//for list & dictionary
using System;						//for Serializable
using UnityEngine.UI;				//to access images
using TeamUtility.IO;				//input manager to recognize button presses

public enum ArgType {
	String,
	Float,
	Int,
	Double,
	Bool
}
[Serializable]
public class ScriptSelection {
	public string targetTag = "";
	public string targetName = "";
	public string functionName = "";
	public string functionArgument = "";
	public ArgType argumentType = ArgType.String;
	public bool dontDestroyOnUse = false;
	public bool executeFunctionOnDrop = false;
	public bool allowDropping = true;
}
[Serializable]
public class ItemDictionary {
	public string itemName = "";
	public Sprite UIImage;
	public GameObject droppableItem = null;
	public ScriptSelection scriptToExecute = null;
}
public class InventoryManager : MonoBehaviour {
	[SerializeField] private GUIStyle textStyling;
	[SerializeField] private string fullInventroyMessage;
	[Tooltip("Parent UI Gameobject of Inventory")]
//	[SerializeField] private GameObject UIIventory;
	[SerializeField] private ItemDictionary[] itemDictionary;
	private Dictionary<string, Sprite> dictionary = new Dictionary<string, Sprite>();
	private Dictionary<string, GameObject> dropItemDictionary = new Dictionary<string, GameObject>();
	private Dictionary<string, ScriptSelection> itemScriptDictionary = new Dictionary<string, ScriptSelection>();
	private List<string> playerInventory = new List<string>();
	[SerializeField] private GameObject icons = null;
	[SerializeField] private GameObject text = null;
	private bool showGUI = false;
	private float guiAlpha = 0.0f;
	private string message;
	private float buttonPressTimer = 0.0f;
	private bool droppedItem = false;
	private bool canUseInventory = true;
	[HideInInspector] public string lastDroppedItem;

	void Start() {
		for (int i=0; i<itemDictionary.Length; i++) {
			dictionary.Add (itemDictionary[i].itemName.ToLower(), itemDictionary[i].UIImage);
			if (itemDictionary [i].droppableItem != null) {
				dropItemDictionary.Add (itemDictionary [i].itemName.ToLower (), itemDictionary [i].droppableItem);
			}
			if (itemDictionary [i].scriptToExecute != null) {
				itemScriptDictionary.Add (itemDictionary [i].itemName.ToLower (), itemDictionary [i].scriptToExecute);
			}
		}
		if (text == null) { 
			text = GameObject.FindGameObjectWithTag ("UIText");
		}
		if (icons == null) {
			icons = GameObject.FindGameObjectWithTag ("UIIcons");
		}
	}
	void Update() {
		if (showGUI) {
			guiAlpha -= Time.deltaTime;
			if (guiAlpha <= 0) {
				showGUI = false;
			}
		}
		if (InputManager.GetButton ("InventorySlot1") || InputManager.GetButton ("InventorySlot2") ||
			InputManager.GetButton ("InventorySlot3") || InputManager.GetButton ("InventorySlot4") ||
			InputManager.GetButton ("InventorySlot5")) {
			buttonPressTimer += Time.deltaTime;
			if (buttonPressTimer > 2.0f && playerInventory.Count > 0) {
				droppedItem = true;
				buttonPressTimer = 0.0f;
				if (InputManager.GetButton ("InventorySlot1") && playerInventory.Count > 0) {
					DropItemAtValue (1);
				} else if (InputManager.GetButton ("InventorySlot2") && playerInventory.Count > 1) {
					DropItemAtValue (2);
				} else if (InputManager.GetButton ("InventorySlot3") && playerInventory.Count > 2) {
					DropItemAtValue (3);
				} else if (InputManager.GetButton ("InventorySlot4") && playerInventory.Count > 3) {
					DropItemAtValue (4);
				} else if (InputManager.GetButton ("InventorySlot5") && playerInventory.Count > 4) {
					DropItemAtValue (5);
				}
			}
		} 
		if (InputManager.GetButtonUp ("InventorySlot1") && playerInventory.Count > 0) {
			if (buttonPressTimer < 2.0 && droppedItem == false) {
				UseItemAtValue (1);
			} 
			droppedItem = false;
			buttonPressTimer = 0.0f;
		}
		if (InputManager.GetButtonUp ("InventorySlot2") && playerInventory.Count > 1) {
			if (buttonPressTimer < 2.0) {
				UseItemAtValue (2);
			} 
			buttonPressTimer = 0.0f;
		}
		if (InputManager.GetButtonUp ("InventorySlot3") && playerInventory.Count > 2) {
			if (buttonPressTimer < 2.0) {
				UseItemAtValue (3);
			} 
			buttonPressTimer = 0.0f;
		}
		if (InputManager.GetButtonUp ("InventorySlot4") && playerInventory.Count > 3) {
			if (buttonPressTimer < 2.0) {
				UseItemAtValue (4);
			} 
			buttonPressTimer = 0.0f;
		}
		if (InputManager.GetButtonUp ("InventorySlot5") && playerInventory.Count > 4) {
			if (buttonPressTimer < 2.0) {
				UseItemAtValue (5);
			} 
			buttonPressTimer = 0.0f;
		}
	}
	void FixedUpdate() {
		if (playerInventory.Count == 0) {
			if (icons != null) {
				foreach (Transform icon in icons.transform) {
					Color color = icon.gameObject.GetComponent<Image> ().color;
					color.a = 0;
					icon.gameObject.GetComponent<Image> ().color = color;
				}
				foreach (Transform child in text.transform) {
					Color color = child.gameObject.GetComponent<Text> ().color;
					color.a = 0;
					child.gameObject.GetComponent<Text> ().color = color;
				}
			}
		} 
	}
	public void AddToInventory (string value) {
		playerInventory.Add (value);
		//show icon image
		GameObject icon = icons.transform.Find ("icon" + playerInventory.Count).gameObject;
		icon.GetComponent<Image> ().sprite = dictionary [value];
		Color imageColor = icon.GetComponent<Image> ().color;
		imageColor.a = 1;
		icon.GetComponent<Image> ().color = imageColor;
		//show icon text
		GameObject textChild = text.transform.Find ("text" + playerInventory.Count).gameObject;
		Color textColor = textChild.gameObject.GetComponent<Text> ().color;
		textColor.a = 1;
		textChild.GetComponent<Text> ().color = textColor;
	}
	public void UseItemAtValue(int number) {
		if (canUseInventory) {
			string item = playerInventory [number - 1];
			GameObject icon = GameObject.FindGameObjectWithTag("UIIcons").transform.Find ("icon" + number).gameObject;
			//hide icon image
			Color imageColor = icon.GetComponent<Image> ().color;
			imageColor.a = 0;
			icon.GetComponent<Image> ().color = imageColor;
			//hide icon text
			GameObject textChild = GameObject.FindGameObjectWithTag("UIText").transform.Find ("text" + number).gameObject;
			Color textColor = textChild.gameObject.GetComponent<Text> ().color;
			textColor.a = 0;
			textChild.GetComponent<Text> ().color = textColor;
			ScriptSelection script = null;
			if (itemScriptDictionary.TryGetValue (item, out script)) { 
				if (script.targetTag != "") {							//execute script according to tag
					switch (script.argumentType) {
					case ArgType.Float:
						GameObject.FindGameObjectWithTag (script.targetTag).SendMessage (script.functionName, float.Parse (script.functionArgument));
						break;
					case ArgType.Int:
						GameObject.FindGameObjectWithTag (script.targetTag).SendMessage (script.functionName, int.Parse (script.functionArgument));
						break;
					case ArgType.String:
						GameObject.FindGameObjectWithTag (script.targetTag).SendMessage (script.functionName, script.functionArgument);
						break;
					case ArgType.Double:
						GameObject.FindGameObjectWithTag (script.targetTag).SendMessage (script.functionName, double.Parse (script.functionArgument));
						break;
					case ArgType.Bool:
						bool inputArg = script.functionArgument == "true";
						GameObject.FindGameObjectWithTag (script.targetTag).SendMessage (script.functionName, inputArg);
						break;
					}
				} else if (script.targetName != "") {							//execute script according to object name
					if (GameObject.Find (script.targetName)) {
						switch (script.argumentType) {
							case ArgType.Float:
								GameObject.Find (script.targetName).SendMessage (script.functionName, float.Parse (script.functionArgument));
								break;
							case ArgType.Int:
								GameObject.Find (script.targetName).SendMessage (script.functionName, int.Parse (script.functionArgument));
								break;
							case ArgType.String:
								GameObject.Find (script.targetName).SendMessage (script.functionName, script.functionArgument);
								break;
							case ArgType.Double:
								GameObject.Find (script.targetName).SendMessage (script.functionName, double.Parse (script.functionArgument));
								break;
							case ArgType.Bool:
								bool inputArg = script.functionArgument == "true";
								GameObject.Find (script.targetName).SendMessage (script.functionName, inputArg);
								break;
						}
					}
				} else {
					//try to drop the item
					GameObject foundItem = null;
					if (dropItemDictionary.TryGetValue (item, out foundItem)) {
						//Drop item listed
						Vector3 position = GameObject.FindGameObjectWithTag ("PlayerCamera").transform.position;
						Quaternion rotation = GameObject.FindGameObjectWithTag ("PlayerCamera").transform.rotation;
						position = new Vector3 (position.x, position.y, position.z + 1);
						Instantiate (foundItem, position, rotation);
					} 
				}
				if (script.dontDestroyOnUse == false) {
					playerInventory.RemoveAt (number - 1);
				}
			} 
			ReassignUIImages ();
		}
	}
	public void DropItemAtValue(int number) {
		if (canUseInventory) {
			string item = playerInventory [number - 1];
			lastDroppedItem = item;
			ScriptSelection script = null;
			if (itemScriptDictionary.TryGetValue (item, out script)) { 
				if (script.executeFunctionOnDrop == true) {
					UseItemAtValue (number);
				}
				if (script.allowDropping == true) {
					playerInventory.RemoveAt (number - 1);
					GameObject icon = icons.transform.Find ("icon" + number).gameObject;
					//hide icon image
					Color imageColor = icon.GetComponent<Image> ().color;
					imageColor.a = 0;
					icon.GetComponent<Image> ().color = imageColor;
					//hide icon text
					GameObject textChild = text.transform.Find ("text" + number).gameObject;
					Color textColor = textChild.gameObject.GetComponent<Text> ().color;
					textColor.a = 0;
					textChild.GetComponent<Text> ().color = textColor;
					GameObject foundItem = null;
					if (dropItemDictionary.TryGetValue (item, out foundItem)) {
						//Drop item listed
						GameObject playerCam = GameObject.FindGameObjectWithTag ("PlayerCamera");
						Quaternion rotation = GameObject.FindGameObjectWithTag ("PlayerCamera").transform.rotation;
						Vector3 position = playerCam.transform.position + playerCam.GetComponent<Camera>().transform.forward*0.5f;
						Instantiate (foundItem, position, rotation);
					} 
				}
			} 
			ReassignUIImages ();
		}
	}
	private void ReassignUIImages() {
		foreach (Transform icon in GameObject.FindGameObjectWithTag("UIIcons").transform) {
			Color color = icon.gameObject.GetComponent<Image> ().color;
			color.a = 0;
			icon.gameObject.GetComponent<Image> ().color = color;
		}
		foreach (Transform child in GameObject.FindGameObjectWithTag("UIText").transform) {
			Color color = child.gameObject.GetComponent<Text> ().color;
			color.a = 0;
			child.gameObject.GetComponent<Text> ().color = color;
		}
		for (int i = 0; i < playerInventory.Count; i++) {
			//show icon image
			GameObject icon = GameObject.FindGameObjectWithTag("UIIcons").transform.Find ("icon" + (i+1)).gameObject;
			icon.GetComponent<Image> ().sprite = dictionary [playerInventory[i]];
			Color imageColor = icon.GetComponent<Image> ().color;
			imageColor.a = 1;
			icon.GetComponent<Image> ().color = imageColor;
			//show icon text
			GameObject textChild = text.transform.Find ("text" + (i+1)).gameObject;
			Color textColor = textChild.gameObject.GetComponent<Text> ().color;
			textColor.a = 1;
			textChild.GetComponent<Text> ().color = textColor;
		}
	}
	public bool InventoryFull() {
		if(playerInventory.Count >= 5) {
			guiAlpha = 1.0f;
			message = fullInventroyMessage;
			showGUI = true;
			return true;
		}
		else {
			return false;
		}
	}
	public bool HasItem(string name) {
		for (int i = 0; i < playerInventory.Count; i++) {
			if (playerInventory [i] == name.ToLower()) {
				return true;
			}
		}
		return false;
	}
	public void RemoveItem(string name) {
		for (int i = 0; i < playerInventory.Count; i++) {
			if (playerInventory [i] == name.ToLower()) {
				playerInventory.RemoveAt (i);
				return;
			}
		}
		ReassignUIImages ();
	}
	public List<string> GetPlayerInventory() {
		return playerInventory;
	}
	public void SetPlayerInventory(List<string> items) {
		playerInventory = items;
		ReassignUIImages ();
	}
	public void SetInventoryState(bool state) {
		canUseInventory = state;
	}
	void OnGUI() {
		if (showGUI) {
			GUI.Label (new Rect (50, Screen.height - 50, 200, 10), message, textStyling);
			Color color = textStyling.normal.textColor;
			color.a = guiAlpha;
			GUI.color = color;
			textStyling.normal.textColor = GUI.color;
		}
	}
}
