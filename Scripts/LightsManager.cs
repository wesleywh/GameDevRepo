using UnityEngine;
using System.Collections;

public class LightsManager : MonoBehaviour {
	GameObject manager = null;
	[SerializeField] private Light spotLight = null;
	[SerializeField] private Light pointLight = null;
	[SerializeField] private string workingVarName = "mansion_entrance_lights_working";
	[SerializeField] private string lightsOnVarName = "mansion_entrance_lights_on";
	private bool lastState;
	void Start() {
        manager = dontDestroy.currentGameManager;
        lastState = (bool)manager.GetComponent<AreaManager> ().GetTargetValue(lightsOnVarName);
	}
	// Use this for initialization
	void Update () {
        bool working = (bool)manager.GetComponent<AreaManager> ().GetTargetValue(workingVarName);
		if (working == true) {
            bool state = (bool)manager.GetComponent<AreaManager>().GetTargetValue(lightsOnVarName);
			if (state == true) {
				if (lastState != state) {
					lastState = state;
					StartCoroutine (FlickerLightsOn ());
				}
			}
			else {
				lastState = false;
				ChangeLightState (false);
			}
		} else {
			lastState = false;
			ChangeLightState (false);
		}
	}
	void ChangeLightState(bool state) {
		spotLight.enabled = state;
		pointLight.enabled = state;
	}
	IEnumerator FlickerLightsOn() {
		ChangeLightState (true);
		yield return new WaitForSeconds (Random.Range(0.05f,0.1f));
		ChangeLightState (false);
		yield return new WaitForSeconds (Random.Range(0.04f, 0.05f));
		ChangeLightState (true);
		yield return new WaitForSeconds (Random.Range(0.3f, 0.5f));
		ChangeLightState (false);
		yield return new WaitForSeconds (Random.Range(0.2f,0.3f));
		ChangeLightState (true);
		yield return new WaitForSeconds (Random.Range(0.5f,0.8f));
		ChangeLightState (false);
		yield return new WaitForSeconds (Random.Range(0.5f,1.0f));
		ChangeLightState (true);
	}
}
