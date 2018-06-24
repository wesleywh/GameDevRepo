[Back To Navigation Tree](https://wesleywh.github.io/githubpages/docs/navigation.html)
# GUIManager

## Description:
 Used to control anything gui related for the player. This controls things like the inventory screen, cutscene look, pause menu, start screen, etc. You access and minipulate these objects via this script. This script should know everything.

## Variables:
List of variables that you can modify in the inspector.

|Access|Name|Type|Default Value|Description|
|---|---|---|---|---|
|public|bullet|Image|no default|GUIAmmo option, UI Image for loaded bullets.|
|public|clip|Image|no default|GUIAmmo option, UI Image for reload bullets.|
|public|clip_number|Text|no default|GUIAmmo option, UI Text to show number of bullets that can be reloaded.|
|public|bullets_left|Text|no default|GUIAmmo option, UI Text to show number of bullets remaining in selected weapon.|
|public|parent|GameObject|null|LoadingScreen option, for traversal/master control of loading screen elements.|
|public|background|RawImage|null|LoadingScreen option, Backgroun image to show while loading.|
|public|title|Text|null|LoadingScreen option, Title text for loading.|
|public|description|Text|null|LoadingScreen option, Description text for loading.|
|public|loadingBar|GameObject|null|LoadingScreen option, gameobject holding scrollbar to indicate how far along the loading process is.|
|public|others|GameObject[]|null|LoadingScreen option, gameobjects that are only involved with the loading screen. Enables/Disables with loading screen.|
|public|popUp|Text|null|InteractiveElements option, UI Text for interactable items.|
|public|objectives|Text|null|InteractiveElements option, UI Text to show when pressing the objectives button.|
|public|dialog|Text|null|InteractiveElements option, UI Text for showing dialog.|
|public|scrollBackground|Image|null|GUIScroll option, UI Image to display underneath the UI Text.|
|public|scroll|Text|null|GUIScroll option, UI Text to display for the player to read.|
|public|closeHint|Text|null|GUIScroll option, UI Text to display to notify the player how to close this screen.|
|public|parent|GameObject|null|GUIInventory option, for traversal/master control|
|public|mainWindow|GameObject|null|GUIInventory option, gameobject to enable when inventory button is pressed. For inventory screen.|
|public|quickSlots|GameObject|null|GUIInventory option, gameobject to enable when inventory button is pressed. For quickslots.|
|public|parent|GameObject|null|GUIMenu option, parent object holding everything in GUIMenu options, for master control/traversal.|
|public|mainMenu|GameObject|null|GUIMenu option, menu to enable when opening your menu screen.|
|public|newGameBtn|GameObject|null|GUIMenu option, gameobject holding UI button that is designed to trigger a new game|
|public|saveGameBtn|GameObject|null|GUIMenu option, gameobject holding UI button that is designed to trigger saving your game|
|public|picture|GameObject|null|GUIMenu option, a large picture to show when the menu is active|
|public|otherMenus|GameObject[]|null|GUIMenu option, other menus to disable when menu is disabled|
|public|bossHealthBar|GameObject|null|GUIMenu option, parent gameobject for both scrollbars|
|public|bossHealthRedBar|GameObject|null|GUIMenu option, gameobject holding scrollbar for top health bar color|
|public|bossHealthYellowBar|GameObject|null|GUIMenu option, gameobject holding scrollbar for underneath health bar color.|
|public|title|Text|null|GUILoadGameSlots option, location of this save.|
|public|description|Text|null|GUILoadGameSlots option, a few character stats to help identify this save.|
|public|saveTime|Text|null|GUILoadGameSlots option, the computer time when this game was saved.|
|public|save|Image|null|GUILoadGameSlots option, this is the snapshot when the game was saved.|
|private|inventoryTimeScale|float|0.1f|Between 0 - 1. How slow everything else will move while the inventory is open.|
|private|menuTimeScale|float|0.0f|Between 0 - 1. How slow everything else will move while the menu is open. (0 = don't move at all)|
|public|cutsceneDisable|GameObject[]|no default|What GUI objects to disable while in "cutscene" mode.|
|public|qs|InventoryQuickSlot[]|null|Outside script reference, for manipulating gui quick slots on the screen.|
|public|objectives|GameObject|null|Object that holds the objective text, used for showing/hiding objectives on gui.|
|public|saveGameScreen|GameObject|null|gameobject holding UI Image component. Object to enable when saving your game.|
|public|saveWarningScreen|GameObject|null|object to enable when trying to save a game to a slot that already exists.|
|private|inventoryOpen|bool|false|Debugging purposes only, enable inventory gui|
|private|menuOpen|bool|true|Debugging purposes only, enable menu gui|
|private|isCutscene|bool|false|Debugging purposes only, enable cutscene gui (This also disables others)|
|public|ReturnPopUp|Text|no default|No description.|
|public|ReturnQuickSlots()|GameObject|no default|No description.|
|public|MenuOrInventoryOpen()|bool|no default|No description.|
|public|MenuOpen()|bool|no default|No description.|
