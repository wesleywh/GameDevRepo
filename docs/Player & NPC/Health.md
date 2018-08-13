[Back To Navigation Tree](https://wesleywh.github.io/GameDevRepo/docs/navigation.html)
# Health

## Description:
File has no description.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|anims|Animator[]|null|No description.|
|public|deathNumbers|float[]|null|No description.|
|public|ragdollDeath|bool|true|No description.|
|public|delayRagdollEffects|float|0.0f|No description.|
|public|parentCameraOnDeath|Transform|null|No description.|
|public|displayEffects|bool|true|No description.|
|public|SplatterEffects|RainCameraController|null|No description.|
|public|BloodCorners|RainCameraController|null|No description.|
|public|guiOnHit|Texture2D|null|No description.|
|public|guiFadeSpeed|float|2.0f|No description.|
|public|lastDamager|GameObject|null|No description.|
|public|health|float|100.0f|No description.|
|public|maxHealth|float|100.0f|No description.|
|public|regeneration|float|0.0f|No description.|
|public|player|Camera|null|No description.|
|public|changeTagInChildren|bool|false|No description.|
|public|changeLayerInChildren|bool|true|No description.|
|public|changeTagOnDeath|string|"Untagged"|No description.|
|public|changeLayerOnDeath|string|"Dead"|No description.|
|public|destroyOnDeath|bool|false|No description.|
|public|destroyWait|float|120.0f|No description.|
|public|eventsOnDeath|UnityEvent|no default|No description.|
|public|OnDamaged|UnityEvent|no default|No description.|
|public|volume|float|0.5f|No description.|
|public|audioSource|AudioSource|null|No description.|
|public|hitSounds|AudioClip[]|null|sounds to play when this object is damaged|
|public|gainHealthSounds|AudioClip[]|null|sound to play when gaining health|
|public|deathSounds|AudioClip[]|null|No description.|
|public|showHealth|bool|false|for debugging|
|public|detectDirHit|bool|false|for debugging|
|public|applyDamage|bool|false|for debugging|
|public|applyHealth|bool|false|No description.|
|private|isPlayer|bool|false|No description.|
|private|baseSettings|HealthSettings|no default|No description.|
|private|ui|HealthUI|no default|No description.|
|private|animations|HealthAnimators|no default|No description.|
|private|sounds|HealthSounds|no default|No description.|
|private|memory|HealthMemory|no default|No description.|
|private|debugging|HealthDebugging|no default|No description.|
|public|GetHealth() {|float|no default|No description.|
|public|GetRegeneration() {|float|no default|No description.|
|public|GetMaxHealth()|float|no default|No description.|
|public|GetLastDamager()|GameObject|no default|No description.|
