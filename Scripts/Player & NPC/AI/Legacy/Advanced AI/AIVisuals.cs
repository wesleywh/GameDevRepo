using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pandora.Weapons;

namespace Pandora {
    namespace AI {
        [RequireComponent(typeof(AudioSource))]
        [RequireComponent(typeof(AIAnimator))]
        public class AIVisuals : MonoBehaviour {
            #region Variables
            #region Adjustables
            public int attacksPerNumber = 1;
            public float attackSpeed = 0.2f;
            public int ammo = 6;
            public bool ignoreAmmo = false;
            public ParticleSystem muzzleFlash = null;
            public GameObject casing = null;
            public Transform ejectPoint = null;
            public W_Eject ejectDirection = W_Eject.Right;
            public AudioSource audioSrouce = null;
            public AIBehavior behavior = null;
            public AIAnimator animator = null;
            #endregion

            #region Internal Only
            private int fullAmmo = 7;
            private bool attacking = false;
            private int attacks = 0;
            #endregion
            #endregion

            #region Initalize
            void Start() 
            {
                fullAmmo = ammo;
                audioSrouce = (audioSrouce == null) ? GetComponent<AudioSource>() : audioSrouce;
                behavior = (behavior == null) ? GetComponent<AIBehavior>() : behavior;
                animator = (animator == null) ? GetComponent<AIAnimator>() : animator;
            }
            #endregion
            #region Heartbeat
            void Update()
            {
                if (attacking == false && behavior._attack_number > 0)
                {
                    attacking = true;
                    StartCoroutine(TriggerAttack());
                }
            }
            #endregion
            IEnumerator TriggerAttack()
            {
                ammo = (ignoreAmmo == false) ? ammo - 1 : ammo;
                if (ammo > 0)
                {
                    attacks++;
                    if (muzzleFlash != null)
                    {
                        muzzleFlash.Play();
                    }
                    if (casing != null && ejectPoint != null)
                    {
                        WeaponHelpers.EjectShell(casing, ejectPoint, ejectDirection);
                    }
                    yield return new WaitForSeconds(attackSpeed);
                    if (attacks >= attacksPerNumber)
                    {
                        attacking = false;
                    }
                    yield return TriggerAttack();
                }
                else
                {
                    animator.PlayReload();
                    yield return new WaitForSeconds(behavior.combat.reloadSpeed);
                    ammo = fullAmmo;
                    attacks = 0;
                    attacking = false;
                }
            }

        }
    }
}