using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
class KeyMaterialList {
	public string name = "";
	public int number = 0;
	[Range(0.0F, 1.0F)]
	public float volume = 1.0f;
	public AudioClip[] soundsToPlay = null;
}
[RequireComponent (typeof(AudioSource))]
public class FootStepKeyFrame : MonoBehaviour {
	public bool useTerrain = false;
	[SerializeField] private KeyMaterialList[] materialList;
	[SerializeField] private float objectHeight = 1.0f;
	[SerializeField] private Vector3 positionAdjust = new Vector3 (0, 0, 0);
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private bool useOverrideVolume = false;
	[Range(0.0F, 1.0F)]
	[SerializeField] public float overrideVolume = 1.0f;
	[SerializeField] private bool debugSurface = false;
	[SerializeField] private bool debugHeight = false;

	private string hitMaterial;
	private int surfaceIndex = 0;

	void Start()
	{
		if (audioSource == null) {
			audioSource = this.GetComponent<AudioSource> ();
		}
	}
	void OnDrawGizmos()
	{
		if (debugHeight == true) 
		{
			Gizmos.color = Color.red;
			Vector3 direction = transform.TransformDirection (Vector3.down) * objectHeight;
			Gizmos.DrawRay (transform.position + positionAdjust, direction);
		}
	}
	GameObject GetCollider()
	{
		RaycastHit hit;
		if (Physics.Raycast (transform.position + positionAdjust, Vector3.down, out hit, objectHeight)) {
			return hit.transform.gameObject;
		}
		return null;
	}
	void PlayFootStepSoundKeyFrame()
	{
		GameObject collision = GetCollider ();
		if (collision != null) {
			if (debugSurface == true) {
				Debug.Log ("Tag: " + collision.tag + " Name: " + collision.name);
			}
			if (collision.tag == "Terrain" || useTerrain == true) {
				surfaceIndex = TerrainSurface.GetMainTexture (transform.position);
				if (debugSurface == true) {
					Debug.Log ("Surface Index: " + surfaceIndex);
				}
				foreach (KeyMaterialList material in materialList) {
					if (surfaceIndex == material.number) {
						if (useOverrideVolume) {
							audioSource.volume = overrideVolume;
						} else {
							audioSource.volume = material.volume;
						}
						audioSource.clip = material.soundsToPlay [UnityEngine.Random.Range (0, material.soundsToPlay.Length)];
						audioSource.Play ();
						return;
					}
				}
			} else {
				foreach (KeyMaterialList material in materialList) {
					if (collision.GetComponent<MeshRenderer> ()) {
						hitMaterial = collision.GetComponent<MeshRenderer> ().material.name;
						if (debugSurface == true) {
							Debug.Log ("Material Name: " + hitMaterial);
						}
						if (useTerrain == false && (hitMaterial == material.name || hitMaterial == material.name + " (Instance)"
						    && audioSource.isPlaying == false)) {
							if (useOverrideVolume) {
								audioSource.volume = overrideVolume;
							} else {
								audioSource.volume = material.volume;
							}
							audioSource.clip = material.soundsToPlay [UnityEngine.Random.Range (0, material.soundsToPlay.Length)];
							audioSource.Play ();
							return;
						} 
					} else {
						return;
					}
				}
			}
		}
	}
}
