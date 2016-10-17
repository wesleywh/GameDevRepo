using UnityEngine;
using System.Collections;

public class SetVariable : MonoBehaviour {
	enum findType { ObjectsName, ObjectsTag };
	[SerializeField] private findType holdingObjectFindBy = findType.ObjectsTag;
	[SerializeField] private string holdingObject = null;
	[SerializeField] private string scriptName = null;
	[SerializeField] private GameObject enableObject = null;
	[SerializeField] private string variableName = null;
	[SerializeField] private string variableSetTo = null;
	enum varType { Bool, String, Integer, Float};
	[SerializeField] private varType variableType = varType.Bool;
	enum actionType {OnTriggerEnter, OnTriggerExit, OnStart, IsCalled}
	[SerializeField] private actionType actionToSetVar = actionType.OnTriggerEnter; 
	[SerializeField] private float delayVarSet = 0.0f;
	private GameObject manager = null;
	private bool finished = false;
	// Use this for initialization
	void Start () {
		SetManagerVar ();
		if (actionToSetVar == actionType.OnStart && finished == false) {
			StartCoroutine(SetVarValue());
		}
	}
	void Call() {
		StartCoroutine(SetVarValue());
	}
	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player" && finished == false) {
			StartCoroutine(SetVarValue());
		}
	}

	void OnTriggerExit(Collider col) {
		if (col.tag == "Player" && finished == false) {
			StartCoroutine(SetVarValue());
		}
	}
	IEnumerator SetVarValue() {
		finished = true;
		if (enableObject != null) {
			enableObject.SetActive (true);
		}
		yield return new WaitForSeconds (delayVarSet);
		if (string.IsNullOrEmpty (scriptName) == false) {
			switch (variableType) {
				case varType.Bool:
					manager.GetComponent (scriptName).GetType ().GetField (variableName).SetValue (manager.GetComponent (scriptName), bool.Parse (variableSetTo));
					break;
				case varType.Float:
					manager.GetComponent (scriptName).GetType ().GetField (variableName).SetValue (manager.GetComponent (scriptName), float.Parse (variableSetTo));
					break;
				case varType.Integer:
					manager.GetComponent (scriptName).GetType ().GetField (variableName).SetValue (manager.GetComponent (scriptName), int.Parse (variableSetTo));
					break;
				case varType.String:
					manager.GetComponent (scriptName).GetType ().GetField (variableName).SetValue (manager.GetComponent (scriptName), bool.Parse (variableSetTo));
					break;
			}
		}
	}
	private void SetManagerVar() {
		switch (holdingObjectFindBy) {
		case findType.ObjectsName:
			manager = GameObject.Find (holdingObject);
			break;
		case findType.ObjectsTag:
			manager = GameObject.FindGameObjectWithTag (holdingObject);
			break;
		}
	}
}
