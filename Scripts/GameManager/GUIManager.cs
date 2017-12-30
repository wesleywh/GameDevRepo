using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using Pandora.Cameras;
using UnityEngine.UI;
using Pandora.Controllers;
using Pandora.Weapons;

namespace Pandora.GameManager {
    #region Helper Classes
    [System.Serializable]
    public class LoadingScreen {
        public float fadeOutTime = 0.5f;
        public GameObject parent = null;
        public RawImage background = null;
        public Text title = null;
        public Text description = null;
        public GameObject loadingBar = null;
        public GameObject[] others = null;
    }

    [System.Serializable]
    public class InteractiveElements {
        public Text popUpText = null;
        public Text objectives = null;
        public GUIScroll scroll = null;
    }

    [System.Serializable]
    public class GUIScroll {
        public Image scrollBackground = null;
        public Text scrollText = null;
        public Text closeHint = null;
        public Text closeButton = null;
    }

    [System.Serializable]
    public class GUIInventory {
        public GameObject parent = null;
        public GameObject mainWindow = null;
        public GameObject quickSlots = null;
    }

    [System.Serializable]
    public class GUIMenu {
        public GameObject parent = null;
        public GameObject mainMenu = null;
        public GameObject picture = null;
        public GameObject[] otherMenus = null;
    }
    #endregion
    [RequireComponent(typeof(Pandora.GameManager.PlayerManager))]
    public class GUIManager : MonoBehaviour {
        #region Variables
        [Header("General Settings")]
        [Range(0,1)]
        [SerializeField] private float inventoryTimeScale = 0.1f;
        [Range(0,1)]
        [SerializeField] private float menuTimeScale = 0.0f;
        public GUIInventory inventory = null;
        public GUIMenu menu;
        public GameObject[] cutsceneDisable;

        [Header("Individual Elements Control")]
        public InventoryQuickSlot[] qs = null;
        public LoadingScreen loadingScreen;
        public InteractiveElements interactiveElements;
        public GameObject objectives = null;
        [Space(10)]
        [Header("Debugging Purposes Only")]
        [SerializeField] private bool inventoryOpen = false;
        [SerializeField] private bool menuOpen = true;
        [SerializeField] private bool isCutscene = false;

        #region Fading Of Loading Screen
        private Color fadeColor;
        private bool lsFadeOut = false;
        #endregion

        private PlayerManager playerManager = null;
        private bool menuEnabled = true;
        #endregion

        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            EnableLoadingScreen(false);
            EnableMenu(true, false);
        }
            
        void OnLevelWasLoaded(int level)
        {
            EnableLoadingScreen(false);
        }

        void Update() {
            if (InputManager.GetButtonDown("OpenMenu") && menuEnabled == true)
            {
                EnableIventory(false);
                EnableWeaponControllers(menuOpen);
                EnableMenu(!menuOpen);
            }
            if (menuOpen == false && InputManager.GetButtonDown("Inventory"))
            {
                EnableIventory(!inventoryOpen);
                bool action = (inventoryOpen == true) ? false : true;
                EnableWeaponControllers(action);
            }
            if (lsFadeOut == true)
            {
                PerformFade();
            }
        }

        #region Various Enables/Disables
        public void EnableMenu(bool state)                  //This is so I can call it via UnityEvent on "NewGame" button press
        {
            EnableMenu(state, true);
        }
        public void EnableMenu(bool state, bool stopTime = true)
        {
            if (isCutscene == true)
                return;
            menuOpen = state;
            foreach (GameObject obj in menu.otherMenus)
            {
                obj.SetActive(false);
            }
            menu.mainMenu.SetActive(state);
            menu.picture.SetActive(state);
            menu.parent.SetActive(state);
            LockMouse(!state);
            if (playerManager == null)
                playerManager = GetComponent<PlayerManager>();
            playerManager.SetPlayerControllable(!state);
            EnableQuickSlots(!state);
            EnableIventory(false);
            Time.timeScale = (menuOpen == true && stopTime == true) ? menuTimeScale : 1;
        }
        public void EnableIventory(bool state)
        {
            if (menuOpen == true || isCutscene == true)
                return;
            inventoryOpen = state;                       //Used to control on/off of inventory and weapon controllers - very important!
            inventory.mainWindow.SetActive(state);       //Enable selected gameobjects when inventory is open
            LockMouse(!state);                           //Allow to see and move mouse while inventory is open
            playerManager.SetPlayerControllable(!state); //Don't allow the player to move around while inventory is open
            Time.timeScale = (inventoryOpen == true) ? inventoryTimeScale : 1;
        }
        public void EnableQuickSlots(bool state)
        {
            inventory.quickSlots.SetActive(state);
        }
        public void EnableCutscene(bool state)
        {
            isCutscene = state;
            foreach (GameObject item in cutsceneDisable)
            {
                item.SetActive(state);
            }
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
        public void LockMouse(bool state)
        {
            Cursor.lockState = (state == true) ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !state;
        }
        #endregion

        #region Interactive Elements 
        public void SetObjectivesText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                objectives.SetActive(false);
            }
            else
            {
                objectives.GetComponent<Text>().text = text;
                objectives.SetActive(true);
            }
        }
        public void SetPopUpText(string text)
        {
            Color msgColor = interactiveElements.popUpText.color;
            interactiveElements.popUpText.text = text;
            msgColor.a = 1;
            interactiveElements.popUpText.color = msgColor;
        }
        public Text ReturnPopUpText(){
            return interactiveElements.popUpText;
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
        public GameObject ReturnQuickSlots()
        {
            return inventory.quickSlots;
        }
        #endregion

        #region Loading Screen
        public void EnableLoadingScreen(bool state, string title="", string description="", Texture2D background = null)
        {
            loadingScreen.loadingBar.GetComponent<Slider>().value = 0;
            if (state == false && loadingScreen.fadeOutTime > 0.0f)
            {
                lsFadeOut = true;
            }
            else if (loadingScreen.background)
            {
                loadingScreen.parent.gameObject.SetActive(state);
                loadingScreen.background.texture = (state == false) ? loadingScreen.background.texture : background;
                loadingScreen.background.enabled = state;
            }

            LockMouse(true);
            if (state == false)
            {
                EnableMenu(false);
            }
            EnableIventory(false);
            EnableQuickSlots(!state);
            playerManager.EnableCameraControl(!state);

            if (loadingScreen.title)
            {
                loadingScreen.title.text = (state == false) ? loadingScreen.title.text : title;
                loadingScreen.title.enabled = state;
            }
            if (loadingScreen.description)
            {
                loadingScreen.description.text = (state == false) ? loadingScreen.description.text : description;
                loadingScreen.description.enabled = state;
            }
            if (loadingScreen.loadingBar)
            {
                loadingScreen.loadingBar.SetActive(state);
            }
            if (loadingScreen.others.Length > 0)
            {
                foreach (GameObject image in loadingScreen.others)
                {
                    image.SetActive(state);
                }
            }
        }
        public void SetLoadingScreenBackground(Texture newTexture)
        {
            StartCoroutine(UpdateTexture(newTexture));
        }
        public void SetLoadingBarValue(float value)
        {
            loadingScreen.loadingBar.GetComponent<Slider>().value = value;
        }
        IEnumerator UpdateTexture(Texture newTexture)
        {
            yield return new WaitForSeconds(0.1f);
            while (lsFadeOut == true)
            {
                yield return new WaitForSeconds(0.1f);
            }
            loadingScreen.background.texture = newTexture;
        }
        void PerformFade()                                              //Fade the GUI Load screen background out slowly (No other elements effected)
        {
            fadeColor = loadingScreen.background.color;
            fadeColor.a -= loadingScreen.fadeOutTime * Time.deltaTime;
            loadingScreen.background.color = fadeColor;
            if (fadeColor.a <= 0)
            {
                loadingScreen.background.enabled = false;
                fadeColor.a = 1;
                loadingScreen.background.color = fadeColor;
                lsFadeOut = false;
            }
        }
        #endregion

        public bool MenuOrInventoryOpen()
        {
            if (menuOpen == true || inventoryOpen == true)
            {
                return true;
            }
            return false;
        }
        public bool MenuOpen()
        {
            return menuOpen;
        }
    }
}