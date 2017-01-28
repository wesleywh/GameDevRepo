using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using UnityEngine.UI;

public class Weapon : MonoBehaviour {
	public GameObject smokeTrail = null;
	public int showSmokeEveryXShots = 3;
	public Transform smokeTrailPosition = null;
	public float smokeTrailLife = 2.0f;
	public float smokeTrailAppearDelay = 1.0f;
	public float smokeTrailFadeSpeed = 0.2f;
	public string weaponName = "";
	public float delayPerShot = 0.2f;
	public int bulletsPerShot = 1;
	public float damage = 10.0f;
	public float kickBackAmount = 5.0f;
	public AudioSource soundSource = null;
	public AudioClip[] shotSounds = null;
	[Range(0,1)]
	public float shotVolume = 0.5f;
	public GameObject muzzleFlash = null;
	public Transform muzzleFlashPosition = null;
	public float muzzleFlashLifetime = 0.1f;
	[Tooltip("Offset accuracy when aiming")]
	public float aimFireSpread = 0.0f;
	[Tooltip("Offset accuracy when not aiming")]
	public float hipFireSpread = 0.2f;
//	public Transform cameraAimPosition = null;
	[Tooltip("Where the raycast will start from")]
	public Transform firePoint = null;
	[Tooltip("How many bullets you want to start with when this weapon is picked up")]
	public int bulletsLeft = 8;
	[Tooltip("How many bullets each clip will replenish")]
	public int bulletsPerClip = 8;
	[Tooltip("How many reloads this weapon has left.")]
	public int numberOfClips = 3;
	[Tooltip("How long to wait until you can fire again when activating reload.")]
	public float reloadTime = 0.2f;
	[Tooltip("How many objects this bullet will pass through")]
	public int maxPenetration = 1;
	[Tooltip("How far this raycast will go.")]
	public float range = 300.0f;
	[Tooltip("Whether to generate a 'shell' and eject at a random angle every shot")]
	[SerializeField] private bool ejectShell = true;
	[Tooltip("Where to generate the 'shell' object.")]
	[SerializeField] private Transform ejectPoint = null;
	[Tooltip("'shell' object is this one")]
	[SerializeField] private GameObject shellObject = null;
	[SerializeField] private float pickupDistance = 2.0f;
	[SerializeField] private bool canPickup = false;
	public bool destroyOnPickup = true;
	[HideInInspector] public bool isReloading = false;
	public float aimSpeed = 2.0f;
	private GameObject player = null;
	private bool playerSet = false;
	private bool fired = false;
	private int smokeShots = 0;
	private GUIControl GUI = null;

	void Start() {
		GUI = GameObject.FindGameObjectWithTag ("GUIParent").GetComponent<GUIControl> ();
		if (soundSource == null) {
			soundSource = this.GetComponent<AudioSource> ();
		}
		if (GameObject.FindGameObjectWithTag ("Player")) {
			player = GameObject.FindGameObjectWithTag ("Player");
			playerSet = true;
		}
	}

	void Update() {
		if (bulletsPerClip <= 0 && numberOfClips > 0) {
			StartCoroutine (Reload ());
		}
		if (canPickup == true) {
			if (playerSet == false) {
				if (GameObject.FindGameObjectWithTag ("Player")) {
					player = GameObject.FindGameObjectWithTag ("Player");
					playerSet = true;
				}
			} else {
				if (Vector3.Distance (player.transform.position, this.transform.position) <= pickupDistance &&
				   InputManager.GetButtonDown ("Action")) {
					player.GetComponent<WeaponManager> ().PickupWeapon (this.GetComponent<Weapon> ());
				}
			}
		}
	}
	//eject an object in random arc from gun
	public void EjectShell(){
		if (ejectShell == true) {
			Vector3 position = ejectPoint.position; // ejectile spawn point at gun's ejection point
			if (shellObject) {
				Rigidbody newShell = Instantiate (shellObject, position, transform.parent.rotation) as Rigidbody; // create empty shell
				//give ejectile a slightly random ejection velocity and direction
				newShell.velocity = transform.TransformDirection (Random.Range (-2, 2) - 3.0f, Random.Range (-1, 2) + 3.0f, -Random.Range (-2, 2) + 1.0f);
			}
		}
	}
	public IEnumerator ShowSmokeTrail() {
		if (smokeTrail != null) {
			GameObject smoke = Instantiate (smokeTrail, smokeTrail.transform.position, smokeTrail.transform.rotation) as GameObject;
			smoke.name = "Smoke Trail Object";
			smoke.GetComponent<LockToTransform> ().lockTransform = smokeTrailPosition;
			smoke.GetComponent<FadeOut> ().life = smokeTrailLife;
			smoke.GetComponent<FadeOut> ().fadeSpeed = smokeTrailFadeSpeed;
			smoke.GetComponent<FadeOut> ().start = true;
		}
		yield return null;
	}

	//Kick back
	public void Kick(){
		if (GameObject.FindGameObjectWithTag("PlayerCamera") && GameObject.FindGameObjectWithTag("WeaponCamera")){
			float kick = Random.Range((kickBackAmount / 2), kickBackAmount);
			this.transform.root.GetComponent<WeaponManager>().kickPoint.offsetY = kick;
		}
	}

	public bool Fire(bool isAiming) {
		if (fired == false) {
			float delayTime = 0;
			for (int i = 0; i < bulletsPerShot; i++) {
				StartCoroutine (FireShot (isAiming, delayTime));
				delayTime += delayPerShot;
			}
			fired = true;
			StartCoroutine (LoadNextRound ());
			return true;
		}
		return false;
	}

	private IEnumerator LoadNextRound() {
		yield return new WaitForSeconds (delayPerShot);
		fired = false;
	}

	private IEnumerator DestroyMuzzleFlash(GameObject muzzleFlashObj) {
		yield return new WaitForSeconds (muzzleFlashLifetime);
		Destroy (muzzleFlashObj);
	}
	public IEnumerator FireShot(bool isAiming, float delay=0) {
		bulletsLeft -= 1;
		GUI.AmmoBulletsLeft.GetComponent<Text> ().text = (bulletsLeft >= 0) ? bulletsLeft.ToString () : "0";
		if (bulletsLeft <= -1) {
			bulletsLeft = 0;
			StartCoroutine(Reload ());
		} else {
			smokeShots += 1;
			if (smokeShots > showSmokeEveryXShots) {
				smokeShots = 0;
				StartCoroutine (ShowSmokeTrail ());
			}
			yield return new WaitForSeconds (delay);
			soundSource.clip = shotSounds [Random.Range (0, shotSounds.Length)];
			soundSource.volume = shotVolume;
			soundSource.Play ();
			StartCoroutine (DestroyMuzzleFlash (Instantiate (muzzleFlash, muzzleFlashPosition.position, muzzleFlashPosition.rotation) as GameObject));
			GameObject CamObj = GameObject.FindGameObjectWithTag ("PlayerCamera");
			Ray ray = CamObj.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			Vector3 direction = ray.direction;
			if (isAiming == false) {
				direction.x += Random.Range (-hipFireSpread, hipFireSpread);
				direction.y += Random.Range (-hipFireSpread, hipFireSpread);
				direction.z += Random.Range (-hipFireSpread, hipFireSpread);
				ray.direction = direction;
			} else {
				direction.x += Random.Range (-aimFireSpread, aimFireSpread);
				direction.y += Random.Range (-aimFireSpread, aimFireSpread);
				direction.z += Random.Range (-aimFireSpread, aimFireSpread);
				ray.direction = direction;
			}
			RaycastHit[] hits = Physics.RaycastAll (ray, range);
			int hitCount = 0;
			for (int i = 0; i < hits.Length; i++) {
				if (hitCount >= maxPenetration) {
					yield break;
				}

				RaycastHit hit = hits [i];

				if (hit.collider.GetComponent<Health> ()) {
					hit.collider.GetComponent<Health> ().ApplyDamage (damage, this.transform.root.gameObject, true, false);
				} else {
					this.transform.root.GetComponent<WeaponManager> ().RunHitDictionaryCheck (hit.collider.gameObject, hits [i]);
				}
				hitCount++;
			}
		}
	}
		
	// reload your weapon
	public IEnumerator Reload(){
		if (isReloading){
			yield break; // if already reloading... exit and wait till reload is finished
		}
		if (bulletsLeft < bulletsPerClip && numberOfClips > 0) {
			isReloading = true; // we are now reloading
			numberOfClips = numberOfClips - 1; // take away a clip
			GUI.AmmoClips.GetComponent<Text> ().text = numberOfClips.ToString();
			yield return new WaitForSeconds(reloadTime); // wait for set reload time
			bulletsLeft = bulletsPerClip; // fill up the gun
			GUI.AmmoBulletsLeft.GetComponent<Text> ().text = bulletsLeft.ToString();
		}
		isReloading = false; // done reloading
	}
	void OnDrawGizmosSelected() {
		if (canPickup == true) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (transform.position, pickupDistance);
		}
	}

	public void SetStats(Weapon stats) {
		bulletsPerShot = stats.bulletsPerShot;
		bulletsLeft = stats.bulletsLeft;
		numberOfClips = stats.numberOfClips;
	}
}
