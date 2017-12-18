using UnityEngine;
using System.Collections;
using TeamUtility.IO;					//for input
using UnityEngine.UI;					//for UI stuff
using UnityEngine.SceneManagement;		//for loading scenes
using Pandora.Controllers;

namespace Pandora {
    namespace GameManager {
        public class ExitCityArea : MonoBehaviour {

        	[SerializeField] private float distance;
        	[SerializeField] private Texture fadeOutTexture;

        	private float alpha = 1;
        	private bool showFail = false;
        	private bool distanceFail = false;
        	[SerializeField] private GUIStyle guiStyle;
        	private Text textObj;

        	void Start() {
        		textObj = GameObject.FindGameObjectWithTag ("PopUpText").GetComponent<Text>();
        	}
        	void TryToMoveToArea() {
        		if (Vector3.Distance (transform.position, GameObject.FindGameObjectWithTag ("Player").transform.position) < distance) {
        			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().RemoveItem ("qi_GateKey");
                    GameObject.FindGameObjectWithTag ("Player").GetComponent<MovementController> ().moveLocked = true;
        			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<PlayerManager> ().StorePlayerInfo ();
        			StartCoroutine (LoadScene ());
        		} else {
        			textObj.text = "Unable To Use Key";
        			distanceFail = true;
        			alpha = 1;
        		}
        	}
        	IEnumerator LoadScene() {
        		yield return new WaitForSeconds (0.2f);
        		SceneManager.LoadScene ("Courtyard");
        	}
        	void Update() {
        		if (GameObject.FindGameObjectWithTag ("Player") && 
        			Vector3.Distance (transform.position, GameObject.FindGameObjectWithTag ("Player").transform.position) < distance && 
        			InputManager.GetButtonDown("Action")) {
        			textObj.text = "Requires Gate Key";
        			showFail = true;
        			alpha = 1;
        		}
        		if (showFail || distanceFail) {
        			alpha -= Time.deltaTime / 2;
        			Color mycolor = textObj.color;
        			mycolor.a = alpha;
        			textObj.color = mycolor;
        			if (alpha <= 0) {
        				showFail = false;
        				distanceFail = false;
        			}
        		}
        	}
        	void OnDrawGizmos() {
        		Gizmos.color = Color.green;
        		Gizmos.DrawWireSphere (transform.position, distance);
        	}
        }
    }
}