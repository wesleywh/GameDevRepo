using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CyberBullet.Controllers;

namespace CyberBullet.AI {
    [System.Serializable]
    public class AITaskAnimation {
        public Animation anim = null;
        public string[] idle = null;
        public string[] attack = null;
        public string[] walk = null;
        public string[] run = null;
        public string[] damage = null;
    }
    [System.Serializable]
    public class AITaskAnimator {
        public Animator anim = null;
        public float[] attackNumber = null;
        public float[] blockNumber = null;
    }
    [System.Serializable]
    public class AIAttacks {
        public Transform firePoint = null;
        public LayerMask ignoreLayers;
        public float damagePerHit = 5.0f;
        public float inaccuracy = 0.5f;
        public float notifyDistance = 45.0f;
        public float notifyCooldown = 5.0f;
        public LayerMask notifyLayers;
        public string callEvent = "call_target";
    }

    public class AITasks : MonoBehaviour {
        [SerializeField] private bool forPlayer = false;
        [SerializeField] private AITaskAnimation animationcomp = null;
        [SerializeField] private AITaskAnimator animator = null;
        [SerializeField] private AIAttacks attackInfo = null;
        [SerializeField] private AIVisualsLib lib = null;
        private float lastTime = 0f;
        private float currentTime = 999999f;

        void Start()
        {
            if (forPlayer == false && (animationcomp.anim == null && animator.anim == null) || (animator.anim != null && animationcomp.anim !=null))
            {
                Debug.LogError("CyberBullet.AI.AITasks - You need to supply 1 animation and animator but not both.");
            }
            if (lib == null)
                lib = GetComponent<AIVisualsLib>();
        }

        public float GetAttackNumber()
        {
            return animator.attackNumber[Random.Range(0, animator.attackNumber.Length - 1)];
        }

        public void MakeAttack(GameObject target, float distance, Vector3 offset)
        {
            if (target == null)
                return;
            Vector3 dir = (target.transform.position + offset) - attackInfo.firePoint.position;
            dir.x += UnityEngine.Random.Range(-attackInfo.inaccuracy, attackInfo.inaccuracy);
            dir.y += UnityEngine.Random.Range(-attackInfo.inaccuracy, attackInfo.inaccuracy);
            dir.z += UnityEngine.Random.Range(-attackInfo.inaccuracy, attackInfo.inaccuracy);
            RaycastHit hit;
            if (Physics.Raycast(attackInfo.firePoint.position, dir, out hit, distance, ~attackInfo.ignoreLayers)) 
            {
                lib.PlayMuzzleFlash();
                lib.SpawnParticle(hit.transform.tag, hit);
                if (hit.transform.root.GetComponent<Health>())
                {
                    hit.transform.root.GetComponent<Health>().ApplyDamage(attackInfo.damagePerHit, gameObject);
                }
            }
        }

        public void CallTarget(Vector3 lastPosition)
        {
            currentTime = Time.time;
            if (lastTime != currentTime)
            {
                if ((currentTime - lastTime) < attackInfo.notifyCooldown)
                {
                    currentTime = lastTime;
                    return;
                }
                currentTime = lastTime;
            }
            BehaviorDesigner.Runtime.SharedVector3 pos = (BehaviorDesigner.Runtime.SharedVector3)lastPosition;
            Collider[] hits = Physics.OverlapSphere(this.transform.position, attackInfo.notifyDistance, attackInfo.notifyLayers);

            foreach (Collider hit in hits)
            {
                if (hit.transform.root.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>())
                {
                    hit.transform.root.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().SetVariable("last_target_position", pos);
                    hit.transform.root.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().SendEvent(attackInfo.callEvent);
                }
            }
        }
    }
}