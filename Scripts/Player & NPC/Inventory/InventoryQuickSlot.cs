using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;

namespace Pandora.GameManager {
    public class InventoryQuickSlot : MonoBehaviour {
        public Image icon = null;
        public int id = 9999999;
        public int slot = 9999999;
        private InventoryManagerNew invMg;
        [SerializeField] private InventoryQuickSlot[] allQuickSlots;
        private InventoryItem heldItem = null;

        void Start()
        {
            invMg = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManagerNew>();
        }

        public InventoryItem GetHeldItem()
        {
            return heldItem;
        }
        public void AddToSlot()
        {
            if (slot != 9999999)
                RemoveFromSlot();
            SlotItem item = invMg.GetTargetItem();
            RemoveAllOccurances(item.id);
            heldItem = item.target;
            if (item.target != null)
            {
                icon.sprite = item.target.icon;
                SetIconAlpha(1);
                slot = item.slot;
                id = item.id;
                invMg.RemoveTargetItem();
            }
        }
        public void RemoveFromSlot()
        {
            icon.sprite = null;
            slot = 9999999;
            id = 9999999;
            SetIconAlpha(0);
            heldItem = null;
        }
        void SetIconAlpha(float amount)
        {
            Color col = icon.color;
            col.a = amount;
            icon.color = col;
        }
        public void RemoveAllOccurances(int id)
        {
            foreach (InventoryQuickSlot slot in allQuickSlots)
            {
                if (slot.id == id)
                    slot.RemoveFromSlot();
            }
        }
    }
}