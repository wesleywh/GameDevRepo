using UnityEngine;
using System;
using TeamUtility.IO;
using UnityEngine.UI;
using Pandora.GameManager;

[Serializable]
class textBoxStyle {
	public float width = 200.0f;
	public float height = 150.0f;
}
public class TextEvent : MonoBehaviour {
	enum style {
		left, middle, right
	}
	private float guiAlpha = 1.0f;
	private bool beginFade = false;
	private bool shown = false;
	private bool fadeUp = false;
	private bool fadeDown = false;
	private float timer = 0.0f;
	[Header("'<BUTTON1>','<BUTTON2>','<ALTBUTTON1>' & '<ALTBUTTON2>'")]
	[Header("will be replaced by the actual buttons")]
	[Header("according to the index that you specify in 'buttonNumber'")]
	[Header("This requires the custom input manager by daemon 3000 'TeamUtility'")]
	[Space(10)]
	[SerializeField] private string message = "";
	[SerializeField] private GUIStyle textStyling;
	[SerializeField] private style textPlacement;
	[SerializeField] private textBoxStyle boxProperties;
	[SerializeField] private bool fadeInText = true;
	[SerializeField] private bool showOnce = true;
	[SerializeField] private bool disableOnTriggerExit = false;
	[SerializeField] private float showTextForSeconds = 3.0f;
	[SerializeField] private int buttonNumber1 = 10;
	[SerializeField] private int buttonNumber2 = 10;
	[SerializeField] private int altButtonNumber1 = 10;
	[SerializeField] private int altButtonNumber2 = 10;

	private Color newColor;

    private GUIManager guiManager = null;

	void Start() {
        guiManager = dontDestroy.currentGameManager.GetComponent<GUIManager>();
        newColor = guiManager.ReturnPopUpText().color;
	}
	void OnTriggerEnter(Collider enteringObject) {
		if(shown == false && enteringObject.tag == "Player") {
			string button1 = InputManager.GetInputConfiguration (PlayerID.One).axes [buttonNumber1].positive.ToString();
			string button2 = InputManager.GetInputConfiguration(PlayerID.One).axes[buttonNumber2].positive.ToString();
			string button3 = InputManager.GetInputConfiguration(PlayerID.One).axes[altButtonNumber1].altPositive.ToString();
			string button4 = InputManager.GetInputConfiguration(PlayerID.One).axes[altButtonNumber2].altPositive.ToString();
			message = message.Replace ("<BUTTON1>", button1);
			message = message.Replace ("<BUTTON2>", button2);
			message = message.Replace ("<ALTBUTTON1>", button3);
			message = message.Replace ("<ALTBUTTON2>", button4);
            guiManager.SetPopUpText(message);

			if (fadeInText) {
				fadeUp = true;
				guiAlpha = 0.0f;
				timer = 0.0f;
			} else {
				guiAlpha = 1.0f;
				timer = 0.0f;
				fadeDown = true;
			}
			beginFade = true;
			if (showOnce == true) {
				shown = true;
			}
		}		
	}
	void OnTriggerExit(Collider col) {
		if (col.tag == "Player" && disableOnTriggerExit == true) {
            guiManager.SetPopUpText("");
		}
	}
	void Update() {
		if (beginFade) {
			if (fadeUp) {
				guiAlpha += Time.deltaTime; 
				newColor.a = guiAlpha;
                guiManager.ReturnPopUpText().color = newColor;
				if (guiAlpha >= 1.0f) {
					fadeDown = true;
					fadeUp = false;
					guiAlpha = 1.0f;
				}
			}
			if (fadeDown) {
				timer += Time.deltaTime;
				if (timer > showTextForSeconds) {
					guiAlpha -= Time.deltaTime;
					newColor.a = guiAlpha;
                    guiManager.ReturnPopUpText().color = newColor;
					if (guiAlpha <= 0.0f) {
						fadeDown = false;
						beginFade = false;
					}
				}
			}
		}
	}
}
