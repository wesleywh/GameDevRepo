using UnityEngine;
using System.Collections;
using TeamUtility.IO;				//for input management
using UnityEngine.UI;				//For UI
using System.Collections.Generic; 	//for list & dictionary

public class ReassignButton : MonoBehaviour {
	[SerializeField] private GameObject reassignedButton;
	[SerializeField] private string originalKey;
	[SerializeField] private GameObject backButton;
	public void SetBackButton(GameObject button) {
		backButton = button;
	}
	public void SetReturnButton(GameObject button) {
		reassignedButton = button;
	}
	public void StartListening(string keyIndexAndisPostive) {
		if (InputManager.IsScanning) {
			return;
		}
		originalKey = reassignedButton.transform.GetChild (0).GetComponent<Text> ().text;
		reassignedButton.transform.GetChild (0).GetComponent<Text> ().text = "_";
		InputManager.StartKeyboardButtonScan((key, arg) => {
			if(KeyAssigned(key.ToString()) == true) {
				backButton.SetActive(true);
				reassignedButton.transform.GetChild (0).GetComponent<Text> ().text = originalKey;
				return true;
			}
			else {
				int index = int.Parse(keyIndexAndisPostive.Split(',')[0]);
				bool positive = (keyIndexAndisPostive.Split(',')[1].ToLower() == "true") ? true : false; 
				if(positive) {
					if(index == 0) {
						InputManager.GetInputConfiguration (PlayerID.One).axes [index].positive = key;
						InputManager.GetInputConfiguration (PlayerID.One).axes [4].positive = key;
					}
					else if(index == 1) {
						InputManager.GetInputConfiguration (PlayerID.One).axes [index].positive = key;
						InputManager.GetInputConfiguration (PlayerID.One).axes [5].positive = key;
					}
					else if(index == 17) {
						InputManager.GetInputConfiguration (PlayerID.One).axes [index].positive = key;
						InputManager.GetInputConfiguration (PlayerID.One).axes [8].positive = key;
					}
					else {
						InputManager.GetInputConfiguration (PlayerID.One).axes [index].positive = key;
					}
				}
				else {
					if(index == 0) {
						InputManager.GetInputConfiguration (PlayerID.One).axes [index].negative = key;
						InputManager.GetInputConfiguration (PlayerID.One).axes [4].negative = key;
					}
					else if(index == 1) {
						InputManager.GetInputConfiguration (PlayerID.One).axes [index].negative = key;
						InputManager.GetInputConfiguration (PlayerID.One).axes [5].negative = key;
					}
					else if(index == 17) {
						InputManager.GetInputConfiguration (PlayerID.One).axes [index].negative = key;
						InputManager.GetInputConfiguration (PlayerID.One).axes [8].negative = key;
					}
					else {
						InputManager.GetInputConfiguration (PlayerID.One).axes [index].negative = key;
					}
				}
				reassignedButton.transform.GetChild(0).GetComponent<Text>().text = key.ToString();
				backButton.SetActive(true);
				return true;
			}
		}, 10.0f, null);
	}

	bool KeyAssigned(string key) {
		bool value = false;
		for (int i = 0; i < InputManager.GetInputConfiguration (PlayerID.One).axes.Capacity; i++) {
			if (key == InputManager.GetInputConfiguration (PlayerID.One).axes [i].negative.ToString () || 
				key == InputManager.GetInputConfiguration (PlayerID.One).axes [i].positive.ToString ()) {
				value = true;
			}
		}
		return value;
	}
}
