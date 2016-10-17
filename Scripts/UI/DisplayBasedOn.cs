using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayBasedOn : MonoBehaviour {
	[SerializeField] private GameObject basedOnObject;
	[SerializeField] private bool mimicFillAmount = false;
	[SerializeField] private bool mimicAlpha = false;
	private Color setColor;

	// Update is called once per frame
	void Update () {
		if (mimicAlpha == true) {
			this.GetComponent<Text> ().color = basedOnObject.GetComponent<Text> ().color;
		} else if (mimicFillAmount == true) {
			this.GetComponent<Image> ().fillAmount = basedOnObject.GetComponent<Image> ().fillAmount;
		}
	}
}
