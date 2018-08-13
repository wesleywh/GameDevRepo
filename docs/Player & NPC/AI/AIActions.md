[Back To Navigation Tree](https://wesleywh.github.io/GameDevRepo/docs/navigation.html)
# AIActions

## Description:
File has no description.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|muzzleFlash|ParticleSystem|null|No description.|
|public|bodyHit|ParticleSystem|null|No description.|
|public|heal|ParticleSystem|null|No description.|
|public|anim|Animator|null|No description.|
|public|rangeAttacks|float[]|float[1]{1.0f}|No description.|
|public|standAttacks|float[]|float[9]{0.1818182f,0.2727273f,0.3636364f,0.4545455f,0.5454546f,0.7272727f,0.8181818f,0.9090909f,1.0f}|No description.|
|public|crouchAttacks|float[]|float[2]{0.0f,1.0f}|No description.|
|public|standBlock|float[]|}|No description.|
|public|takedownAttack|float[]|float[12]{0.0f,0.09090f,0.18181f,0.27272f,0.363636f,0.454545f,0.545454f,0.636363f,0.727272f,0.818181f,0.909090f,1.0f}|No description.|
|public|hostile|string|"hostile"|No description.|
|public|suspicious|string|"suspicious"|No description.|
|public|sneak|string|"sneak"|No description.|
|public|climbing|string|"climbing"|No description.|
|public|rangeAttackTrigger|string|"rangeAttack"|No description.|
|public|meleeAttackTrigger|string|"melee"|No description.|
|public|blockNumber|string|"blockNumber"|No description.|
|public|climbNumber|string|"climbNumber"|No description.|
|public|takedownNumber|string|"takedownNumber"|No description.|
|public|meleeNumber|string|"meleeNumber"|No description.|
|public|rangeNumber|string|"rangeNumber"|No description.|
|public|deadNumber|string|"deadNumber"|No description.|
|public|damagedNumber|string|"damagedNumber"|No description.|
|public|defaultSource|AudioSource|null|No description.|
|public|weapons|AIWeaponSounds|null|No description.|
|public|voice|AIVoiceSounds|null|No description.|
|public|physicals|AIPhysicalSounds|null|No description.|
|public|grabPoint|Transform|null|No description.|
|public|resetTime|float|4.0f|No description.|
|public|throwDistance|float|5.0f|No description.|
|public|throwHeight|float|2.0f|No description.|
|public|throwMoveTime|float|1.0f|No description.|
|public|grabMoveSpeed|float|4.0f|No description.|
|public|grabRotateSpeed|float|4.0f|No description.|
|public|layingTime|float|1.0f|No description.|
|public|standTime|float|2.0f|No description.|
|public|target|GameObject|null|No description.|
|public|triggerGrab|bool|false|No description.|
|public|triggerThrow|bool|false|No description.|
|public|curTarget|GameObject|null|No description.|
|public|throwing|bool|false|No description.|
|public|stand_up|bool|false|No description.|
|public|grabbing|bool|false|No description.|
|public|grabbed|bool|false|No description.|
|public|timer|float|0.0f|No description.|
|public|can_stand|bool|false|No description.|
|public|targetRotation|Quaternion|no default|No description.|
|public|cur_pos|Vector3|Vector3.zero|No description.|
|public|final_pos|Vector3|Vector3.zero|No description.|
|public|start_pos|Vector3|Vector3.zero|No description.|
|public|start_rot|Quaternion|Quaternion.identity|No description.|
|public|pm|PlayerManager|null|No description.|
|public|p_control|bool|true|No description.|
|public|weaponSource|AudioSource|null|No description.|
|public|weaponSounds|AudioClip[]|null|No description.|
|public|voiceSource|AudioSource|null|No description.|
|public|effortSounds|AudioClip[]|null|No description.|
|public|hurt|AudioClip[]|null|No description.|
|public|physicalSource|AudioSource|null|No description.|
|public|swings|AudioClip[]|null|No description.|
|public|punchHits|AudioClip[]|null|No description.|
|private|AIVisuals visuals||AIVisuals()|No description.|
|private|AIAnimator animator||AIAnimator()|No description.|
|private|AISounds sounds||AISounds()|No description.|
|private|AIGrabThrow grabthrowSettings||AIGrabThrow()|No description.|
|private|memory|AIMemory|null|No description.|
|private|isDebugging|bool|false|No description.|
|public|GetNearestPlayer()|GameObject|no default|No description.|
|public|GetWeaponSoundSource()|AudioSource|no default|No description.|
|public|GetWeaponSound()|AudioClip|no default|No description.|
|public|GetWeapounSoundsLength()|int|no default|No description.|
