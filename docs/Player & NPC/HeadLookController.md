[Back To Navigation Tree](https://wesleywh.github.io/githubpages/docs/navigation.html)
# HeadLookController

## Description:
 Script used to make the head or eyes smoothly look toward target.

 Original script found here: http://wiki.unity3d.com/index.php/HeadLookController

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|first|Transform|no default|No description.|
|public|last|Transform|no default|No description.|
|public|thresholdAngleDifference|float|0|No description.|
|public|bendingMultiplier|float|0.6f|No description.|
|public|maxAngleDifference|float|30|No description.|
|public|maxBendingAngle|float|80|No description.|
|public|responsiveness|float|5|No description.|
|public|joint|Transform|no default|No description.|
|public|effect|float|0|No description.|
|public|lockToPlayer|bool|true|No description.|
|public|currentTarget|Transform|null|No description.|
|public|rootNode|Transform|no default|No description.|
|public|segments|BendingSegment[]|no default|No description.|
|public|nonAffectedJoints|NonAffectedJoints[]|no default|No description.|
|public|headLookVector|Vector3|Vector3.forward|No description.|
|public|headUpVector|Vector3|Vector3.up|No description.|
|public|target|Vector3|Vector3.zero|No description.|
|public|effect|float|1|No description.|
|public|overrideAnimation|bool|false|No description.|
