using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using UnityEngine.UI;

public class ReplaceText : MonoBehaviour {

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
	[SerializeField] private ButtonOptions buttonToShow = ButtonOptions.Left;
	void Start () {
		UpdateText ();
	}
	public void ShowEmpty() {
		this.GetComponent<Text> ().text = "__Waiting Input__";
	}
	public void UpdateText() {
		switch (buttonToShow) {
			case ButtonOptions.Backwards:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [1].negative.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.Forwards:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [1].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.Left:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [0].negative.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.Right:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [0].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.Submit:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [7].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.Jump:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [6].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.Run:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [9].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.Close:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [17].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.Action:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [10].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.Inventory:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [11].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.InventorySlot1:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [12].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.InventorySlot2:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [13].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.InventorySlot3:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [14].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.InventorySlot4:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [15].positive.ToString ().Replace ("Alpha", "");
				break;
			case ButtonOptions.InventorySlot5:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [16].positive.ToString ().Replace ("Alpha", "");
				break;
		case ButtonOptions.Objectives:
			this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [19].positive.ToString ().Replace ("Alpha", "");
			break;
		}
	}
}
