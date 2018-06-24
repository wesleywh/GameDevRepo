[Back To Navigation Tree](https://wesleywh.github.io/githubpages/docs/navigation.html)
# SwimController

## Description:
File has no description.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|volume|float|1.0f|No description.|
|public|loop|bool|false|No description.|
|public|sound|AudioClip|null|No description.|
|public|underwater_offset|float|0.25f|No description.|
|public|swimming|bool|false|No description.|
|private|water_height|float|0.0f|No description.|
|private|swimSpeed|float|0.1f|No description.|
|private|swimFastSpeed|float|0.2f|No description.|
|private|swimTowardCamLook|string|"PlayerCamera"|No description.|
|public|climbController|ClimbController|no default|No description.|
|public|moveController|MovementController|no default|No description.|
|public|weaponManager|InvWeaponManager|no default|No description.|
|public|fogAmount|float|0.04f|No description.|
|public|enterWaterSplash|GameObject|no default|No description.|
|public|underwaterBubbles|GameObject|no default|No description.|
|public|topOfWaterDisturbance|GameObject|no default|No description.|
|public|enterPool|RainCameraController|no default|No description.|
|public|exitPool|RainCameraController|no default|No description.|
|public|anims|Animator[]|no default|No description.|
|public|slowSpeed|float|0.5f|No description.|
|public|fastSpeed|float|1.0f|No description.|
|public|swimClipName|string|"swim"|No description.|
|public|orgClipName|string|"camera_sway"|No description.|
|public|camAnimHolder|Animation|no default|No description.|
|public|headBobScript|HeadBobber|no default|No description.|
|public|noMoveSoundDelay|float|3f|No description.|
|public|moveSlowSoundDelay|float|1f|No description.|
|public|moveFastSoundDelay|float|0.5f|No description.|
|public|aboveWaterMovement|SoundOptions[]|no default|No description.|
|public|underWaterMovement|SoundOptions[]|no default|No description.|
|public|enterWaterSounds|SoundOptions[]|no default|No description.|
|public|exitWaterSounds|SoundOptions[]|no default|No description.|
|public|enterUnderWaterSounds|SoundOptions[]|no default|No description.|
|public|exitUnderWaterSounds|SoundOptions[]|no default|No description.|
|public|soundSource|AudioSource|null|No description.|
|public|moveSource|AudioSource|null|No description.|
