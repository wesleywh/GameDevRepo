using UnityEngine;
using System.Collections;
using TeamUtility.IO;					//for buttons
using UnityEngine.SceneManagement;		//for switching scenes
using UnityEngine.UI;					//for UI Access

public class TravelToArea : MonoBehaviour {
	[SerializeField] private string sceneIndexOrNameToLoad = "1";
	[SerializeField] private Texture2D loadImage = null;
	[SerializeField] private string loadingTitle = null;
	[SerializeField] private string[] loadingDesc = null;
	[SerializeField] private AudioClip loadingMusic = null;
	[SerializeField] private float keyAccessDistance = 3.0f;
	[SerializeField] private bool keyAccessOnly = false;
	[Range(0,1)]
	[SerializeField] private float audioVolume = 1f;
	private bool canPress = false;
	private GameObject loadingBar = null;					//progress bar
	private Text loadTitle = null;							//area loading name
	private Text loadDesc = null;							//loading description
	private RawImage loadingBackground = null;				//background loading image
	private GameObject loadingBarSprites = null;
	private AsyncOperation ao;								//allows to track progress of loading

	void Start() {
		loadingBackground = GameObject.FindGameObjectWithTag ("LoadingBackground").GetComponent<RawImage>();
		loadingBarSprites = GameObject.FindGameObjectWithTag ("LoadingBarSprites");
		loadingBar = GameObject.FindGameObjectWithTag ("LoadingBar");
		loadDesc  = GameObject.FindGameObjectWithTag ("LoadingDesc").GetComponent<Text>();
		loadTitle = GameObject.FindGameObjectWithTag ("LoadingTitle").GetComponent<Text>();
		SetGUIState (false);
	}

	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") {
			canPress = true;
		}
	}
	void OnTriggerExit(Collider col) {
		if (col.tag == "Player") {
			canPress = false;
		}
	}
	// Update is called once per frame
	void Update () {
		if (InputManager.GetButton ("Action") && canPress == true && keyAccessOnly == false) {
			canPress = false;
			//set text
			loadingBackground.texture = loadImage;
			loadTitle.text = loadingTitle;
			loadDesc.text = loadingDesc[Random.Range(0,loadingDesc.Length)];
			StartCoroutine (LoadLevel ());
		}
	}

	public void LoadTargetLevel() {
		StartCoroutine (LoadLevel ());
	}
	IEnumerator LoadLevel() {
		//setup a temp player (so real player isn't effected during loading)
		GameObject tempPlayer = GameObject.CreatePrimitive (PrimitiveType.Cube);
		tempPlayer.transform.position = GameObject.FindGameObjectWithTag ("Player").transform.position;
		Destroy (GameObject.FindGameObjectWithTag ("Player"));
		tempPlayer.tag = "Player";
		AudioSource audioSrc = tempPlayer.AddComponent<AudioSource> ();
		tempPlayer.AddComponent<AudioListener> ();
		//stop all audio & load menu loading music
		audioSrc.clip = loadingMusic;
		audioSrc.volume = audioVolume;
		audioSrc.Play ();

		//turn gui on
		SetGUIState(true);
		yield return new WaitForSeconds (1);
		ao = SceneManager.LoadSceneAsync (sceneIndexOrNameToLoad);
		//ao.allowSceneActivation = false;						
		while (!ao.isDone) {
			loadingBar.GetComponent<Slider>().value = ao.progress;
			if (ao.progress == 0.9f) {			//loading is done
				loadingBar.GetComponent<Slider>().value = 1f;
			}
			yield return null;					//this allows the while loop to keep going
		}
		loadingBar.GetComponent<Slider>().value = 0f;
		//Note: The loading screen should be turned off by the next area
	}

	void SetGUIState(bool state) {
		loadDesc.enabled = state;
		loadTitle.enabled = state;
		loadingBackground.enabled = state;
		SetLoadingBarState (state);
	}
	void KeyAccess(string keyName) {
		if (Vector3.Distance (GameObject.FindGameObjectWithTag ("Player").transform.position,
			   this.transform.position) <= keyAccessDistance) {
			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().RemoveItem (keyName);
			loadingBackground.texture = loadImage;
			loadTitle.text = loadingTitle;
			loadDesc.text = loadingDesc[Random.Range(0,loadingDesc.Length)];
			StartCoroutine (LoadLevel ());
		}
	}
	void SetLoadingBarState(bool state) {
		loadingBar.GetComponentInChildren<Image> ().enabled = state;
		loadingBar.transform.FindChild ("Fill Area").GetChild (0).GetComponent<Image> ().enabled = state;
		loadingBar.transform.FindChild ("Star_1").GetComponent<Image> ().enabled = state;
		loadingBar.transform.FindChild ("Star_2").GetComponent<Image> ().enabled = state;
		loadingBarSprites.transform.FindChild ("Swirls_1").GetComponent<Image> ().enabled = state;
		loadingBarSprites.transform.FindChild ("Swirls_2").GetComponent<Image> ().enabled = state;
	}
}
