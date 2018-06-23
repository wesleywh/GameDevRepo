using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
class KeyMaterialList {
	public string name = "";
//	public int number = 0;
	[Range(0.0F, 1.0F)]
	public float volume = 1.0f;
	public AudioClip[] footstepSounds = null;
	public AudioClip[] landingSounds = null;
	public AudioClip[] bodyfallSounds = null;
}
[RequireComponent (typeof(AudioSource))]
public class FootStepKeyFrame : MonoBehaviour {
	[SerializeField] private bool noSounds = false;
	[SerializeField] private KeyMaterialList[] materialList;
	[SerializeField] private float objectHeight = 1.0f;
	[SerializeField] private Vector3 positionAdjust = new Vector3 (0, 0, 0);
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private bool useOverrideVolume = false;
	[Range(0.0F, 1.0F)]
	[SerializeField] public float overrideVolume = 1.0f;
	[SerializeField] private bool debugSurface = false;
	[SerializeField] private bool debugHeight = false;
	[SerializeField] private bool debugDrawRayCasts = false;

	private string hitSurfaceName;

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
	private GameObject GetCollider()
	{
		RaycastHit hit;
		if(debugDrawRayCasts == true){
			Debug.DrawRay (this.transform.position + positionAdjust, Vector3.down, Color.blue, 10.0f);
		}
		if (Physics.Raycast (this.transform.position + positionAdjust, Vector3.down, out hit, objectHeight)) {
			return hit.transform.gameObject;
		}
		return null;
	}
	void PlayFootStepSoundKeyFrame()
	{
		if (noSounds == true) {
			return;
		}
		GameObject collision = GetCollider ();
		if (collision != null) {
			if (debugSurface == true) {
				Debug.Log ("Tag: " + collision.tag + " Name: " + collision.name);
			}
			if (collision.tag == "Terrain") {
				hitSurfaceName = TerrainSurface.GetMainTexture (transform.position);
				if (debugSurface == true) {
					Debug.Log ("Surface: " + hitSurfaceName);
				}
				foreach (KeyMaterialList material in materialList) {
					if (hitSurfaceName == material.name) {
						if (useOverrideVolume) {
							audioSource.volume = overrideVolume;
						} else {
							audioSource.volume = material.volume;
						}
						audioSource.clip = material.footstepSounds [UnityEngine.Random.Range (0, material.footstepSounds.Length)];
						audioSource.Play ();
						return;
					}
				}
			} else {
				foreach (KeyMaterialList material in materialList) {
					if (collision.GetComponent<MeshRenderer> ()) {
						hitSurfaceName = collision.GetComponent<MeshRenderer> ().material.name;
						if (debugSurface == true) {
							Debug.Log ("Material Name: " + hitSurfaceName);
						}
						if (hitSurfaceName == material.name || hitSurfaceName == material.name + " (Instance)"
						    && audioSource.isPlaying == false) {
							if (useOverrideVolume) {
								audioSource.volume = overrideVolume;
							} else {
								audioSource.volume = material.volume;
							}
							audioSource.clip = material.footstepSounds [UnityEngine.Random.Range (0, material.footstepSounds.Length)];
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
	public void PlayLandAudio() {
		GameObject collision = GetCollider ();
		if (collision != null) {
			if (debugSurface == true) {
				Debug.Log ("Tag: " + collision.tag + " Name: " + collision.name);
			}
			if (collision.tag == "Terrain") {
				hitSurfaceName = TerrainSurface.GetMainTexture (transform.position);
				if (debugSurface == true) {
					Debug.Log ("Surface: " + hitSurfaceName);
				}
				foreach (KeyMaterialList material in materialList) {
					if (hitSurfaceName == material.name) {
						if (useOverrideVolume) {
							audioSource.volume = overrideVolume;
						} else {
							audioSource.volume = material.volume;
						}
						audioSource.clip = material.landingSounds [UnityEngine.Random.Range (0, material.landingSounds.Length)];
						audioSource.Play ();
						return;
					}
				}
			} else {
				foreach (KeyMaterialList material in materialList) {
					if (collision.GetComponent<MeshRenderer> ()) {
						hitSurfaceName = collision.GetComponent<MeshRenderer> ().material.name;
						if (debugSurface == true) {
							Debug.Log ("Material Name: " + hitSurfaceName);
						}
						if (hitSurfaceName == material.name || hitSurfaceName == material.name + " (Instance)"
							&& audioSource.isPlaying == false) {
							if (useOverrideVolume) {
								audioSource.volume = overrideVolume;
							} else {
								audioSource.volume = material.volume;
							}
							audioSource.clip = material.landingSounds [UnityEngine.Random.Range (0, material.landingSounds.Length)];
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
	public void PlayBodyFallAudio() {
		GameObject collision = GetCollider ();
		if (collision != null) {
			if (debugSurface == true) {
				Debug.Log ("Tag: " + collision.tag + " Name: " + collision.name);
			}
			if (collision.tag == "Terrain") {
				hitSurfaceName = TerrainSurface.GetMainTexture (transform.position);
				if (debugSurface == true) {
					Debug.Log ("Surface: " + hitSurfaceName);
				}
				foreach (KeyMaterialList material in materialList) {
					if (hitSurfaceName == material.name) {
						if (useOverrideVolume) {
							audioSource.volume = overrideVolume;
						} else {
							audioSource.volume = material.volume;
						}
						audioSource.clip = material.bodyfallSounds [UnityEngine.Random.Range (0, material.bodyfallSounds.Length)];
						audioSource.Play ();
						return;
					}
				}
			} else {
				foreach (KeyMaterialList material in materialList) {
					if (collision.GetComponent<MeshRenderer> ()) {
						hitSurfaceName = collision.GetComponent<MeshRenderer> ().material.name;
						if (debugSurface == true) {
							Debug.Log ("Material Name: " + hitSurfaceName);
							Debug.Log ("Material Name Material: " + material.name);
						}
						if (hitSurfaceName == material.name || hitSurfaceName == material.name + " (Instance)"
							&& audioSource.isPlaying == false) {
							if (useOverrideVolume == true) {
								audioSource.volume = overrideVolume;
							} else {
								audioSource.volume = material.volume;
							}
							audioSource.clip = material.bodyfallSounds [UnityEngine.Random.Range (0, material.bodyfallSounds.Length)];
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
