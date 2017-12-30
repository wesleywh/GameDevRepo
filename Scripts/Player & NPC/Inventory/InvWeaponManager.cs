using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pandora.Weapons;
using Pandora.GameManager;
using TeamUtility.IO;

namespace Pandora.Controllers {
    public class InvWeaponManager : MonoBehaviour {
        [SerializeField] private GameObject NoWeapon = null;

        [SerializeField] private WeaponNew[] weaponOptions;
        private InventoryManagerNew ingMg = null;
        [SerializeField] private string weaponAmmoUITag = "GUIAmmo";
        [SerializeField] private float scrollSelectDelay = 0.3f;
        [Space(10)]
        [Header("Debugging Purposes Only")]
        [SerializeField] private WeaponNew holding = null;
        [SerializeField] private GameObject GUIAmmoUI = null;
        private int current_index = 9999999;
        private bool can_scroll = true;

        void Start()
        {
            ingMg = dontDestroy.currentGameManager.GetComponent<InventoryManagerNew>();
            StartCoroutine(SetAmmoUIObject());
            EnableHands();
        }
        void Update()
        {
            if (InputManager.GetAxis("Scroll") > 0)
            {
                StartCoroutine(ScrollSelect(1));
            }
            else if (InputManager.GetAxis("Scroll") < 0)
            {
                StartCoroutine(ScrollSelect(-1));
            }
        }

        IEnumerator ScrollSelect(int direction)
        {
            if (can_scroll == true)
            {
                can_scroll = false;
                InventoryList[] weapons = ingMg.GetWeapons();
                if (direction == 1)
                {
                    current_index = (current_index == 9999999) ? 0 : current_index + 1;
                }
                else if (direction == -1)
                {
                    current_index = (current_index == 9999999) ? weapons.Length - 1 : current_index - 1;
                }
                if (current_index < 0 || current_index > weapons.Length - 1)
                {
                    EnableHands();
                }
                else
                {
                    InventoryList weapon = ingMg.GetWeaponAtIndex(current_index);
                    if (weapon.id != 9999999)
                        SelectWeapon(weapon.id);
                }
                yield return new WaitForSeconds(scrollSelectDelay);
                can_scroll = true;
            }
            else
            {
                yield return null;
            }
        }
        IEnumerator SetAmmoUIObject()
        {
            while (!GameObject.FindGameObjectWithTag(weaponAmmoUITag))
            {
                yield return new WaitForSeconds(0.1f);
            }
            GUIAmmoUI = GameObject.FindGameObjectWithTag(weaponAmmoUITag);
        }
        public GameObject getUI()
        {
            return GUIAmmoUI;
        }
        public void SelectWeapon(int id)
        {
            DisableAllWeapons();
            GetWeaponWithId(id).SetActive(true);
            holding = GetWeaponWithId(id).GetComponent<WeaponNew>();
            current_index = ingMg.GetWeaponIndex(id);
        }
        public void DeselectWeapon(int id)
        {
            if (holding.id == id)
            {
                EnableHands();
            }
        }
        public void DisableAllWeapons()
        {
            NoWeapon.SetActive(false);
            for (int i = 0; i < weaponOptions.Length; i++)
            {
                weaponOptions[i].gameObject.SetActive(false);
            }
        }
        public GameObject GetWeaponWithId(int id)
        {
            GameObject ret_val = null;
            foreach (WeaponNew weapon in weaponOptions)
            {
                if (weapon.id == id)
                {
                    ret_val = weapon.gameObject;
                }
            }
            return ret_val;
        }
        public void EnableHands()
        {
            holding = null;
            DisableAllWeapons();
            current_index = 9999999;
            NoWeapon.SetActive(true);
        }
        public void AddAmmo(int weaponid, int amount)
        {
            GetWeaponWithId(weaponid).GetComponent<WeaponNew>().AddAmmo(amount);
        }
        int GetIndex(int id)
        {
            InventoryList[] weapons = ingMg.GetWeapons();
            for (int i=0; i < weapons.Length; i++)
            {
                if (weapons[i].id == id)
                    return i;
            }
            return 9999999;
        }
        public GameObject GetHoldingObject()
        {
            return (holding == null) ? NoWeapon : holding.transform.gameObject;
        }
    }
}