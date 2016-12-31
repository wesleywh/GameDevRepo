using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour {

	public bool playerDead = false;
	[SerializeField] private GameObject parentObject;
	[SerializeField] private RawImage background;
	[SerializeField] private Text title;
	[SerializeField] private Text loadSave;
	[SerializeField] private Text reloadLevel;
	[SerializeField] private Text quit;
	[SerializeField] private float guiFadeSpeed = 4.0f;

	private float backgroundAlpha = 0.0f;
	private float titleAlpha = 0.0f;
	private float loadSaveAlpha = 0.0f;
	private float quitAlpha = 0.0f;
	private float reloadLevelAlpha = 0.0f;
	private bool previous = false;

	private Color backgroundColor;
	private Color titleColor;
	private Color loadSaveColor;
	private Color quitColor;
	private Color reloadLevelColor;

	private bool isSet = false;

	void Start() {
		previous = playerDead;
		parentObject.SetActive (playerDead);
		backgroundColor = background.color;
		titleColor = title.color;
		loadSaveColor = loadSave.color;
		quitColor = quit.color;
		reloadLevelColor = reloadLevel.color;
	}

	// Update is called once per frame
	void Update () {
		if (previous != playerDead) {
			previous = playerDead;
			parentObject.SetActive (playerDead);
		}
		if (playerDead == true) {
			isSet = false;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			//calculate new alphas
			backgroundAlpha += (backgroundAlpha < 1) ? Time.deltaTime / guiFadeSpeed : 0;
			if (backgroundAlpha > 0.5f) {
				titleAlpha += (titleAlpha < 1) ? Time.deltaTime / guiFadeSpeed : 0;
			}
			if (titleAlpha > 0.5f) {
				loadSaveAlpha += (loadSaveAlpha < 1) ? Time.deltaTime / guiFadeSpeed : 0;
			}
			if (loadSaveAlpha > 0.5f) {
				reloadLevelAlpha += (reloadLevelAlpha < 1) ? Time.deltaTime / guiFadeSpeed : 0;
			}
			if (reloadLevelAlpha > 0.5f) {
				quitAlpha += (quitAlpha < 1) ? Time.deltaTime / guiFadeSpeed : 0;
			}
			//save the correct color
			backgroundColor.a = backgroundAlpha;
			titleColor.a = titleAlpha;
			loadSaveColor.a = loadSaveAlpha;
			reloadLevelColor.a = reloadLevelAlpha;
			quitColor.a = quitAlpha;
			//assign that color
			background.color = backgroundColor;
			title.color = titleColor;
			loadSave.color = loadSaveColor;
			reloadLevel.color = reloadLevelColor;
			quit.color = quitColor;
		} 
		else if(isSet == false)
		{
			isSet = true;
			//reset colors
			backgroundAlpha = 0;
			titleAlpha = 0;
			loadSaveAlpha = 0;
			reloadLevelAlpha = 0;
			quitAlpha = 0;
			//save resets
			backgroundColor.a = backgroundAlpha;
			titleColor.a = titleAlpha;
			loadSaveColor.a = loadSaveAlpha;
			reloadLevelColor.a = reloadLevelAlpha;
			quitColor.a = quitAlpha;
			//assign resets
			background.color = backgroundColor;
			title.color = titleColor;
			loadSave.color = loadSaveColor;
			reloadLevel.color = reloadLevelColor;
			quit.color = quitColor;
			parentObject.SetActive (false);
		}
	}
}
