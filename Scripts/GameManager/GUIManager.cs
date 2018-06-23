/// <summary>
/// Used to control anything gui related for the player.
/// This controls things like the inventory screen, cutscene look, pause menu, start screen, etc.
/// You access and minipulate these objects via this script. This script should know everything.
﻿/// </summary>

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using CyberBullet.Cameras;
using UnityEngine.UI;
using CyberBullet.Controllers;
using CyberBullet.Weapons;

namespace CyberBullet.GameManager {
    #region Helper Classes
    [System.Serializable]
    public class GUIAmmo {
        public Image bullet;                                                    #GUIAmmo option, UI Image for loaded bullets.
        public Image clip;                                                      #GUIAmmo option, UI Image for reload bullets.
        public Text clip_number;                                                #GUIAmmo option, UI Text to show number of bullets that can be reloaded.
        public Text bullets_left;                                               #GUIAmmo option, UI Text to show number of bullets remaining in selected weapon.
        public Image background;                                                #GUIAmmo option, (Optional) UI Image to show under all gui ammo elements.
    }

    [System.Serializable]
    public class LoadingScreen {
        public float fadeOutTime = 0.5f;                                        #LoadingScreen option, How long it will take for the loading screen to disappear. (Fade out)
        public GameObject parent = null;                                        #LoadingScreen option, for traversal/master control of loading screen elements.
        public RawImage background = null;                                      #LoadingScreen option, Backgroun image to show while loading.
        public Text title = null;                                               #LoadingScreen option, Title text for loading.
        public Text description = null;                                         #LoadingScreen option, Description text for loading.
        public GameObject loadingBar = null;                                    #LoadingScreen option, gameobject holding scrollbar to indicate how far along the loading process is.
        public GameObject[] others = null;                                      #LoadingScreen option, gameobjects that are only involved with the loading screen. Enables/Disables with loading screen.
    }

    [System.Serializable]
    public class InteractiveElements {
        public Text popUpText = null;                                           #InteractiveElements option, UI Text for interactable items.
        public Text objectives = null;                                          #InteractiveElements option, UI Text to show when pressing the objectives button.
        public GUIScroll scroll = null;                                         #InteractiveElements option, GUIScroll class reference.
        public Text dialog = null;                                              #InteractiveElements option, UI Text for showing dialog.
    }

    [System.Serializable]
    public class GUIScroll {
        public Image scrollBackground = null;                                   #GUIScroll option, UI Image to display underneath the UI Text.
        public Text scrollText = null;                                          #GUIScroll option, UI Text to display for the player to read.
        public Text closeHint = null;                                           #GUIScroll option, UI Text to display to notify the player how to close this screen.
        public Text closeButton = null;                                         #(LEGACY) GUIScroll option, UI text object to display close text, for screens.
    }

    [System.Serializable]
    public class GUIInventory {
        public GameObject parent = null;                                        #GUIInventory option, for traversal/master control
        public GameObject mainWindow = null;                                    #GUIInventory option, gameobject to enable when inventory button is pressed. For inventory screen.
        public GameObject quickSlots = null;                                    #GUIInventory option, gameobject to enable when inventory button is pressed. For quickslots.
    }

    [System.Serializable]
    public class GUIMenu {
        public GameObject parent = null;                                        #GUIMenu option, parent object holding everything in GUIMenu options, for master control/traversal.
        public GameObject mainMenu = null;                                      #GUIMenu option, menu to enable when opening your menu screen.
        public GameObject newGameBtn = null;                                    #GUIMenu option, gameobject holding UI button that is designed to trigger a new game
        public GameObject saveGameBtn = null;                                   #GUIMenu option, gameobject holding UI button that is designed to trigger saving your game
        public GameObject picture = null;                                       #GUIMenu option, a large picture to show when the menu is active
        public GameObject[] otherMenus = null;                                  #GUIMenu option, other menus to disable when menu is disabled
        public GameObject boss = null;                                          #GUIMenu option, npc to track Health script for calculate scrollbar(healthbar) bar size
        public GameObject bossHealthBar = null;                                 #GUIMenu option, parent gameobject for both scrollbars
        public GameObject bossHealthRedBar = null;                              #GUIMenu option, gameobject holding scrollbar for top health bar color
        public GameObject bossHealthYellowBar = null;                           #GUIMenu option, gameobject holding scrollbar for underneath health bar color.
    }

    [System.Serializable]
    public class GUILoadGameSlots {
        public GameObject parent = null;                                        #GUILoadGameSlots option, for traversal (legacy?)
        public Text title = null;                                               #GUILoadGameSlots option, location of this save.
        public Text description = null;                                         #GUILoadGameSlots option, a few character stats to help identify this save.
        public Text saveTime = null;                                            #GUILoadGameSlots option, the computer time when this game was saved.
        public Image saveImage = null;                                          #GUILoadGameSlots option, this is the snapshot when the game was saved.
    }
    #endregion
    [RequireComponent(typeof(CyberBullet.GameManager.PlayerManager))]
    public class GUIManager : MonoBehaviour {
        #region Variables
        [Header("General Settings")]
        [Range(0,1)]
        [SerializeField] private float inventoryTimeScale = 0.1f;               #Between 0 - 1. How slow everything else will move while the inventory is open.
        [Range(0,1)]
        [SerializeField] private float menuTimeScale = 0.0f;                    #Between 0 - 1. How slow everything else will move while the menu is open. (0 = don't move at all)
        public GUIInventory inventory = null;                                   #List of GUIInventory class items.
        public GUIMenu menu;                                                    #List GUIMenu class items.
        public GameObject[] cutsceneDisable;                                    #What GUI objects to disable while in "cutscene" mode.
        public GUIAmmo gui_ammo;                                                #GUIAmmo class selection items.

        [Header("Individual Elements Control")]
        public InventoryQuickSlot[] qs = null;                                  #Outside script reference, for manipulating gui quick slots on the screen.
        public LoadingScreen loadingScreen;                                     #LoadingScreen class reference, additional options for the loading screen.
        public InteractiveElements interactiveElements;                         #InteractiveElements class reference, options for interactable gui elements.
        public GameObject objectives = null;                                    #Object that holds the objective text, used for showing/hiding objectives on gui.
        public GUILoadGameSlots[] loadGameSlots;                                #GUILoadGameSlots class reference, GUI buttons that will load saved games.
        public GameObject saveGameScreen = null;                                #gameobject holding UI Image component. Object to enable when saving your game.
        public GUILoadGameSlots[] saveGameSlots;                                #GUILoadGameSlots class reference, GUI buttons that will save games.
        public GameObject saveWarningScreen = null;                             #object to enable when trying to save a game to a slot that already exists.
        [Space(10)]
        [Header("Debugging Purposes Only")]
        [SerializeField] private bool inventoryOpen = false;                    #Debugging purposes only, enable inventory gui
        [SerializeField] private bool menuOpen = true;                          #Debugging purposes only, enable menu gui
        [SerializeField] private bool isCutscene = false;                       #Debugging purposes only, enable cutscene gui (This also disables others)

        #region Fading Of Loading Screen
        private Color fadeColor;
        private bool lsFadeOut = false;
        #endregion

        private PlayerManager playerManager = null;
        private bool menuEnabled = true;
        private float fillAmount = 0.0f;
        private bool setYellowBar = false;
        private bool bar_waiting = false;
        private bool can_quit_menu = false;
        #endregion

        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            EnableLoadingScreen(false);
            EnableMenu(true, false);
            EnableBossHealthBar(false);
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
            if (menu.boss != null)
            {
                WatchBossHealth();
            }
        }

        #region Various Enables/Disables
        public void EnableMenu(bool state)                  //This is so I can call it via UnityEvent on "NewGame" button press
        {
            EnableMenu(state, true);
        }
        public void EnableMenu(bool state, bool stopTime = true)
        {
            if (isCutscene == true || (can_quit_menu == false && state == false))
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
            if (state == true)
            {
                EnableMenu(false);
                EnableIventory(false);
            }
            isCutscene = state;
            foreach (GameObject item in cutsceneDisable)
            {
                item.SetActive(!state);
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
        public void EnableSaveWarning(bool state)
        {
            saveWarningScreen.SetActive(state);
            saveGameScreen.SetActive(!state);
        }
        public void EnableBossHealthBar(bool state)
        {
            menu.bossHealthBar.SetActive(state);
        }
        public void SetBoss(GameObject target)
        {
            if (target.transform.root.GetComponent<Health>())
            {
                float amount = target.transform.root.GetComponent<Health>().GetHealth() / target.transform.root.GetComponent<Health>().GetMaxHealth();
                menu.boss = target;
                menu.bossHealthRedBar.GetComponent<Image>().fillAmount = amount;
                menu.bossHealthYellowBar.GetComponent<Image>().fillAmount = amount;
            }
        }
        public void RemoveBoss()
        {
            EnableBossHealthBar(false);
            menu.boss = null;
        }
        public void SetMenuAllowQuit(bool state)
        {
            can_quit_menu = state;
        }
        #endregion

        #region Interactive Elements
        public void SetActivatedMenu(bool state)
        {
            menu.newGameBtn.SetActive(!state);
            menu.saveGameBtn.SetActive(state);
        }
        public void SetActiveInventory(bool state)
        {
            inventory.parent.SetActive(state);
        }
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
        public void SetDialogText(string text)
        {
            Color msgColor = interactiveElements.dialog.color;
            interactiveElements.dialog.text = text;
            msgColor.a = 1;
            interactiveElements.dialog.color = msgColor;
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
        public void PopulateLoadGameSlot(int slot)
        {
            playerManager.LoadSaveInfoOnly(slot,loadGameSlots[slot]);
        }
        public void PopulateSaveGameSlot(int slot)
        {
            playerManager.LoadSaveInfoOnly(slot,saveGameSlots[slot]);
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
            if (playerManager != null)
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

        #region HealthBar
        IEnumerator SyncYellowBar()
        {
            if (bar_waiting == false)
            {
                bar_waiting = true;
                yield return new WaitForSeconds(0.5f);
                setYellowBar = true;
            }
            yield return null;
        }
        private void WatchBossHealth()
        {
            fillAmount = menu.boss.transform.root.GetComponent<Health>().GetHealth() / menu.boss.transform.root.GetComponent<Health>().GetMaxHealth();
            menu.bossHealthRedBar.GetComponent<Image>().fillAmount = fillAmount;
            if (menu.bossHealthYellowBar.GetComponent<Image>().fillAmount != fillAmount)
            {
                StartCoroutine(SyncYellowBar());
            }
            if (setYellowBar == true)
            {
                menu.bossHealthYellowBar.GetComponent<Image>().fillAmount -= Time.deltaTime;
                if (menu.bossHealthYellowBar.GetComponent<Image>().fillAmount <= fillAmount)
                {
                    setYellowBar = false;
                    bar_waiting = false;
                    menu.bossHealthYellowBar.GetComponent<Image>().fillAmount = fillAmount;
                }
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
