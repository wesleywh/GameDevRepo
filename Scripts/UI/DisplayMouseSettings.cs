using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayMouseSettings : MonoBehaviour {
	enum MouseOptions {
		MouseX, MouseY
	}
	[SerializeField] private MouseOptions option = MouseOptions.MouseX;
	[SerializeField] private GameObject UISlider;

	void Start() {
		if (UISlider != null) {
			if (option == MouseOptions.MouseX) {
				float mouse = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MouseSettings> ().mouseSpeedX;
				UISlider.GetComponent<Slider> ().value = (mouse / 30);
			} else if (option == MouseOptions.MouseY) {
				float mouse = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MouseSettings> ().mouseSpeedY;
				UISlider.GetComponent<Slider>().value = (mouse / 30);
			}
		}
	}
	void Update() {
		if (UISlider == null) {
			if (option == MouseOptions.MouseX) {
				this.GetComponent<Text> ().text = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MouseSettings> ().mouseSpeedX.ToString ();
			} else if (option == MouseOptions.MouseY) {
				this.GetComponent<Text> ().text = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MouseSettings> ().mouseSpeedY.ToString ();
			}
		}
	}
}
