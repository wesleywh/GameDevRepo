using UnityEngine;
using System.Collections;
using TeamUtility.IO;
using UnityEngine.UI;

public class Weapon : MonoBehaviour {
	[Header("-----==== Required Parameters ====-----")]
	[Tooltip("Identity of this weapon in your WeaponManager dictionary")]
	public string weaponName = "";

	[Header("Animators")]
	[Tooltip("Animators to play player specific animations on")]
	[SerializeField] private Animator[] anims = null;

	[Header("Shot Statistics")]
	[Tooltip("How fast you can shot this weapon")]
	public float delayPerShot = 0.2f;
	[Tooltip("How much damage each bullet does")]
	public float damage = 10.0f;
	[Tooltip("How much upward movement each shot causes to the camera")]
	public float kickBackAmount = 5.0f;
	[Tooltip("Offset accuracy when aiming")]
	public float aimFireSpread = 0.0f;
	[Tooltip("Offset accuracy when not aiming")]
	public float hipFireSpread = 0.2f;

	[Header("Other")]
	[Tooltip("How long after reload starts you have to wait until you can fire again.")]
	public float reloadTime = 0.2f;
	[Tooltip("How long the camera takes to get into the aim position")]
	public float aimSpeed = 2.0f;

	[Header("Ammo")]
	[Tooltip("How many bullets are ejected for each shot")]
	public int bulletsPerShot = 1;
	[Tooltip("How many bullets you want to start with when this weapon is picked up")]
	public int bulletsLeft = 8;
	[Tooltip("How many bullets each clip will replenish")]
	public int bulletsPerClip = 8;
	[Tooltip("How many reloads this weapon has left.")]
	public int numberOfClips = 3;

	[Header("Raycast Specifics")]
	[Tooltip("Where the raycast will start from")]
	public Transform firePoint = null;
	[Tooltip("How many objects this bullet is allowed to hit before stopping.")]
	public int maxPenetration = 1;
	[Tooltip("How far this raycast will go.")]
	public float range = 300.0f;
	[Header("Smoke Trails")]
	[Header("-----==== Optional Parameters ====-----")]
	[Tooltip("Gameobject holding the particle effect")]
	public GameObject smokeTrail = null;
	[Tooltip("Wait until this many shots have been fired before making this particle effect")]
	public int showSmokeEveryXShots = 3;
	[Tooltip("Where to spawn the particle effect")]
	public Transform smokeTrailPosition = null;
	[Tooltip("How long to wait until the particle effect fades out")]
	public float smokeTrailLife = 2.0f;
	[Tooltip("How long to wait after the X shot is fired before making this particle effect.")]
	public float smokeTrailAppearDelay = 1.0f;
	[Tooltip("How fast to fade out this particle effect.")]
	public float smokeTrailFadeSpeed = 0.2f;
	[Header("Weapon Sounds")]
	[Tooltip("The source of all sound on this weapon.")]
	public AudioSource soundSource = null;
	[Tooltip("Random sound to make for each bullet shot.")]
	public AudioClip[] shotSounds = null;
	[Tooltip("How loud the shot sounds are.")]
	[Range(0,1)]
	public float shotVolume = 0.5f;
	[Range(0,3)]
	public float shotPitch = 1.0f;
	[Tooltip("Random sound to play when reloading")]
	public AudioClip[] reloadSounds = null;
	[Tooltip("How loud the reload sound will be.")]
	[Range(0,1)]
	public float reloadVolume = 0.5f;
	[Range(0,3)]
	public float reloadPitch = 1.0f;
	public AudioClip[] pickupAmmoSounds = null;
	[Range(0,1)]
	public float pickupAmmoVolume = 0.5f;
	[Header("Muzzle Flash")]
	[Tooltip("GameObject holding a muzzle flash particle effect.")]
	public GameObject muzzleFlash = null;
	[Tooltip("Where to place the particle effect when a bullet is fired.")]
	public Transform muzzleFlashPosition = null;
	[Tooltip("How long to leave the particle effect before destroying it.")]
	public float muzzleFlashLifetime = 0.1f;
	[Header("Eject Shell Specifics")]
	[Tooltip("Whether to generate a 'shell' and eject at a random angle every shot")]
	[SerializeField] private bool ejectShell = true;
	[Tooltip("Where to generate the 'shell' object.")]
	[SerializeField] private Transform ejectPoint = null;
	[Tooltip("'shell' object is this one")]
	[SerializeField] private GameObject shellObject = null;
	[Tooltip("How long the shell will last before getting removed")]
	[SerializeField] private float shellLife = 3.0f;
	[Space(10)]
	[Header("As An Item Specifics")]
	[Tooltip("How far away you can be to allow the 'Action' button to pickup this weapon.")]
	[SerializeField] private float pickupDistance = 2.0f;
	[Tooltip("Are you allowed to pickup this weapon?")]
	[SerializeField] private bool canPickup = false;
	[Tooltip("Remove this gameobject when this is picked up?")]
	public bool destroyOnPickup = true;

	//None inspector items
	[HideInInspector] public bool isReloading = false;
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
			if (shellObject) {
				GameObject newShell = Instantiate (shellObject, ejectPoint.position, ejectPoint.rotation) as GameObject; // create empty shell
				//give ejectile a slightly random ejection velocity and direction
				if (newShell.GetComponent<Rigidbody> () == false) {
					newShell.AddComponent<Rigidbody> ();
				}
				newShell.GetComponent<Rigidbody>().velocity = transform.TransformDirection (Random.Range (-2, 2) - 3.0f, Random.Range (-1, 2) + 3.0f, -Random.Range (-2, 2) + 1.0f);
				StartCoroutine (DelayDestory (newShell, shellLife));
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

	private IEnumerator DelayDestory(GameObject obj, float delay) {
		yield return new WaitForSeconds (delay);
		Destroy (obj);
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
			soundSource.pitch = shotPitch;
			soundSource.Play ();
			StartCoroutine (DelayDestory (Instantiate (muzzleFlash, muzzleFlashPosition.position, muzzleFlashPosition.rotation) as GameObject, muzzleFlashLifetime));
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
			if (reloadSounds.Length > 0) {
				soundSource.clip = reloadSounds [Random.Range (0, reloadSounds.Length)];
				soundSource.volume = reloadVolume;
				soundSource.pitch = reloadPitch;
				soundSource.Play ();
			}
			if (anims != null && anims.Length > 0) {
				foreach (Animator anim in anims) {
					anim.SetTrigger ("reload");
				}
			}
			this.transform.root.GetComponent<WeaponManager> ().canAim = false;
			isReloading = true; 							// we are now reloading
			numberOfClips = numberOfClips - 1; 				// take away a clip
			GUI.AmmoClips.GetComponent<Text> ().text = numberOfClips.ToString();
			yield return new WaitForSeconds(reloadTime); 	// wait for set reload time
			bulletsLeft = bulletsPerClip; 					// fill up the gun
			GUI.AmmoBulletsLeft.GetComponent<Text> ().text = bulletsLeft.ToString();
		}
		this.transform.root.GetComponent<WeaponManager> ().canAim = true;
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
