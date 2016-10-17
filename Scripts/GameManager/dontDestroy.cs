using UnityEngine;
using System.Collections;

public class dontDestroy : MonoBehaviour {
	public static GameObject currentGameManager;
	public static GameObject currentGUI;

	[SerializeField] private bool isGameManager = false;
	// Use this for initialization
	void Awake () {
		if (isGameManager) {
			if (currentGameManager == null) {
				DontDestroyOnLoad (this.gameObject);
				currentGameManager = this.gameObject;
			} else if (currentGameManager != this.gameObject) {
				Destroy (this.gameObject);
			}
		} else {
			if (currentGUI == null) {
				DontDestroyOnLoad (this.gameObject);
				currentGUI = this.gameObject;
			} else if (currentGUI != this.gameObject) {
				Destroy (this.gameObject);
			}
		}
	}
}
