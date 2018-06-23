using UnityEngine;
using System.Collections;

public class WetClothing : MonoBehaviour {
	public SkinnedMeshRenderer[] listOfMeshes;
    public MeshRenderer[] meshs;
	[SerializeField] private float GlossyValue = 0.9f;
	[SerializeField] private float DefaultValue = 0.25f;
	public bool isWet = false;
	private bool applied = false;
	private bool previous = false;
	// Update is called once per frame
	void Update () {
		if (previous != isWet) {
			previous = isWet;
			applied = false;
		}
		if (isWet == true && applied == false) {
			applied = true;
			foreach (SkinnedMeshRenderer mesh in listOfMeshes) {
				mesh.material.SetFloat ("_Glossiness", GlossyValue);
			}
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.SetFloat("_Glossiness", GlossyValue);
            }
		} else if (isWet == false && applied == false) {
			applied = true;
			foreach (SkinnedMeshRenderer mesh in listOfMeshes) {
				mesh.material.SetFloat ("_Glossiness", DefaultValue);
			}
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.SetFloat("_Glossiness", DefaultValue);
            }
		}
	}
}
