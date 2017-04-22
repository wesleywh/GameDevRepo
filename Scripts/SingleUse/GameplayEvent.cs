using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;					//convert dictionary to array
using System;						//for dynamic typing (enable/disable components)
using TeamUtility.IO;				//Custom Input Manager

[System.Serializable]
public class ITypes {
	public enum ParameterType {Boolean, Float, Int, String};
}
[System.Serializable]
class EventParams {
	public ITypes.ParameterType ParameterType = ITypes.ParameterType.Boolean;
	public string param = "";
}
[System.Serializable]
class EventAction {
	public string name = "";					//not this is only for referencing in the inspector
	public float delay = 0.0f;
	[Header("Spawn an object at a location")]
	public GameObject enableObject = null;
	public GameObject disableObject = null;
	[Header("Spawn an object at a location")]
	public GameObject spawnObject = null;
	public Transform spawnLocation = null;
	[Space(10)]
	[Header("Set an AreaManager variable")]
	public string gameManagerVarToSet = null;
	public string varType = "boolean";
	public string setVarTo = null;
	[Space(10)]
	[Header("Set/Call A Component/Function on the player")]
	public string tagOfHolder = null;
	public string nameOfHolder = null;
	public string Component = null;
	public string componentFunction = null;
	public bool componentEnabled = true;
	public EventParams[] parameters = null;
}

public class GameplayEvent : MonoBehaviour {
	public bool triggerForPlayerOnly = true;
	public bool triggerOnceOnly = true;
	private bool triggered = false;
	private enum eventType {Delayed, TriggerEnter, TriggerExit, IfVarSet, CallPerformEvent, ButtonInTrigger}
	private enum fieldType {Boolean, Int, Float}
	[SerializeField] private eventType TypeOfEvent = eventType.Delayed;
	[SerializeField] private float delay = 2.0f;
	[SerializeField] private string button = "Action";
	[SerializeField] private fieldType varType = fieldType.Boolean;
	[SerializeField] private string varname = "";
	[SerializeField] private bool varSetTo = false;
	[SerializeField] private bool performCheckOnUpdate = false;
	[Space(10)]
	[SerializeField] private EventAction[] actionsToTake;

	private AreaManager manager = null;
	private bool output;
	private bool performing = false;

	void Start() {
		if (TypeOfEvent == eventType.Delayed) {
			StartCoroutine (DelayStart ());
		}
		if (TypeOfEvent == eventType.IfVarSet) {
			manager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ();
		}
	}
	void Update() {
		if (TypeOfEvent == eventType.IfVarSet && performCheckOnUpdate==true) {
			switch (varType) {
			case fieldType.Boolean:
				output = ((bool)manager.GetType ().GetField (varname).GetValue (manager) == bool.Parse (varname));
				break;
			case fieldType.Float:
				output = ((float)manager.GetType ().GetField (varname).GetValue (manager) == float.Parse (varname));
				break;
			case fieldType.Int:
				output = ((int)manager.GetType ().GetField (varname).GetValue (manager) == int.Parse (varname));
				break;
			}
			if (output == true) {
				PerformEvent ();
			}
		}
	}
	void FixedUpdate() {
		if (TypeOfEvent == eventType.IfVarSet && performCheckOnUpdate==false) {
			switch (varType) {
				case fieldType.Boolean:
					output = ((bool)manager.GetType ().GetField (varname).GetValue (manager) == bool.Parse (varname));
					break;
				case fieldType.Float:
					output = ((float)manager.GetType ().GetField (varname).GetValue (manager) == float.Parse (varname));
					break;
				case fieldType.Int:
					output = ((int)manager.GetType ().GetField (varname).GetValue (manager) == int.Parse (varname));
					break;
			}
			if (output == varSetTo) {
				PerformEvent ();
			}
		}
	}

	void OnTriggerEnter(Collider col) {
		if (TypeOfEvent == eventType.TriggerEnter) {
			if (triggerForPlayerOnly == true && col.transform.tag == "Player")
				PerformEvent ();
			else if (triggerForPlayerOnly == false)
				PerformEvent ();
		} else if (TypeOfEvent == eventType.ButtonInTrigger) {
			if (col.transform.tag == "Player" && InputManager.GetButtonDown(button)) {
				PerformEvent ();
			}
		}
	}

	void OnTriggerStay(Collider col) {
		if (TypeOfEvent == eventType.ButtonInTrigger) {
			if (col.transform.tag == "Player" && InputManager.GetButtonDown(button)) {
				PerformEvent ();
			}
		}
	}

	void OnTriggerExit(Collider col) {
		if (TypeOfEvent == eventType.TriggerExit) {
			if (triggerForPlayerOnly == true && col.transform.tag == "Player")
				PerformEvent ();
			else if (triggerForPlayerOnly == false)
				PerformEvent ();
		}
	}
	private IEnumerator DelayStart() {
		yield return new WaitForSeconds (delay);
		PerformEvent ();
	}

	public void PerformEvent() {
		if (triggered == true && triggerOnceOnly == true) {
			return;
		}
		if (performing == false) {
			triggered = true;
			performing = true;
			foreach (EventAction action in actionsToTake) {
				StartCoroutine(PerformEventAction (action));
			}
		} else {
			return;
		}
	}

	IEnumerator PerformEventAction(EventAction action) {
		yield return new WaitForSeconds (action.delay);
		if (action.enableObject) {
			action.enableObject.SetActive (true);
		}
		if (action.disableObject) {
			action.disableObject.SetActive (false);
		}
		if (action.spawnObject) {
			Instantiate (action.spawnObject, action.spawnLocation.position, action.spawnLocation.rotation);
		}
		if (string.IsNullOrEmpty(action.Component) == false) {
			GameObject holder;
			holder = (string.IsNullOrEmpty(action.tagOfHolder) == false) ? GameObject.FindGameObjectWithTag (action.tagOfHolder) : GameObject.Find(action.nameOfHolder);

			if (holder.GetComponent (action.Component) != null) {
				Type mytype = holder.GetComponent (action.Component).GetType ();
				if (mytype.GetProperty("enabled") != null)
				{
					System.Reflection.PropertyInfo rpi = mytype.GetProperty("enabled");
					rpi.SetValue(holder.GetComponent (action.Component), action.componentEnabled, null);
				}

				Dictionary<int, object> values = new Dictionary<int, object> ();
				for (int i = 0; i < action.parameters.Length; i++) {
					switch (action.parameters [i].ParameterType) {
					case ITypes.ParameterType.Boolean:
						values.Add (i, bool.Parse (action.parameters [i].param));
						break;
					case ITypes.ParameterType.Float:
						values.Add (i, float.Parse (action.parameters [i].param));
						break;
					case ITypes.ParameterType.Int:
						values.Add (i, int.Parse (action.parameters [i].param));
						break;
					case ITypes.ParameterType.String:
						values.Add (i, action.parameters [i].param);
						break;
					}
				}
				var paramArray = values.Values.ToArray ();
				if (string.IsNullOrEmpty(action.componentFunction)==false && paramArray.Length >= 0) {
					holder.GetComponent (action.Component).GetType ().GetMethod (action.componentFunction).
					Invoke (holder.GetComponent (action.Component), paramArray); 
				} 
				else if (string.IsNullOrEmpty(action.componentFunction)==false && paramArray.Length <= 0) {
					holder.GetComponent (action.Component).GetType ().GetMethod (action.componentFunction).
					Invoke (holder.GetComponent (action.Component), null); 
				}
			} 
		}
		if (string.IsNullOrEmpty(action.gameManagerVarToSet) == false) {
			if(action.varType.ToLower() == "boolean" || action.varType.ToLower() == "bool")
				GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ().GetType ().GetField (action.gameManagerVarToSet).SetValue (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> (),bool.Parse(action.setVarTo));
			else if(action.varType.ToLower() == "int" || action.varType.ToLower() == "integer")
				GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ().GetType ().GetField (action.gameManagerVarToSet).SetValue (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> (),int.Parse(action.setVarTo));
			else if(action.varType.ToLower() == "float")
				GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ().GetType ().GetField (action.gameManagerVarToSet).SetValue (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> (),float.Parse(action.setVarTo));
		}
	}
}
