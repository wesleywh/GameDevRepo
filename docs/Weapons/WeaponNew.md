[Back To Navigation Tree](https://wesleywh.github.io/GameDevRepo/docs/navigation.html)
# WeaponNew

## Description:
 Add this to a weapon that will be attached to a weapon manager. Settings specific to a particular weapon.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|W_Type {Sniper, Shotgun, Revolver, Pistol, AK47, M4A1, Skorpin, UMP45, AssaultRifle, Ammo, Grenade, Melee, Rifle }|enum|no default|No description.|
|public|ignoreLayersOnShot|LayerMask|LayerMask()|No description.|
|public|shotDistance|float|400|No description.|
|public|shotPoint|Transform|null|No description.|
|public|addedHipInaccuracy|int|0|No description.|
|public|aimInaccuracy|int|0|No description.|
|public|waitBetweenShots|float|0.05f|No description.|
|public|aimSpeed|float|3f|No description.|
|public|recoilAmount|float|5f|No description.|
|public|damagePerShot|float|20f|No description.|
|public|canReload|bool|true|No description.|
|public|timedReload|bool|true|No description.|
|public|reloadTime|float|2.0f|No description.|
|public|infiniteAmmo|bool|false|No description.|
|public|loaded_ammo|int|7|No description.|
|public|bullets_left|int|10|No description.|
|public|ammo_per_reload|int|7|No description.|
|public|max_ammo|int|7|No description.|
|public|raysPerShot|int|1|No description.|
|public|auto_reload|bool|false|No description.|
|public|grenadeScript|Grenade|null|No description.|
|public|fireWeaponOnDownPress|bool|true|No description.|
|public|canAim|bool|true|No description.|
|public|walkWhenAiming|bool|true|No description.|
|public|weaponSoundSource|AudioSource|null|No description.|
|public|equip|AudioClip[]|no default|No description.|
|public|unequip|AudioClip[]|no default|No description.|
|public|fire|AudioClip[]|no default|No description.|
|public|reload|AudioClip[]|no default|No description.|
|public|empty|AudioClip[]|no default|No description.|
|public|fire_volume|float|1.0f|No description.|
|public|equip_volume|float|1.0f|No description.|
|public|unequip_volume|float|1.0f|No description.|
|public|reload_volume|float|1.0f|No description.|
|public|empty_volume|float|1.0f|No description.|
|public|sniperAim|Texture2D|null|No description.|
|public|zoomSpeed|float|70f|No description.|
|public|aimZoom|float|15|No description.|
|public|zoomEffect|Camera|null|No description.|
|public|recoilEffect|Camera|null|No description.|
|public|muzzleFlash|ParticleSystem|null|No description.|
|public|muzzle|Light|null|No description.|
|public|tracer|ParticleSystem|null|No description.|
|public|ejectShell|GameObject|null|No description.|
|public|ejectPoint|Transform|null|No description.|
|public|ejectDirection|W_Eject|W_Eject.Left|No description.|
|public|blurEdgeAmount|float|0.7f|No description.|
|public|anim|Animator|null|No description.|
|public|recoilSpeed|float|160.0f|No description.|
|public|cameraEffects|Animation|null|No description.|
|public|calmEffect|string|"camera_sway"|No description.|
|public|shotEffect|string|"weapon_shot"|No description.|
|public|disableOverrides|bool|false|No description.|
|public|leftPos|Vector3|Vector3.zero|No description.|
|public|leftRot|Vector3|Vector3.zero|No description.|
|public|rightPos|Vector3|Vector3.zero|No description.|
|public|rightRot|Vector3|Vector3.zero|No description.|
|public|posSpeed|float|0.15f|No description.|
|public|rotSpeed|float|10.0f|No description.|
|public|errorRange|float|0.0001f|No description.|
|public|hipSway|bool|true|No description.|
|public|hipswayMaxAmount|float|0.03f|No description.|
|public|hipswayForwardOffset|float|1.0f|No description.|
|public|hipswayPosSmoothing|float|0.5f|No description.|
|public|hipswayRotSmoothing|float|0.5f|No description.|
|public|hipswayTiltAngle|float|1.0f|No description.|
|public|aimSway|bool|false|No description.|
|public|aimswayMaxAmount|float|0.03f|No description.|
|public|aimswayForwardOffset|float|1.0f|No description.|
|public|aimswayPosSmoothing|float|0.5f|No description.|
|public|aimswayRotSmoothing|float|0.5f|No description.|
|public|aimswayTiltAngle|float|0.5f|No description.|
|public|hipBob|bool|true|No description.|
|public|aimBob|bool|false|No description.|
|public|idleBobSpeed|Vector3|Vector3.zero|No description.|
|public|walkBobAmount|float|0.01f|No description.|
|public|runBobAmount|float|0.03f|No description.|
|public|bobMidpoint|float|0|No description.|
|public|reloadTrigger|string|"reload"|No description.|
|public|reloadBool|string|"reloading"|No description.|
|public|attackTrigger|string|"attack"|No description.|
|public|aimBool|string|"block"|No description.|
|public|shot_offset|Vector3|Vector3.zero|No description.|
|public|hipPosition|Vector3|Vector3.zero|No description.|
|public|hipRotation|Vector3|Vector3.zero|No description.|
|public|hipMoveVelocity|Vector3|Vector3.zero|No description.|
|public|hipMoveSpeed|float|0.3f|No description.|
|public|hipRotationSpeed|float|60f|No description.|
|public|aimPosition|Vector3|Vector3.zero|No description.|
|public|aimRotation|Vector3|Vector3.zero|No description.|
|public|aimMoveVelocity|Vector3|Vector3.zero|No description.|
|public|aimMoveSpeed|float|0.05f|No description.|
|public|aimRotationSpeed|float|60f|No description.|
|public|moveForReload|bool|true|No description.|
|public|reloadPosition|Vector3|Vector3.zero|No description.|
|public|reloadRotation|Vector3|Vector3.zero|No description.|
|public|reloadMoveVelocity|Vector3|Vector3.zero|No description.|
|public|reloadMoveSpeed|float|0.05f|No description.|
|public|reloadRotationSpeed|float|60f|No description.|
|public|name|string|Section"|No description.|
|public|tags|string[]|null|No description.|
|public|particles|ParticleSystem[]|null|No description.|
|public|decals|WeaponPEDecal[]|null|No description.|
|public|bulletDecal|GameObject|null|No description.|
|public|invert|bool|false|No description.|
|public|aimPosition|Vector3|Vector3.zero|No description.|
|public|aimRotation|Vector3|Vector3.zero|No description.|
|public|aimerHorizontal|Texture2D|null|No description.|
|public|aimerVertical|Texture2D|null|No description.|
|public|aimerHit|Texture2D|null|No description.|
|public|increaseAimerEveryShot|float|10f|No description.|
|public|aimerMoveOutSpeed|float|50.0f|No description.|
|public|aimerMoveInSpeed|float|30.0f|No description.|
|public|hideAimerWhenAiming|bool|false|No description.|
|public|disableWhenAiming|GameObject[]|null|No description.|
|public|displayUI|bool|true|No description.|
|public|clipImage|Sprite|null|No description.|
|public|bulletImage|Sprite|null|No description.|
|public|OnReload|UnityEvent|no default|No description.|
|public|FinishedReloading|UnityEvent|no default|No description.|
|public|OnFire|UnityEvent|no default|No description.|
|public|debug_raycast|bool|false|No description.|
|public|aiming|bool|false|No description.|
|public|manuallySetPosition|bool|false|No description.|
|public|autoCalculateRotation|bool|false|No description.|
|public|reloadPosition|bool|false|No description.|
|public|show_screen_hitpoint|bool|false|No description.|
|private|W_FireMode fireMode||W_FireMode.Auto|No description.|
|private|W_ZoomType zoomType||W_ZoomType.Simple|No description.|
|public|weaponType|W_Type|W_Type.Pistol|No description.|
|public|mc|MovementController|null|No description.|
|public|wm|InvWeaponManager|null|No description.|
|public|events|WeaponEvents|null|No description.|
|public|weaponSounds|WeaponSounds|no default|No description.|
|public|baseSettings|WeaponBaseSettings|no default|No description.|
|public|weaponEffects|WeaponEffects|no default|No description.|
|public|animationSettings|WeaponAnimationSettings|no default|No description.|
|public|overridePosition|WeaponPositioning|no default|No description.|
|public|particleEffects|WeaponParticleEffects[]|no default|No description.|
|public|aimerVisuals|WeaponAimerVisuals|no default|No description.|
|public|weaponUI|WeaponUI|no default|No description.|
|public|id|int|9999999|No description.|
|public|debugging|WeaponDebugging|no default|No description.|
|public|CanReload()|bool|no default|No description.|
|public|IsFull()|bool|no default|No description.|
|public|GetEquipSounds() {|AudioClip[]|no default|No description.|
