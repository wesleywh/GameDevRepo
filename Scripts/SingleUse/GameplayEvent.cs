using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;					//convert dictionary to array
using System;						//for dynamic typing (enable/disable components)
using TeamUtility.IO;				//Custom Input Manager
using UnityEngine.Events;

#region Classes
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
    public GameObject destroyObject = null;
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
    public UnityEvent other;
}
#endregion
public class GameplayEvent : MonoBehaviour {
    #region Variables
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
	[SerializeField] private string varSetTo = "";
    [SerializeField] private bool onlyAllowIfVarSet = false;
    private bool varIsSet = false;
    private bool dontCheckForVar = false;
	[SerializeField] private bool performCheckOnUpdate = false;
	[Space(10)]
	[SerializeField] private EventAction[] actionsToTake;
    [SerializeField] private bool debugEventCall = false;
    [SerializeField] private bool debugVarCheck = false;

	private AreaManager manager = null;
	private bool output;
	private bool performing = false;
    #endregion
	
    void Start() {
		if (TypeOfEvent == eventType.Delayed) {
			StartCoroutine (DelayStart ());
		}
		manager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ();
        if (String.IsNullOrEmpty(varname) == true)
        {
            dontCheckForVar = true;
        }
	}

    private IEnumerator DelayStart() {
        yield return new WaitForSeconds (delay);
        PerformEvent ();
    }

    #region VarCheck
    void Update() {
        if (dontCheckForVar == false && performCheckOnUpdate==true) {
            CheckForVarSet();
		}
	}
	void FixedUpdate() {
        if (dontCheckForVar == false && performCheckOnUpdate==false) {
            CheckForVarSet();
		}
	}
    void CheckForVarSet() {
        if (debugVarCheck == true)
            Debug.Log("Var Check Status: "+varIsSet);
        switch (varType) {
            case fieldType.Boolean:
                if ((bool)manager.GetTargetValue(varname) == bool.Parse(varSetTo)) {
                    varIsSet = true;
                    if (TypeOfEvent == eventType.IfVarSet)
                        PerformEvent ();
                }
                else
                {
                    varIsSet = false;
                }
                break;
            case fieldType.Float:
                if ((float)manager.GetTargetValue(varname) == float.Parse(varSetTo)) {
                    varIsSet = true;
                    if (TypeOfEvent == eventType.IfVarSet)
                        PerformEvent ();
                }
                else
                {
                    varIsSet = false;
                }
                break;
            case fieldType.Int:
                if ((int)manager.GetTargetValue(varname) == int.Parse(varSetTo))
                {
                    varIsSet = true;
                    if (TypeOfEvent == eventType.IfVarSet)
                        PerformEvent();
                }
                else
                {
                    varIsSet = false;
                }
                break;
        }

    }
    #endregion

    #region Triggers
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
    #endregion

    #region Event Logic
	public void PerformEvent() {
        if (debugEventCall == true)
            Debug.Log("Event Called");
        if (onlyAllowIfVarSet == true && varIsSet == false)
            return;
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
		if (action.disableObject != null) {
			action.disableObject.SetActive (false);
		}
        if (action.destroyObject != null)
        {
            DestroyImmediate(action.destroyObject);
        }
		if (action.spawnObject) {
			Instantiate (action.spawnObject, action.spawnLocation.position, action.spawnLocation.rotation);
		}
		if (string.IsNullOrEmpty(action.Component) == false) {
			GameObject holder;
            if (action.nameOfHolder == "this" || action.tagOfHolder == "this") {
                holder = this.gameObject;
            }
            else {
			    holder = (string.IsNullOrEmpty(action.tagOfHolder) == false) ? GameObject.FindGameObjectWithTag (action.tagOfHolder) : GameObject.Find(action.nameOfHolder);
            }
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
        action.other.Invoke();
    }
    #endregion
}
