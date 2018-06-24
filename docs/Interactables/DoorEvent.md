[Back To Navigation Tree](https://wesleywh.github.io/GameDevRepo/docs/navigation.html)
# DoorEvent

## Description:
File has no description.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|DoorEventType {|enum|no default|No description.|
|public|type|DoorEventType|DoorEventType.Open|No description.|
|public|sounds						//sounds to play for the door event type|AudioClip[]|no default|No description.|
|public|doorSoundsOnly|bool|no default|No description.|
|public|allDoorSounds|DoorSounds[]|no default|No description.|
|private|anim|Animation|no default|No description.|
|private|soundSource|AudioSource|no default|No description.|
|private|breakable|bool|no default|No description.|
|private|goToState|DoorEventType|no default|No description.|
|private|goToAnimSpeed|float|1.0f|No description.|
|private|goBackState|DoorEventType|no default|No description.|
|private|goBackAnimSpeed|float|1.0f|No description.|
|private|animBreakSpeed|float|1.0f|No description.|
|private|effectOnBreak|GameObject|no default|No description.|
|private|eventDistance|float|1.5f|No description.|
|private|isLocked|bool|false|No description.|
|private|debugMode|bool|false|No description.|
