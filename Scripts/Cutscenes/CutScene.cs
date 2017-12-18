using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic; 	//for list & dictionary
using Pandora.Controllers;

namespace Pandora {
    namespace Cameras {
        [Serializable]
        class Points {
        	[Header("Player")]
        	public string PlayerLookAtTag = "";
        	public string PlayerLookAtName = "";
        	public float playerLookSpeed = 0.5f;
        	public bool playerLockLook = false;
        	public GameObject playerPoint = null;
        	public float playerSpeedToPoint = 0.5f;
        	public float playerGetClostPercent = 1.0f;
        	public string playerAnimAtPoint = null;
        	public float playerAnimTransitionTime = 1.0f;
        	public RuntimeAnimatorController animControllerAtPoint = null;
        	[HideInInspector] public bool playerAnimHasPlayed = false;
        	[HideInInspector] public bool playerPlayingSound = false;
        	public AudioClip playerAudioClipAtPoint = null;
        	[SerializeField] public AudioSource playerOverrideAudioSource = null;
        	[Range(0,1)]
        	[SerializeField] public float playerAudioVolume = 1.0f;
        	[SerializeField] public bool playerLoopAudio = false;
        	[Space(10)]
        	[Header("Camera")]
        	public string LookAtTag = "";
        	public string LookAtName = "";
        	public float camLookSpeed = 0.5f;
        	public bool camLookLockPoint = false;
        	public GameObject camMovePoint = null;
        	public float camSpeedToPoint = 0.0f;
        	public float camGetClosePercent = 1.0f;
        	[HideInInspector] public bool objectCreated = false; 
        	public GameObject instantiation = null;
        	public GameObject instantiationPoint = null;
        	public bool fadeIn = false;
        	public bool fadeOut = false;
        	public float fadeSpeed = 0.5f;
        	public bool jumpToNextPoint = false;
        	public AudioClip audioClipAtPoint = null;
        	[SerializeField] public AudioSource overrideAudioSource = null;
        	[HideInInspector] public bool camPlayingSound = false;
        	[Range(0,1)]
        	[SerializeField] public float audioVolume = 1.0f;
        	[SerializeField] public bool loopAudio = false;
        }
        public class CutScene : MonoBehaviour {
        	public bool playerCanMove = true;
        	[SerializeField] private string checkVar = null;
        	[SerializeField] private bool checkVarState = false;
        	[SerializeField] private GameObject Cam;
        	[SerializeField] private Texture fadeInOutImage;
        	[SerializeField] Points[] cutsceneInfo;
        	[SerializeField] private bool startImmediatly = false;
        	[Range(0,1)]
        	[SerializeField] private float imageAlphaStart = 1.0f;
        	[SerializeField] private GameObject createAtEnd;
        	[SerializeField] private GameObject createLoc;
        	[SerializeField] private bool enablePlayerCamWhenDone = true;
        	[SerializeField] private AudioSource audioSource;
        	[SerializeField] private bool destroyAtEnd = false;

        	//for lerping camera
        	private float startTime = 0;
        	private float journeyLength = 0;
        	private float distCovered = 0;
        	private float fracJourney = 0;

        	//for player
        	private float startTimePlayer = 0;
        	private float journeyLengthPlayer = 0;
        	private float distCoveredPlayer = 0;
        	private float fracJourneyPlayer = 0;
        	private int j = 0;

        	//for control
        	private bool startCutscene = false;
        	private bool camMoving = false;
        	private bool playerMoving = false;
        	private float guiAlpha = 1.0f;
        	private GameObject player;
        	private int i = 0;
        	private bool created = false;
        //	private bool playingSound = false;

        	//for debugging
        	[SerializeField] private bool debugPlayerState = false;
        	[SerializeField] private bool debugCameraJourney = false;
        	[SerializeField] private bool debugCameraFades = false;
        	void Start() {
        		if (Cam) {
        			Cam.GetComponent<Camera> ().enabled = false;
        		}
        		if (startImmediatly) {
        			if (checkVar != null) {
        				GameObject manager = GameObject.FindGameObjectWithTag ("GameManager");
        				bool setValue = (bool)manager.GetComponent<AreaManager> ().GetType ().GetField (checkVar).GetValue (manager.GetComponent<AreaManager> ());
        				if (setValue == checkVarState) {
        					StartCutScene ();
        				}
        			} else {
        				StartCutScene ();
        			}
        		}
        	}
        	void Update () {
        		if(startCutscene) {
        			//camera logic
        			if (camMoving) {
        				if (cutsceneInfo [i].camMovePoint != null) {
        					distCovered = (Time.time - startTime) * cutsceneInfo [i].camSpeedToPoint;
        					fracJourney = (float.IsNaN (distCovered / journeyLength)) ? 0 : distCovered / journeyLength;
        					Cam.transform.position = Vector3.Lerp (Cam.transform.position, cutsceneInfo [i].camMovePoint.transform.position, fracJourney);
        				} else {
        					fracJourney += Time.deltaTime * cutsceneInfo [i].camSpeedToPoint;
        				}
        				if (debugCameraJourney) {
        					Debug.Log (i + "'s Fraction Journey = " + fracJourney);
        				}
        				if (cutsceneInfo [i].fadeIn == true) {
        					guiAlpha -= Time.deltaTime * cutsceneInfo[i].fadeSpeed;
        					if (debugCameraFades) {
        						Debug.Log (i + "'s fading in");
        					}
        				} else if (cutsceneInfo [i].fadeOut == true) {
        					guiAlpha += Time.deltaTime * cutsceneInfo[i].fadeSpeed;
        					if (debugCameraFades) {
        						Debug.Log (i + "'s fading out");
        					}
        				}
        				if ((i < cutsceneInfo.Length && cutsceneInfo [i].instantiation) || (j < cutsceneInfo.Length && cutsceneInfo [j].instantiation) && created == false) {
        					created = true;
        					if (cutsceneInfo [i].instantiation && cutsceneInfo [i].objectCreated == false) {
        						cutsceneInfo [i].objectCreated = true;
        						Instantiate (cutsceneInfo [i].instantiation, cutsceneInfo [i].instantiationPoint.transform.position, cutsceneInfo [i].instantiationPoint.transform.rotation);
        					} 
        					if (cutsceneInfo [j].instantiation && cutsceneInfo [j].objectCreated == false){
        						cutsceneInfo [j].objectCreated = true;
        						Instantiate (cutsceneInfo [j].instantiation, cutsceneInfo [j].instantiationPoint.transform.position, cutsceneInfo [j].instantiationPoint.transform.rotation);
        					}
        				}
        				if (cutsceneInfo [i].jumpToNextPoint) {
        					Cam.transform.position = cutsceneInfo [i + 1].camMovePoint.transform.position;
        				}
        				if (cutsceneInfo [i].LookAtName != ""){
        					if (cutsceneInfo [i].camLookLockPoint == false) {
        						Quaternion neededRotation = Quaternion.LookRotation (GameObject.Find (cutsceneInfo [i].LookAtName).transform.position - Cam.transform.position);
        						Cam.transform.rotation = Quaternion.Lerp (Cam.transform.rotation, neededRotation, Time.deltaTime * cutsceneInfo [i].camLookSpeed);
        					} else {
        						Cam.transform.LookAt (GameObject.Find (cutsceneInfo [i].LookAtName).transform);
        					}
        				}
        				if (cutsceneInfo [i].LookAtTag != "") {
        					if (cutsceneInfo [i].camLookLockPoint == false) {
        						Quaternion neededRotation = Quaternion.LookRotation (GameObject.FindGameObjectWithTag (cutsceneInfo [i].LookAtTag).transform.position - Cam.transform.position);
        						Cam.transform.rotation = Quaternion.Lerp (Cam.transform.rotation, neededRotation, Time.deltaTime * cutsceneInfo [i].camLookSpeed);
        					} else {
        						Cam.transform.LookAt (GameObject.Find (cutsceneInfo [i].LookAtTag).transform);
        					}
        				}
        				if (cutsceneInfo [i].audioClipAtPoint && cutsceneInfo [i].camPlayingSound == false) {
        					cutsceneInfo [i].camPlayingSound = true;
        					if (cutsceneInfo [i].overrideAudioSource) {
        						cutsceneInfo [i].overrideAudioSource.clip = cutsceneInfo [i].audioClipAtPoint;
        						cutsceneInfo [i].overrideAudioSource.volume = cutsceneInfo [i].audioVolume;
        						cutsceneInfo [i].overrideAudioSource.Play ();
        						if (cutsceneInfo [i].loopAudio == true) {
        							cutsceneInfo [i].overrideAudioSource.loop = true;
        						} else {
        							cutsceneInfo [i].overrideAudioSource.loop = false;
        						}
        					} else {
        						audioSource.clip = cutsceneInfo [i].audioClipAtPoint;
        						audioSource.volume = cutsceneInfo [i].audioVolume;
        						audioSource.Play ();
        						if (cutsceneInfo [i].loopAudio == true) {
        							audioSource.loop = true;
        						} else {
        							audioSource.loop = false;
        						}
        					}
        //					playingSound = true;
        				}
        				if (fracJourney >= cutsceneInfo [i].camGetClosePercent) {
        					i += 1;
        					camMoving = (i >= cutsceneInfo.Length) ? false : true;
        					startTime = Time.time;
        					if (i < cutsceneInfo.Length) {
        						if (cutsceneInfo [i].camMovePoint) {
        							journeyLength = (i < cutsceneInfo.Length) ? Vector3.Distance (this.transform.position, cutsceneInfo [i].camMovePoint.transform.position) : i - 1;
        						}
        					} else {
        						camMoving = false;
        					}
        					created = false;
        //					playingSound = false;
        				}
        			}

        			//player logic
        			if (playerMoving) {
        				if (cutsceneInfo [j].playerPoint) {
        					if (debugPlayerState) {
        						Debug.Log (j + "'s Fraction Journey = " + fracJourneyPlayer);
        					}
        					distCoveredPlayer = (Time.time - startTimePlayer) * cutsceneInfo [j].playerSpeedToPoint;
        					fracJourneyPlayer = (float.IsNaN (distCoveredPlayer / journeyLengthPlayer)) ? 0 : distCoveredPlayer / journeyLengthPlayer;
        					player.transform.position = Vector3.Lerp (player.transform.position, cutsceneInfo [j].playerPoint.transform.position, fracJourneyPlayer);
        				}
        				if (cutsceneInfo [j].animControllerAtPoint != null) {
        					player.GetComponent<Animator> ().runtimeAnimatorController = cutsceneInfo [j].animControllerAtPoint;
        				}
        				if (cutsceneInfo [j].playerAnimAtPoint != null && cutsceneInfo [j].playerAnimHasPlayed == false) {
        					player.GetComponent<Animator> ().CrossFade (cutsceneInfo [j].playerAnimAtPoint, cutsceneInfo [j].playerAnimTransitionTime);
        					cutsceneInfo [j].playerAnimHasPlayed = true;
        				}
        				if (cutsceneInfo [j].PlayerLookAtTag != "") {
        					if (cutsceneInfo [j].playerLockLook == false) {
        						Quaternion neededRotation = Quaternion.LookRotation (GameObject.FindGameObjectWithTag (cutsceneInfo [j].PlayerLookAtTag).transform.position - player.transform.position);
        						player.transform.rotation = Quaternion.Lerp (player.transform.rotation, neededRotation, Time.deltaTime * cutsceneInfo [j].playerLookSpeed);
        					}
        					else {
        						player.transform.LookAt (GameObject.FindGameObjectWithTag (cutsceneInfo [j].PlayerLookAtTag).transform);
        					}
        				}
        				if (cutsceneInfo [j].PlayerLookAtName != ""){
        					if (cutsceneInfo [j].playerLockLook == false) {
        						Quaternion neededRotation = Quaternion.LookRotation (GameObject.Find (cutsceneInfo [j].PlayerLookAtName).transform.position - player.transform.position);
        						player.transform.rotation = Quaternion.Lerp (player.transform.rotation, neededRotation, Time.deltaTime * cutsceneInfo [j].playerLookSpeed);
        					}
        					else {
        						player.transform.LookAt (GameObject.Find (cutsceneInfo [j].PlayerLookAtName).transform);
        					}
        				}
        				if (cutsceneInfo [j].playerAudioClipAtPoint && cutsceneInfo [j].playerPlayingSound == false) {
        					cutsceneInfo [j].playerPlayingSound = true;
        					if (cutsceneInfo [j].playerOverrideAudioSource) {
        						cutsceneInfo [j].playerOverrideAudioSource.clip = cutsceneInfo [j].playerAudioClipAtPoint;
        						cutsceneInfo [j].playerOverrideAudioSource.volume = cutsceneInfo [j].playerAudioVolume;
        						cutsceneInfo [j].playerOverrideAudioSource.Play ();
        						if (cutsceneInfo [j].playerLoopAudio == true) {
        							cutsceneInfo [j].playerOverrideAudioSource.loop = true;
        						} else {
        							cutsceneInfo [j].playerOverrideAudioSource.loop = false;
        						}
        					} else {
        						audioSource.clip = cutsceneInfo [j].playerAudioClipAtPoint;
        						audioSource.volume = cutsceneInfo [j].playerAudioVolume;
        						audioSource.Play ();
        						if (cutsceneInfo [j].playerLoopAudio == true) {
        							audioSource.loop = true;
        						} else {
        							audioSource.loop = false;
        						}
        					}
        //					playingSound = true;
        				}
        				if (fracJourneyPlayer >= cutsceneInfo [j].playerGetClostPercent || cutsceneInfo [j].playerPoint == null) {
        					j += 1;
        					playerMoving = (j >= cutsceneInfo.Length) ? false : true;
        					startTimePlayer = Time.time;
        					if (j < cutsceneInfo.Length && cutsceneInfo [j].playerPoint) {
        						journeyLengthPlayer = (j < cutsceneInfo.Length) ? Vector3.Distance (this.transform.position, cutsceneInfo [j].playerPoint.transform.position) : j - 1;
        					} else {
        						playerMoving = false;
        					}
        				}
        			}
        			//ending cutscene logic
        			if (playerMoving == false && camMoving == false) {
        				if (createAtEnd) {
        					Debug.Log ("Created");
        					Instantiate (createAtEnd, createLoc.transform.position, createLoc.transform.rotation);
        				}
        				if (Cam) {
        					Cam.GetComponent<Camera> ().enabled = false;
        				}
        				if (GameObject.FindGameObjectWithTag ("Player") && enablePlayerCamWhenDone) {
        					GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<Camera> ().enabled = true;
        				}
        				player.GetComponent<MovementController> ().moveLocked = false;
        				player.GetComponent<MouseLook> ().enabled = true;
                        player.GetComponent<MovementController> ().groundLocked = false;
        //				player.GetComponent<Animator> ().Play ("", -1, 0.0f);
        				GameObject.FindGameObjectWithTag ("CameraHolder").GetComponent<MouseLook> ().enabled = false;
        				GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<AudioListener>().enabled = false;
        				//EnableUI ();
        				if (destroyAtEnd) {
        					Destroy (this.gameObject);
        				} else {
        					this.gameObject.GetComponent<CutScene> ().enabled = false;
        				}
        			}
        		}
        	}
        	public void StartCutScene() {
        		i = 0;
        		j = 0;
        		player = GameObject.FindGameObjectWithTag ("Player");
        		if (GameObject.FindGameObjectWithTag ("Player")) {
        			GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<Camera> ().enabled = false;
        		}
        		if (Cam) {
        			if (Cam.activeSelf == false) {
        				Cam.SetActive (true);
        			}
        			Cam.GetComponent<Camera> ().enabled = true;
        			Cam.GetComponentInChildren<Camera> ().enabled = true;
        			camMoving = true;
        			startTime = Time.time;
        			if (cutsceneInfo [i].camMovePoint) {
        				journeyLength = Vector3.Distance (this.transform.position, cutsceneInfo [i].camMovePoint.transform.position);
        			}
        		} else {
        			camMoving = false;
        		}
        		if (cutsceneInfo [i].playerPoint) {
        			startTimePlayer = Time.time;
        			journeyLengthPlayer = Vector3.Distance (this.transform.position, cutsceneInfo [i].playerPoint.transform.position);
        			playerMoving = true;
        		}
        		startCutscene = true;
        		guiAlpha = imageAlphaStart;
        		//DisableUI ();
        		if (playerCanMove == false) {
                    if (player.GetComponent<Controllers.MovementController> ()) {
                        player.GetComponent<Controllers.MovementController> ().moveLocked = true;
        			}
                    if (player.GetComponent<Controllers.MovementController> ()) {
                        player.GetComponent<Controllers.MovementController> ().groundLocked = true;
        			}
        			if (player.GetComponent<MouseLook> ()) {
        				player.GetComponent<MouseLook> ().enabled = false;
        			}
        			if (GameObject.FindGameObjectWithTag ("CameraHolder").GetComponent<MouseLook> ()) {
        				GameObject.FindGameObjectWithTag ("CameraHolder").GetComponent<MouseLook> ().enabled = false;
        			}
        			if (GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<AudioListener> ()) {
        				GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<AudioListener> ().enabled = false;
        			}
        		}
        	}
        	/// GUI
        	/// 
        	void OnGUI() {
        		if (startCutscene && fadeInOutImage) {
        			Color newColor = GUI.color;
        			newColor.a = guiAlpha;
        			GUI.color = newColor;
        			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeInOutImage);
        		}
        	}

        /// 
        /////////////////////////////////////////////// 
        	private void DisableUI() {
        		Transform UIHealthBar = GameObject.FindGameObjectWithTag ("UIHealthbar").transform;
        		foreach (Transform item in UIHealthBar) {
        			if (item.GetComponent<RawImage> ()) {
        				Color itemColor = item.GetComponent<RawImage> ().color;
        				itemColor.a = 0;
        				item.GetComponent<RawImage> ().color = itemColor;
        			} else if (item.GetComponent<Image> ()) {
        				Color itemColor = item.GetComponent<Image> ().color;
        				itemColor.a = 0;
        				item.GetComponent<Image> ().color = itemColor;
        			} else if (item.GetComponent<Text> ()) {
        				Color itemColor = item.GetComponent<Text> ().color;
        				itemColor.a = 0;
        				item.GetComponent<Text> ().color = itemColor;
        			}
        		}
        		Color InvColor = GameObject.FindGameObjectWithTag ("UIInventory").GetComponent<Image> ().color;
        		InvColor.a = 0;
        		GameObject.FindGameObjectWithTag ("UIInventory").GetComponent<Image> ().color = InvColor;
        		GameObject.FindGameObjectWithTag ("UIInventory").transform.GetChild (0).gameObject.SetActive (false);
        		Transform icons = GameObject.FindGameObjectWithTag ("UIInventory").transform.GetChild (1).transform;//icons
        		Transform texts = GameObject.FindGameObjectWithTag ("UIInventory").transform.GetChild (2).transform;//text
        		foreach (Transform icon in icons) {
        			Color imageColor = icon.GetComponent<Image> ().color;
        			imageColor.a = 0;
        			icon.GetComponent<Image> ().color = imageColor;
        		}
        		foreach(Transform text in texts) {
        			Color textColor = text.GetComponent<Text> ().color;
        			textColor.a = 0;
        			text.GetComponent<Text> ().color = textColor;
        		}
        	}
        	private void EnableUI() {
        		Transform UIHealthBar = GameObject.FindGameObjectWithTag ("UIHealthbar").transform;
        		int count = 0;
        		foreach (Transform item in UIHealthBar) {
        			if (item.GetComponent<RawImage> ()) {
        				Color itemColor = item.GetComponent<RawImage> ().color;
        				itemColor.a = 1;
        				item.GetComponent<RawImage> ().color = itemColor;
        			} else if (item.GetComponent<Image> ()) {
        				Color itemColor = item.GetComponent<Image> ().color;
        				itemColor.a = 1;
        				item.GetComponent<Image> ().color = itemColor;
        			} else if (item.GetComponent<Text> ()) {
        				Color itemColor = item.GetComponent<Text> ().color;
        				itemColor.a = 1;
        				item.GetComponent<Text> ().color = itemColor;
        			}
        			count++;
        			if (count >= GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().GetPlayerInventory ().Count) {
        				break;
        			}
        		}
        		Color InvColor = GameObject.FindGameObjectWithTag ("UIInventory").GetComponent<Image> ().color;
        		InvColor.a = 1;
        		GameObject.FindGameObjectWithTag ("UIInventory").GetComponent<Image> ().color = InvColor;
        		GameObject.FindGameObjectWithTag ("UIInventory").transform.GetChild (0).gameObject.SetActive (true);
        		Transform icons = GameObject.FindGameObjectWithTag ("UIInventory").transform.GetChild (1).transform;//icons
        		Transform texts = GameObject.FindGameObjectWithTag ("UIInventory").transform.GetChild (2).transform;//text
        		foreach (Transform icon in icons) {
        			Color imageColor = icon.GetComponent<Image> ().color;
        			imageColor.a = 1;
        			icon.GetComponent<Image> ().color = imageColor;
        		}
        		foreach(Transform text in texts) {
        			Color textColor = text.GetComponent<Text> ().color;
        			textColor.a = 1;
        			text.GetComponent<Text> ().color = textColor;
        		}
        	}
        }
    }
}