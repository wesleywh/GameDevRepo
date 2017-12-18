using UnityEngine;
using System;
using System.Collections;
using TeamUtility.IO;			//Custome InputManager Manager

namespace Pandora.Interactables {
    [Serializable]
    class OnAction {
    	[Header("Execute Script")]
    	public GameObject holderObject = null;
    	public string holderTag = null;
    	public string holderName = null;
    	public string scriptName = null;
    	public string functionName = null;
    	public string variable = null;
    }
    [Serializable]
    class OffAction {
    	[Header("Execute Script")]
    	public GameObject holderObject = null;
    	public string holderTag = null;
    	public string holderName = null;
    	public string scriptName = null;
    	public string functionName = null;
    	public string variable = null;
    }
    [Serializable]
    class ImageProperties {
    	public Texture2D floatingImage = null;
    	public Vector2 overrideImageSize = Vector2.zero;
    	public Vector2 textureOffset = Vector2.zero;
    }
    [Serializable]
    class SoundProperties {
    	public AudioClip onSound = null;
    	public AudioClip offSound = null;
    }
    [RequireComponent(typeof(AudioSource))]
    public class Button : MonoBehaviour {
    	[Header("Actions to take when the button is pressed")]
    	[SerializeField] private OnAction[] turnOnActions = null;
    	[SerializeField] private OffAction[] turnOffActions = null;
    	[SerializeField] private SoundProperties soundProperties = null;
    	[SerializeField] private ImageProperties floatingImageSettings = null;
    	enum SwitchActions { TurnOnOnly,TurnOffOnly,TurnOnAndOff }
    	[SerializeField] private SwitchActions switchType = SwitchActions.TurnOnAndOff;
    	[SerializeField] private float distanceToPress = 3.0f;
    	[SerializeField] private bool state = false;

    	private bool pressing = false;
    	private bool drawImage = false;
    	private Vector3 objPos = Vector3.zero;

    	void Update() {
    		if (Vector3.Distance (GameObject.FindGameObjectWithTag ("Player").transform.position, this.transform.position) <= distanceToPress) {
    			objPos = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>().WorldToScreenPoint (this.transform.position);
    			drawImage = true;
    			if (InputManager.GetButtonDown ("Action") && pressing == false) {
    				pressing = true;
    				PressButton ();
    			}
    		} else {
    			drawImage = false;
    		}
    	}
    	void PressButton() {
    		switch (switchType) {
    			case SwitchActions.TurnOffOnly:
    				state = false;
    				break;
    			case SwitchActions.TurnOnOnly:
    				state = true;
    				break;
    			case SwitchActions.TurnOnAndOff:
    				state = !state;
    				break;
    		}
    		if (state == false) {
    			if (soundProperties.offSound != null) {
    				this.GetComponent<AudioSource> ().clip = soundProperties.offSound;
    				this.GetComponent<AudioSource> ().Play ();
    			}
    			foreach (OnAction action in turnOnActions) {
    				GameObject target = null;
    				if (action.holderObject != null) {
    					target = action.holderObject;
    				} else if (string.IsNullOrEmpty (action.holderTag) == false) {
    					target = GameObject.FindGameObjectWithTag (action.holderTag);
    				} else {
    					target = GameObject.Find (action.holderName);
    				}
    				target.GetComponent (action.scriptName).SendMessage (action.functionName, action.variable);
    			}
    		} else if (state == true) {
    			if (soundProperties.onSound != null) {
    				this.GetComponent<AudioSource> ().clip = soundProperties.onSound;
    				this.GetComponent<AudioSource> ().Play ();
    			}
    			foreach (OffAction action in turnOffActions) {
    				GameObject target = null;
    				if (action.holderObject != null) {
    					target = action.holderObject;
    				} else if (string.IsNullOrEmpty (action.holderTag) == false) {
    					target = GameObject.FindGameObjectWithTag (action.holderTag);
    				} else {
    					target = GameObject.Find (action.holderName);
    				}
    				target.GetComponent (action.scriptName).SendMessage (action.functionName, action.variable);
    			}
    		}
    		pressing = false;
    	}
    	void OnDrawGizmos() {
    		Gizmos.color = Color.green;
    		Gizmos.DrawWireSphere (this.transform.position, distanceToPress);
    	}
    	void OnGUI(){
    		if (drawImage) {
    			if (floatingImageSettings.overrideImageSize.x > 0 || floatingImageSettings.overrideImageSize.y > 0) {
    				GUI.DrawTexture (new Rect (objPos.x + floatingImageSettings.textureOffset.x, (Screen.height - objPos.y) + floatingImageSettings.textureOffset.y, floatingImageSettings.overrideImageSize.x, floatingImageSettings.overrideImageSize.y), floatingImageSettings.floatingImage);
    			} else {
    				GUI.DrawTexture (new Rect (objPos.x + floatingImageSettings.textureOffset.x, (Screen.height - objPos.y) + floatingImageSettings.textureOffset.y, 20, 20), floatingImageSettings.floatingImage);
    			}
    		}
    	}
    }
}