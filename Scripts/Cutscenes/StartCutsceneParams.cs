using UnityEngine;
using System.Collections;

public class StartCutsceneParams : MonoBehaviour {
	[SerializeField] private GameObject player;
	[SerializeField] private string varToCheck = null;
	[SerializeField] private bool varToCheckState = false;

	// Use this for initialization
	void Start () {
		GameObject manager = GameObject.FindGameObjectWithTag ("GameManager");
		bool setValue = (bool)manager.GetComponent<AreaManager> ().GetType ().GetField (varToCheck).GetValue (manager.GetComponent<AreaManager> ());
		if (setValue != varToCheckState) {
			player.SetActive (true);
			this.gameObject.SetActive (false);
		}
	}
}
