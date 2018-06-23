using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {
    public string sceneName = "";
    public Slider slider = null;
    private AsyncOperation ao;

	// Use this for initialization
	void Start () {
        StartCoroutine(LoadTargetLevel(sceneName));
	}

    IEnumerator LoadTargetLevel(string target_scene) {
        yield return new WaitForSeconds (0.1f);                     //Give some refresh time (To visually load objects)
        ao = SceneManager.LoadSceneAsync (target_scene);            //Start loading the other area asyncronously
        float progress = 0.0f;                                      //Track the loading progress
        while (!ao.isDone) {                                        //Keep updating the load bar until area is fully loaded
            progress = Mathf.Clamp01(ao.progress / 0.9f);           //Calculate to make sure bar fills when it hits 0.9f (100% loaded)
            slider.value = progress;                                //Visually set the loading bar progress
            yield return null;                                      //this allows the while loop to keep going
        }
    }
}
