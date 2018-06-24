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
|public|weaponSource|AudioSource|null|No description.|
|public|weaponSounds|AudioClip[]|null|No description.|
|private|AIVisuals visuals||AIVisuals()|No description.|
|private|AIAnimator animator||AIAnimator()|No description.|
|private|AISounds sounds||AISounds()|No description.|
|private|memory|AIMemory|null|No description.|
|private|isDebugging|bool|false|No description.|
|public|GetNearestPlayer()|GameObject|no default|No description.|
|public|GetWeaponSoundSource()|AudioSource|no default|No description.|
|public|GetWeaponSound()|AudioClip|no default|No description.|
|public|GetWeapounSoundsLength()|int|no default|No description.|
