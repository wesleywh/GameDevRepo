using UnityEngine;
using System.Collections;
using TeamUtility.IO;							//Custom Input Manager

public class HeadBobber : MonoBehaviour {

	private float timer = 0.0f;
	float bobbingSpeed = 0.36f;
	[SerializeField] float walkBobbingSpeed = 0.18f;
	[SerializeField] float runBobbingSpeed = 0.18f;
	[SerializeField] float bobbingAmount = 0.2f;
	[SerializeField] float midpoint = 2.0f;

	void Update () {
		float waveslice = 0.0f;
		float horizontal = InputManager.GetAxis("Horizontal");
		float vertical = InputManager.GetAxis("Vertical");
		bool running = InputManager.GetButton ("Run");
		if (running) {
			bobbingSpeed = runBobbingSpeed;
		} else {
			bobbingSpeed = walkBobbingSpeed;
		}
		Vector3 cSharpConversion = transform.localPosition; 

		if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) {
			timer = 0.0f;
		}
		else {
			waveslice = Mathf.Sin(timer);
			timer = timer + bobbingSpeed;
			if (timer > Mathf.PI * 2) {
				timer = timer - (Mathf.PI * 2);
			}
		}
		if (waveslice != 0) {
			float translateChange = waveslice * bobbingAmount;
			float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
			translateChange = totalAxes * translateChange;
			cSharpConversion.y = midpoint + translateChange;
		}
		else {
			cSharpConversion.y = midpoint;
		}

		transform.localPosition = cSharpConversion;
	}
}
