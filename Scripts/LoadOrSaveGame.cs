using UnityEngine;
using System.Collections;			//for screenshots
using System.Collections.Generic; 	//for list & dictionary
using TeamUtility.IO;				//input manager to recognize button presses
using System;						//Allow Serialization of classes
using System.Runtime.Serialization.Formatters.Binary;//allow writing in binary
using System.IO;					//Allow Input & Output
using UnityEngine.SceneManagement;	//allow loading scenes
using UnityEngine.UI;				//for UI elements
using Pandora.Controllers;
using Pandora.UI;

namespace Pandora {
    namespace GameManager {
        public class LoadOrSaveGame : MonoBehaviour {

        	[SerializeField] private GameObject[] saveSlots;
        	[SerializeField] private GameObject saveWarning;
        	[SerializeField] private string[] noSaveAreas;
        	[SerializeField] private GameObject GUI;
        	[SerializeField] private GameObject playerDeathLoadScreen;

        	private void Start() {
        		SetVisuals (1);
        		SetVisuals (2);
        		SetVisuals (3);
        	}
        	private IEnumerator noSaveAllowed() {
        		string orignal = saveWarning.GetComponent<Text> ().text;
        		saveWarning.GetComponent<Text> ().text = "You are not allowed to save in this area.";
        		yield return new WaitForSeconds (3);
        		saveWarning.GetComponent<Text> ().text = orignal;
        		saveWarning.SetActive (false);
        	}
        	public void SaveGame(int slot) {
        		bool canSave = true;
        		foreach (string areaName in noSaveAreas) {
        			if (SceneManager.GetActiveScene ().name == areaName) {
        				//StartCoroutine (noSaveAllowed ());
        				canSave = false;
        			}
        		}
        		if(canSave == true) {
        			BinaryFormatter bf = new BinaryFormatter ();
        			Debug.Log ("saving to: " + Application.persistentDataPath+"/gameData"+slot+".dat");
        			FileStream file = File.Open (Application.persistentDataPath + "/gameData"+slot+".dat", FileMode.OpenOrCreate);//make a file or open it if already made

        			//fill an object to save to this file
        			PlayerData data = new PlayerData ();
        			data.health = GameObject.FindGameObjectWithTag ("Player").GetComponent<Health> ().GetHealth ();
                    data.regeneration = GameObject.FindGameObjectWithTag ("Player").GetComponent<Health> ().GetRegeneration ();
        			data.inventory = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().GetPlayerInventory ();
        			data.area = SceneManager.GetActiveScene ().name;
        			data.objectives = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<ObjectiveManager> ().ShowAllObjectives ();
        			data.lastSave = System.DateTime.Now;
        			data.vectorX = (float)GameObject.FindGameObjectWithTag ("Player").transform.position.x;
        			data.vectorY = (float)GameObject.FindGameObjectWithTag ("Player").transform.position.y;
        			data.vectorZ = (float)GameObject.FindGameObjectWithTag ("Player").transform.position.z;
        			//save area managers
        			var properties = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ().GetType ().GetProperties(System.Reflection.BindingFlags.Public);
        			foreach (var property in properties) {
        				string valueName = property.Name.ToString();
        				bool value = (bool)property.GetValue (this, null);
        				data.areaManager.Add (valueName,value); 
        			}
        			InputManager.Save (Application.persistentDataPath + "/keybindings.xml");	//saves the keyboard layout to an XML file (players can edit this I don't care)

        			bf.Serialize (file, data);//overwrite data or just add it to the file (basically input object made above to file)
        			file.Close ();	//done!
        			SetVisuals(slot);
        			//GameObject.FindGameObjectWithTag("GameManager").transform.Find("OpenMenu").GetComponent<OpenMenu>().MenuClose();
        		}
        		saveWarning.gameObject.SetActive (false);
        	}

        	public void LoadGame(int slot) {
        		Debug.Log ("saving to: " + Application.persistentDataPath+"/gameData"+slot+".dat");
        		if (File.Exists (Application.persistentDataPath + "/gameData" + slot + ".dat")) {				//only load if file exists
        			BinaryFormatter bf = new BinaryFormatter ();						
        			FileStream file = File.Open (Application.persistentDataPath + "/gameData" + slot + ".dat", FileMode.Open);	//open the file
        			PlayerData data = (PlayerData)bf.Deserialize (file);							//cast generic object to Playerdata and read info from file into PlayerData
        			file.Close ();

        			//Start assigning information based on pulled information from the file
        			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<PlayerManager> ().currentPlayerHealth = data.health;
        			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<PlayerManager> ().currentPlayerRegen = data.regeneration;
        			InputManager.Load (Application.persistentDataPath + "/keybindings.xml");	//load keyboard layout from XML file
        			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<ObjectiveManager> ().AddObjectList (data.objectives);
        			GameObject.FindGameObjectWithTag("GameManager").transform.Find("OpenMenu").GetComponent<OpenMenu>().MenuClose();
        			SceneManager.LoadScene (data.area);
        			GUI.SetActive (true);
        			GUI.GetComponent<PlayerDeath> ().playerDead = false;
        			//load area manager
        			var properties = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ().GetType ().GetProperties (System.Reflection.BindingFlags.Public);
        			foreach (var property in properties) {
        				string valueName = property.Name.ToString ();
        				GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ().GetType ().GetField (valueName).SetValue (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> (), data.areaManager [valueName]);
        			}
        			playerDeathLoadScreen.SetActive (false);
        			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().SetPlayerInventory (data.inventory);
        			//GameObject.FindGameObjectWithTag ("Player").transform.position = new Vector3 (data.vectorX, data.vectorY, data.vectorZ);
        		} 
        	}
        	public void ReloadLevel() {
        		GameObject.FindGameObjectWithTag ("GUIParent").GetComponent<PlayerDeath> ().playerDead = false;
        		if (SceneManager.GetActiveScene ().name == "StartingArea") {
        			Destroy (GameObject.FindGameObjectWithTag ("GameManager"));
        			Destroy (GameObject.FindGameObjectWithTag ("GUIParent"));
        		}
        		Time.timeScale = 1;
        		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
        	}
        	public void BackToMainMenu() {
        		Destroy (GameObject.FindGameObjectWithTag ("GameManager"));
        		SceneManager.LoadScene ("StartingArea");
        		Destroy (this.gameObject);
        	}
        	private void SetVisuals(int slot) {
        		string info = "";
        		if (File.Exists (Application.persistentDataPath + "/gameData" + slot + ".dat")) {				//only load if file exists
        			BinaryFormatter bf = new BinaryFormatter ();						
        			FileStream file = File.Open (Application.persistentDataPath + "/gameData" + slot + ".dat", FileMode.Open);	//open the file
        			PlayerData data = (PlayerData)bf.Deserialize (file);							//cast generic object to Playerdata and read info from file into PlayerData
        			file.Close ();
        			info = "Location: "+data.area+"\nHealth: "+data.health+"\nLast Save: "+data.lastSave;
        		}
        		else {
        			info = "\nNo Saved Data.\n";
        		}
        		foreach (GameObject save in saveSlots) {
        			if(save.tag == "saveSlot"+slot) {
        				save.GetComponent<Text>().text = info;
        			}
        		}
        	}
        }
        [Serializable]
        class PlayerData {
        	public float health;
        	public float regeneration;
        	public List<string> inventory = new List<string> ();
        	public string area = "";
        	//public Vector3 worldLocation = Vector3.zero;
        	public Dictionary<string, bool> areaManager = new Dictionary<string, bool> ();
        	public List<Objectives> objectives = new List<Objectives> ();
        	public System.DateTime lastSave;
        	//work around for saving vector3 (since Unity specific objects are not serializable)
        	public float vectorX = 0.0f;
        	public float vectorY = 0.0f;
        	public float vectorZ = 0.0f;
        }
    }
}