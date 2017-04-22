/// <summary>
/// Add this to a weapon that will be attached to a weapon manager. 
/// Settings specific to a particular weapon.
/// </summary>
//////////////////
/// Written by: Wesley Haws
/// 
/// Liscense: May use this for any reason. Even Commercially
/// ///////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;					//Custom Input Manager
using UnityEngine.UI;

public enum W_Type {Sniper, Shotgun, Revolver, Pistol, AK47, M4A1, Skorpin, UMP45, AssaultRifle, Ammo }

[RequireComponent(typeof(AudioSource))]
public class WeaponNew : MonoBehaviour {

	enum W_FireMode {Auto, Semi }
	enum W_ZoomType {Simple, Sniper }
	enum W_Eject {Left, Right }
	[SerializeField] W_FireMode fireMode = W_FireMode.Auto;
	[SerializeField] W_ZoomType zoomType = W_ZoomType.Simple;
	public W_Type weaponType = W_Type.Pistol;

	[Header("Weapon Sounds")]
	[SerializeField] AudioSource weaponSoundSource = null;
	[SerializeField] AudioClip[] equip;
	[SerializeField] AudioClip[] unequip;
	[SerializeField] AudioClip[] fire;
	[SerializeField] AudioClip[] reload;
	[SerializeField] AudioClip[] empty;

	[Header("Base Weapon Settings")]
	[SerializeField] int aimInaccuracy = 0;
	[SerializeField] float waitBetweenShots = 0.05f;
	[SerializeField] float aimSpeed = 3f;
	[SerializeField] float recoilAmount = 5f;
	[SerializeField] float damagePerShot = 20f;
	[SerializeField] float reloadTime = 2.0f;
	[SerializeField] int bullet_left = 7;
	public int clips_left = 10;
	[SerializeField] int bulletsPerClip = 7;
	[SerializeField] int bulletsPerShot = 1;

	[Header("Animation Settings")]
	[SerializeField] GameObject[] hideDuringZoom;
	[SerializeField] Vector3 aimPosition = Vector3.zero;
	[SerializeField] private Vector3 startPosition;
	[SerializeField] float recoilSpeed = 160.0f;

	[Header("Weapon Effects")]
	[SerializeField] private Texture2D sniperAim = null;
	[SerializeField] float zoomSpeed = 70f;
	[SerializeField] float aimZoom = 15;
	[SerializeField] Camera zoomEffect = null;
	[SerializeField] Camera recoilEffect = null;
	[SerializeField] ParticleSystem muzzleFlash = null;
	[SerializeField] Light muzzleLight = null;
	[SerializeField] Vector2 fireTracerBetweenXShots = new Vector2(2,4);
	[SerializeField] ParticleSystem tracer = null;
	[SerializeField] GameObject ejectShell = null;
	[SerializeField] Transform ejectPoint = null;
	[SerializeField] W_Eject ejectDirection = W_Eject.Left;

	[Header("Particle Effects")]
	[SerializeField] string tagConcrete = "Concrete";
	[SerializeField] string tagWater = "Water";
	[SerializeField] string tagDirt = "Dirt";
	[SerializeField] string tagBody = "NPC";
	[SerializeField] string tagWood = "Wood";
	[SerializeField] ParticleSystem particleConcrete = null;
	[SerializeField] ParticleSystem particleWater = null;
	[SerializeField] ParticleSystem particleDirt = null;
	[SerializeField] ParticleSystem particleBody = null;
	[SerializeField] ParticleSystem particleWood = null;
	[SerializeField] GameObject bulletDecal = null;

	[Header("Aimer Visuals")]
	[SerializeField] Texture2D aimerHorizontal;
	[SerializeField] Texture2D aimerVertical;
	[SerializeField] Texture2D aimerHit;
	[SerializeField] float increaseAimerEveryShot = 10f;
	[SerializeField] Vector2 aimerHorizontalSize = new Vector2(20,20);
	[SerializeField] Vector2 aimerVerticalSize = new Vector2(20,20);
	[SerializeField] Vector2 aimerHitSize = new Vector2(20,20);
	[SerializeField] Vector2 aimerCenter = new Vector2(10,10);
	[SerializeField] Vector2 aimerEdge = new Vector2 (30, 30);
	[SerializeField] float aimerMoveOutSpeed = 50.0f;
	[SerializeField] float aimerMoveInSpeed = 30.0f;

	[Header("Ammo Counter UI")]
	[SerializeField] Sprite clipImage = null;
	[SerializeField] Sprite bulletImage = null;
	[SerializeField] string UIParent = "GUIAmmo";
	[SerializeField] string UIBulletsLeftTag = "GUIBulletsLeft";
	[SerializeField] string UIClipsLeft = "GUIClips";
	[SerializeField] string UIBulletImageTag = "GUIBulletImage";
	[SerializeField] string UIClipImage = "GUIClipImage";
	[SerializeField] Vector2 UIBulletImageSize = new Vector2(10,50);
	[SerializeField] Vector2 UIClipImageSize = new Vector2(50,50);
	[Space(20)]
	[Header("Debugging")]
	[SerializeField] bool debug_raycast = false;

	private float shot_timer = 0;

	//for base weapon settings
	private Vector2 hit_point;

	//For original values
	private bool aiming = false;
	private bool prev_aim = false;
	private float original_zoom;
	private float original_recoil;
	private float aim_zoom;
	private float aim_recoil;

	[Space(10)]
	//-----For Lerps-----//
	//For Aiming
	private Vector3 endMarker;								//target destination for aim positioning
	private float startTime;								//time marker for aiming positioning
	private float journeyLength;							//for distance coverted with aim positioning
	private float distCovered;								//for aim movement positioning
	private float fracJourney;								//for aim positioning
	private float targetZoom;
	//For Recoil
	private float targetRecoil;
	private bool recoil_backward = false;
	private bool initial_equip = true;
	//------------------//

	//for visuals
	private bool show_snip_texture;
	private int tracer_count = 0;
	private int tracer_shot = 0;
	private float inputX = 0f;
	private float inputY = 0f;
	private float aim_pos_horz = 0;
	private float aim_pos_vert = 0;
	private Text ui_bullets;
	private Text ui_clips;

	private bool can_reload = true;
	private bool can_aim = true;

	void OnStart() {
		startPosition = (startPosition == null) ? this.transform.position : startPosition;
		if (weaponSoundSource == null)
			weaponSoundSource = this.GetComponent<AudioSource> ();
	}

	// When Weapon Is Equiped
	void OnEnable () {
		original_zoom = zoomEffect.fieldOfView;
		original_recoil = recoilEffect.fieldOfView;
		muzzleLight.enabled = false;
		tracer_shot = Random.Range ((int)fireTracerBetweenXShots.x, (int)fireTracerBetweenXShots.y+1);
		PlayEquipSound ();
		SetUIVisuals (true);
	}

	void SetUIVisuals(bool enable) {
		if (GameObject.FindGameObjectWithTag (UIParent) == null)
			return;
		GameObject UI = GameObject.FindGameObjectWithTag (UIParent);
		foreach (Transform obj in UI.transform) {
			if (obj.GetComponent<Image> ()) { 
				obj.GetComponent<Image> ().enabled = (bulletImage == null || clipImage == null) ? false : enable;
				if (obj.GetComponent<Image> ().enabled == true && obj.tag == UIBulletImageTag ) {
					obj.GetComponent<Image> ().sprite = bulletImage;
					obj.GetComponent<RectTransform> ().sizeDelta = UIBulletImageSize;
				}
				else if (obj.GetComponent<Image> ().enabled == true && obj.tag == UIClipImage ) {
					obj.GetComponent<Image> ().sprite = clipImage;
					obj.GetComponent<RectTransform> ().sizeDelta = UIClipImageSize;
				}
			} else if (obj.GetComponent<Text> ()) {
				obj.GetComponent<Text> ().enabled = enable;
			}
		}
		if (enable == true) {
			ui_bullets = GameObject.FindGameObjectWithTag (UIBulletsLeftTag).GetComponent<Text>();
			ui_clips = GameObject.FindGameObjectWithTag (UIClipsLeft).GetComponent<Text>();
			ui_bullets.text = bullet_left.ToString();
			ui_clips.text = clips_left.ToString ();
		}
	}

	//When weapon is dropped or put away
	void OnDisable() {
		SetUIVisuals (false);
		zoomEffect.fieldOfView = original_zoom;
		aiming = false;
		show_snip_texture = false;
		PlayUnequipSound ();
	}

	//-----Actions
	void Update () {
		inputX = InputManager.GetAxis ("Horizontal");
		inputY = InputManager.GetAxis("Vertical");
		SetAimerPosition (inputX, inputY);

		shot_timer = (shot_timer > 0) ? shot_timer - Time.deltaTime : 0;
		if (InputManager.GetButtonDown ("Reload")) {
			Reload ();
		}
		if (InputManager.GetButton ("Attack") && shot_timer == 0 && fireMode == W_FireMode.Auto) {
			FireShot ();
		}
		else if (InputManager.GetButtonDown ("Attack") && shot_timer == 0) {
			FireShot ();
		}
		if (can_aim==true && InputManager.GetButtonDown ("Block")) aiming = true;
		if (InputManager.GetButtonUp ("Block")) aiming = false;
		if (prev_aim != aiming) {
			SetAimValues ();
			SetZoomValues ();
			prev_aim = aiming;
		}
		Aim (aiming);
		Zoom (aiming);
		Recoil ();
	}

	//-----Logic
	void Aim(bool isAiming) {
		//for physical location
		if (isAiming == true && this.gameObject.transform.localPosition != aimPosition) {
			distCovered = (Time.time - startTime) * aimSpeed;
			fracJourney = distCovered / journeyLength;
			this.gameObject.transform.localPosition = Vector3.Lerp (startPosition, aimPosition, fracJourney);
			if(fracJourney > 0.9f && zoomType == W_ZoomType.Sniper) {
				SetSniperTexture (true);
			}
		} else if(isAiming == false && this.gameObject.transform.localPosition != startPosition){
			SetSniperTexture (false);
			distCovered = (Time.time - startTime) * aimSpeed;
			fracJourney = distCovered / journeyLength;
			this.gameObject.transform.localPosition = Vector3.Lerp (aimPosition, startPosition, fracJourney);
		}
	}

	void Zoom(bool isAiming) {
		if (targetZoom == 0) {
			return;
		}
		if (isAiming == true && zoomEffect.fieldOfView != targetZoom) {
			zoomEffect.fieldOfView = (zoomEffect.fieldOfView > targetZoom) ? zoomEffect.fieldOfView - (Time.deltaTime * zoomSpeed) : targetZoom;
		} 
		else if (isAiming == false) {
			zoomEffect.fieldOfView = (zoomEffect.fieldOfView < targetZoom) ? zoomEffect.fieldOfView + (Time.deltaTime * zoomSpeed) : targetZoom;
		}
	}

	void SetSniperTexture(bool enabled) {
		if (enabled == true) {
			show_snip_texture = true;
			foreach (GameObject obj in hideDuringZoom) {
				obj.SetActive (false);
			}
		} else {
			show_snip_texture = false;
			foreach (GameObject obj in hideDuringZoom) {
				obj.SetActive (true);
			}
		}
	}
		
	void Recoil() {
		if (initial_equip == true)
			return;
		if (recoil_backward == true) {
			recoilEffect.fieldOfView = (recoilEffect.fieldOfView > targetRecoil) ? recoilEffect.fieldOfView - (Time.deltaTime * recoilSpeed) : targetRecoil;
			if (recoilEffect.fieldOfView == targetRecoil) {
				SetRecoilValues (false);
			}
		} 
		else if (recoil_backward == false) {
			recoilEffect.fieldOfView = (recoilEffect.fieldOfView < targetRecoil) ? recoilEffect.fieldOfView + (Time.deltaTime * recoilSpeed) : targetRecoil;
		}
	}

	//Effects On fire
	void FireShot() {
		if (shot_timer != 0) return;
		shot_timer = waitBetweenShots;
		if (bullet_left == 0) {
			PlayEmptySound ();
			return;
		}
		bullet_left -= (bullet_left > 0) ? 1 : 0;
		ui_bullets.text = bullet_left.ToString();
		SetRecoilValues(true);
		StartCoroutine(MuzzleFlash ());
		Tracer ();
		PlayFireSound ();
		EjectShell ();
		AddAimerPosition (increaseAimerEveryShot);
		for (int i = 0; i < bulletsPerShot; i++) {
			RaycastDamage ();
		}
	}
		
	void EjectShell() {
		if (ejectPoint == null || ejectShell == null)
			return; 
		GameObject shell = Instantiate (ejectShell, ejectPoint.position, ejectPoint.rotation);
		shell.transform.parent = ejectPoint.transform;
		shell.transform.localPosition = Vector3.zero;
		if (ejectDirection == W_Eject.Right) {
			shell.GetComponent<Rigidbody> ().velocity = ejectPoint.transform.TransformDirection (Vector3.right * Random.Range (1, 3));
		} else {
			shell.GetComponent<Rigidbody> ().velocity = ejectPoint.transform.TransformDirection (Vector3.left * Random.Range (1, 3));
		}
		StartCoroutine(DestroyGameObject (shell, 3));
	}

	void Reload() {
		if (clips_left == 0 || can_reload == false || bulletsPerClip == bullet_left)
			return;
		aiming = false;
		can_aim = false;
		can_reload = false;
		PlayReloadSound ();
		this.GetComponent<Animator> ().SetTrigger ("reload");
		shot_timer = reloadTime;
		clips_left -= 1;
		bullet_left = 0;
		StartCoroutine (UpdateReloadUI ());
	}

	IEnumerator UpdateReloadUI() {
		yield return new WaitForSeconds (reloadTime-0.1f);
		bullet_left += bulletsPerClip;
		ui_bullets.text = bullet_left.ToString();
		ui_clips.text = clips_left.ToString ();
		can_reload = true;
		can_aim = true;
	}

	void RaycastDamage() {
		hit_point = GetHitPoint (aiming);
		Ray ray = zoomEffect.ScreenPointToRay (new Vector2(hit_point.x, hit_point.y));
		if (debug_raycast == true)
			Debug.DrawRay (ray.origin, ray.direction * 1000, new Color (1f, 0.922f, 0.016f, 1f),5.0f);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 400)) {
			if (debug_raycast == true) {
				Debug.Log ("NAME: "+hit.transform.name+", DISTANCE: "+hit.distance+", TAG: "+hit.transform.tag+", POINT: "+hit.point);
			}
			SpawnParticle (hit);
			if (hit.transform.GetComponent<Health> ()) {
				hit.transform.GetComponent<Health> ().ApplyDamage (damagePerShot);
			} else {
				SpawnDecal (hit);
			}
		}
	}

	Vector2 GetHitPoint(bool isAiming) {
		Vector2 hitpoint;
		float x;
		float y;
		if (isAiming == false) {
			x = Random.Range (InputManager.mousePosition.x - aim_pos_horz, InputManager.mousePosition.x + aim_pos_horz);
			y = Random.Range (InputManager.mousePosition.y - aim_pos_vert, InputManager.mousePosition.y + aim_pos_vert);
		} else {
			x = InputManager.mousePosition.x;
			y = InputManager.mousePosition.y;
		}
		x += Random.Range (0, aimInaccuracy);
		y += Random.Range (0, aimInaccuracy);
		hitpoint = new Vector2 (x, y);
		return hitpoint;
	}

	ParticleSystem GetParticle(string tag) {
		ParticleSystem particle = null;

		if (tag == tagConcrete)
			particle = particleConcrete;
		else if(tag == tagBody)
			particle = particleBody;
		else if (tag == tagDirt)
			particle = particleDirt;
		else if (tag == tagWater)
			particle = particleWater;
		else if (tag == tagWood)
			particle = particleWood;
		
		return particle;
	}

	IEnumerator MuzzleFlash() {
		muzzleFlash.Play ();
		if (muzzleLight == null)
			yield break;
		muzzleLight.enabled = true;
		yield return new WaitForSeconds (0.05f);
		muzzleLight.enabled = false;
	}

	void Tracer() {
		tracer_count += 1;
		if (tracer_count >= tracer_shot) {
			tracer.Play ();
			tracer_count = 0;
			tracer_shot = Random.Range ((int)fireTracerBetweenXShots.x, (int)fireTracerBetweenXShots.y+1);
		}
	}

	void SpawnDecal(RaycastHit hit) {
		if (bulletDecal != null){
			GameObject bullet_hit = Instantiate (bulletDecal, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
			StartCoroutine (DestroyGameObject (bullet_hit, 20.0f));
		}
	}

	void SpawnParticle(RaycastHit hit) {
		ParticleSystem particle = GetParticle (hit.transform.tag);
		if (particle == null)
			return;
		ParticleSystem spawned = Instantiate (particle, hit.point, Quaternion.LookRotation(hit.normal)) as ParticleSystem;
		spawned.Play ();
		StartCoroutine (DestroyGameObject (spawned.transform.gameObject, 1.0f));
	}

	//Play Sounds
	void PlayFireSound() {
		if (fire.Length <= 0)
			return;
		weaponSoundSource.clip = fire [Random.Range (0, fire.Length)];
		weaponSoundSource.Play ();
	}

	public void PlayEquipSound() {
		if (equip.Length <= 0)
			return;
		weaponSoundSource.clip = equip [Random.Range (0, equip.Length)];
		weaponSoundSource.Play ();
	}

	void PlayUnequipSound() {
		if (equip.Length <= 0)
			return;
		weaponSoundSource.clip = unequip [Random.Range (0, unequip.Length)];
		weaponSoundSource.Play ();
	}

	void PlayEmptySound() {
		if (equip.Length <= 0)
			return;
		weaponSoundSource.clip = empty [Random.Range (0, empty.Length)];
		weaponSoundSource.Play ();
	}

	void PlayReloadSound() {
		if (reload.Length <= 0)
			return;
		weaponSoundSource.clip = reload [Random.Range (0, reload.Length)];
		weaponSoundSource.Play ();
	}

	//Set Values
	void SetAimValues() {
		startTime = Time.time;
		endMarker = (aiming == true) ? aimPosition : startPosition;
		journeyLength = Vector3.Distance (this.transform.localPosition, endMarker);
	}

	void SetZoomValues() {
		aim_zoom = original_zoom - aimZoom;
		targetZoom = (aiming == true) ? aim_zoom : original_zoom;
	}

	void SetRecoilValues(bool perform_recoil) {
		recoil_backward = perform_recoil;
		aim_recoil = original_recoil - recoilAmount;
		targetRecoil = (perform_recoil == true) ? aim_recoil : original_recoil;
		initial_equip = false;
	}

	void SetAimerPosition(float x, float y) {
		if (x > 0 || y > 0 || x < 0 || y < 0) {
			aim_pos_horz = (aim_pos_horz < aimerEdge.x) ? aim_pos_horz + (Time.deltaTime * aimerMoveOutSpeed) : aimerEdge.x;
			aim_pos_vert = (aim_pos_vert < aimerEdge.x) ? aim_pos_vert + (Time.deltaTime * aimerMoveOutSpeed) : aimerEdge.x;
		} 
		else {
			aim_pos_horz = (aim_pos_horz > aimerCenter.x) ? aim_pos_horz - (Time.deltaTime * aimerMoveInSpeed) : aimerCenter.x;
			aim_pos_vert = (aim_pos_vert > aimerCenter.x) ? aim_pos_vert - (Time.deltaTime * aimerMoveInSpeed) : aimerCenter.x;
		}
	}

	void AddAimerPosition(float amount) {
		if (aim_pos_horz < aimerEdge.x || aim_pos_vert < aimerEdge.y) {
			aim_pos_horz += amount;
			aim_pos_vert += amount;
		} else {
			aim_pos_horz = aimerEdge.x;
			aim_pos_vert = aimerEdge.y;
		}
	}

	public void AddAmmo(int amount) {
		clips_left += amount;
		ui_clips.text = clips_left.ToString();
	}

	public AudioClip[] GetEquipSounds() {
		return equip;
	}

	IEnumerator DestroyGameObject(GameObject target, float delay) {
		yield return new WaitForSeconds (delay);
		Destroy (target);
	}

	void OnGUI() {
		if (zoomType == W_ZoomType.Sniper && show_snip_texture == true) {
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), sniperAim);
		} else {
			if (aimerHorizontal != null) {
				GUI.DrawTexture (new Rect ((Screen.width / 2) - aim_pos_horz, Screen.height / 2, aimerHorizontalSize.x, aimerHorizontalSize.y), aimerHorizontal);
				GUI.DrawTexture (new Rect ((Screen.width / 2) + aim_pos_horz, Screen.height / 2, aimerHorizontalSize.x, aimerHorizontalSize.y), aimerHorizontal);
			}
			if (aimerVertical != null) {
				GUI.DrawTexture (new Rect (Screen.width / 2, (Screen.height / 2) - aim_pos_vert, aimerVerticalSize.x, aimerVerticalSize.y), aimerVertical);
				GUI.DrawTexture (new Rect (Screen.width / 2, (Screen.height / 2) + aim_pos_vert, aimerVerticalSize.x, aimerVerticalSize.y), aimerVertical);
			}
		}
	}
}
