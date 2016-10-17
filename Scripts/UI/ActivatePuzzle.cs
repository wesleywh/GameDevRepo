using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using UnityEngine.UI;
using System.Collections.Generic; 	//for list & dictionary
using UnityEngine.Events;			//for inspector custom script execution

[RequireComponent(typeof(SphereCollider))]
public class ActivatePuzzle : MonoBehaviour {
	[SerializeField] private string giveItem = "";
	[SerializeField] private GameObject LockUIInfo;
	[SerializeField] private UnityEvent onStartPuzzle;
	[SerializeField] private UnityEvent onExitPuzzle;
	[SerializeField] private UnityEvent onResetPuzzle;
	[SerializeField] private UnityEvent onSuccess;
	[Header("Split by spaces")]
	[SerializeField] private string correctCombo;
	private List<string> input = new List<string> ();
	private bool inPuzzle = false;

	void Start() {
		this.GetComponent<SphereCollider> ().isTrigger = true;
		this.gameObject.layer = 2;
	}
	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player" && InputManager.GetButtonDown ("Action")) {
			EnablePuzzle ();
		}
	}
	void OnTriggerStay(Collider col) {
		if (col.tag == "Player" && InputManager.GetButtonDown ("Action")) {
			EnablePuzzle ();
		}
	}
	void Update() {
		if (inPuzzle) {
			if (InputManager.GetButtonDown ("Cancel")) {
				ExitPuzzle ();
			}
		}
	}
	void ExitPuzzle() {
		GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().SetInventoryState (true);
		inPuzzle = false;
		onExitPuzzle.Invoke ();
	}
	void EnablePuzzle() {
		GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().SetInventoryState (false);
		onStartPuzzle.Invoke ();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		inPuzzle = true;
	}
	public void ResetPuzzle() {
		input.Clear();
		onResetPuzzle.Invoke ();
	}
	public void AddButton(string text) {
		input.Add (text);
		if (input.Count > 4) {
			ResetPuzzle ();
		} else {
			LockUIInfo.transform.GetChild (input.Count - 1).GetComponent<Text> ().text = text;
			CheckCombo ();
		}
	}
	void CheckCombo() {
		for (int i = 0; i < input.Count; i++) {
			if (input [i] != correctCombo.Split (' ') [i]) {
				return;
			}
		}
		if (input.Count > 3) {
			GameObject gameManager = GameObject.FindGameObjectWithTag ("GameManager");
			inPuzzle = false;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			if (giveItem != "" && gameManager.GetComponent<InventoryManager> ().InventoryFull () == false) {
				onSuccess.Invoke ();
				GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().SetInventoryState (true);
				if (giveItem != "") {
					gameManager.GetComponent<InventoryManager> ().AddToInventory (giveItem);
				}
			} else if(giveItem == ""){
				onSuccess.Invoke ();
			}
		}
	}
	public void ClosePuzzleUI() {
		GameObject.FindGameObjectWithTag ("PuzzleLockUI").SetActive (false);
	}
}