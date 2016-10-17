using UnityEngine;
using System.Collections;
using TeamUtility.IO;

public class OpenMenu : MonoBehaviour {
	[SerializeField] private GameObject menu;
	[SerializeField] private GameObject healthBar;
	[SerializeField] private GameObject inventory;
	[SerializeField] private bool menuOpen = false;

	public bool IsMenuOpen() {
		return menuOpen;
	}
	void Update () {
		if (InputManager.GetButtonUp ("OpenMenu") && menuOpen == false) {
			MenuOpen ();
			return;
		}
		if (InputManager.GetButtonUp ("OpenMenu") && menuOpen == true) {
			MenuClose ();
		}
	}
	public void MenuOpen() {
		MouseLook[] mouseLooks = GameObject.FindObjectsOfType (typeof(MouseLook)) as MouseLook[];
		foreach (MouseLook mouseLook in mouseLooks) {
			mouseLook.enabled = false;
		}
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Time.timeScale = 0;
		menu.SetActive (true);
		healthBar.SetActive (false);
		inventory.SetActive (false);
		menuOpen = true;
	}
	public void MenuClose(){
		MouseLook[] mouseLooks = GameObject.FindObjectsOfType (typeof(MouseLook)) as MouseLook[];
		foreach (MouseLook mouseLook in mouseLooks) {
			mouseLook.enabled = true;
		}
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 1;
		menu.SetActive (false);
		healthBar.SetActive (true);
		inventory.SetActive (true);
		menuOpen = false;
	}
}
