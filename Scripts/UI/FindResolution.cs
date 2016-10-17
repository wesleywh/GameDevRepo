using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FindResolution : MonoBehaviour {
	[SerializeField] private GameObject[] resolutionButtons;
	[SerializeField] private bool showCurrentResolution = false;

	void Start () {
		if (showCurrentResolution == false) {
			Resolution[] resolutions = Screen.resolutions;	
			for (int i = 0; i < resolutions.Length; i++) {
				if (i < resolutionButtons.Length) {
					resolutionButtons [i].GetComponentInChildren<Text> ().text = resolutions [i].width + " x " + resolutions [i].height;
					resolutionButtons [i].name = resolutions [i].width + " x " + resolutions [i].height+" button";
					resolutionButtons [i].transform.GetChild(0).name = resolutions [i].width + " x " + resolutions [i].height;
				} else {
					break;
				}
			}
			for (int i = resolutions.Length; i < resolutionButtons.Length; i++) {
				resolutionButtons [i].SetActive (false);
			}
		} else {
			foreach (GameObject res in resolutionButtons) {
				res.GetComponent<Text> ().text = Screen.width+" x "+Screen.height;
			}
		}
	}
	public void UpdateResolution() {
		Start ();
	}
	public void SetResolution(GameObject textObject) {
		bool fullScreen = GameObject.FindGameObjectWithTag ("FullScreenToggle").GetComponent<Toggle> ().isOn;
		string width = textObject.GetComponent<Text> ().text.Split ('x') [0];
		string height = textObject.GetComponent<Text> ().text.Split ('x') [1];
		GameObject.FindGameObjectWithTag ("CurrentResolution").GetComponent<Text> ().text = width+"x"+height;
		Screen.SetResolution (int.Parse(width), int.Parse(height), fullScreen);
	}
}
