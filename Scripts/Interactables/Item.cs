using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using Pandora.Helpers;

public class Item : MonoBehaviour {
    #region Variables
	private bool moveObject = false;
	private bool selectable = true;
	private GameObject target = null;
	private float start = 0.0f;
	private float length = 0.0f;
	private GameObject cam;
	private bool showMessage = false;
	private bool showLockedMessage = false;
	private float guiAlpha = 1.0f;
	private float timer = 0.0f;
	private bool addToInventory = false;
	public bool isLocked = false;
	[SerializeField] private string itemName = "";
	[SerializeField] private GameObject visualEffect;
	[SerializeField] private bool autoAdd = false;
	[SerializeField] private float guiFadeSpeed = 0.5f;
	[SerializeField] private GUIStyle textStyling;
	[SerializeField] private string message = "";
	[SerializeField] private string lockedMessage = "";
	[SerializeField] private bool destroyOnPickup = false;
	[SerializeField] private bool moveToCamera = false;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip pickupSound;
	[SerializeField] private AudioClip storeSound;
	[SerializeField] private float distancePickupRange = 2.0f;
    [Space(10)]
	[SerializeField] private Texture floatingImage;
	[SerializeField] private Vector2 overrideImageSize = Vector2.zero;
	[SerializeField] private Vector2 textureOffset = Vector2.zero;
    [Space(10)]
    [SerializeField] private string displayText = "<ACTION>";
    [SerializeField] private ButtonOptions replaceActionWith = ButtonOptions.Action;
    [SerializeField] private Vector2 textOffset = Vector2.zero;
    [SerializeField] private GUIStyle style = null;
    [Space(10)]
	[SerializeField] private bool destroyIfVarNotSet = false;
	[SerializeField] private string variable = null;
	[SerializeField] private string state = null;
	enum itemVarType {Bool, String, Integer, Float}
	[SerializeField] private itemVarType varType = itemVarType.Bool;

	private bool dontDraw = false;
	private bool drawImage = false;
	private GameObject UI;
	private Vector3 objPos = Vector3.zero;
    private Vector3 screenPos;
    private Camera playerCam;
    #endregion

	[ExecuteInEditMode]
	void Start() {
        playerCam = GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ();
		UI = GameObject.FindGameObjectWithTag ("GameManager");
		if (audioSource == null) {
			audioSource = this.GetComponent<AudioSource> ();
		}
		if (destroyIfVarNotSet == true) {
			GameObject target = GameObject.FindGameObjectWithTag ("GameManager");
			switch (varType) {
				case itemVarType.Bool:
					bool curState0 = (bool)target.GetComponent<AreaManager> ().GetType ().GetField (variable).GetValue (target.GetComponent<AreaManager> ());
					if (curState0 != bool.Parse(state)) {
						Destroy (this.gameObject);
					}
					break;
				case itemVarType.Float:
					float curState1 = (float)target.GetComponent<AreaManager> ().GetType ().GetField (variable).GetValue (target.GetComponent<AreaManager> ());
					if (curState1 != float.Parse(state)) {
						Destroy (this.gameObject);
					}
					break;
				case itemVarType.Integer:
					int curState2 = (int)target.GetComponent<AreaManager> ().GetType ().GetField (variable).GetValue (target.GetComponent<AreaManager> ());
					if (curState2 != int.Parse(state)) {
						Destroy (this.gameObject);
					}
					break;
				case itemVarType.String:
					string curState3 = (string)target.GetComponent<AreaManager> ().GetType ().GetField (variable).GetValue (target.GetComponent<AreaManager> ());
					if (curState3 != state) {
						Destroy (this.gameObject);
					}
					break;
			}
		}
	}
    void FixedUpdate() {
        if (playerCam == null)
        {
            if (GameObject.FindGameObjectWithTag ("PlayerCamera") && GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ())
                playerCam = GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ();
        }
        if (UI == null)
            if (GameObject.FindGameObjectWithTag ("GameManager"))
                UI = GameObject.FindGameObjectWithTag ("GameManager");
    }
	void Update () {
		if (dontDraw == false && GameObject.FindGameObjectWithTag("Player") && Vector3.Distance (transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < distancePickupRange) {
			if (floatingImage) {
				drawImage = true;
				objPos = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>().WorldToScreenPoint (this.transform.position);
			}
		} else if(floatingImage){
			drawImage = false;
		}

		if (InputManager.GetButton ("Action") && selectable) {
			target = ClosestPlayer ();
			if (Vector3.Distance (this.transform.position, target.transform.position) < distancePickupRange) {
				drawImage = false;
				dontDraw = true;
				if (isLocked) {
					showLockedMessage = true;
					StartCoroutine (CloseGUI ());
					return;
				}
				if (visualEffect) {
					Instantiate (visualEffect, this.transform.position, this.transform.rotation);
				}
				if (UI.GetComponent<InventoryManager>().InventoryFull ()) {
					return;
				}
				if (audioSource != null) {
					audioSource.clip = pickupSound;
					audioSource.Play ();
				}
				if (destroyOnPickup) {
					addToInventory = true;
				}
				if (moveToCamera == true) {
					start = Time.time;
					cam = GameObject.FindGameObjectWithTag ("PlayerCamera");
					length = Vector3.Distance (this.transform.position, cam.transform.position + cam.transform.position*0.02f);
					moveObject = true;
				}
				showMessage = true;
				selectable = false;
			}
		}
		if (moveObject) {
			if(this.GetComponent<Rigidbody>().isKinematic == false) {
				this.GetComponent<Rigidbody> ().isKinematic = true;
			}
			Quaternion neededRotation = Quaternion.LookRotation(target.transform.position - this.transform.position);
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, neededRotation, Time.deltaTime * 0.5f);
			float distance = (Time.time - start) * 0.5f;
			float fraction = distance / length;
			this.transform.position = Vector3.Lerp (this.transform.position, cam.transform.position + cam.GetComponent<Camera>().transform.forward*0.5f, fraction);
			if (fraction > 0.5f && autoAdd) {
				timer += Time.deltaTime;

				if (timer > 0.5f) {
					start = Time.time;
					length = Vector3.Distance (this.transform.position, cam.transform.position - cam.transform.up * 2.0f);
					moveObject = false;
					addToInventory = true;
					audioSource.clip = storeSound;
					audioSource.Play ();
					UI.GetComponent<InventoryManager> ().AddToInventory (itemName.ToLower());
				}
			}
			guiAlpha -= Time.deltaTime / guiFadeSpeed;
		}
		if (addToInventory) {
			guiAlpha -= Time.deltaTime / guiFadeSpeed;
			float distance = (Time.time - start) * 0.5f;
			float fraction = distance / length;
			if (this.GetComponent<Rigidbody> ()) {
				this.GetComponent<Rigidbody> ().useGravity = false;
				this.GetComponent<Rigidbody> ().isKinematic = true;
			}
			if (destroyOnPickup) {
				UI.GetComponent<InventoryManager> ().AddToInventory (itemName.ToLower());
				Destroy (this.gameObject);
				return;
			}
			this.transform.position = Vector3.Lerp (this.transform.position, cam.transform.position - cam.transform.up * 0.5f, fraction);
			if (fraction > 1.0f) {
				Destroy (this.gameObject);
			}
		}
	}
	GameObject ClosestPlayer() {
		GameObject closest = null;
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in players) {
			if (closest == null) {
				closest = player;
			} 
			else if(Vector3.Distance(closest.transform.position, this.transform.position) >
				Vector3.Distance(player.transform.position, this.transform.position)){
				closest = player;
			}
		}
		return closest;
	}
	IEnumerator CloseGUI() {
		yield return new WaitForSeconds (5.0f);
		showMessage = false;
		showLockedMessage = false;
	}

	void OnGUI() {
		if (drawImage) {
			if (overrideImageSize.x > 0 || overrideImageSize.y > 0) {
				GUI.DrawTexture (new Rect (objPos.x + textureOffset.x, (Screen.height - objPos.y) + textureOffset.y, overrideImageSize.x, overrideImageSize.y), floatingImage);
			} else {
				GUI.DrawTexture (new Rect (objPos.x + textureOffset.x, (Screen.height - objPos.y) + textureOffset.y, 20, 20), floatingImage);
			}
            screenPos = playerCam.WorldToScreenPoint(transform.position);
            GUI.TextArea(new Rect(screenPos.x +  textOffset.x, (Screen.height - screenPos.y) + textOffset.y, 0, 0), Helpers.ModifiedText(replaceActionWith,displayText),1000,style);
		}
		if (showMessage) {
			if (textStyling != null) {
				GUI.Label (new Rect (50, Screen.height - 100, 200, 150), message, textStyling);
				Color color = textStyling.normal.textColor;
				color.a = guiAlpha;
				GUI.color = color;
				textStyling.normal.textColor = GUI.color;
			} else {
				GUI.Label (new Rect (50, Screen.height - 100, 200, 150), message);
			}
		}
		if (showLockedMessage) {
			if (textStyling != null) {
				GUI.Label (new Rect (50, Screen.height - 100, 200, 150), lockedMessage, textStyling);
				Color color = textStyling.normal.textColor;
				color.a = guiAlpha;
				GUI.color = color;
				textStyling.normal.textColor = GUI.color;
			} else {
				GUI.Label (new Rect (50, Screen.height - 100, 200, 150), lockedMessage);
			}
		}
	}
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (transform.position, distancePickupRange);
	}
}
