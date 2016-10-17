using UnityEngine;
using System.Collections;

public class MouseSettings : MonoBehaviour {
	public float mouseSpeedY = 3.0f;
	public float mouseSpeedX = 3.0f;

	void Start() {
		mouseSpeedY = 3.0f;
		mouseSpeedX = 3.0f;
	}
	public void AdjustMouseY(float value) {
		mouseSpeedY = value;
	}
	public void AdjustMouseX(float value) {
		mouseSpeedX = value;
	}

	public void UIAdjustMouseY(float value) {
		mouseSpeedY = Mathf.Round(20 * value);
	}
	public void UIAdjustMouseX(float value) {
		mouseSpeedX = Mathf.Round(20 * value);
	}
}
