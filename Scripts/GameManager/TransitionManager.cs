using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pandora.GameManager {
    [RequireComponent(typeof(GUIManager))]
    public class TransitionManager : MonoBehaviour {
        public bool gameManager = true;
        [Header("Debugging")]
        public bool disable = false;
        public string travelToNamedObject = "";

        private GUIManager guiManager = null;
        private AsyncOperation ao;

    	// Use this for initialization
    	void Start () {
            guiManager = dontDestroy.currentGameManager.GetComponent<GUIManager>();
            if (gameManager == false && disable == false)
            {
                string targetName = dontDestroy.currentGameManager.GetComponent<TransitionManager>().travelToNamedObject;
                if (!string.IsNullOrEmpty(targetName))
                {
                    GameObject target = GameObject.Find(targetName);
                    this.transform.position = target.transform.position;
                    this.transform.rotation = target.transform.rotation;
                }
            }
    	}
        public void LoadTargetLevel(string name, string travelPoint, AudioClip clip = null, float volume = 0.5f, string title = "", string description = "", Texture2D background = null)
        {
            StartCoroutine(LoadLevel(name, travelPoint,clip,volume,title,description,background));
        }
        IEnumerator LoadLevel(string name, string travelPoint, AudioClip clip, float volume, string title, string description, Texture2D background) {
            //Create a temp music player and player it.
            GameObject musicPlayer = GameObject.CreatePrimitive (PrimitiveType.Cube);
            musicPlayer.transform.position = GameObject.FindGameObjectWithTag ("Player").transform.position;
            Destroy (GameObject.FindGameObjectWithTag ("Player"));
            musicPlayer.tag = "Player";
            AudioSource audioSrc = musicPlayer.AddComponent<AudioSource> ();
            musicPlayer.AddComponent<AudioListener> ();
            audioSrc.clip = clip;
            audioSrc.volume = volume;
            audioSrc.loop = true;
            audioSrc.spatialBlend = 0.0f;
            audioSrc.Play ();

            //Set Transition Manager Var
            travelToNamedObject = travelPoint;                          //Set the Transition Manager travel to point for next area (Makes the player jump to this object on area load)
            guiManager.EnableLoadingScreen(true,title,description,background); //Turn on GUI and set loading bar to zero
            yield return new WaitForSeconds (0.1f);                     //Give some refresh time (To visually load objects)
            ao = SceneManager.LoadSceneAsync (name);                    //Start loading the other area asyncronously
            float progress = 0.0f;                                      //Track the loading progress
            while (!ao.isDone) {                                        //Keep updating the load bar until area is fully loaded
                progress = Mathf.Clamp01(ao.progress / 0.9f);           //Calculate to make sure bar fills when it hits 0.9f (100% loaded)
                guiManager.SetLoadingBarValue(progress);                //Visually set the loading bar progress
                yield return null;                                      //this allows the while loop to keep going
            }
            //Note: The loading screen should be turned off by the next area
        }
    }
}