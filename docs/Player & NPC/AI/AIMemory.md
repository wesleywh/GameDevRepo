[Back To Navigation Tree](https://wesleywh.github.io/GameDevRepo/docs/navigation.html)
# AIMemory

## Description:
File has no description.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|AIActiveState {Hostile,Suspicious,Calm}|enum|no default|No description.|
|public|fleeDistance|float|5.0f|No description.|
|public|predictionTime|float|1.0f|No description.|
|public|fieldOfView|float|130.0f|No description.|
|public|lightVisualRange|float|15.0f|No description.|
|public|darkVisualRange|float|3.0f|No description.|
|public|hearingDistance|float|10.0f|No description.|
|public|hearThreshold|float|0.05f|No description.|
|public|shoutDistance|float|20.0f|No description.|
|public|meleeRange|float|2.0f|No description.|
|public|fireRange|float|15.0f|No description.|
|public|inaccuracy|float|1.2f|No description.|
|public|rotationSpeed|float|1|No description.|
|public|ignoreLayers|LayerMask|0|No description.|
|public|enemyLayers|LayerMask|0|No description.|
|public|coverLayers|LayerMask|0|No description.|
|public|alertSoundLayers|LayerMask|0|No description.|
|public|friendTags|string[]|null|No description.|
|public|enemyTags|string[]|null|No description.|
|public|waypoints|GameObject[]|null|No description.|
|public|offset|Vector3|Vector3.zero|No description.|
|public|offsetRange|float|1.0f|No description.|
|public|minRangeDamage|float|2.0f|No description.|
|public|maxRangeDamage|float|5.0f|No description.|
|public|minMeleeDamage|float|2.0f|No description.|
|public|maxMeleeDamage|float|5.0f|No description.|
|public|minAttackWait|float|0.5f|No description.|
|public|maxAttackWait|float|5.0f|No description.|
|public|minNumberOfAttacks|int|1|No description.|
|public|maxNumberOfAttacks|int|5|No description.|
|public|runSpeed|float|4.0f|No description.|
|public|walkSpeed|float|1.0f|No description.|
|public|arriveDistance|float|0.1f|No description.|
|public|hostileTime|float|20.0f|No description.|
|public|suspiciousTime|float|5.0f|No description.|
|public|calmReactTIme|float|2.0f|No description.|
|public|minWanderDistance|float|5.0f|No description.|
|public|maxWanderDistance|float|10.0f|No description.|
|public|wanderRate|float|1.0f|No description.|
|public|minPause|float|0.0f|No description.|
|public|maxPause|float|3.0f|No description.|
|public|minStrafeDistance|float|1.0f|No description.|
|public|maxStrafeDistance|float|3.0f|No description.|
|public|coverOffset|float|1.0f|No description.|
|public|startingState|AIActiveState|AIActiveState.Calm|No description.|
|public|extraTrees|ExternalBehaviorTree[]|null|No description.|
|public|calmTree|ExternalBehaviorTree|null|No description.|
|public|suspiciousTree|ExternalBehaviorTree|null|No description.|
|public|hostileTree|ExternalBehaviorTree|null|No description.|
|public|alwaysRootGameObject|bool|true|No description.|
|public|lite|bool|true|No description.|
|public|last_damager|GameObject|null|No description.|
|public|target|GameObject|null|No description.|
|public|waypoint|GameObject|null|No description.|
|public|activeState|AIActiveState|AIActiveState.Calm|No description.|
|public|last_target_position|Vector3|Vector3.zero|No description.|
|public|eyes|GameObject|null|No description.|
|public|groundCast|GameObject|null|No description.|
|private|traits|AITraits|AITraits()|No description.|
|private|positions|AIPositions|AIPositions()|No description.|
|private|health|Health|no default|No description.|
|private|state|AIState|AIState()|No description.|
|public|GetCalmReactTime()|float|no default|No description.|
|public|GetCoverOffset()|float|no default|No description.|
|public|GetCoverLayers()|LayerMask|no default|No description.|
|public|GetAlertSoundLayers()|LayerMask|no default|No description.|
|public|GetStrafeDistance()|float|no default|No description.|
|public|GetMinNumberOfAttacks()|int|no default|No description.|
|public|GetMaxNumberOfAttacks()|int|no default|No description.|
|public|GetNumberOfAttacks()|int|no default|No description.|
|public|GetFleeDistance()|float|no default|No description.|
|public|GetPredictionTime()|float|no default|No description.|
|public|GetCurrentTargetPosition()|Vector3|no default|No description.|
|public|GetCurrentState()|AIActiveState|no default|No description.|
|public|GetRotationSpeed()|float|no default|No description.|
|public|GetFriendTags()|string[]|no default|No description.|
|public|GetShoutDistance()|float|no default|No description.|
|public|GetLastTargetPosition()|Vector3|no default|No description.|
|public|GetPauseTime()|float|no default|No description.|
|public|GetMinPause()|float|no default|No description.|
|public|GetMaxPause()|float|no default|No description.|
|public|GetWanderRate()|float|no default|No description.|
|public|GetMinWanderDistance()|float|no default|No description.|
|public|GetMaxWanderDistance()|float|no default|No description.|
|public|GetArriveDistance()|float|no default|No description.|
|public|GetHostileTime()|float|no default|No description.|
|public|GetSuspiciousTime()|float|no default|No description.|
|public|GetRunSpeed()|float|no default|No description.|
|public|GetWalkSpeed()|float|no default|No description.|
|public|GetMinAttackWait()|float|no default|No description.|
|public|GetMaxAttackWait()|float|no default|No description.|
|public|GetAttackWait()|float|no default|No description.|
|public|GetRangeDamageAmount()|float|no default|No description.|
|public|GetMeleeDamageAmount()|float|no default|No description.|
|public|GetOffsetRange()|float|no default|No description.|
|public|GetOffset()|Vector3|no default|No description.|
|public|GetInaccuracy()|float|no default|No description.|
|public|GetEyes()|GameObject|no default|No description.|
|public|GetGroundCast()|GameObject|no default|No description.|
|public|GetFOV()|float|no default|No description.|
|public|GetFireRange()|float|no default|No description.|
|public|GetMeleeRange()|float|no default|No description.|
|public|GetHearingDistance()|float|no default|No description.|
|public|GetHearingThreshold()|float|no default|No description.|
|public|GetWaypointArray()|GameObject[]|no default|No description.|
|public|GetWaypointList()|List<GameObject>|no default|No description.|
|public|GetWaypo|int|no default|No description.|
|public|GetLastDamagedBy()|GameObject|no default|No description.|
|public|GetTarget()|GameObject|no default|No description.|
|public|GetVisualRange()|float|no default|No description.|
|public|GetNextWaypoint()|GameObject|no default|No description.|
|public|GetClosestWaypoint()|GameObject|no default|No description.|
|public|GetCurrentWaypoint()|GameObject|no default|No description.|
|public|GetIgnoreLayers()|LayerMask|no default|No description.|
|public|GetEnemyLayers()|LayerMask|no default|No description.|
