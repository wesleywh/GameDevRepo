using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CyberBullet.Controllers;

namespace CyberBullet.GameManager {
    public enum ItemType { Consumable, Weapon }
    [System.Serializable]
    public class InventoryItem {
        public string name = "";
        public int id = 0;
        public string description = "";
        public bool dropable = true;
        public ItemType type = ItemType.Consumable;
        public Sprite icon = null;
        public GameObject drop = null;
        public bool allowMultiple = true;
        public bool destroyOnUse = true;
        public UnityEvent eventsOnUse = null;
        public UnityEvent eventsOnMultiple = null;
        public AudioClip[] useSounds;
        public AudioClip[] dropSounds;
        public AudioClip[] pickupSounds;
    }
    [System.Serializable]
    public class InventoryList {
        public int id = 9999999;
        public int amount = 0;
    }
    [System.Serializable]
    public class SlotItem {
        public InventoryItem target = null;
        public int id = 9999999;
        public int slot = 9999999;
    }
    public class InventoryManagerNew : MonoBehaviour {
        [HideInInspector] public float float_var = 0;
        [SerializeField] private  InventoryItem[] dictionary;
        [Space(10)]
        [SerializeField] private GameObject ui = null;
        [SerializeField] private AudioSource soundPlayer;

        [SerializeField] private string inventoryFull = "You cannot carry anymore items in your inventory.";
        [SerializeField] private string dropFail = "You are not allowed to drop this item.";
        [SerializeField] private string dropPointTag = "ItemDropPoint";

        [Space(10)]
        [Header("Debug Purposes Only, Don't Edit Directly")]
        [SerializeField] private InventoryList[] weapons = new InventoryList[2];
        [SerializeField] private InventoryList[] consumables = new InventoryList[3];
        [SerializeField] private SlotItem targetItem = new SlotItem();
        private GameObject gameManager = null;
        private InvWeaponManager wm = null;
        private GUIManager guiManager = null;

        void Start()
        {
            gameManager = dontDestroy.currentGameManager;
            guiManager = gameManager.GetComponent<GUIManager>();
            wm = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<InvWeaponManager>();
        }
        private IEnumerator RefreshUI()
        {
            guiManager.ReturnQuickSlots().GetComponent<UseQuickSlots>().UpdateQuickSlots();
            ui.SetActive(false);
            yield return new WaitForSeconds(0.0001f);
            ui.SetActive(true);
        }
        public bool AddItem(int id) 
        {
            bool ret_val = true;
            InventoryItem item = GetItemInDictionary(id);
            if (item.allowMultiple == false && HasItem(id))
            {
                item.eventsOnMultiple.Invoke();
                PlaySound(item.pickupSounds);
            }
            else if (InventoryIsFull(item.type))
            {
                guiManager.SetPopUpText(inventoryFull);
                ret_val = false;
            }
            else
            {
                if (HasItem(id))
                {
                    item.eventsOnMultiple.Invoke();
                }
                PlaySound(item.pickupSounds);
                AddType(item.type, id);
            }
            return ret_val;
        }
        public void DropItem(int id)
        {
            InventoryItem item = GetItemInDictionary(id);
            if (!item.dropable)
            {
                guiManager.SetPopUpText(dropFail);
            }
            else
            {
                PlaySound(item.dropSounds);
                RemoveFromList(id,item.type);
                Transform dropPoint = GameObject.FindGameObjectWithTag(dropPointTag).transform;
                Instantiate(item.drop, dropPoint.position, dropPoint.rotation);
            }
            if (item.type == ItemType.Weapon)
            {
                AssignInvWeaponManager();
                wm.DeselectWeapon(item.id);
            }
            StartCoroutine(RefreshUI());
        }
        public void DestroyItem(int id)
        {
            InventoryItem item = GetItemInDictionary(id);
            if (!item.dropable)
            {
                guiManager.SetPopUpText(dropFail);
            }
            else
            {
                PlaySound(item.dropSounds);
                RemoveFromList(id,item.type);
            }
            if (item.type == ItemType.Weapon)
            {
                AssignInvWeaponManager();
                wm.DeselectWeapon(item.id);
            }
            StartCoroutine(RefreshUI());
        }
        void AssignInvWeaponManager()
        {
            if (wm == null)
                wm = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<InvWeaponManager>();
        }
        public void RemoveFromList(int id, ItemType type)
        {
            switch (type)
            {
                case ItemType.Consumable:
                    foreach (InventoryList consumable in consumables)
                    {
                        if (consumable.id == id)
                        {
                            consumable.amount = 0;
                            consumable.id = 9999999;
                            break;
                        }
                    }
                    break;
                case ItemType.Weapon:
                    foreach (InventoryList weapon in weapons)
                    {
                        if (weapon.id == id)
                        {
                            weapon.amount = 0;
                            weapon.id = 9999999;
                            break;
                        }
                    }
                    break;
            }
            StartCoroutine(RefreshUI());
        }
        public InventoryItem GetItemInDictionary(int id)
        {
            InventoryItem ret_val = null;
            foreach (InventoryItem item in dictionary)
            {
                if (item.id == id)
                {
                    ret_val = item;
                    break;
                }
            }
            return ret_val;
        }
        bool InventoryIsFull(ItemType type)
        {
            bool ret_val = true;
            switch (type)
            {
                case ItemType.Consumable:
                    foreach (InventoryList consumable in consumables)
                    {
                        if (consumable == null || consumable.id == 9999999)
                        {
                            ret_val = false;
                            break;
                        }
                    }
                    break;
                case ItemType.Weapon:
                    foreach (InventoryList weapon in weapons)
                    {
                        if (weapon.amount == 0)
                        {
                            ret_val = false;
                            break;
                        }
                    }
                    break;
            }
            return ret_val;
        }
        void AddType(ItemType type, int id)
        {
            switch (type)
            {
                case ItemType.Consumable:
                    for (int i=0; i < consumables.Length; i++)
                    {
                        if (consumables[i] == null || consumables[i].id == 9999999 || consumables[i].amount == 0)
                        {
                            InventoryList newItem = new InventoryList();
                            newItem.id = id;
                            newItem.amount = 1;
                            consumables[i] = newItem;
                            break;
                        }
                    }
                    break;
                case ItemType.Weapon:
                    for(int i = 0; i < weapons.Length; i++)
                    {
                        if (weapons[i] == null || weapons[i].id == 9999999 || weapons[i].amount == 0)
                        {
                            InventoryList newItem = new InventoryList();
                            newItem.id = id;
                            newItem.amount = 1;
                            weapons[i] = newItem;
                            break;
                        }
                    }
                    break;
            }
        }
        public InventoryList[] GetWeapons()
        {
            return weapons;
        }
        public InventoryList[] GetConsumables()
        {
            return consumables;
        }
        public void AddTargetItem(InventoryItem item, int guiindex)
        {
            if (item == null)
                return;
            if (item.id == 9999999)
                return;
            SlotItem newItem = new SlotItem();
            newItem.target = item;
            newItem.slot = guiindex;
            newItem.id = item.id;
            targetItem = newItem;
        }
        InventoryItem GetItem(int index, ItemType type)
        {
            InventoryItem ret_val = null;
            switch (type)
            {
                case ItemType.Consumable:
                    ret_val = GetItemInDictionary(consumables[index].id);
                    break;
                case ItemType.Weapon:
                    ret_val = GetItemInDictionary(weapons[index].id);
                    break;
            }
            return ret_val;
        }
        public void RemoveTargetItem()
        {
            targetItem = null;
        }
        public SlotItem GetTargetItem()
        {
            return targetItem;
        }
        public bool UseItem(int id)
        {
            bool destroyMe = false;
            InventoryItem item = GetItemInDictionary(id);
            PlaySound(item.useSounds);
            item.eventsOnUse.Invoke();
            if (item.destroyOnUse == true)
            {
                destroyMe = true;
                RemoveFromList(id, item.type);
            }
            StartCoroutine(RefreshUI());
            return destroyMe;
        }
        void PlaySound(AudioClip[] sounds)
        {
            if (sounds.Length < 1)
                return;
            soundPlayer.clip = sounds[Random.Range(0, sounds.Length - 1)];
            soundPlayer.Play();
        }
        public bool HasItem(int id)
        {
            foreach (InventoryList weapon in weapons)
            {
                if (weapon.id == id)
                    return true;
            }
            foreach (InventoryList consumable in consumables)
            {
                if (consumable.id == id)
                    return true;
            }
            return false;
        }
        public InventoryList GetWeaponAtIndex(int index)
        {
            return weapons[index];
        }
        public int GetWeaponIndex(int id)
        {
            for (int i =0; i<weapons.Length; i++)
            {
                if (weapons[i].id == id)
                    return i;
            }
            return 9999999;
        }
        public InventoryList[] ReturnInventoryWeapons()
        {
            return weapons;
        }
        public InventoryList[] ReturnInventoryConsumables()
        {
            return consumables;
        }
        public void SetInventoryWeapons(InventoryList[] weaponsToSet)
        {
            weapons = weaponsToSet;
        }
        public void SetInventoryConsumables(InventoryList[] consumablesToSet)
        {
            consumables = consumablesToSet;
        }
    }
}