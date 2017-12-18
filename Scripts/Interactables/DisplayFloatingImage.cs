using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using Pandora.Helpers;

public class DisplayFloatingImage : MonoBehaviour {

    [SerializeField] private bool enableOffScreenIndicator = true;
    [SerializeField] private float clampOffsetX = 12.0f;
    [SerializeField] private float clampOffsetY = 12.0f;
	[SerializeField] private float distance = 5.0f;
	[SerializeField] private Texture icon;
	[SerializeField] private Vector3 offset = Vector3.zero;
	[SerializeField] Vector2 size = new Vector2(20,20);
	[SerializeField] private bool hideWithActionPress = true;
    [Space(10)]
    [Header("Replaced \"<ACTION>\" keyword")] 
    public string text = "";//Press <ACTION> to pickup.
    [SerializeField] private Vector3 Textoffset = Vector3.zero;
    [SerializeField] ButtonOptions replaceActionWith = ButtonOptions.Action;
    [SerializeField] private GUIStyle style = null;
	private bool showTexture = false;

	private bool hiding = false;
    private Vector3 screenPos;
    private Camera cam;
    private Vector2 onScreenPos;
    private float max;

	void Update () {
		if (GameObject.FindGameObjectWithTag ("Player") &&
		    Vector3.Distance (transform.position, GameObject.FindGameObjectWithTag ("Player").transform.position) < distance) {
            if (GameObject.FindGameObjectWithTag ("PlayerCamera") && GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ())
                cam = GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera> ();
            if (hiding == false) {
				showTexture = true;
			}
		} else {
			showTexture = false;
			hiding = false;
		}
		if (showTexture && hideWithActionPress && InputManager.GetButtonDown ("Action")) {
			showTexture = false;
			hiding = true;
		}
	}
	void OnGUI() {
		if (showTexture) {
            screenPos = cam.WorldToScreenPoint(transform.position);
            if (enableOffScreenIndicator == false || (enableOffScreenIndicator==true && screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1))
            {
                if (icon != null)
                {
                    GUI.DrawTexture(new Rect(screenPos.x + offset.x, (Screen.height - screenPos.y) + offset.y, size.x, size.y), icon);
                }
//                GUI.DrawTexture(new Rect(screenPos.x +  offset.x, (Screen.height - screenPos.y) + offset.y, size.x, size.y), icon);
                GUI.TextArea(new Rect(screenPos.x + Textoffset.x, (Screen.height - screenPos.y) + Textoffset.y, 0, 0), Helpers.ModifiedText(replaceActionWith, text), 1000, style);
            }
            else
            {
                screenPos.x = Mathf.Clamp(screenPos.x, 0f + clampOffsetX, Screen.width - clampOffsetX);
                screenPos.y = Mathf.Clamp(screenPos.y, 0f + clampOffsetY, Screen.width - clampOffsetY);
                if (icon != null)
                {
                    GUI.DrawTexture(new Rect(screenPos.x + offset.x, (Screen.height - screenPos.y) + offset.y, size.x, size.y), icon);
                }
                GUI.TextArea(new Rect(screenPos.x + Textoffset.x, (Screen.height - screenPos.y) + Textoffset.y, 0, 0), Helpers.ModifiedText(replaceActionWith, text), 1000, style);
            }
        }
	}
}
