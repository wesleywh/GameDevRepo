[Back To Navigation Tree](https://wesleywh.github.io/GameDevRepo/docs/navigation.html)
# GameplayEvent

## Description:
File has no description.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|ParameterType {Boolean, Float, Int, String, Vector3, AudioClip, GameObject}|enum|no default|No description.|
|public||ParameterType|ParameterType.Boolean|No description.|
|public|param|string|""|No description.|
|public|param|Vector3|Vector3.zero|No description.|
|public|paramConvertToVector3|Transform|null|No description.|
|public|clip|AudioClip|null|No description.|
|public|gameObject|GameObject|null|No description.|
|public|name|string|""|not this is only for referencing in the inspector|
|public|delay|float|0.0f|No description.|
|public|enableObject|GameObject|null|No description.|
|public|disableObject|GameObject|null|No description.|
|public|destroyObject|GameObject|null|No description.|
|public|destroyWithName|string|""|No description.|
|public|destroyWithTag|string|""|No description.|
|public|destroyThis|bool|false|No description.|
|public|spawnObject|GameObject|null|No description.|
|public|spawnLocation|Transform|null|No description.|
|public|gameManagerVarToSet|string|null|No description.|
|public|setVarTo|string|null|No description.|
|public|useThisObject|bool|false|No description.|
|public|tagOfHolder|string|null|No description.|
|public|nameOfHolder|string|null|No description.|
|public|Component|string|null|No description.|
|public|componentFunction|string|null|No description.|
|public|componentEnabled|bool|true|No description.|
|public|parameters|EventParams[]|null|No description.|
|public|other|UnityEvent|no default|No description.|
|public|triggerForPlayerOnly|bool|false|No description.|
|public|triggerOnceOnly|bool|true|No description.|
|public|triggerOnceOnSuccess|bool|false|No description.|
|public|triggerForTagOnly|string|""|No description.|
|public|triggerForNameOnly|string|""|No description.|
|private|TypeOfEvent|eventType|eventType.Delayed|No description.|
|private|refresh_type|refreshType|refreshType.FixedUpdate|No description.|
|private|delay|float|2.0f|No description.|
|private|button|string|"Action"|No description.|
|private|varType|fieldType|fieldType.Boolean|No description.|
|private|varname|string|""|No description.|
|private|additionalVars|string[]|null|No description.|
|private|varSetTo|string|""|No description.|
|private|onlyAllowIfVarSet|bool|false|No description.|
|private|performCheckOnUpdate|bool|false|No description.|
|private|actionsToTake|EventAction[]|no default|No description.|
|private|debugEventCall|bool|false|No description.|
|private|debugVarCheck|bool|false|No description.|
|private|callEvent|bool|false|No description.|
|private|debugParams|bool|false|No description.|
