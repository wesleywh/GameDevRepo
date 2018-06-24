[Back To Navigation Tree](https://wesleywh.github.io/githubpages/docs/navigation.html)
# MovementController

## Description:
File has no description.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|WaterTag|string|"Water"|No description.|
|public|slowSpeed|float|0.10f|No description.|
|public|fastSpeed|float|0.30f|No description.|
|public|climbOutSpeed|float|2.0f|No description.|
|public|soundSource|AudioSource|null|No description.|
|public|gravity|float|1.0f|No description.|
|public|enter|mvSwimSound[]|no default|No description.|
|public|exit|mvSwimSound[]|no default|No description.|
|public|volume|float|1.0f|No description.|
|public|loop|bool|false|No description.|
|public|sound|AudioClip|null|No description.|
|public|walkSpeed|float|4.0f|No description.|
|public|runSpeed|float|8.0f|No description.|
|public|toggleRun|bool|false|No description.|
|public|limitDiagonalSpeed|bool|true|No description.|
|public|crouchHeight|float|0.50f|No description.|
|public|standingHeight|float|2.0f|No description.|
|public|toggleCrouch|bool|false|No description.|
|public|jumpSpeed|float|8.0f|No description.|
|public|gravity|float|20.0f|No description.|
|public|fallingDamageThreshold|float|10.0f|No description.|
|public|airControl|bool|false|No description.|
|public|antiBumpFactor|float|.75f|No description.|
|public|antiBunnyHopFactor|int|1|No description.|
|public|notEffectedByGravity|bool|false|No description.|
|public|canJump|bool|true|No description.|
|public|slideOnTagged|string|"Slide"|No description.|
|public|slideWhenOverSlopeLimit|bool|false|No description.|
|public|slideOnTaggedObjects|bool|false|No description.|
|public|slideSpeed|float|12.0f|No description.|
|public|animators|Animator[]|no default|No description.|
|public|swimming_bool|string|"swimming"|No description.|
|public|swimming_water_hit|bool|false|No description.|
|public|swimming_show_exit_point|bool|false|No description.|
|private|wm|InvWeaponManager|no default|No description.|
|private|healthScript|Health|no default|No description.|
|private|playerCam|Camera|null|No description.|
|private|fsController|FootStepKeyFrame|null|No description.|
|private|swimming|MvSwimming|no default|No description.|
|private|groundMovement|MvGround|no default|No description.|
|public|crouching|bool|false|No description.|
|private|jumping|MvJumping|no default|No description.|
|private|sliding|MvSliding|no default|No description.|
|private|anims|MvAnimations|no default|No description.|
|private|debugging|MvDebugging|no default|No description.|
|public|GetForceWalk()|bool|no default|No description.|
|public|GetCurrentMoveSpeed()|float|no default|No description.|
|public|GetWalkSpeed()|float|no default|No description.|
|public|GetRunSpeed()|float|no default|No description.|
|public|GetLockedMovement()|bool|no default|No description.|
|public|GetGravity()|float|no default|No description.|
|public|GetSwimState()|bool|no default|No description.|
