using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CyberBullet.GameManager;

namespace CyberBullet.UI {
    public class ItemOptions : MonoBehaviour {
        private Vector3 mouse;
        public Vector3 offset = Vector3.zero;
        public InventoryManagerNew invMg = null;
        public GUIManager guimg = null;
        public ExamineWindow examineWindow = null;

        void OnEnable()
        {
            if (invMg.GetTargetItem().slot == 9999999)
                this.gameObject.SetActive(false);
        }
        public void DropTargetItem()
        {
            SlotItem item = invMg.GetTargetItem();
            invMg.RemoveTargetItem();
            invMg.DropItem(item.target.id);
            guimg.RemoveQuickSlotItem(item.slot);
            this.gameObject.SetActive(false);
        }
        public void ExamineTargetItem()
        {
            SlotItem item = invMg.GetTargetItem();
            examineWindow.SetIcon(item.target.icon);
            examineWindow.SetDescription(item.target.description);
            examineWindow.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
        public void UseTargetItem()
        {
            SlotItem item = invMg.GetTargetItem();
            if (invMg.UseItem(item.target.id))
            {
                guimg.RemoveQuickSlotItem(item.slot);
            }
            this.gameObject.SetActive(false);
        }
    }
}