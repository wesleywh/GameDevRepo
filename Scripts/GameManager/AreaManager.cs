using UnityEngine;
using System.Collections;

public class AreaManager : MonoBehaviour {
	//========= area specific ==============//
	//STARTING AREA
	public bool starting_runestone_placed = false;
	public bool started_game = false;
	[Space(10)]
	// MANSION INTERIOR
	public bool mansion_first_entrance = true;
	public bool mansion_grand_hall_lights_on = false;
	public bool mansion_entrance_lights_on = false;
	public bool mansion_entrance_lights_working = false;
	public bool mansion_stairsuproom_lights_on = false;
	public bool mansion_stairsuproom_lights_working = false;
	public bool mansion_library_body_found = false;
	[Space(10)]
	// MANSION BACKYARD
	public bool backyard_maze_rock_fallen = false;
	public bool backyard_maze_cutscene_complete = false;
	[Space(10)]
	// UNERGROUND CAVE
	public bool underground_cave_entered = false;
	public bool underground_cave_lever1 = false;
	public bool underground_cave_lever2 = false;
	public bool underground_cave_bridge01 = false;
	public bool underground_cave_bridge02 = false;
	[Space(10)]
	//items
	public bool item_mansion_entrance_scroll = true;
	public bool item_mansion_backyard_gate_key = true;

	public void SetValueTrue(string name) {
		GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ().GetType ().GetField (name).SetValue (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> (), true);
	}
	public void SetValueFalse(string name) {
		GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ().GetType ().GetField (name).SetValue (GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> (), false);
	}
}
