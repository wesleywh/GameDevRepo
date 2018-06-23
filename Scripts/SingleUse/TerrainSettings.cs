using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSettings : MonoBehaviour {
    public bool collectDetailPatches = false;
	// Use this for initialization
	void Start () {
        Terrain.activeTerrain.collectDetailPatches = collectDetailPatches;
	}
}
