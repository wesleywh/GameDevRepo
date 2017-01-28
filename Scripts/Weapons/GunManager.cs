using UnityEngine;
using System;
using System.Collections;
[System.Serializable]
class HitVisuals {
	public string materialName = "";
	public string tag = "";
	public GameObject visual = null;
	public float visualDestroyDelay = 1.0f;
	public GameObject decal = null;
	public float decalDestroyDelay = 20.0f;
	public AudioClip[] soundToPlay = null;
	[Range(0,1)]
	public float volume = 1.0f;
}	

public class GunManager : MonoBehaviour {

	[SerializeField] private GameObject muzzle_flash = null;
	[SerializeField] private float destroyMuzzleFlash = 0.1f;
	[SerializeField] private AudioClip[] gunshots = null;
	[SerializeField] private AudioSource overrideAudioSource = null;
	[Range(0,1)]
	[SerializeField] private float audioSourceVolume = 0.5f;
	[SerializeField] private Transform overrideFiringPoint = null;
	[SerializeField] private float accuracy = 0.2f;//the lower the number the more accurate
	[SerializeField] private float damage = 10.0f;
	[SerializeField] private float raycastDistance = 100.0f;
	[SerializeField] private HitVisuals[] hitDictionary;
	[Space(10)]
	[SerializeField] private bool debugRay = false;
	[SerializeField] private bool debugRayHit = false;

	[HideInInspector] public GameObject target = null;

	private AudioSource audioSource;

	void Start() {
		if (overrideAudioSource) {
			audioSource = overrideAudioSource;
		} else {
			audioSource = this.gameObject.GetComponent<AudioSource> ();
		}
		audioSource.loop = false;
	}
	public void pistolFire() {
		Shot ();
	}
		
	private void Shot() {
		StartCoroutine(playMuzzleFlash());
		StartCoroutine(playGunShotSound());
		Collider coll;
		Vector3 ray;
		if (overrideFiringPoint) {
			ray = overrideFiringPoint.position;
		} else {
			ray = transform.position;
		}
		//make less accurate
		GameObject enemy = GameObject.FindGameObjectWithTag("Player");
		if (enemy != null) {
			Vector3 direction = (enemy.transform.position - transform.position);
			direction.x += UnityEngine.Random.Range (-accuracy, accuracy);
			direction.y += UnityEngine.Random.Range (-accuracy, accuracy);
			direction.z += UnityEngine.Random.Range (-accuracy, accuracy);
			RaycastHit hit;
			if (debugRay == true) {
				Debug.DrawRay (ray, direction, Color.red, 2.0f);
			}
			//Hit something!
			if (Physics.Raycast (ray, direction, out hit, raycastDistance)) {
				if (debugRayHit == true) {
					Debug.Log ("GAMEOBJECT NAME: " + hit.collider.gameObject.name + ", TAG: " + hit.collider.gameObject.tag);
					if (hit.collider.gameObject.GetComponent<MeshRenderer> ()) {
						Debug.Log ("MESH: " + hit.collider.gameObject.GetComponent<MeshRenderer> ().name);
					}
				}
				coll = hit.collider;
				//hit an object with health
				if (coll.gameObject.GetComponent<Health> ()) {
					coll.gameObject.GetComponent<Health> ().ApplyDamage (damage);
				} else if (coll.gameObject.transform.root.GetComponent<Health> ()) {
					coll.gameObject.transform.root.GetComponent<Health> ().ApplyDamage (damage);
				}
				foreach (HitVisuals hitObj in hitDictionary) {
					if (coll.gameObject.tag == hitObj.tag) {
						hitObject (hit, hitObj);
						return;
					}
				}
				//hit a terrain
				if (coll.gameObject.tag == "Terrain") {
					string hitSurfaceName = TerrainSurface.GetMainTexture (transform.position);
					foreach (HitVisuals hitPoint in hitDictionary) {
						if (hitSurfaceName == hitPoint.materialName) {
							hitObject (hit, hitPoint);
							return;
						}
					}
				}
			//hit something other than a terrain
			else if (coll.gameObject.GetComponent<MeshRenderer> ()) {
					string hitSurfaceName = coll.gameObject.GetComponent<MeshRenderer> ().material.name;
					foreach (HitVisuals hitPoint in hitDictionary) {
						if (hitSurfaceName == hitPoint.materialName) {
							hitObject (hit, hitPoint);
							return;
						}
					}
				} 
			}
		}
	}

	public void SetDamage(float setDamage) {
		damage = setDamage;
	}
	public void Setaccuracy(float amount) {
		accuracy = amount;
	}
	private IEnumerator playGunShotSound() {
		if (gunshots.Length > 0) {
			audioSource.volume = audioSourceVolume;
			audioSource.clip = gunshots [UnityEngine.Random.Range (0, gunshots.Length)]; 
			audioSource.Play ();
		}
		yield return true;
	}
	private IEnumerator playMuzzleFlash() {
		muzzle_flash.SetActive (true);
		yield return new WaitForSeconds (destroyMuzzleFlash);
		muzzle_flash.SetActive (false);
	}
	private IEnumerator destroyObject(GameObject obj, float delay) {
//		if (delay <= 0) {
//			delay = 1.0f;
//		}
//		yield return new WaitForSeconds (delay);
		Destroy(obj, delay);
		yield return true;
	}
	private void hitObject(RaycastHit hit, HitVisuals hitObj) {
		if (hitObj.visual != null) {
			GameObject hitVisual = (GameObject)Instantiate (hitObj.visual, hit.point + (hit.normal * 0.00001f), Quaternion.LookRotation (hit.normal));
			hitVisual.name = "GunManager - HitVisualObject";
			StartCoroutine (destroyObject (hitVisual, hitObj.visualDestroyDelay));
		}
			GameObject empty = new GameObject ("Placeholder Object");
			GameObject hitDecal = (hitObj.decal != null) ? 
				(GameObject)Instantiate (hitObj.decal, hit.point + (hit.normal * 0.00001f), Quaternion.LookRotation (hit.normal)) : 
				(GameObject)Instantiate (empty, hit.point + (hit.normal * 0.00001f), Quaternion.LookRotation (hit.normal)) as GameObject;
			hitDecal.name = "GunManager - HitDecal";
			StartCoroutine (destroyObject (empty, hitObj.decalDestroyDelay));

		if (hitObj.soundToPlay.Length > 1) {
			hitDecal.AddComponent<AudioSource> ();
			AudioSource audioSource = hitDecal.GetComponent<AudioSource> ();
			audioSource.volume = hitObj.volume;
			audioSource.clip = hitObj.soundToPlay [UnityEngine.Random.Range (0, hitObj.soundToPlay.Length)];
			audioSource.loop = false;
			audioSource.Play ();
		}
		StartCoroutine (destroyObject (hitDecal, hitObj.decalDestroyDelay));
	}
}
