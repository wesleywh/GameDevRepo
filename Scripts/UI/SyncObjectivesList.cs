using UnityEngine;
using System.Collections;
using TeamUtility.IO;						//Custome InputManager Manager
using System.Collections.Generic; 			//use list & dictionaries
using UnityEngine.UI;						//to access UI elements
using CyberBullet.GameManager;

namespace CyberBullet.UI {
    public class SyncObjectivesList : MonoBehaviour {

    	private bool btnPressed = false;
        private ObjectiveManager objectiveManager = null;
        private GUIManager guiManager = null;
        void Start()
        {
            objectiveManager = dontDestroy.currentGameManager.GetComponent<ObjectiveManager> ();
            guiManager = dontDestroy.currentGameManager.GetComponent<GUIManager>();
        }
    	void Update () {
            if (InputManager.GetButton ("Objectives") && guiManager.MenuOpen() == false) {
                List<Objectives> currentObjectives = objectiveManager.ShowInProgressObjectives ();
    			string formatedString = "";
    			foreach(Objectives objective in currentObjectives) {
    				formatedString += "Objective: " + objective.title+"\n\r";
    			}
                if (string.IsNullOrEmpty(formatedString))
                {
                    formatedString = "No Active Objectives";
                }
                guiManager.SetObjectivesText(formatedString);
    			btnPressed = true;
    		}
    		if(btnPressed == true && InputManager.GetButton ("Objectives") == false){
    			btnPressed = false;
                guiManager.SetObjectivesText("");
    		}
    	}
    }
}