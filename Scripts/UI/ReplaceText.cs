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
		QuickSlot1,
        QuickSlot2,
        QuickSlot3,
		Objectives,
        Dismount
	}
	[SerializeField] private ButtonOptions buttonToShow = ButtonOptions.Left;
	void OnEnable () {
		UpdateText ();
	}
	public void ShowEmpty() {
		this.GetComponent<Text> ().text = "__Waiting Input__";
	}
	public void UpdateText() {
		switch (buttonToShow) {
			case ButtonOptions.Backwards:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [1].negative.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
			case ButtonOptions.Forwards:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [1].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
			case ButtonOptions.Left:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [0].negative.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
			case ButtonOptions.Right:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [0].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
			case ButtonOptions.Submit:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [7].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
			case ButtonOptions.Jump:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [6].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
			case ButtonOptions.Run:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [9].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
			case ButtonOptions.Close:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [17].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
			case ButtonOptions.Action:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [10].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
			case ButtonOptions.Inventory:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [11].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
            case ButtonOptions.QuickSlot1:
				this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [12].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
            case ButtonOptions.QuickSlot2:
                this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [13].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
            case ButtonOptions.QuickSlot3:
                this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [14].positive.ToString ().ToUpper().Replace ("ALPHA", "");
				break;
            case ButtonOptions.Objectives:
                this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [19].positive.ToString ().ToUpper().Replace ("ALPHA", "");
			    break;
            case ButtonOptions.Dismount:
                this.GetComponent<Text> ().text = InputManager.GetInputConfiguration (PlayerID.One).axes [22].positive.ToString ().ToUpper().Replace ("ALPHA", "");
                break;
		}
	}
}
