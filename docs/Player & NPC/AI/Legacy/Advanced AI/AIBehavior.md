[Back To Navigation Tree](https://wesleywh.github.io/GameDevRepo/docs/navigation.html)
# AIBehavior

## Description:
File has no description.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|fieldOfView|float|130f|No description.|
|public|visualRange|float|15.0f|No description.|
|public|eyes|Transform|null|No description.|
|public|crouch|Transform|null|No description.|
|public|coverEndEdgeDistance|float|0.25f|No description.|
|public|coverDetectDistance|float|0.75f|No description.|
|public|susiciousReactionTime|float|0.2f|No description.|
|public|hostileReactionTime|float|0.4f|No description.|
|public|susiciousResetTime|float|2.0f|No description.|
|public|hostileResetTime|float|4.0f|No description.|
|public|searchRadius|float|15.0f|No description.|
|public|stopSearchingAfter|float|5.0f|No description.|
|public|visualCheckEveryXFrames|int|3|No description.|
|public|climbLayer|string|"Climb"|No description.|
|public|climbResetTime|float|1.0f|No description.|
|public|walkTurningSpeed|float|1.0f|No description.|
|public|runTurningSpeed|float|2.0f|No description.|
|public|runSpeed|float|4.0f|No description.|
|public|walkSpeed|float|2.0f|No description.|
|public|vaultSpeed|float|0.5f|No description.|
|public|waypoints|GameObject[]|no default|No description.|
|public|waypointType|WaypointType|WaypointType.Loop|No description.|
|public|calmToSuspiciousWait|float|1.0f|No description.|
|public|ccAdjustSpeed|float|1.3f|No description.|
|public|ccDistance|float|1.5f|No description.|
|public|ccOrigin|Transform|null|No description.|
|public|reloadSpeed|float|3.0f|No description.|
|public|fireSpeed|float|1.0f|No description.|
|public|meleeDistance|float|2.0f|No description.|
|public|shotDistance|float|15.0f|No description.|
|public|moveEveryXAttacks|int|1|No description.|
|public|moveRadius|float|3.0f|No description.|
|public|type|CombatType|CombatType.Melee|No description.|
|public|meleeNumbers|float[]|null|No description.|
|public|shotNumbers|float[]|null|No description.|
|public|advanceAfterDamageTime|float|3.0f|No description.|
|public|coverTags|string[]|null|No description.|
|public|showFOV|bool|false|No description.|
|public|showDetections|bool|false|No description.|
|public|showAIState|bool|false|No description.|
|public|currentState|AIStates|AIStates.Calm|No description.|
|public|forceAIState|bool|false|No description.|
|public|setState|AIStates|AIStates.Calm|No description.|
|public|showWaypointPath|bool|false|No description.|
|public|showMeleeDistance|bool|false|No description.|
|public|showShotDistance|bool|false|No description.|
|public|attackNumber|float|no default|No description.|
|public|showCCDistance|bool|false|No description.|
|public|showCCPoints|bool|false|No description.|
|public|triggerDamage|bool|false|No description.|
|public|showCurrentEnemyLocation|bool|false|No description.|
|public|showSearchRadius|bool|false|No description.|
|public|showSelectedCover|bool|false|No description.|
|public|showCoverDetectors|bool|false|No description.|
|public|coverPosition|Vector3|Vector3.zero|No description.|
|public|target|GameObject|null|No description.|
|public|enemyTags|string[]|null|No description.|
|public|senses|Senses|no default|No description.|
|public|movement|Movement|no default|No description.|
|public|combat|Combat|no default|No description.|
|public|updateEveryFrame|int|10|No description.|
|public|debugSettings|Debugging|null|No description.|
|public|currentState|AIStates|AIStates.Calm|No description.|
|public|_attack_number|float|0.0f|No description.|
|public|takenDamage|bool|false|No description.|
|public|inStandingCover|bool|false|No description.|
|public|inCrouchCover|bool|false|No description.|
|public|endLeftCover|bool|false|No description.|
|public|endRightCover|bool|false|No description.|
|public|hlController|HeadLookController|null|No description.|
|public|leader|GameObject|null|No description.|
|public|followPosition|Vector3|Vector3.zero|No description.|
|public|ccLeft|GameObject|null|No description.|
|public|ccRight|GameObject|null|No description.|
|public|vaulting|bool|false|No description.|
|public|climbing|bool|false|No description.|
