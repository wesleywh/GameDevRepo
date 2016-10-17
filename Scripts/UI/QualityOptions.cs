using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QualityOptions : MonoBehaviour {
	[SerializeField] private GameObject AAFilteringButton;
	[SerializeField] private GameObject[] qualityButtons;
	[SerializeField] private GameObject currentQualitySetting;
	private bool settingQuality = false;
	[SerializeField] private GameObject errorDialog;
	[SerializeField] private GameObject[] onErrorDisable;
	private bool originalAASetting;
	private string originalQuality;

	void Start() {
		originalAASetting = AAFilteringButton.GetComponent<Toggle> ().isOn;
		originalQuality = currentQualitySetting.GetComponent<Text> ().text;
	}
	public void ApplySettings() {
		string[] names = QualitySettings.names;
		bool setQuality = false;
		for (int i=0; i<names.Length; i++) {
			if (names[i].ToLower () == currentQualitySetting.GetComponent<Text> ().text.ToLower ()) {
				QualitySettings.SetQualityLevel (i, AAFilteringButton.GetComponent<Toggle>().isOn);
				setQuality = true;
				settingQuality = false;
			}
			if (setQuality) {
				break;
			}
		}
	}
	public void SetQuality(GameObject textObject) {
		originalQuality = currentQualitySetting.GetComponent<Text> ().text;
		currentQualitySetting.GetComponent<Text>().text = textObject.GetComponent<Text> ().text;
		settingQuality = true;
	}
	public void ChangedAAFiltering() {
		originalAASetting = !AAFilteringButton.GetComponent<Toggle> ().isOn;
		settingQuality = true;
	}
	public void DiscardQualitySettings() {
		AAFilteringButton.GetComponent<Toggle> ().isOn = originalAASetting;
		currentQualitySetting.GetComponent<Text> ().text = originalQuality;
		settingQuality = false;
	}
	public void IsSettingQuality() {
		if (settingQuality == true) {
			errorDialog.SetActive (true);
			foreach(GameObject obj in onErrorDisable) {
				obj.SetActive (false);
			}
		}
	}
}
