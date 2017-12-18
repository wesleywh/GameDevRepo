using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using Pandora.Cameras;
using UnityEngine.UI;
using Pandora.Controllers;
using Pandora.Weapons;

namespace Pandora.GameManager {
    public class GUIManager : MonoBehaviour {
        [Range(0,1)]
        [SerializeField] private float inventoryTimeScale = 0.1f;
        [Range(0,1)]
        [SerializeField] private float menuTimeScale = 0.0f;
        public GameObject[] inventoryWindows = null;
        public GameObject[] enableOnMenu;
        public GameObject[] disableOnMenu;
        public Text messages = null;
        public InventoryQuickSlot[] qs = null;
        [Space(10)]
        [Header("Debugging Purposes Only")]
        [SerializeField] private bool inventoryOpen = false;
        [SerializeField] private bool menuOpen = true;

        void OnLevelWasLoaded() {
            EnableIventory(false);
        }

        void Update() {
            if (InputManager.GetButtonDown("OpenMenu"))
            {
                EnableIventory(false);
//                bool action = (menuOpen == true) ? true : false;
                EnableWeaponControllers(menuOpen);
                EnableMenu(!menuOpen);
            }
            if (menuOpen == false && InputManager.GetButtonDown("Inventory"))
            {
                EnableIventory(!inventoryOpen);
                bool action = (inventoryOpen == true) ? false : true;
                EnableWeaponControllers(action);
            }
        }
        public void EnableMenu(bool state)
        {
            menuOpen = state;
            foreach (GameObject obj in enableOnMenu)
            {
                obj.SetActive(state);
            }
            foreach(GameObject obj in disableOnMenu)
            {
                obj.SetActive(!state);
            }
            LockMouse(!state);
            EnableCameraControl(!state);
            if (menuOpen == true)
                Time.timeScale = menuTimeScale;
            else
                Time.timeScale = 1;
        }
        public void EnableIventory(bool state)
        {
            inventoryOpen = state;
            foreach (GameObject window in inventoryWindows)
                window.SetActive(inventoryOpen);
            EnableCameraControl(!inventoryOpen);
            LockMouse(!inventoryOpen);
            if (inventoryOpen == true)
                Time.timeScale = inventoryTimeScale;
            else
                Time.timeScale = 1;
        }
        public void EnableCameraControl(bool state)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player.GetComponent<MouseLook>())
                player.GetComponent<MouseLook>().enable = state;
            foreach (MouseLook ml in player.GetComponentsInChildren<MouseLook>())
            {
                ml.enabled = state;
            }
        }
        public void LockMouse(bool state)
        {
            Cursor.lockState = (state == true) ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !state;
        }
        public void DisplayString(string text)
        {
            Color msgColor = messages.color;
            msgColor.a = 1;
            messages.color = msgColor;
            messages.text = text;
        }
        public void EnableWeaponControllers(bool state)
        {
            Transform wm = GameObject.FindGameObjectWithTag("WeaponManager").transform;
            if (wm)
            {
                wm.GetComponent<InvWeaponManager>().enabled = state;
                foreach (Transform child in wm)
                {
                    if (child.gameObject.activeInHierarchy)
                    {
                        if (child.GetComponent<WeaponNew>())
                        {
                            child.GetComponent<WeaponNew>().EnableWeaponUse(state);
                        }
                    }
                }
            }
        }
        public void RemoveQuickSlotItem(int slot)
        {
            for (int i =0; i < qs.Length; i++)
            {
                if (qs[i].slot == slot)
                {
                    qs[i].RemoveFromSlot();
                    break;
                }
            }
        }
    }
}