using UnityEngine;
using System.Collections;
using System.Collections.Generic; 			//use list & dictionaries
using System;								//allow serialization
using UnityEngine.UI;						//to access UI elements

namespace CyberBullet.GameManager {
    [Serializable]
    public class Objectives {
    	public string tag = null;
    	public string title = null;
    	public string state = "InProgress"; //InProgress, Failed, Complete
    }

    [RequireComponent(typeof(CyberBullet.GameManager.GUIManager))]
    public class ObjectiveManager : MonoBehaviour {
        [Header("AddObjective(string input)")]
        [Space(-10)]
        [Header("AddObjectiveNew(string tag, string title)")]
        [Space(-10)]
        [Header("RemoveObjective(string tag)")]
        [Space(-10)]
        [Header("CompleteObjective(string tag)")]
        [Space(-10)]
        [Header("FailObjective(string input)")]
        [Space(-10)]
        [Header("ShowCompleteObjectives()")]
        [Space(10)]
    	[SerializeField] private AudioClip addedSound = null;
    	[SerializeField] private AudioClip failedSound = null;
    	[SerializeField] private AudioClip completeSound = null;

    	[Header("Don't edit this here. This is simply for debugging purposes")]
    	[SerializeField] private List<Objectives> objectives = new List<Objectives> ();

        private GUIManager guiManager = null;

        void Start()
        {
            guiManager = this.gameObject.GetComponent<GUIManager>();
        }
    	public void AddObjective(string input) {
    		string[] output = input.Split (',');
    		string assignTag = output [0];
    		string title = output [1];
    		Objectives newObjective = new Objectives();
    		newObjective.tag = assignTag;
    		newObjective.title = title;
    		objectives.Add(newObjective);
            guiManager.SetPopUpText("Objective Added: " + title);
    		if (addedSound != null) {
    			this.GetComponent<AudioSource> ().clip = addedSound;
    			this.GetComponent<AudioSource> ().Play ();
    		}
    	}
        public void AddObjectiveNew(string tag, string title) {
            Objectives newObjective = new Objectives();
            newObjective.tag = tag;
            newObjective.title = title;
            objectives.Add(newObjective);
            guiManager.SetPopUpText("Objective Added: " + title);
            if (addedSound != null) {
                this.GetComponent<AudioSource> ().clip = addedSound;
                this.GetComponent<AudioSource> ().Play ();
            }
        }
    	public void RemoveObjective(string tag) {
    		foreach (Objectives currObj in objectives) {
    			if (currObj.tag == tag) {
    				objectives.Remove (currObj);
    			}
    		}
    	}
    	public void CompleteObjective(string tag) {
            bool playSound = false;
    		foreach (Objectives currObj in objectives) {
                if (currObj.tag == tag && currObj.state != "Complete") {
                    guiManager.SetPopUpText("Objective Completed: " + currObj.title);
    				currObj.state = "Complete";
                    playSound = true;
    			}
    		}
            if (completeSound != null && playSound == true) {
    			this.GetComponent<AudioSource> ().clip = completeSound;
    			this.GetComponent<AudioSource> ().Play ();
    		}
    	}
    	public void FailObjective(string tag) {
    		foreach (Objectives currObj in objectives) {
    			if (currObj.tag == tag) {
    				currObj.state = "Failed";
    			}
    		}
    		if (failedSound != null) {
    			this.GetComponent<AudioSource> ().clip = failedSound;
    			this.GetComponent<AudioSource> ().Play ();
    		}
    	}
    	public List<Objectives> ShowCompleteObjectives() {
    		List<Objectives> completeObjectives = new List<Objectives> ();
    		foreach (Objectives currObj in objectives) {
    			if (currObj.state == "Complete") {
    				completeObjectives.Add (currObj);
    			}
    		}
    		return completeObjectives;
    	}
    	public List<Objectives> ShowFailedObjectives() {
    		List<Objectives> failedObjectives = new List<Objectives> ();
    		foreach (Objectives currObj in objectives) {
    			if (currObj.state == "Failed") {
    				failedObjectives.Add (currObj);
    			}
    		}
    		return failedObjectives;
    	}
    	public List<Objectives> ShowInProgressObjectives() {
    		List<Objectives> InProgressObjectives = new List<Objectives> ();
    		foreach (Objectives currObj in objectives) {
    			if (currObj.state == "InProgress") {
    				InProgressObjectives.Add (currObj);
    			}
    		}
    		return InProgressObjectives;
    	}
    	public List<Objectives> ShowAllObjectives() {
    		return objectives;
    	}
    	public void AddObjectList(List<Objectives> input) {
    		objectives = input;
    	}
        public void SetAllObjectives(List<Objectives> newObjectives)
        {
            objectives.Clear();
            objectives = newObjectives;
        }
    }
}