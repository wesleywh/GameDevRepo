using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; 	//for list & dictionary
using CyberBullet.Controllers;
using CyberBullet.Cameras;
using CyberBullet.GameManager;
using TeamUtility.IO;
using System.Runtime.Serialization.Formatters.Binary;//allow writing in binary
using System.IO;                    //Allow Input & Output
using System;

namespace CyberBullet.GameManager {
    [System.Serializable]
    class PlayerData {
        public float health = 100.0f;
        public float regeneration = 0.0f;
        public float maxHealth = 100.0f;
        public InventoryList[] weapons = null;
        public InventoryList[] consumables = null;
        public string sceneName = "";
        public string travelPoint = "";
        public List<Objectives> objectives = new List<Objectives> ();
        public System.DateTime lastSave;
        public Dictionary<string,bool> areaManager = new Dictionary<string,bool>();
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(InventoryManagerNew))]
    [RequireComponent(typeof(ObjectiveManager))]
    [RequireComponent(typeof(TransitionManager))]
    [RequireComponent(typeof(GUIManager))]
    public class PlayerManager : MonoBehaviour {
    	private float playerHealth = 100.0f;
    	private float playerRegen = 0.0f;
        private float playerMaxHealth = 100.0f;
        private InventoryList[] weapons;
        private InventoryList[] consumables;
        [SerializeField] private AudioClip defaultTransitionAudio = null;
        [Range(0,1)]
        [SerializeField] private float audioVolume = 0.5f;
        [SerializeField] private Texture2D defaultLoadBackground = null;
        private int overrideSlot = 9999;
        private bool overwrite = false;

        private InventoryManagerNew inventoryManager = null;
        private GUIManager guiManager = null;

        void Start()
        {
            inventoryManager = GetComponent<InventoryManagerNew>();
            guiManager = GetComponent<GUIManager>();
        }

        #region Manipulate Player
        public void SetPlayerControllable(bool enabled)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.GetComponent<MovementController>().SetLockMovement(!enabled);
                player.GetComponent<MovementController>().SetLockJump(!enabled);
                EnableCameraControl(enabled);
            }
        }
        public void EnableCameraControl(bool state)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player.GetComponent<MouseLook>())
            {
                player.GetComponent<MouseLook>().enable = state;
            }
            foreach (MouseLook ml in player.GetComponentsInChildren<MouseLook>())
            {
                ml.enable = state;
            }
        }
        #endregion

        #region Save/Load Game Data
        public void OverwriteSave()
        {
            overwrite = true;
            SaveGame(overrideSlot);
        }
        public void SaveGame(int slot) {
            if (File.Exists(Application.persistentDataPath + "/gameData" + slot + ".dat") && overwrite == false)
            {
                overrideSlot = slot;
                guiManager.EnableSaveWarning(true);
                return;
            }
            overwrite = false;
            guiManager.EnableCutscene(true);
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/gameData" + slot + ".png");
            guiManager.EnableCutscene(false);
            GetComponent<GUIManager>().SetPopUpText("Saving Game...");
            BinaryFormatter bf = new BinaryFormatter ();
            Debug.Log ("saving to: " + Application.persistentDataPath+"/gameData"+slot+".dat");
            FileStream file = File.Open (Application.persistentDataPath + "/gameData"+slot+".dat", FileMode.OpenOrCreate);//make a file or open it if already made

            //fill an object to save to this file
            PlayerData data = new PlayerData ();
            Health health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            data.health = health.GetHealth();
            data.regeneration = health.GetRegeneration ();
            data.maxHealth = health.GetMaxHealth();
            data.weapons = inventoryManager.ReturnInventoryWeapons ();
            data.consumables = inventoryManager.ReturnInventoryConsumables();
            data.objectives = GetComponent<ObjectiveManager> ().ShowAllObjectives ();
            data.lastSave = System.DateTime.Now;
            data.areaManager = GetComponent<AreaManager>().ReturnAllValues();
            data.travelPoint = GetComponent<TransitionManager>().travelToNamedObject;
            data.sceneName = SceneManager.GetActiveScene().name;
            InputManager.Save (Application.persistentDataPath + "/keybindings.xml");    //saves the keyboard layout to an XML file (players can edit this I don't care)

            bf.Serialize (file, data);//overwrite data or just add it to the file (basically input object made above to file)
            file.Close ();  //done!
            GetComponent<GUIManager>().SetPopUpText("Success!");
        }
        public void LoadGame(int slot) {
            if (File.Exists (Application.persistentDataPath + "/gameData" + slot + ".dat")) {               //only load if file exists
                BinaryFormatter bf = new BinaryFormatter ();                        
                FileStream file = File.Open (Application.persistentDataPath + "/gameData" + slot + ".dat", FileMode.Open);  //open the file
                PlayerData data = (PlayerData)bf.Deserialize (file);                            //cast generic object to Playerdata and read info from file into PlayerData
                file.Close ();

                //Start assigning information based on pulled information from the file
                playerHealth = data.health;
                playerRegen = data.regeneration;
                playerMaxHealth = data.maxHealth;
                GetComponent<InventoryManagerNew>().SetInventoryWeapons(data.weapons);
                GetComponent<InventoryManagerNew>().SetInventoryConsumables(data.consumables);
                GetComponent<ObjectiveManager> ().AddObjectList (data.objectives);
                GetComponent<AreaManager>().SetAllValues(data.areaManager);
                guiManager.SetMenuAllowQuit(true);                                          //Make it so the player can quit out of the menu screen
                InputManager.Load (Application.persistentDataPath + "/keybindings.xml");    //load keyboard layout from XML file
                guiManager.SetActiveInventory(true);                                        //Allow player to open inventory
                guiManager.SetActivatedMenu(true);                                          //Allow save game option
                GetComponent<TransitionManager>().LoadTargetLevel(data.sceneName, data.travelPoint, defaultTransitionAudio, audioVolume, "Loading Save", "Your save data is being loaded please wait...", defaultLoadBackground);
            } 
        }
        public void LoadSaveInfoOnly(int slot, GUILoadGameSlots slotInfo)
        {
            StartCoroutine(LoadSaveInfo(slot, slotInfo));
        }
        IEnumerator LoadSaveInfo(int slot, GUILoadGameSlots slotInfo)
        {
            slotInfo.description.text = "";
            slotInfo.saveTime.text = "";
            slotInfo.title.text = "Loading, please wait...";
            Color slotImageColor = slotInfo.saveImage.color;
            slotImageColor.a = 0;
            slotInfo.saveImage.color = slotImageColor;
            if (File.Exists(Application.persistentDataPath + "/gameData" + slot + ".png"))
            {
                string image = @"file://" + Application.persistentDataPath + "/gameData" + slot + ".png";
                using (WWW www = new WWW(image))
                {
                    yield return www;
                    slotInfo.saveImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
                    slotImageColor.a = 1;
                    slotInfo.saveImage.color = slotImageColor;
                }
            }
            if (File.Exists(Application.persistentDataPath + "/gameData" + slot + ".dat"))
            {              
                PlayerData data;
                try {
                    BinaryFormatter bf = new BinaryFormatter();                        
                    FileStream file = File.Open(Application.persistentDataPath + "/gameData" + slot + ".dat", FileMode.Open);  //open the file
                    data = (PlayerData)bf.Deserialize(file);                            //cast generic object to Playerdata and read info from file into PlayerData
                    file.Close();

                    slotInfo.description.text = "Health: " + data.health;
                    slotInfo.saveTime.text = "" + data.lastSave;
                    slotInfo.title.text = "Location: " + data.sceneName;
                }
                catch (Exception e)
                {
                    slotInfo.title.text = "Error while loading file.";
                    slotInfo.saveTime.text = e.Message;
                    slotInfo.description.text = "";
                }
            }
            else
            {
                slotInfo.description.text = "";
                slotInfo.saveTime.text = "";
                slotInfo.title.text = "Empty Slot";
            }
        }
        public void SetPlayerObject(GameObject go)
        {
            if (go != null && go.transform.root.GetComponent<Health>())
            {
                go.transform.root.GetComponent<Health>().SetHealth(playerHealth);
                go.transform.root.GetComponent<Health>().SetRegeneration(playerRegen);
                go.transform.root.GetComponent<Health>().SetMaxHealth(playerMaxHealth);
            }
        }
        #endregion
    }
}