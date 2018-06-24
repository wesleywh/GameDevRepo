[Back To Navigation Tree](https://wesleywh.github.io/githubpages/docs/navigation.html)
# InventoryManagerNew

## Description:
File has no description.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|ItemType { Consumable, Weapon }|enum|no default|No description.|
|public|name|string|""|No description.|
|public|id|int|0|No description.|
|public|description|string|""|No description.|
|public|dropable|bool|true|No description.|
|public|type|ItemType|ItemType.Consumable|No description.|
|public|icon|Sprite|null|No description.|
|public|drop|GameObject|null|No description.|
|public|allowMultiple|bool|true|No description.|
|public|destroyOnUse|bool|true|No description.|
|public|eventsOnUse|UnityEvent|null|No description.|
|public|eventsOnMultiple|UnityEvent|null|No description.|
|public|useSounds|AudioClip[]|no default|No description.|
|public|dropSounds|AudioClip[]|no default|No description.|
|public|pickupSounds|AudioClip[]|no default|No description.|
|public|id|int|9999999|No description.|
|public|amount|int|0|No description.|
|public|target|InventoryItem|null|No description.|
|public|id|int|9999999|No description.|
|public|slot|int|9999999|No description.|
|public||float|0|No description.|
|private|dictionary|InventoryItem[]|no default|No description.|
|private|ui|GameObject|null|No description.|
|private|soundPlayer|AudioSource|no default|No description.|
|private|inventoryFull|string|inventory."|No description.|
|private|dropFail|string|item."|No description.|
|private|dropPointTag|string|"ItemDropPoint"|No description.|
|private|weapons|InventoryList[]|InventoryList[2]|No description.|
|private|consumables|InventoryList[]|InventoryList[3]|No description.|
|private|targetItem|SlotItem|SlotItem()|No description.|
|public|GetWeapons()|InventoryList[]|no default|No description.|
|public|GetConsumables()|InventoryList[]|no default|No description.|
|public|GetTargetItem()|SlotItem|no default|No description.|
|public|ReturnInventoryWeapons()|InventoryList[]|no default|No description.|
|public|ReturnInventoryConsumables()|InventoryList[]|no default|No description.|
