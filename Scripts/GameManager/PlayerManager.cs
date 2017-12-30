using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; 	//for list & dictionary
using Pandora.Controllers;
using Pandora.Cameras;
using Pandora.GameManager;
using TeamUtility.IO;
using System.Runtime.Serialization.Formatters.Binary;//allow writing in binary
using System.IO;                    //Allow Input & Output

namespace Pandora.GameManager {
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
    public class PlayerManager : MonoBehaviour {
    	private float playerHealth = 100.0f;
    	private float playerRegen = 0.0f;
        private float playerMaxHealth = 100.0f;
        private List<Objectives> objectives = new List<Objectives>();
        private InventoryList[] weapons;
        private InventoryList[] consumables;
        private string travelToPoint = "";
        private string sceneName = "";
        [SerializeField] private AudioClip defaultTransitionAudio = null;
        [Range(0,1)]
        [SerializeField] private float audioVolume = 0.5f;
        [SerializeField] private Texture2D defaultLoadBackground = null;
        private InventoryManagerNew inventoryManager = null;
        private ObjectiveManager objectiveManager = null;
        private TransitionManager transitionManager = null;

        void Start()
        {
            inventoryManager = GetComponent<InventoryManagerNew>();
            objectiveManager = GetComponent<ObjectiveManager>();
            transitionManager = GetComponent<TransitionManager>();

        }

        #region Manipulate Player
        public void SetPlayerControllable(bool enabled)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<MovementController>().enabled = enabled;
            EnableCameraControl(enabled);
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
        #endregion

        #region Save/Load Game Data
        public void SaveGame(int slot) {
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/gameData" + slot + ".png");
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
                InputManager.Load (Application.persistentDataPath + "/keybindings.xml");    //load keyboard layout from XML file
                GetComponent<TransitionManager>().LoadTargetLevel(data.sceneName, data.travelPoint, defaultTransitionAudio, audioVolume, "Loading Save", "Your save data is being loaded please wait...", defaultLoadBackground);
            } 
        }
        public void LoadSaveInfoOnly(int slot)
        {
            if (File.Exists(Application.persistentDataPath + "/gameData" + slot + ".png"))
            {
                string filepath = @"file://" + Application.persistentDataPath + "/gameData" + slot + ".png";
//                renderer.material.mainTexture = filepath;
            }
            if (File.Exists (Application.persistentDataPath + "/gameData" + slot + ".dat")) {               //only load if file exists
                BinaryFormatter bf = new BinaryFormatter ();                        
                FileStream file = File.Open (Application.persistentDataPath + "/gameData" + slot + ".dat", FileMode.Open);  //open the file
                PlayerData data = (PlayerData)bf.Deserialize (file);                            //cast generic object to Playerdata and read info from file into PlayerData
                file.Close ();

                //Start assigning information based on pulled information from the file
                playerHealth = data.health;
                playerRegen = data.regeneration;
                playerMaxHealth = data.maxHealth;
                System.DateTime lastSave = data.lastSave;
            } 
        }
        #endregion
    }
}