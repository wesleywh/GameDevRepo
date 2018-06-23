using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CyberBullet.UI;
using TeamUtility.IO;

namespace CyberBullet.GameManager {
    public class InventoryGUIItem : MonoBehaviour {
        public Image icon = null;
        public Text nameText = null;
        public Image blockIcon = null;
        public int index = 0;
        public InventoryManagerNew invMg = null;
        public ItemType type = ItemType.Consumable;
        private InventoryItem item = null;
        public FollowMouse fm = null;
        public GameObject optionsMenu = null;

        public void DetectMenu()
        {
            if (icon.sprite == null)
                return;
            if (InputManager.GetMouseButton(0))
            {
                SetOptionsMenu(false);
                SetFollowIcon();
            }
            if (InputManager.GetMouseButton(1))
            {
                SetOptionsMenu(true);
            }
        }
        void Awake()
        {
            invMg = (invMg == null) ? dontDestroy.currentGameManager.GetComponent<InventoryManagerNew>() : invMg;
        }
        void OnEnable()
        {
            InventoryList[] targetList = GetList();
            if (index > targetList.Length - 1)
            {
                icon.sprite = null;
                nameText.text = "";
                blockIcon.enabled = true;
                SetIconAlpha(0);
                item = null;
            }
            else if (targetList[index] == null || targetList[index].id == 9999999)
            {
                icon.sprite = null;
                nameText.text = "";
                blockIcon.enabled = false;
                SetIconAlpha(0);
                item = null;
            }
            else
            {
                item = invMg.GetItemInDictionary(targetList[index].id);
                blockIcon.enabled = false;
                nameText.text = item.name;
                icon.sprite = item.icon;
                SetIconAlpha(1);
            }
        }
        void SetIconAlpha(float amount)
        {
            Color col = icon.color;
            col.a = amount;
            icon.color = col;
        }
        InventoryList[] GetList()
        {
            InventoryList[] ret_val = null;
            switch (type)
            {
                case ItemType.Consumable:
                    ret_val = invMg.GetConsumables();
                    break;
                case ItemType.Weapon:
                    ret_val = invMg.GetWeapons();
                    break;
            }
            return ret_val;
        }
        public void UpdateInventoryTarget()
        {
            invMg.AddTargetItem(item, index);
        }
        public void SetFollowIcon()
        {
            fm.SetIcon(icon.sprite);
            fm.gameObject.SetActive(true);
        }
        public void SetOptionsMenu(bool state)
        {
            Vector3 offset = optionsMenu.GetComponent<ItemOptions>().offset;
            optionsMenu.GetComponent<RectTransform>().position = Input.mousePosition + offset;
            optionsMenu.SetActive(state);
        }
    }
}