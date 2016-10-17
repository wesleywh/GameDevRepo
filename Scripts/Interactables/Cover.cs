using UnityEngine;
using System.Collections;
using System.Collections.Generic;			//for adding to arrays

public class Cover : MonoBehaviour {
	[Header("---- Custome AI Tag Settings----")]
	[SerializeField] private string playerTag = "Player";			//Tag to ID Players
	[SerializeField] private string goodTag = "Good";				//Tag to ID good AI
	[SerializeField] private string evilTag = "Evil";				//Tag to ID evil AI
	[SerializeField] private string neutralTag = "Neutral";			//Tag to ID neutral AI
	[SerializeField] private string zombieTag = "Zombie";			//Tag to ID zombie AI
	[Space(10)]

	[SerializeField] private string coverTakenTag = "CoverTaken";//Tag if this cover is taken by something else
	private string coverOpenTag = "Cover";		//Tag if this is usable cover
	private List<GameObject> NPCsAndPlayers = new List<GameObject>();
	private bool coverTaken = false;
	private float timer = 0.0f;

	void Start(){
		if (this.GetComponent<MeshRenderer> ()) {
			this.GetComponent<MeshRenderer> ().enabled = false;
		}
	}
	// Update is called once per frame
	void Update () {
		if (coverTaken == true) {
			this.gameObject.tag = coverTakenTag;
			timer += Time.deltaTime;
			if (timer >= 2.0f) {
				this.gameObject.tag = coverOpenTag;
				timer = 0;
				coverTaken = false;
			}
		} 
		else {
			foreach (GameObject player in NPCsAndPlayers) {
				if(Vector3.Distance(player.transform.position, this.transform.position) <= 2.5f) {
					coverTaken = true;
					break;
				}
			}
		}
	}
	void FixedUpdate(){
		FindAllAIAndPlayers ();
	}
	void FindAllAIAndPlayers(){
		NPCsAndPlayers.AddRange(GameObject.FindGameObjectsWithTag (playerTag));
		NPCsAndPlayers.AddRange(GameObject.FindGameObjectsWithTag (goodTag));
		NPCsAndPlayers.AddRange(GameObject.FindGameObjectsWithTag (evilTag));
		NPCsAndPlayers.AddRange(GameObject.FindGameObjectsWithTag (neutralTag));
		NPCsAndPlayers.AddRange(GameObject.FindGameObjectsWithTag (zombieTag));
	}
}
