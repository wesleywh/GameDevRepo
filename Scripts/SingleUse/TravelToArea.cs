using UnityEngine;
using System.Collections;
using TeamUtility.IO;					//for buttons
using UnityEngine.SceneManagement;		//for switching scenes
using UnityEngine.UI;					//for UI Access
using Pandora.GameManager;

public class TravelToArea : MonoBehaviour {
    [Header("Trigger this by calling \"LoadTargetLevel\" function")]
	[SerializeField] private string sceneIndexOrNameToLoad = "1";
    [SerializeField] private string objectNameToTravelTo = "";
	[SerializeField] private Texture2D loadImage = null;
	[SerializeField] private string loadingTitle = null;
	[SerializeField] private string[] loadingDesc = null;
	[SerializeField] private AudioClip loadingMusic = null;
	[SerializeField] private float keyAccessDistance = 3.0f;
	[SerializeField] private bool locked = false;
    [SerializeField] private string lockedMessage = "Locked";
    [SerializeField] private string successMessage = "";
    [SerializeField] private string managerLockedName = "";
    [SerializeField] private bool managerSetStateOnUnlock = false;
    [SerializeField] private int itemId = 9999999;
    [SerializeField] private ItemType itemType = ItemType.Consumable;
    [SerializeField] private bool destroyItemOnUse = false;
    [SerializeField] private AudioSource unlockSoundSource = null;
    [SerializeField] private AudioClip unlockSound = null;
    [SerializeField] private AudioClip lockSound = null;
	[Range(0,1)]
	[SerializeField] private float audioVolume = 1f;
	private bool canPress = false;
	private AsyncOperation ao;								//allows to track progress of loading
    private GUIManager guiManager = null;
    private TransitionManager tranManager = null;
    private InventoryManagerNew invManager = null;
    private AreaManager areaManager = null;

	void Start() {
        guiManager = dontDestroy.currentGameManager.GetComponent<GUIManager>();
        tranManager = dontDestroy.currentGameManager.GetComponent<TransitionManager>();
        invManager = dontDestroy.currentGameManager.GetComponent<InventoryManagerNew>();
        areaManager = dontDestroy.currentGameManager.GetComponent<AreaManager>();
	}

    public void Unlock()
    {
        if (unlockSound != null && unlockSoundSource != null)
        {
            unlockSoundSource.clip = unlockSound;
            unlockSoundSource.Play();
        }
        if (!string.IsNullOrEmpty(managerLockedName))
        {
            areaManager.SetValue(managerLockedName, managerSetStateOnUnlock);
        }
        if (destroyItemOnUse == true)
        {
            invManager.RemoveFromList(itemId, itemType);
        }
        guiManager.SetPopUpText(successMessage);
        locked = false;
        itemId = 999999;
    }

	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") {
			canPress = true;
		}
	}
	void OnTriggerExit(Collider col) {
		if (col.tag == "Player") {
			canPress = false;
		}
	}

	public void LoadTargetLevel() {
        if (locked == true && invManager.HasItem(itemId))
        {
            Unlock();
        }
        else if (locked == false)
        {
            string desc = loadingDesc[Random.Range(0, loadingDesc.Length)];
            tranManager.LoadTargetLevel(sceneIndexOrNameToLoad,objectNameToTravelTo,loadingMusic,audioVolume,loadingTitle,desc,loadImage);
        }
        else
        {
            if (lockSound != null && unlockSoundSource != null)
            {
                unlockSoundSource.clip = lockSound;
                unlockSoundSource.Play();
            }
            guiManager.SetPopUpText(lockedMessage);
        }
	}
}
