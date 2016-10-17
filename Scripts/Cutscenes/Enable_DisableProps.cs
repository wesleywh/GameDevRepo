using UnityEngine;
using System.Collections;

public class Enable_DisableProps : MonoBehaviour {
	[SerializeField] private GameObject[] enable;
	[SerializeField] private GameObject[] disable;
	[SerializeField] private GameObject[] destroy;

	public void ExecuteEnableDisable() {
		foreach (GameObject toEnable in enable) {
			toEnable.SetActive (true);
		}
		foreach (GameObject toDisable in disable) {
			toDisable.SetActive (false);
		}
		foreach (GameObject toDestory in destroy) {
			Destroy (toDestory);
		}
	}
}
