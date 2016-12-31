using UnityEngine;
using System.Collections;
using TeamUtility.IO;					//Custom Input Manager
using UnityEngine.UI;					//for UI access	
using System;							//To allow serializing of classes

[Serializable]
class SelectedTarget {
	public GameObject targetGameObject = null;
	public string targetTag = null;
	public string targetName = null;
}
public class IfItemEnable : MonoBehaviour {
	[SerializeField] enum ExecutionType { Action, TriggerEnter, TriggerExit }
	[SerializeField] private ExecutionType ExecuteType = ExecutionType.Action;
	[SerializeField] private float actionRadius = 3.0f;
	[SerializeField] private string itemName = "";
	[SerializeField] private string activationTag = "Player";
	[SerializeField] private string errorMsg = "You seem to be missing something...";
	[SerializeField] private SelectedTarget[] enableTarget = null;
	[SerializeField] private bool removeItem = true;
	private GameObject target;

	void OnTriggerEnter(Collider col) {
		if (col.tag == activationTag && ExecuteType == ExecutionType.TriggerEnter) {
			if (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().HasItem (itemName)) {
				if (removeItem == true) {
					GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().RemoveItem (itemName);
				}
				EnableTarget ();
			} else {
				GameObject.FindGameObjectWithTag("PopUpText").GetComponent<Text>().text = errorMsg;
			}
		}
	}

	void OnTriggerExit(Collider col) {
		if (col.tag == activationTag && ExecuteType == ExecutionType.TriggerExit) {
			if (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().HasItem (itemName)) {
				if (removeItem == true) {
					GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().RemoveItem (itemName);
				}
				EnableTarget ();
			} else {
				GameObject.FindGameObjectWithTag("PopUpText").GetComponent<Text>().text = errorMsg;
			}
		}
	}

	void Update() {
		if (GameObject.FindGameObjectWithTag ("Player")) {
			target = GameObject.FindGameObjectWithTag ("Player");
			if (Vector3.Distance (target.transform.position, transform.position) <= actionRadius && InputManager.GetButton("Action")) {
				if (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().HasItem (itemName)) {
					if (removeItem == true) {
						GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().RemoveItem (itemName);
					}
					EnableTarget ();
				} else {
					GameObject.FindGameObjectWithTag("PopUpText").GetComponent<Text>().text = errorMsg;
				}
			}
		}
	}

	public void ExecuteCheck(bool showError=false) {
		if (GameObject.FindGameObjectWithTag ("Player")) {
			target = GameObject.FindGameObjectWithTag ("Player");
			if (Vector3.Distance (target.transform.position, transform.position) <= actionRadius) {
				if (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().HasItem (itemName)) {
					if (removeItem == true) {
						GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().RemoveItem (itemName);
					}
					EnableTarget ();
				} else if(showError == true){
					GameObject.FindGameObjectWithTag("PopUpText").GetComponent<Text>().text = errorMsg;
				}
			}
		}
	}
	void EnableTarget() {
		foreach (SelectedTarget target in enableTarget) {
			if (target.targetGameObject != null) {
				target.targetGameObject.SetActive (true);
			} else if (target.targetName != null) {
				GameObject.Find (target.targetName).SetActive (true);
			} else {
				GameObject.FindGameObjectWithTag (target.targetTag).SetActive (true);
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (transform.position, actionRadius);
	}
}
