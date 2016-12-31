using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

[Serializable]
class CutsceneExecuteScript {
	[Header("Execute A script at this point")]
	public float delayToExecute = 0.0f;
	public bool enableObject = false;
	public GameObject holderGameObject = null;
	public string holderTag = null;
	public string holderName = null;
	public string scriptName = null;
	public string functionName = null;
	public string passingVariables = null;
}
[Serializable]
class cutSceneAudioOption {
	public AudioClip soundClip = null;
	public bool replaceVolume = false;
	[Range(0, 1)]
	public float overrideVolume = 0.5f;
	public float delayBeforePlay = 0.0f;
}
[Serializable]
class cutSceneSoundProperties {
	public AudioSource overrideAudioSource = null;
	public cutSceneAudioOption[] audio = null;
}
[Serializable]
class CutSceneMovePoint {
	[Header("Only one. Select target to move to.")]
	[Header("Note: Enabling only works with GameObjects")]
	public GameObject pointGameObject = null;
	public string pointTag = null;
	public string pointName = null;
	public float speedToPoint = 0.5f;
}
[Serializable]
class CutSceneFadeProperties {
	[Header("GUI Fade properties while moving to this point.")]
	public Texture2D overrideFadeTexture = null;
	public bool startFadeIn = false;
	public bool startFadeOut = false;
	public float fadeSpeed = 0.5f;
}
[Serializable]
class CutSceneDelayProperties {
	[Header("How long to wait at this point")]
	public float waitAtPoint = 0.0f;
}
[Serializable]
class CutSceneLookProperties {
	[Header("Use this to make the player/camera look at a target")]
	public GameObject lookGameObject = null;
	public string lookTag = null;
	public string lookName = null;
	public bool snapToTarget = false;
	public float transitionToTargetTime = 0.5f;
	public float angleOffset = 0.5f;
}
[Serializable]
class CutscenePoint {
	public CutSceneMovePoint moveToPoint = null;
	public CutSceneFadeProperties fadingProperties = null;
	public CutSceneDelayProperties delayProperties = null;
	public CutSceneLookProperties lookingAtProperties = null;
	public cutSceneSoundProperties soundProperties = null;
	public CutsceneExecuteScript[] executeScripts = null;
}
[Serializable]
class controllingObject {
	[Header("Note: You may only choose one")]
	public bool enableObjectOnStart = false;
	public GameObject controllingThisObject = null;
	public string controllingThisObjectTag = null;
	public string controllingThisObjectName = null;
}
[Serializable]
class StartCutscene {
	[Header("Start Cutscene Based On Time")]
	public float delayStart = 0.0f;
	[Header("Start Cutscene Based On Variable")]
	public bool startBasedOnVariable = false;
	public string gameObjectTag = null;
	public string gameObjectName = null;
	[Space(10)]
	public string scriptName = null;
	public string scriptVariable = null;
	public string scriptVariableState = null;
	public enum varType { Bool, String, Float, Integer};
	public varType variableType = varType.Bool;
	[Space(10)]
	public Texture2D defaultFadeTexture = null;
	[Header("Enable/Disable Objects")]
	public GameObject[] enableList = null;
	public GameObject[] disableList = null;
	public string[] disableTagList = null;
}
[Serializable]
class SpawnOptions {
	[Header("Spawn Options")]
	public GameObject spawnAtGameObject = null;
	public string spawnAtTag = null;
	public string spawnAtName = null;
	public GameObject spawnObject = null;
}
[Serializable]
class EndCutsceneProperties {
	[Header("These happen when the cutscene actions have all finished.")]
	public float delayEnd = 0.0f;
	[Header("Execute Target Script")]
	public string gameObjectTag = null;
	public string gameObjectName = null;
	[Space(10)]
	[Tooltip("If component isn't supplied it will just try to find the function name everywhere on target object")]
	public string component = null;
	public string functionName = null;
	public string functionInput = null;
	public float waitToExecute = 0.0f;
	[Space(10)]
	[Header("Other end properties")]
	public bool turnOffScript = true;
	public bool destroyThisObject = false;
	[Space(10)]
	[Header("Player & Object Spawn/Enable Properties")]
	public SpawnOptions[] spawnOthers = null;
	public GameObject[] enableOthers = null;
	public GameObject[] disableOthers = null;
}
[Serializable]
class DebugProperties {
	public bool setDebugIndex = false;
	public bool setDebugMoveTarget = false;
	public bool setDebugLookTarget = false;
	public bool setDebugTotalCompletionRate = false;
	public bool setDebugStatesCompleted = false;
	public bool setDebugFadeing = false;
	public bool setDebugMoveDistance = false;
}
public class CutsceneManager : MonoBehaviour {
	//for user to fill out
	[SerializeField] private StartCutscene startCutsceneConditions;
	[SerializeField] private EndCutsceneProperties endCutsceneConditions;
	[SerializeField] private controllingObject objectToControl;
	[SerializeField] private CutscenePoint[] AllCutscenePoints;
	[SerializeField] private DebugProperties debugProp;

	//For individual points
	private GameObject moveTarget;				//Current Target To move to
	private GameObject lookTarget;				//Current taget to look at
	private bool currentlyDelaying = false;		//If currently waiting
	private bool doneDelaying = false;			//if started delaying but not finished yet
	private GameObject control = null;			//What object is being affected by all these actions
	private bool moving = true;					//If currently moving
	private int index = 0;						//What point in the list we're at
	private float guiAlpha = 0.0f;				//alpha setting for fading in/out GUIs
	private Texture2D fadeTexture;				//currently selected fade texture;
	private bool performFadeAction = true;		//should fading be tried?
	private bool startDelaying = false;			//if there is a set start delay
	private bool beginCutscene = false;			//should I start the cutscene?
	private int completionRate = 0;				//how many of the states have successfully passed
	private bool endStarted = false;			//have we initiated the end properites?
	private bool scriptComplete = false;		//tells the cutscene it is now allow to destroy/disable itself
	private bool playingSounds = false;			//Check to make sure we don't keep trying to play a playing set of sounds
	private bool soundsComplete = false;		//Check to see if all the sounds have been played
	private bool executingScripts = false;		//Check if you have started to run desired point scripts

	void Start() {
		SetControlObject ();
	}

	// Update is called once per frame
	void Update () {
		if (beginCutscene == false) {
			CheckStartConditions ();
		} 
		else if(beginCutscene == true){
			completionRate = (MoveToPoint () == true) ? 1 : 0;
			completionRate += (LookAtTarget () == true) ? 1 : 0;
			completionRate += (PerformFading () == true) ? 1 : 0;
			completionRate += (PlaySounds () == true) ? 1 : 0;
			completionRate += (ExecuteScripts () == true) ? 1 : 0;
			if (debugProp.setDebugStatesCompleted == true) {
				if (MoveToPoint () == true) {
					Debug.Log ("Index: " + index + " MoveToPoint Complete");
				}
				if (LookAtTarget () == true) {
					Debug.Log ("Index: " + index + " LookTarget Complete");
				}
				if (PerformFading () == true) {
					Debug.Log ("Index: " + index + " PerformFading Complete");
				}
			}
			if (debugProp.setDebugTotalCompletionRate == true) {
				Debug.Log ("Index: " + index + " Total Completion Rate: " + completionRate);
			}
			if (debugProp.setDebugIndex == true) {
				Debug.Log ("Current Index = " + index);
			}
			if (completionRate == 5) {
				if (currentlyDelaying == false && doneDelaying == false) {
					currentlyDelaying = true;
					StartCoroutine (DelayAtPoint ());
				} else if (doneDelaying == true) {
					if (AllCutscenePoints.Length == index + 1 && endStarted == false) {
						endStarted = true;
						StartCoroutine(EndCutsceneActions ());
					} else {
						if (debugProp.setDebugIndex == true) {
							Debug.Log ("Changed Index from " + index + " to " + (index + 1));
						}
						index += 1;
						StartCutscenePoint ();
					}
				}
			}
		}
	}
// ---------- USAGE SECTION ----------- //
	bool ExecuteScripts() {
		if (executingScripts == false) {
			executingScripts = true;
			foreach (CutsceneExecuteScript script in AllCutscenePoints[index].executeScripts) {
				StartCoroutine (ExecuteSingleScript (script));
			}
		}
		return true;
	}
	IEnumerator ExecuteSingleScript(CutsceneExecuteScript script) {
		yield return new WaitForSeconds (script.delayToExecute);
		GameObject target = null;
		if (script.holderGameObject != null) {
			target = script.holderGameObject;
		}
		else if (string.IsNullOrEmpty (script.holderTag) == false) {
			target = GameObject.FindGameObjectWithTag (script.holderTag);
		} else {
			target = GameObject.Find (script.holderName);
		}
		if (target != null) {
			if (script.enableObject == true) {
				target.SetActive (true);
			}
			if (string.IsNullOrEmpty (script.scriptName) == false) {
				target.GetComponent (script.scriptName).SendMessage (script.functionName, script.passingVariables);
			}
		}
	}
	IEnumerator DelayAtPoint() {
		yield return new WaitForSeconds (AllCutscenePoints [index].delayProperties.waitAtPoint);
		doneDelaying = true;
		currentlyDelaying = false;
	}
	bool PlaySounds() {
		if (soundsComplete == true) {
			return true;
		} else if (playingSounds == true) {
			return false;
		} else {
			AudioSource audioSource = null;
			if (AllCutscenePoints [index].soundProperties.overrideAudioSource != null) {
				audioSource = AllCutscenePoints [index].soundProperties.overrideAudioSource;
			} else if (this.GetComponent<AudioSource> ()) {
				audioSource = this.GetComponent<AudioSource> ();
			} else {
				soundsComplete = true;
				return true;
			}
			foreach (cutSceneAudioOption audio in AllCutscenePoints [index].soundProperties.audio){
				StartCoroutine (PlaySoundAtPoint (audio, audioSource));
			}
			soundsComplete = true;
			return true;
		}
	}
	//return true if currently at target point
	bool MoveToPoint() {
		if (endStarted == true) {
			return false;
		}
		if (moving == true && moveTarget != null) {
			float step = AllCutscenePoints [index].moveToPoint.speedToPoint * Time.deltaTime;
			control.transform.position = Vector3.MoveTowards (control.transform.position, moveTarget.transform.position, step);
			if (debugProp.setDebugMoveDistance == true) {
				Debug.Log ("Index " + index + " current:"+control.transform.position+" target:"+moveTarget.transform.position);
			}
			if (control.transform.position == moveTarget.transform.position) {
				moving = false;
				if (debugProp.setDebugMoveDistance == true) {
					Debug.Log ("Reached Target");
				}
				return true;
			}
			return false;
		} 
		return true;
	}
	//return true if currently looking at target
	bool LookAtTarget() {
		if (endStarted == true) {
			return false;
		}
		if (lookTarget != null) {
			if (AllCutscenePoints [index].lookingAtProperties.snapToTarget == true) {
				control.transform.LookAt (lookTarget.transform);
				Quaternion neededRotation = Quaternion.LookRotation (lookTarget.transform.position - control.transform.position);
				if (debugProp.setDebugLookTarget == true) {
					Debug.Log ("Index " + index + " Current: " + control.transform.rotation + " target: " + neededRotation);
				}
				if (control.transform.rotation == neededRotation) {
					if (debugProp.setDebugLookTarget == true) {
						Debug.Log ("Index: " + index + " Reached Target Rotation!");
					}
					return true;
				}
			} else {
				Quaternion neededRotation = Quaternion.LookRotation (lookTarget.transform.position - control.transform.position);
				control.transform.rotation = Quaternion.Slerp (control.transform.rotation, neededRotation, Time.deltaTime * AllCutscenePoints [index].lookingAtProperties.transitionToTargetTime);
				if (debugProp.setDebugLookTarget == true) {
					Debug.Log ("Index " + index + " Offset:" + (control.transform.rotation.eulerAngles - neededRotation.eulerAngles) +
								" Allowed Offset Angle: "+AllCutscenePoints [index].lookingAtProperties.angleOffset);
				}
				if (Quaternion.Angle(control.transform.rotation, neededRotation) <= AllCutscenePoints [index].lookingAtProperties.angleOffset) {
					if (debugProp.setDebugLookTarget == true) {
						Debug.Log ("Index: " + index + " Reached Target Rotation!");
					}
					return true;
				}else {
					return false;
				}
			}
		}
		return false;
	}
	//return true = done fading
	bool PerformFading() {
		if (endStarted == true) {
			return false;
		}
		if (performFadeAction == false) {
			if (debugProp.setDebugFadeing == true) {
				Debug.Log ("Index: "+index+" - Don't perform fading, return true");
			}
			return true;
		}
		if (AllCutscenePoints [index].fadingProperties.startFadeIn) {
			guiAlpha -= (guiAlpha <= 0) ? 0 : Time.deltaTime * AllCutscenePoints [index].fadingProperties.fadeSpeed;
			if (debugProp.setDebugFadeing == true) {
				Debug.Log ("Index: "+index+" - Fading In - guiAplha: "+guiAlpha);
			}
			if (guiAlpha <= 0) {
				performFadeAction = false;
				if (debugProp.setDebugFadeing == true) {
					Debug.Log ("Index: "+index+" - Fading In - Complete, alpha = "+guiAlpha);
				}
				return true;
			}
		} else if (AllCutscenePoints [index].fadingProperties.startFadeOut) {
			guiAlpha += (guiAlpha < 1) ? 1 : Time.deltaTime * AllCutscenePoints [index].fadingProperties.fadeSpeed;
			if (debugProp.setDebugFadeing == true) {
				Debug.Log ("Index: "+index+" - Fading out - guiAplha: "+guiAlpha);
			}
			if (guiAlpha >= 1) {
				if (debugProp.setDebugFadeing == true) {
					Debug.Log ("Index: "+index+" - Fading out - Complete, alpha = "+guiAlpha);
				}
				performFadeAction = false;
				return true;
			}
		} else {
			if (debugProp.setDebugFadeing == true) {
				Debug.Log ("Index: "+index+" - Fade in/out not set");
			}
			return true;
		}
		return false;
	}
	//for fading
	void OnGUI() {
		if (performFadeAction == true && beginCutscene == true) {
			Color newColor = GUI.color;
			newColor.a = guiAlpha;
			GUI.color = newColor;
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeTexture);
		}
	}
	IEnumerator PlaySoundAtPoint(cutSceneAudioOption audio, AudioSource audioSource) {
		yield return new WaitForSeconds (audio.delayBeforePlay);
		audioSource.clip = audio.soundClip;
		if (audio.replaceVolume == true) {
			audioSource.volume = audio.overrideVolume;
		}
		audioSource.Play ();
	}
	IEnumerator EndCutsceneActions() {
		yield return new WaitForSeconds (endCutsceneConditions.delayEnd);
		//execute a script
		if (endCutsceneConditions.gameObjectTag != null || endCutsceneConditions.gameObjectName != null) {
			StartCoroutine(EndCutsceneExecuteScript ());
		}

		//player/npc props
		if (endCutsceneConditions.spawnOthers.Length > 0) {
			foreach (SpawnOptions newSpawn in endCutsceneConditions.spawnOthers) {
				GameObject spawnTarget = null;
				if (newSpawn.spawnAtGameObject != null) {
					spawnTarget = newSpawn.spawnAtGameObject;
				} else if (newSpawn.spawnAtTag != null) {
					spawnTarget = GameObject.FindGameObjectWithTag (newSpawn.spawnAtTag);
				} else {
					spawnTarget = GameObject.Find (newSpawn.spawnAtTag);
				}
				Instantiate (newSpawn.spawnObject, spawnTarget.transform.position, spawnTarget.transform.rotation);
			}
		}
		foreach (GameObject targetObject in endCutsceneConditions.enableOthers) {
			targetObject.SetActive (true);
		}
		foreach (GameObject targetObject in endCutsceneConditions.disableOthers) {
			targetObject.SetActive (false);
		}
		//end
		while (scriptComplete == false) {
			yield return null;
		}
		if (endCutsceneConditions.turnOffScript == true) {
			this.GetComponent<CutsceneManager> ().enabled = false;
		}
		if (endCutsceneConditions.destroyThisObject == true) {
			Destroy (this.gameObject);
		}
		SetUIState (true);
	}
	IEnumerator EndCutsceneExecuteScript() {
		GameObject targetObj = null;
		if (String.IsNullOrEmpty(endCutsceneConditions.gameObjectTag) == false) {
			targetObj = GameObject.FindGameObjectWithTag (endCutsceneConditions.gameObjectTag);
		} else if (String.IsNullOrEmpty(endCutsceneConditions.gameObjectName) == false) {
			targetObj = GameObject.Find (endCutsceneConditions.gameObjectName);
		}
		if (targetObj != null) {
			string componentName = endCutsceneConditions.component;
			string functionName = endCutsceneConditions.functionName;
			string functionInput = endCutsceneConditions.functionInput;
			yield return new WaitForSeconds (endCutsceneConditions.waitToExecute);
			if (String.IsNullOrEmpty(componentName) == true) {
				targetObj.SendMessage (functionName, functionInput);
			} else {
				targetObj.GetComponent(componentName).SendMessage(functionName, functionInput);
			}
		}
		scriptComplete = true;
	}
// --------- INITIALIZATION SECTION ------------//
	//Used to initialize new point values
	void StartCutscenePoint() {
		SetMoveTarget ();
		SetLookTarget ();
		SetFadeTexture ();
		SetFadeProperties ();
		
		doneDelaying = false;
		currentlyDelaying = false;
		moving = true;
		playingSounds = false;
		soundsComplete = false;
		executingScripts = false;
	}
	void SetFadeTexture() {
		if (AllCutscenePoints [index].fadingProperties.overrideFadeTexture == null) {
			fadeTexture = startCutsceneConditions.defaultFadeTexture;
		} else {
			fadeTexture = AllCutscenePoints [index].fadingProperties.overrideFadeTexture;
		}
	}
	//implemented only in StartCutscenePoint()
	void SetFadeProperties() {
		if (AllCutscenePoints [index].fadingProperties.startFadeIn == true) {
			guiAlpha = 1.0f;
			performFadeAction = true;
		} else if (AllCutscenePoints [index].fadingProperties.startFadeOut == true) {
			guiAlpha = 0.0f;
			performFadeAction = true;
		} else {
			performFadeAction = false;
		}
	}
	//implemented only in StartCutscenePoint()
	void SetLookTarget() {
		if (AllCutscenePoints [index].lookingAtProperties.lookGameObject != null) {
			lookTarget = AllCutscenePoints [index].lookingAtProperties.lookGameObject;
		} else if (string.IsNullOrEmpty(AllCutscenePoints [index].lookingAtProperties.lookTag) == false) {
			lookTarget = GameObject.FindGameObjectWithTag (AllCutscenePoints [index].lookingAtProperties.lookTag);
		} else if (string.IsNullOrEmpty(AllCutscenePoints [index].lookingAtProperties.lookName) == false) {
			lookTarget = GameObject.Find (AllCutscenePoints [index].lookingAtProperties.lookName);
		} else {
			lookTarget = null;
		}
	}
	//implemented only in StartCutscenePoint()
	void SetMoveTarget() {
		if (AllCutscenePoints [index].moveToPoint.pointGameObject != null) {
			moveTarget = AllCutscenePoints [index].moveToPoint.pointGameObject;
		} else if (string.IsNullOrEmpty(AllCutscenePoints [index].moveToPoint.pointTag) == false) {
			moveTarget = GameObject.FindGameObjectWithTag (AllCutscenePoints [index].moveToPoint.pointTag);
		} else if (string.IsNullOrEmpty(AllCutscenePoints [index].moveToPoint.pointName) == false) {
			moveTarget = GameObject.Find (AllCutscenePoints [index].moveToPoint.pointName);
		} else {
			moveTarget = null;
		}
		if (debugProp.setDebugMoveTarget == true) {
			Debug.Log ("Current Move Target: " + moveTarget);
		}
	}
	//used only once in Start()
	void SetControlObject() {
		if (objectToControl.controllingThisObject != null) {
			control = objectToControl.controllingThisObject;
			if (objectToControl.enableObjectOnStart == true) {
				control.SetActive (true);
			}
		} else {
			if (string.IsNullOrEmpty(objectToControl.controllingThisObjectTag) == false) {
				control = GameObject.FindGameObjectWithTag(objectToControl.controllingThisObjectTag);
			} else {
				control = GameObject.Find (objectToControl.controllingThisObjectName);
			}
		}
	}
	void CheckStartConditions() {
		if (startCutsceneConditions.startBasedOnVariable == true) {
			GameObject targetObject = null;
			if (startCutsceneConditions.gameObjectTag != null) {
				targetObject = GameObject.FindGameObjectWithTag (startCutsceneConditions.gameObjectTag);
			} else {
				targetObject = GameObject.Find (startCutsceneConditions.gameObjectName);
			}
			string scriptName = startCutsceneConditions.scriptName;
			string variable = startCutsceneConditions.scriptVariable;
			string state = startCutsceneConditions.scriptVariableState;
			switch (startCutsceneConditions.variableType) {
			case StartCutscene.varType.Bool:
				bool curState0 = (bool)targetObject.GetComponent (scriptName).GetType ().GetField (variable).GetValue (targetObject.GetComponent (scriptName));
					if (curState0 == bool.Parse (state)) {
						SetUIState (false);
						beginCutscene = true;
						StartCutscenePoint ();
					}
					break;
				case StartCutscene.varType.Float:
					float curState1 = (float)targetObject.GetComponent (scriptName).GetType ().GetField (variable).GetValue (targetObject.GetComponent (scriptName));
					if (curState1 == float.Parse (state)) {
						SetUIState (false);
						beginCutscene = true;
						StartCutscenePoint ();
					}
					break;
				case StartCutscene.varType.Integer:
					int curState2 = (int)targetObject.GetComponent (scriptName).GetType ().GetField (variable).GetValue (targetObject.GetComponent (scriptName));
					if (curState2 == int.Parse (state)) {
						SetUIState (false);
						beginCutscene = true;
						StartCutscenePoint ();
					}
					break;
				case StartCutscene.varType.String:
					string curState3 = (string)targetObject.GetComponent (scriptName).GetType ().GetField (variable).GetValue (targetObject.GetComponent (scriptName));
					if (curState3 == state) {
						SetUIState (false);
						beginCutscene = true;
						StartCutscenePoint ();
					}
					break;
			}
		} else {
			if (startCutsceneConditions.delayStart > 0) {
				startDelaying = true;
				if (startDelaying) {
					StartCoroutine (DelayCutsceneStart ());
				} 
			}
		}
		if (beginCutscene == true) {
			foreach (GameObject targetObject in startCutsceneConditions.enableList) {
				targetObject.SetActive (true);
			}
			foreach (GameObject targetObject in startCutsceneConditions.disableList) {
				targetObject.SetActive (false);
			}
			foreach (string target in startCutsceneConditions.disableTagList) {
				GameObject.FindGameObjectWithTag (target).SetActive (false);
			}
			SetUIState (false);
		}
	}
	IEnumerator DelayCutsceneStart() {
		yield return new WaitForSeconds (startCutsceneConditions.delayStart);
		SetUIState (false);
		beginCutscene = true;
		StartCutscenePoint ();
	}
	void SetUIState(bool state) {
		//enable/disable health bar
		Image[] allHealthImages = GameObject.FindGameObjectWithTag ("UIHealthbar").GetComponentsInChildren<Image> () as Image[];
		foreach (Image target in allHealthImages) {
			target.enabled = state;
		}
		Text[] allHealthTexts = GameObject.FindGameObjectWithTag ("UIHealthbar").GetComponentsInChildren<Text> () as Text[];
		foreach (Text target in allHealthTexts) {
			target.enabled = state;
		}
		RawImage[] allHealthRawImages = GameObject.FindGameObjectWithTag ("UIHealthbar").GetComponentsInChildren<RawImage> () as RawImage[];
		foreach (RawImage target in allHealthRawImages) {
			target.enabled = state;
		}

		//enable/disable Inventory bar
		Image[] allInvImages = GameObject.FindGameObjectWithTag ("UIInventory").GetComponentsInChildren<Image> () as Image[];
		foreach (Image target in allInvImages) {
			target.enabled = state;
		}
		Text[] allInvTexts = GameObject.FindGameObjectWithTag ("UIInventory").GetComponentsInChildren<Text> () as Text[];
		foreach (Text target in allInvTexts) {
			target.enabled = state;
		}
		RawImage[] allInvRawImages = GameObject.FindGameObjectWithTag ("UIInventory").GetComponentsInChildren<RawImage> () as RawImage[];
		foreach (RawImage target in allInvRawImages) {
			target.enabled = state;
		}
	}
}
