using UnityEngine;
using System.Collections;
using UnityEngine.UI;					//for UI Access

public class CleanUpGUI : MonoBehaviour {

	private GameObject loadingBar = null;					//progress bar
	private Text loadTitle = null;							//area loading name
	private Text loadDesc = null;							//loading description
	private RawImage loadingBackground = null;				//background loading image
	private Text popUp = null;								//Popup text
	private GameObject loadingBarSprites = null;
	private Image loadingBarBackground = null;

	// Use this for initialization
	void Start () {
		loadingBackground = GameObject.FindGameObjectWithTag ("LoadingBackground").GetComponent<RawImage>();
		loadingBarSprites = GameObject.FindGameObjectWithTag ("LoadingBarSprites");
		loadingBar = GameObject.FindGameObjectWithTag ("LoadingBar");
		loadDesc  = GameObject.FindGameObjectWithTag ("LoadingDesc").GetComponent<Text>();
		loadTitle = GameObject.FindGameObjectWithTag ("LoadingTitle").GetComponent<Text>();
		popUp = GameObject.FindGameObjectWithTag ("PopUpText").GetComponent<Text> ();
		loadingBarBackground = GameObject.FindGameObjectWithTag ("LoadingBar").transform.FindChild ("Background").GetComponent<Image> ();
		SetGUIState (false);
	}

	void SetGUIState(bool state) {
		loadDesc.enabled = state;
		loadTitle.enabled = state;
		loadingBackground.enabled = state;
		popUp.text = "";
		loadingBarBackground.enabled = state;
		SetLoadingBarState (state);
	}

	void SetLoadingBarState(bool state) {
		loadingBar.GetComponentInChildren<Image> ().enabled = state;
		loadingBar.transform.FindChild ("Fill Area").GetChild (0).GetComponent<Image> ().enabled = state;
		loadingBar.transform.FindChild ("Star_1").GetComponent<Image> ().enabled = state;
		loadingBar.transform.FindChild ("Star_2").GetComponent<Image> ().enabled = state;
		loadingBar.transform.FindChild ("Handle Slide Area").GetChild (0).GetComponent<Image> ().enabled = state;
		loadingBarSprites.transform.FindChild ("Swirls_1").GetComponent<Image> ().enabled = state;
		loadingBarSprites.transform.FindChild ("Swirls_2").GetComponent<Image> ().enabled = state;
	}
}
