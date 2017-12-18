using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

namespace Pandora.GameManager {
    public class UseQuickSlots : MonoBehaviour {
        [SerializeField] private InventoryQuickSlot[] allQuickSlots;
        [SerializeField] private InventoryManagerNew invMg;

        void Update()
        {
            //use quick slots
            if (InputManager.GetButtonDown("QuickSlot1"))
            {
                if (allQuickSlots[0].GetHeldItem() != null)
                {
                    if (invMg.UseItem(allQuickSlots[0].GetHeldItem().id))
                    {
                        allQuickSlots[0].RemoveFromSlot();
                    }
                }
            }
            else if (InputManager.GetButtonDown("QuickSlot2"))
            {
                if (allQuickSlots[1].GetHeldItem() != null)
                {
                    if (invMg.UseItem(allQuickSlots[1].GetHeldItem().id))
                    {
                        allQuickSlots[1].RemoveFromSlot();
                    }
                }
            }
            else if (InputManager.GetButtonDown("QuickSlot3"))
            {
                if (allQuickSlots[2].GetHeldItem() != null)
                {
                    if (invMg.UseItem(allQuickSlots[2].GetHeldItem().id))
                    {
                        allQuickSlots[2].RemoveFromSlot();
                    }
                }
            }
        }
    }
}