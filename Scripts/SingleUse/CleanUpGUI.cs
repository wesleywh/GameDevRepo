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
    public float fadeSpeed = 0.5f;

    private bool fadeIn = false;
    private Color fadeColor;
	// Use this for initialization
	void Start () {
		loadingBackground = GameObject.FindGameObjectWithTag ("LoadingBackground").GetComponent<RawImage>();
		loadingBarSprites = GameObject.FindGameObjectWithTag ("LoadingBarSprites");
		loadingBar = GameObject.FindGameObjectWithTag ("LoadingBar");
		loadDesc  = GameObject.FindGameObjectWithTag ("LoadingDesc").GetComponent<Text>();
		loadTitle = GameObject.FindGameObjectWithTag ("LoadingTitle").GetComponent<Text>();
		popUp = GameObject.FindGameObjectWithTag ("PopUpText").GetComponent<Text> ();
		loadingBarBackground = GameObject.FindGameObjectWithTag ("LoadingBar").transform.Find ("Background").GetComponent<Image> ();
//		SetGUIState (false);
	}

    public void FadeOutGUI()
    {
        fadeColor = loadingBackground.color;
        fadeIn = true;
    }

	public void SetGUIState(bool state) {
		loadDesc.enabled = state;
		loadTitle.enabled = state;
        if (state == false)
        {
            fadeColor = loadingBackground.color;
            fadeIn = true;
        }
        else
        {
            loadingBackground.enabled = true;
        }
		popUp.text = "";
		SetLoadingBarState (state);
	}

    public void UpdateLoadTexture(Texture newTexture)
    {
        StartCoroutine(UpdateTexture(newTexture));
    }
    IEnumerator UpdateTexture(Texture newTexture)
    {
        yield return new WaitForSeconds(0.1f);
        while (fadeIn == true)
        {
            yield return new WaitForSeconds(0.1f);
        }
        loadingBackground.texture = newTexture;
    }

    void Update()
    {
        if (fadeIn == true)
        {
            fadeColor = loadingBackground.color;
            fadeColor.a -= fadeSpeed * Time.deltaTime;
            loadingBackground.color = fadeColor;
            if (fadeColor.a <= 0)
            {
                loadingBackground.enabled = false;
                fadeColor.a = 1;
                loadingBackground.color = fadeColor;
                fadeIn = false;
            }
        }
    }

	void SetLoadingBarState(bool state) {
		loadingBar.GetComponentInChildren<Image> ().enabled = state;
		loadingBar.transform.Find ("Fill Area").GetChild (0).GetComponent<Image> ().enabled = state;
		loadingBar.transform.Find ("Star_1").GetComponent<Image> ().enabled = state;
		loadingBar.transform.Find ("Star_2").GetComponent<Image> ().enabled = state;
		loadingBar.transform.Find ("Handle Slide Area").GetChild (0).GetComponent<Image> ().enabled = state;
		loadingBarSprites.transform.Find ("Swirls_1").GetComponent<Image> ().enabled = state;
		loadingBarSprites.transform.Find ("Swirls_2").GetComponent<Image> ().enabled = state;
	}
}
