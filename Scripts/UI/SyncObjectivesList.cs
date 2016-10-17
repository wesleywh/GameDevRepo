using UnityEngine;
using System.Collections;
using TeamUtility.IO;						//Custome InputManager Manager
using System.Collections.Generic; 			//use list & dictionaries
using UnityEngine.UI;						//to access UI elements

public class SyncObjectivesList : MonoBehaviour {

	private bool btnPressed = false;
	// Update is called once per frame
	void Update () {
		if (InputManager.GetButton ("Objectives")) {
			List<Objectives> currentObjectives = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<ObjectiveManager> ().ShowInProgressObjectives ();
			string formatedString = "";
			foreach(Objectives objective in currentObjectives) {
				formatedString += "Objective: " + objective.title+"\n\r";
			}
			GameObject.FindGameObjectWithTag("ObjectivesList").GetComponent<Text> ().text = formatedString;
			btnPressed = true;
		}
		if(btnPressed == true && InputManager.GetButton ("Objectives") == false){
			btnPressed = false;
			GameObject.FindGameObjectWithTag("ObjectivesList").GetComponent<Text>().text = "";
		}
	}
}
