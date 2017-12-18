using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour {
    public bool gameManager = true;
    [Header("Debugging")]
    public bool disable = false;
    public string travelToNamedObject = "";
	// Use this for initialization
	void Start () {
        if (gameManager == false && disable == false)
        {
            string targetName = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TransitionManager>().travelToNamedObject;
            if (!string.IsNullOrEmpty(targetName))
            {
                GameObject target = GameObject.Find(targetName);
                this.transform.position = target.transform.position;
                this.transform.rotation = target.transform.rotation;
            }
        }
	}

}
