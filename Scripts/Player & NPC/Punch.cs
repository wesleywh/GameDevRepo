using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using CyberBullet.GameManager;

namespace CyberBullet.Weapons {
    [RequireComponent(typeof(Animator))]
    public class Punch : MonoBehaviour {
        private GUIManager guiManager = null;
        private Animator anim = null;
        private int index = 0;
        private bool attacking = false;
        private bool blocking = false;
        [SerializeField] private float[] attackNumbers;
        [SerializeField] private bool sequence = true;
        [SerializeField] private float attackCooldown = 0.05f;

        void Start()
        {
            guiManager = dontDestroy.currentGameManager.GetComponent<GUIManager>();
            anim = GetComponent<Animator>();
        }
    	void Update () {
            if (InputManager.GetButton("Attack") && blocking == false && guiManager.MenuOrInventoryOpen() == false)
            {
                StartCoroutine(MakeAttack());
            }
            if (InputManager.GetButtonDown("Block") && blocking == false && guiManager.MenuOrInventoryOpen() == false)
            {
                blocking = true;
                anim.SetFloat("blockNumber", 1);
                anim.SetBool("block", true);
            }
            if (InputManager.GetButtonUp("Block") && blocking == true && guiManager.MenuOrInventoryOpen() == false)
            {
                blocking = false;
                anim.SetFloat("blockNumber", 1);
                anim.SetBool("block", false);
            }
    	}

        IEnumerator MakeAttack()
        {
            if (attacking == true)
                yield return null;
            anim.SetFloat("attackNumber",attackNumber());
            anim.SetTrigger("attack");
            yield return new WaitForSeconds(attackCooldown);
            attacking = false;
        }
        float attackNumber()
        {
            if (sequence == true)
            {
                index = (index + 1 <= attackNumbers.Length-1) ? index + 1 : 0;
            }
            else
            {
                index = Random.Range(0, attackNumbers.Length);
            }
            return attackNumbers[index];
        }
    }
}