using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CyberBullet.GameManager;

public class DisplayMouseSettings : MonoBehaviour {
	enum MouseOptions {
		MouseX, MouseY
	}
	[SerializeField] private MouseOptions option = MouseOptions.MouseX;
	[SerializeField] private GameObject UISlider;
    [SerializeField] private MouseSettings mouseSettings;

	void Start() {
        if (mouseSettings == null)
        {
            mouseSettings = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CyberBullet.GameManager.MouseSettings>();
        }
	}
	void Update() {
		if (option == MouseOptions.MouseX) {
            this.GetComponent<Text> ().text = mouseSettings.mouseSpeedX.ToString ();
		} else if (option == MouseOptions.MouseY) {
            this.GetComponent<Text> ().text = mouseSettings.mouseSpeedY.ToString ();
		}
	}
}
