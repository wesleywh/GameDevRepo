using UnityEngine;
using System.Collections;
using UnityEngine.UI;			//To Access Text Components
using TeamUtility.IO;			//To Access Key Bindings

public class InventoryText : MonoBehaviour {
	enum Button{
		InventorySlot1, 
		InventorySlot2, 
		InventorySlot3, 
		InventorySlot4, 
		InventorySlot5
	}
	[SerializeField] 
	private Button buttonToShow = Button.InventorySlot1;
	void Start() {
		string type = "";
		switch (buttonToShow) {
			case Button.InventorySlot1:
				type = "InventorySlot1";
				break;
			case Button.InventorySlot2:
				type = "InventorySlot2";
				break;
			case Button.InventorySlot3:
				type = "InventorySlot3";
				break;
			case Button.InventorySlot4:
				type = "InventorySlot4";
				break;
			case Button.InventorySlot5:
				type = "InventorySlot5";
				break;
			default:
				break;
		}
		UpdateText (type);
	}

	public void UpdateText (string type) {
		switch (type) {
			case "InventorySlot1":
				this.GetComponent<Text>().text = InputManager.GetInputConfiguration (PlayerID.One).axes [12].positive.ToString()+":";
				break;
			case "InventorySlot2":
				this.GetComponent<Text>().text = InputManager.GetInputConfiguration (PlayerID.One).axes [13].positive.ToString()+":";
				break;
			case "InventorySlot3":
				this.GetComponent<Text>().text = InputManager.GetInputConfiguration (PlayerID.One).axes [14].positive.ToString()+":";
				break;
			case "InventorySlot4":
				this.GetComponent<Text>().text = InputManager.GetInputConfiguration (PlayerID.One).axes [15].positive.ToString()+":";
				break;
			case "InventorySlot5":
				this.GetComponent<Text>().text = InputManager.GetInputConfiguration (PlayerID.One).axes [16].positive.ToString()+":";
				break;
			default:
				break;
		}
		this.GetComponent<Text> ().text = this.GetComponent<Text> ().text.Replace ("Alpha", "");
		this.GetComponent<Text> ().text = this.GetComponent<Text> ().text.Replace ("KeyPad", "");
	}
}
