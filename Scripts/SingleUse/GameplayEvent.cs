using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;					//convert dictionary to array
using System;						//for dynamic typing (enable/disable components)
using TeamUtility.IO;				//Custom Input Manager
using UnityEngine.Events;

namespace CyberBullet.Events {
    public enum ParameterType {Boolean, Float, Int, String, Vector3, AudioClip, GameObject};
    #region Classes
    [System.Serializable]
    class EventParams {
    	public ParameterType ParameterType = ParameterType.Boolean;
    	public string param = "";
        public Vector3 paramVector3 = Vector3.zero;
        public Transform paramConvertToVector3 = null;
        public AudioClip clip = null;
        public GameObject gameObject = null;
    }
    [System.Serializable]
    class EventAction {
        [Tooltip("Arbitrary name of this node. Does nothing other than helps you remember what this node does.")]
    	public string name = "";					//not this is only for referencing in the inspector
        [Tooltip("How long to wait before calling this node.")]
    	public float delay = 0.0f;
    	[Header("Spawn an object at a location")]
        [Tooltip("Gameobject to enable when this node is called.")]
    	public GameObject enableObject = null;
        [Tooltip("GameObject to disable when this node is called.")]
    	public GameObject disableObject = null;
        [Tooltip("Gameobject to destroy when this node is called.")]
        public GameObject destroyObject = null;
        [Tooltip("Find all Gameobjects with name and destroys them")]
        public string destroyWithName = "";
        [Tooltip("Find all Gameobjects with tag and destroys them")]
        public string destroyWithTag = "";
        [Tooltip("Destroy this gameobject when this node is called.")]
        public bool destroyThis = false;
    	[Header("Spawn an object at a location")]
    	public GameObject spawnObject = null;
    	public Transform spawnLocation = null;
    	[Space(10)]
    	[Header("Set an AreaManager variable")]
        [Tooltip("Variable name to set in the AreaManager script of the GameManager GameObject.")]
    	public string gameManagerVarToSet = null;
        [Tooltip("What to set the variable to. EX: true/false")]
    	public string setVarTo = null;
    	[Space(10)]
    	[Header("Set/Call A Component/Function on the player")]
        [Tooltip("This object has ths script I am targeting.")]
        public bool useThisObject = false;
        [Tooltip("Tag of the owner of the script you are going to call. (Only supply tag or name - not both.)")]
    	public string tagOfHolder = null;
        [Tooltip("Gameobject name of the onwer of the script you are going to call.")]
    	public string nameOfHolder = null;
        [Tooltip("Name of the script to call.")]
    	public string Component = null;
        [Tooltip("Function in the script to call.")]
    	public string componentFunction = null;
        [Tooltip("Turn off or on the script. If calling this needs to be enabled.")]
    	public bool componentEnabled = true;
        [Tooltip("Parameters to pass to the target function.")]
    	public EventParams[] parameters = null;
        [Tooltip("Classic unity event script that is called last after every other event in this node is called.")]
        public UnityEvent other;
    }
    #endregion
    public class GameplayEvent : MonoBehaviour {
        #region Variables
    	public bool triggerForPlayerOnly = false;
    	public bool triggerOnceOnly = true;
        public bool triggerOnceOnSuccess = false;
        public string triggerForTagOnly = "";
        public string triggerForNameOnly = "";
        private bool done = false;
    	private bool triggered = false;
        private enum eventType {Delayed, TriggerEnter, TriggerExit, IfVarSet, CallPerformEvent, ButtonInTrigger, OnEnable, IfAllVarsSet}
    	private enum fieldType {Boolean, Int, Float}
        private enum refreshType {Update,FixedUpdate}
    	[SerializeField] private eventType TypeOfEvent = eventType.Delayed;
        [SerializeField] private refreshType refresh_type = refreshType.FixedUpdate;
    	[SerializeField] private float delay = 2.0f;
    	[SerializeField] private string button = "Action";
    	[SerializeField] private fieldType varType = fieldType.Boolean;
    	[SerializeField] private string varname = "";
        [Tooltip("If you want to check more than just 1 variable at a time, varname left for compatability reasons")]
        [SerializeField] private string[] additionalVars = null;
    	[SerializeField] private string varSetTo = "";
        [SerializeField] private bool onlyAllowIfVarSet = false;
        private bool varIsSet = false;
        private bool dontCheckForVar = false;
    	[SerializeField] private bool performCheckOnUpdate = false;
    	[Space(10)]
    	[SerializeField] private EventAction[] actionsToTake;
        [SerializeField] private bool debugEventCall = false;
        [SerializeField] private bool debugVarCheck = false;
        [SerializeField] private bool callEvent = false;
        [SerializeField] private bool debugParams = false;

        private int completed_actual = 0;
        private int completed_total = 0;
    	private AreaManager manager = null;
    	private bool output;
    	private bool performing = false;
        #endregion
    	
        void OnEnable()
        {
            if (TypeOfEvent == eventType.OnEnable)
            {
                CheckAllVars();
                PerformEvent();
            }
        }

        void Start() {
            manager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ();
            if (TypeOfEvent == eventType.Delayed) {
    			StartCoroutine (DelayStart ());
    		}
            if (String.IsNullOrEmpty(varname) == true && additionalVars.Length < 1)
            {
                dontCheckForVar = true;
            }
            completed_total = actionsToTake.Length;
    	}

        private IEnumerator DelayStart() {
            yield return new WaitForSeconds (delay);
            if (string.IsNullOrEmpty(varname))
            {
                PerformEvent();
            }
            else 
            {
                CheckAllVars();
                PerformEvent();
            }
        }

        #region Heartbeat
        void Update() {
            if (dontCheckForVar == false && performCheckOnUpdate==true && (
                TypeOfEvent == eventType.IfAllVarsSet || TypeOfEvent == eventType.IfVarSet
            )) {
                CheckAllVars();
    		}
            if (refresh_type == refreshType.Update)
            {
                if (performing == true && triggerOnceOnly == false && completed_total == completed_actual)
                {
                    completed_actual = 0;
                    performing = false;
                }
            }
            if (callEvent == true)
            {
                callEvent = false;
                CheckAllVars();
                PerformEvent();
            }
    	}
    	void FixedUpdate() {
            if (dontCheckForVar == false && performCheckOnUpdate==false && (
                TypeOfEvent == eventType.IfAllVarsSet || TypeOfEvent == eventType.IfVarSet
            )) {
                CheckAllVars();
    		}
            if (refresh_type == refreshType.FixedUpdate)
            {
                if (performing == true && triggerOnceOnly == false && completed_total == completed_actual)
                {
                    completed_actual = 0;
                    performing = false;
                }
            }
    	}
        #endregion

        #region VarCheck
        void CheckAllVars() {
            if (TypeOfEvent == eventType.IfAllVarsSet)
            {
                if (string.IsNullOrEmpty(varname) == false)
                {
                    if (CheckForVarSet(varname) == false)
                    {
                        return;
                    }
                }
                foreach (string varitem in additionalVars)
                {
                    if (CheckForVarSet(varitem) == false)
                    {
                        return;
                    }
                }
                CheckVarPerform();
            }
            else if (TypeOfEvent == eventType.IfVarSet || onlyAllowIfVarSet == true)
            {
                if (string.IsNullOrEmpty(varname) == false)
                {
                    if (CheckForVarSet(varname) == true)
                    {
                        CheckVarPerform();
                    }
                }
                if (additionalVars.Length > 0)
                {
                    foreach (string varitem in additionalVars)
                    {
                        if (CheckForVarSet(varitem) == true)
                        {
                            CheckVarPerform();
                        }
                    }
                }
            }
        }
        bool CheckForVarSet(string varinput) {
            if (debugVarCheck == true)
                Debug.Log("Var Check Status: "+varIsSet);
            bool retVal = false;
            switch (varType) {
                case fieldType.Boolean:
                    if ((bool)manager.GetTargetValue(varinput) == bool.Parse(varSetTo)) {
                        retVal = true;
                    }
                    break;
                case fieldType.Float:
                    if ((float)manager.GetTargetValue(varinput) == float.Parse(varSetTo)) {
                        retVal = true;
                    }
                    break;
                case fieldType.Int:
                    if ((int)manager.GetTargetValue(varinput) == int.Parse(varSetTo))
                    {
                        retVal = true;
                    }
                    break;
            }
            return retVal;
        }
        void CheckVarPerform()
        {
            if (debugVarCheck == true)
                Debug.Log("Passed Var check");
            varIsSet = true;
            if (TypeOfEvent == eventType.IfVarSet || TypeOfEvent == eventType.IfAllVarsSet)
            {
                PerformEvent();
                if (triggerOnceOnly == true)
                {
                    done = true;
                }
            }
        }
        #endregion

        #region Triggers
    	void OnTriggerEnter(Collider col) {
    		if (TypeOfEvent == eventType.TriggerEnter) {
                if (triggerForPlayerOnly == true && col.transform.root.tag == "Player" && 
                    string.IsNullOrEmpty(triggerForTagOnly) && string.IsNullOrEmpty(triggerForNameOnly))
                {
                    CheckAllVars();
                    PerformEvent();
                }
                else if (!string.IsNullOrEmpty(triggerForTagOnly) && col.transform.root.tag == triggerForTagOnly)
                {
                    CheckAllVars();
                    PerformEvent();
                }
                else if (!string.IsNullOrEmpty(triggerForNameOnly) && col.transform.root.name == triggerForNameOnly)
                {
                    CheckAllVars();
                    PerformEvent();
                }
                else if (!string.IsNullOrEmpty(triggerForNameOnly) && 
                    !string.IsNullOrEmpty(triggerForTagOnly) && 
                    triggerForPlayerOnly == false)
                {
                    CheckAllVars();
                    PerformEvent();
                }
    		} else if (TypeOfEvent == eventType.ButtonInTrigger) {
    			if (col.transform.tag == "Player" && InputManager.GetButtonDown(button)) {
                    CheckAllVars();
    				PerformEvent ();
    			}
    		}
    	}

    	void OnTriggerStay(Collider col) {
    		if (TypeOfEvent == eventType.ButtonInTrigger) {
    			if (col.transform.tag == "Player" && InputManager.GetButtonDown(button)) {
                    CheckAllVars();
    				PerformEvent ();
    			}
    		}
    	}

        void OnTriggerExit(Collider col) {
    		if (TypeOfEvent == eventType.TriggerExit) {
                if (triggerForPlayerOnly == true && col.transform.root.tag == "Player" &&
                    string.IsNullOrEmpty(triggerForTagOnly) && string.IsNullOrEmpty(triggerForNameOnly))
                {
                    CheckAllVars();
                    PerformEvent();
                }
                else if (!string.IsNullOrEmpty(triggerForTagOnly) && col.transform.root.tag == triggerForTagOnly)
                {
                    CheckAllVars();
                    PerformEvent();
                }
                else if (!string.IsNullOrEmpty(triggerForNameOnly) && col.transform.root.name == triggerForNameOnly)
                {
                    CheckAllVars();
                    PerformEvent();
                }
                else if (!string.IsNullOrEmpty(triggerForNameOnly) && 
                    !string.IsNullOrEmpty(triggerForTagOnly) && 
                    triggerForPlayerOnly == false)
                {
                    CheckAllVars();
                    PerformEvent();
                }
    		}
    	}
        #endregion

        #region Event Logic
    	public void PerformEvent() {
            if (done == true)
                return;
            if (debugEventCall == true)
                Debug.Log("Event Called");
            if (onlyAllowIfVarSet == true && varIsSet == false)
            {
                if (debugEventCall == true)
                {
                    Debug.Log("Event Failed: All Vars Not Set");
                }
                return;
            }
    		if (triggered == true && triggerOnceOnly == true) {
                if (debugEventCall == true)
                {
                    Debug.Log("Event Failed: Only allowed to trigger once.");
                }
    			return;
    		}
    		if (performing == false) 
            {
    			triggered = true;
    			performing = true;
    			foreach (EventAction action in actionsToTake) 
                {
    				StartCoroutine(PerformEventAction (action));
    			}
    		} 
            else 
            {
                if (debugEventCall == true)
                {
                    Debug.Log("Event Failed: Event already running.");
                }
    			return;
    		}
            if (debugEventCall == true)
            {
                Debug.Log("Event Success!");
            }
    	}
        IEnumerator PerformEventAction(EventAction action) {
            if (done == true)
                yield return null;
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
            if (string.IsNullOrEmpty(action.destroyWithName) == false)
            {
                foreach (GameObject item in FindObjectsOfType(typeof(GameObject)) as GameObject[])
                {
                    if (item.name == action.destroyWithName)
                    {
                        Destroy(item);
                    }
                }
            }
            if (string.IsNullOrEmpty(action.destroyWithTag) == false)
            {
                foreach (GameObject item in GameObject.FindGameObjectsWithTag(action.destroyWithTag))
                {
                    Destroy(item);
                }
            }
            if (action.destroyThis == true)
            {
                DestroyImmediate(this.gameObject);
            }
    		if (action.spawnObject) {
    			Instantiate (action.spawnObject, action.spawnLocation.position, action.spawnLocation.rotation);
    		}
    		if (string.IsNullOrEmpty(action.Component) == false) {
    			GameObject holder;
                if (action.useThisObject == true)
                {
                    holder = this.gameObject;
                }
                else 
                {
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
        					case ParameterType.Boolean:
                                values.Add (i, bool.Parse (action.parameters [i].param));
        						break;
                            case ParameterType.Float:
                                values.Add(i, float.Parse(action.parameters[i].param));
        						break;
        					case ParameterType.Int:
                                values.Add (i, int.Parse (action.parameters [i].param));
        						break;
        					case ParameterType.String:
                                values.Add (i, action.parameters [i].param);
        						break;
                            case ParameterType.Vector3:
                                    if (action.parameters[i].paramConvertToVector3 != null)
                                {
                                    values.Add(i, action.parameters[i].paramConvertToVector3.position);
                                }
                                else
                                {
                                    values.Add(i, action.parameters[i].paramVector3);
                                }
                                break;
                            case ParameterType.AudioClip:
                                values.Add(i, (AudioClip)action.parameters[i].clip);
                                break;
                            case ParameterType.GameObject:
                                values.Add(i, (GameObject)action.parameters[i].gameObject);
                                break;
    					}
    				}
    				var paramArray = values.Values.ToArray ();
                    if (debugParams == true)
                    {
                        Debug.Log("Calling "+holder+"'s Component: "+action.Component+" with the following params");
                        for (int i = 0; i < paramArray.Length; i++)
                        {
                            Debug.Log(paramArray[i]+" as type "+paramArray[i].GetType());
                        }
                    }
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
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<AreaManager>().SetValue(action.gameManagerVarToSet, bool.Parse(action.setVarTo));
    		}
            action.other.Invoke();
            completed_actual += 1;
            if (triggerOnceOnSuccess == true && completed_actual == completed_total)
            {
                done = true;
            }
        }
        #endregion
    }
}