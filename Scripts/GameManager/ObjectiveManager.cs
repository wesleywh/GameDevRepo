using UnityEngine;
using System.Collections;
using System.Collections.Generic; 			//use list & dictionaries
using System;								//allow serialization
using UnityEngine.UI;						//to access UI elements

[Serializable]
public class Objectives {
	public string tag = null;
	public string title = null;
	public string state = "InProgress"; //InProgress, Failed, Complete
}
public class ObjectiveManager : MonoBehaviour {
	[SerializeField] private AudioClip addedSound = null;
	[SerializeField] private AudioClip failedSound = null;
	[SerializeField] private AudioClip completeSound = null;
	[SerializeField] private float fadeOutSpeed = 0.5f;
	[SerializeField] private float priorFadeDelay = 4.0f;
	[Header("Don't edit this here. This is simply for debugging purposes")]
	[SerializeField] private List<Objectives> objectives = new List<Objectives> ();

	private float guiAlpha = 1.0f;
	private bool startFade = false;
	private float timer = 0;

	public void AddObjective(string input) {
		string[] output = input.Split (',');
		string assignTag = output [0];
		string title = output [1];
		Objectives newObjective = new Objectives();
		newObjective.tag = assignTag;
		newObjective.title = title;
		objectives.Add(newObjective);
		GameObject.FindGameObjectWithTag ("PopUpText").GetComponent<Text> ().text = "Objective Added: " + title;
		if (addedSound != null) {
			this.GetComponent<AudioSource> ().clip = addedSound;
			this.GetComponent<AudioSource> ().Play ();
		}
		startFade = true;
	}

	public void RemoveObjective(string tag) {
		foreach (Objectives currObj in objectives) {
			if (currObj.tag == tag) {
				objectives.Remove (currObj);
			}
		}
	}

	public void CompleteObjective(string tag) {
		foreach (Objectives currObj in objectives) {
			if (currObj.tag == tag) {
				currObj.state = "Complete";
			}
		}
		if (completeSound != null) {
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

	void Update() {
		if (startFade == true) {
			timer += Time.deltaTime;
			if (timer >= priorFadeDelay) {
				guiAlpha -= Time.deltaTime * fadeOutSpeed;
				Color org = GameObject.FindGameObjectWithTag ("PopUpText").GetComponent<Text> ().color;
				org.a = guiAlpha;
				GameObject.FindGameObjectWithTag ("PopUpText").GetComponent<Text> ().color = org;
				if (guiAlpha <= 0) {
					startFade = false;
					org.a = 0;
					GameObject.FindGameObjectWithTag ("PopUpText").GetComponent<Text> ().color = org;
					guiAlpha = 1;
					timer = 0;
				}
			}
		}
	}
}
