using UnityEngine;
using System.Collections;

public class TerrainSurface : MonoBehaviour {

	public static float[] GetTextureMix(Vector3 worldPos) {
		// returns an array containing the relative mix of textures
		// on the main terrain at this world position.
		// The number of values in the array will equal the number
		// of textures added to the terrain.
		Terrain terrain = Terrain.activeTerrain;
		TerrainData terrainData = terrain.terrainData;
		Vector3 terrainPos = terrain.transform.position;
		// calculate which splat map cell the worldPos falls within (ignoring y)
		int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
		int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
		// get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
		float[,,] splatmapData = terrainData.GetAlphamaps(mapX,mapZ,1,1);
		// extract the 3D array data to a 1D array:
		float[] cellMix = new float[splatmapData.GetUpperBound(2)+1];
		for (int n=0; n < cellMix.Length; ++n)
		{
			cellMix[n] = splatmapData[0,0,n];    
		}
		return cellMix;        
	}
	public static string GetMainTexture(Vector3 worldPos) {
		// returns the zero-based index of the most dominant texture
		// on the main terrain at this world position.
		float[] mix = GetTextureMix(worldPos);
		float maxMix = 0;
		int maxIndex = 0;
		// loop through each mix value and find the maximum
		for (int n=0; n < mix.Length; n++)
		{
			if (mix[n] > maxMix)
			{
				maxIndex = n;
				maxMix = mix[n];
			}
		}
		Terrain terrain = Terrain.activeTerrain;
		TerrainData terrainData = terrain.terrainData;
		return terrainData.splatPrototypes[maxIndex].texture.name;
	}
}
