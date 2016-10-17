using UnityEngine;
using System.Collections;
using System.Collections.Generic; 	//for list & dictionary
using TeamUtility.IO;				//input manager to recognize button presses
using System;						//Allow Serialization of classes
using System.Runtime.Serialization.Formatters.Binary;//allow writing in binary
using System.IO;					//Allow Input & Output
using UnityEngine.SceneManagement;	//allow loading scenes

public class LoadOrSaveGame : MonoBehaviour {

	public void SaveGame() {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/gameData.dat", FileMode.OpenOrCreate);//make a file or open it if already made

		//fill an object to save to this file
		PlayerData data = new PlayerData ();
		data.health = GameObject.FindGameObjectWithTag ("Player").GetComponent<Health> ().GetHealth ();
		data.regeneration = GameObject.FindGameObjectWithTag ("Player").GetComponent<Health> ().GetRegeneration ();
		data.inventory = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager> ().GetPlayerInventory ();
		data.area = SceneManager.GetActiveScene ().name;
		InputManager.Save (Application.persistentDataPath + "/keybindings.xml");	//saves the keyboard layout to an XML file (players can edit this I don't care)

		bf.Serialize (file, data);//overwrite data or just add it to the file (basically input object made above to file)
		file.Close ();	//done!
	}

	public void LoadGame() {
		if (File.Exists (Application.persistentDataPath + "/gameData.dat")) {				//only load if file exists
			BinaryFormatter bf = new BinaryFormatter ();						
			FileStream file = File.Open (Application.persistentDataPath + "/gameData.dat", FileMode.Open);	//open the file
			PlayerData data = (PlayerData)bf.Deserialize (file);							//cast generic object to Playerdata and read info from file into PlayerData
			file.Close ();

			//Start assigning information based on pulled information from the file
			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<PlayerManager> ().currentPlayerHealth = data.health;
			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<PlayerManager> ().currentPlayerRegen = data.regeneration;
			InputManager.Load (Application.persistentDataPath + "/keybindings.xml");	//load keyboard layout from XML file
			GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryManager>().SetPlayerInventory(data.inventory);
			SceneManager.LoadScene (data.area);
		}
	}
}
[Serializable]
class PlayerData {
	public float health;
	public float regeneration;
	public List<string> inventory = new List<string> ();
	public string area = "";
}