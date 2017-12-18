using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pandora.Controllers;

namespace Pandora {
    namespace AI {
        [RequireComponent(typeof(Animator))]
        [RequireComponent(typeof(NavMeshAgent))]
//        [RequireComponent(typeof(AIBehavior))]
        public class AIAnimator : MonoBehaviour {
            #region Adjustables
            public Animator anim = null;
            public int updateEveryXFrames = 3;
            public NavMeshAgent agent = null;
            public AIBehavior behavior = null;
            public Health health = null;
            public Vector2 crouchResetTime = new Vector2(1,4);
            public Transform groundRaycast = null;
            public bool showGroundCast = false;
            #endregion

            #region Memory
            private int frames = 0;
            private float lastHealth = 100;
            private float angle = 0;
            private Vector3 currentFacing = Vector3.zero;
            private Vector3 lastFacing = Vector3.zero;
            private bool inCover = false;
            private bool originalCrouchState = false;
            private bool runningCrouchReset = false;
            private bool vaulting = false;
            private float prev_y = 0;
            private bool prev_climbing = false;
            private bool reloading = false;
            #endregion

            void Start()
            {
                anim = (anim == null) ? GetComponent<Animator>() : anim;
                agent = (agent == null) ? GetComponent<NavMeshAgent>() : agent;
                behavior = (behavior == null) ? GetComponent<AIBehavior>() : behavior;
                health = (health == null) ? GetComponent<Health>() : health;
                lastHealth = health.GetHealth();
                groundRaycast = (groundRaycast == null) ? transform : groundRaycast;
            }

            void Update()
            {
                frames++;
                if (frames % updateEveryXFrames == 0)
                {
                    frames = 0;
                    UpdateMovementSpeeds();
                    UpdateRotationSpeeds();
//                    UpdateInCover();
                    UpdateState();
                }
                if (behavior._attack_number > 0)
                {
                    UpdateAttackState();
                }
                UpdateHealthChanges();
                UpdateVaultingState();
                UpdateClimbing();
            }
            void FixedUpdate()
            {
                UpdateGroundedState();
                UpdateLayerWeights();
            }
            void UpdateMovementSpeeds()
            {
                Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
                anim.SetFloat("velocityX", localVelocity.x);
                anim.SetFloat("velocityZ", localVelocity.z);
                anim.SetFloat("moveSpeed", agent.velocity.magnitude);
            }
            void UpdateLayerWeights()
            {
                if (anim.GetBool("inCover") == true || anim.GetBool("grounded") == false 
                    || anim.GetBool("climbing") == true)
                {
                    anim.SetLayerWeight(1, 0.0f);
                }
                else
                {
                    anim.SetLayerWeight(1, 1.0f);
                }
            }
            void UpdateState()
            {
                switch (behavior.currentState)
                {
                    case AIStates.Calm:
                        anim.SetBool("suspicious", false);
                        anim.SetBool("hostile", false);
                        break;
                    case AIStates.Suspicious:
                        anim.SetBool("suspicious", true);
                        anim.SetBool("hostile", false);
                        break;
                    case AIStates.Hostile:
                        anim.SetBool("suspicious", false);
                        anim.SetBool("hostile", true);
                        break;
                }
            }
            void UpdateAttackState()
            {
                if (behavior.inCrouchCover == true)
                {
                    originalCrouchState = anim.GetBool("inCover");
                    anim.SetBool("inCover", false);
                }
                else if (behavior.inStandingCover == true && behavior.inCrouchCover == false)
                {
                    originalCrouchState = anim.GetBool("inCover");
                    anim.SetBool("inCover", true);
                }
                switch (behavior.combat.type)
                {
                    case CombatType.Melee:
                        anim.SetTrigger("melee");
                        anim.SetFloat("meleeNumber", behavior._attack_number);
                        break;
                    case CombatType.Shooter:
                        anim.SetTrigger("rangeAttack");
                        anim.SetFloat("rangeNumber", behavior._attack_number);
                        break;
                    default:
                        break;
                }
                behavior._attack_number = 0;
                StartCoroutine(ResetCrouchState());
            }
            void UpdateHealthChanges()
            {
                if (health.GetHealth() <= 0)
                {
                    anim.SetBool("dead", true);
                }
//                else if (lastHealth != health.GetHealth())
//                {
//                    lastHealth = health.GetHealth();
//                    anim.SetTrigger("damaged");
//                }
            }
            void UpdateRotationSpeeds()
            {
                currentFacing = transform.forward;
                angle = Vector3.Angle(currentFacing, lastFacing) / Time.deltaTime;
                lastFacing = currentFacing;
                anim.SetFloat("rotation", angle);
            }
            void UpdateInCover()
            {
                if (behavior.currentState == AIStates.Hostile)
                {
                    inCover = (behavior.inStandingCover == true || behavior.inCrouchCover == true) ? true : false;
                    anim.SetBool("standingCover", behavior.inStandingCover);
                    anim.SetBool("crouchCover", behavior.inCrouchCover);
                    if (behavior._attack_number == 0 && runningCrouchReset == false)
                    {
                        anim.SetBool("inCover", inCover);
                        anim.SetBool("crouch", behavior.inCrouchCover);
                    }
                }
            }
            void UpdateVaultingState()
            {
                anim.SetBool("vault", behavior.vaulting);
                if (behavior.vaulting == true && vaulting == false)
                {
                    vaulting = true;
                    anim.SetTrigger("vaulting");
                }
                else if (behavior.vaulting == false)
                {
                    vaulting = false;
                }
            }
            IEnumerator ResetCrouchState()
            {
                if (runningCrouchReset == false)
                {
                    runningCrouchReset = true;
                    anim.SetBool("inCover", true);
                    yield return new WaitForSeconds(UnityEngine.Random.Range(crouchResetTime.x, crouchResetTime.y));
                    anim.SetBool("inCover", originalCrouchState);
                    runningCrouchReset = false;
                }
            }
            void UpdateClimbing()
            {
                anim.SetBool("climbing", behavior.climbing);
                if (behavior.climbing == true)
                {
                    if (prev_y > transform.position.y)
                    {
                        anim.SetFloat("climbNumber", 0);
                    }
                    else
                    {
                        anim.SetFloat("climbNumber", 1);
                    }
                    prev_y = transform.position.y;
                }
            }
            public void PlayReload()
            {
                anim.SetBool("reloading", true);
                StartCoroutine(ResetReloading());
            }
            IEnumerator ResetReloading()
            {
                AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
                while (currentState.IsName("Reload"))
                {
                    yield return null;
                }
                anim.SetBool("reloading", false);
            }
            void UpdateGroundedState()
            {
                if (anim.GetBool("climbing") == true)
                {
                    anim.SetBool("grounded", true);
                }
                else
                {
                    if (AIHelpers.RaycastHitting(transform, groundRaycast, Vector3.down, 0.4f))
                    {
                        anim.SetBool("grounded", true);
                    }
                    else
                    {
                        anim.SetBool("grounded", false);
                    }
                }
            }

            void OnDrawGizmosSelected()
            {
                if (showGroundCast == true)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(groundRaycast.position, Vector3.down * 0.4f);
                }
            }
        }
    }
}