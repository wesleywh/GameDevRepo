using UnityEngine;
using System.Collections;

public class GUIControl : MonoBehaviour {
	[Header("----Weapons Ammo GUI----")]
	public GameObject AmmoGUI = null;
	public GameObject AmmoClips = null;
	public GameObject AmmoBulletsLeft = null;

    [Header("Debug")]
    [SerializeField] bool lockMouseToCenter = false;
    bool prevState = false;
//	[Space(10)]
//	[Header("----Weapons Selections----")]

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (prevState != lockMouseToCenter)
        {
            prevState = lockMouseToCenter;
            if (lockMouseToCenter == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
	}
}
