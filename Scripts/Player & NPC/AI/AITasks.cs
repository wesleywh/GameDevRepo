using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;
using Pandora.AI;
using Pandora.Controllers;

namespace Panda.AI {
    [RequireComponent(typeof(NavMeshAgent))]
    public class AITasks : MonoBehaviour {
        #region Internal Varialbes
        private AIMemory memory = null;
        private NavMeshAgent agent;

        void Awake()
        {
            memory = GetComponent<AIMemory>();
            agent = GetComponent<NavMeshAgent>();
        }
        #endregion

        #region Timers
        [Task] 
        void IncreaseSeeEnemyTimer()
        {
            memory.see_enemy_timer += Mathf.Abs(Time.deltaTime);
            memory.missing_enemy_timer = 0;
            Task.current.Succeed();
        }

        [Task] 
        void DecreaseSeeEnemyTimer()
        {
            memory.see_enemy_timer = (memory.see_enemy_timer > 0) ? memory.see_enemy_timer - Time.deltaTime : 0;
            memory.missing_enemy_timer = (memory.missing_enemy_timer > 9999) ? 1000  : memory.missing_enemy_timer - Time.deltaTime;
            Task.current.Succeed();
        }

        [Task]
        void TimerSetStats()
        {
            switch (memory.status)
            {
                case AIStatus.Calm:
                    if (memory.see_enemy_timer >= memory.become_suspicious)
                    {
                        memory.see_enemy_timer = 0;
                        SetStatus(AIStatus.Suspicious);
                    }
                    else
                    {
                        SetStatus(AIStatus.Calm);
                    }
                    break;
                case AIStatus.Suspicious:
                    if (memory.see_enemy_timer >= memory.become_hostile)
                    {
                        memory.see_enemy_timer = 0;
                        SetStatus(AIStatus.Hostile);
                    }
                    else if (Mathf.Abs(memory.missing_enemy_timer) >= memory.suspicious_lower_status)
                    {
                        memory.see_enemy_timer = 0;
                        SetStatus(AIStatus.Calm);
                    }
                    else
                    {
                        SetStatus(AIStatus.Suspicious);
                    }
                    break;
                case AIStatus.Hostile:
                    if (Mathf.Abs(memory.missing_enemy_timer) >= memory.hostile_lower_status)
                    {
                        SetStatus(AIStatus.Suspicious);
                    }
                    else
                    {
                        SetStatus(AIStatus.Hostile);
                    }
                    break;
            }
            Task.current.Succeed();
        }
        #endregion

        #region Movement
        [Task]
        public void StopMoving()
        {
            memory.movePoint = this.transform.position;
            agent.SetDestination(transform.position);
            Task.current.Succeed();
        }

        [Task]
        public void MoveToWaypoint()
        {
            memory.movePoint = AIHelpers.FindNextWaypoint(ref memory.waypoint_index,memory.waypoints,memory.waypoint_travel,memory.waypoint_dir).transform.position;
            agent.SetDestination(memory.movePoint);
            Task.current.Succeed();
        }

        [Task]
        public void MoveToLastEnemyLocation()
        {
            memory.movePoint = memory.last_enemy_loc.position;
            agent.SetDestination(memory.last_enemy_loc.position);
            Task.current.Succeed();
        }

        [Task]
        public void MoveToEnemy()
        {
            memory.movePoint = memory.enemy.transform.position;
            agent.SetDestination(memory.enemy.transform.position);
            Task.current.Succeed();
        }

        [Task]
        public void MoveToRandomPoint()
        {
            memory.movePoint = AIHelpers.ChooseRandomPoint(transform.position,memory.wander_radius);
            agent.SetDestination(memory.movePoint);
            Task.current.Succeed();
        }

        [Task]
        public void WaitArrival()
        {
            var task = Task.current;
            float d = agent.remainingDistance;
            if (!task.isStarting && agent.remainingDistance <= 1e-2)
            {
                task.Succeed();
                d = 0.0f;
            }

            if( Task.isInspected )
                task.debugInfo = string.Format("d-{0:0.00}", d );
        }

        [Task]
        public void LookAtEnemy()
        {
            if (memory.enemy != null)
            {
                AIHelpers.lookAtTarget(memory.enemy.transform.position, transform, 1);
            }
            Task.current.Succeed();
        }

        [Task]
        public void LookAtLastEnemyLocation()
        {
            AIHelpers.lookAtTarget(memory.last_enemy_loc.position, transform, 30);
        }

        [Task]
        public void LookAtDestination()
        {
            AIHelpers.lookAtTarget(memory.movePoint, transform, 30);
            if (Vector3.Angle(transform.position, memory.movePoint) < 1.0f)
            {
                Task.current.Succeed();
            }
        }
            
        [Task]
        public bool HasLastEnemyLocation()
        {
            if (memory.last_enemy_loc != null)
                return true;
            else
                return false;
        }

        [Task]
        public void RemoveLastEnemyLocation()
        {
            memory.last_enemy_loc = null;
            Task.current.Succeed();
        }
        #endregion

        #region Senses
        [Task]
        bool CanSeeEnemy()
        {
            bool ret_val = false;
            memory.enemy = AIHelpers.FindClosestEnemy(memory.enemy_tags, transform, false, memory.visualRange, memory.fieldOfView);
            if (memory.enemy != null)
            {
                ret_val = AIHelpers.CanSeeObject(memory.enemy, memory.eyes, memory.fieldOfView, memory.visualRange, memory.showVisuals);
                if (ret_val == true)
                    memory.last_enemy_loc = memory.enemy.transform;
            }

            return ret_val;
        }
        #endregion

        #region Combat
        [Task]
        public void PlayRandomMeleeAttack()
        {
            if (memory.anim.GetBool("crouch") == false)
            {
                memory.anim.SetFloat("melee_number", memory.melee_stand_numbers[Random.Range(0, memory.melee_stand_numbers.Length - 1)]);
            }
            else
            {
                memory.anim.SetFloat("melee_number", memory.melee_crouch_numbers[Random.Range(0, memory.melee_crouch_numbers.Length-1)]);
            }
            memory.anim.SetTrigger("melee");
            Task.current.Succeed();
        }

        [Task]
        public bool InValidCover()
        {
            if (memory.cover == null)
                return false;
            if (!AIHelpers.CanSeeObject(memory.enemy, memory.cover.transform, 360, memory.visualRange, memory.showVisuals))
            {
                return true;
            }
            return false;
        }

        [Task]
        public void SetCover(bool value, bool crouch)
        {
            if (memory.anim.GetBool("crouch") != crouch)
                memory.anim.SetBool("crouch", crouch);
            if (memory.anim.GetBool("inCover") != value)
                memory.anim.SetBool("inCover", value);
            Task.current.Succeed();
        }

        [Task]
        public void MakeRangedAttack()
        {
            memory.anim.SetBool("inCover", false);
            memory.anim.SetBool("crouch", false);
            memory.anim.SetFloat("rangeNumber",memory.range_numbers[Random.Range(0,memory.range_numbers.Length-1)]);
            memory.anim.SetTrigger("rangeAttack");
            Task.current.Succeed();
        }
        public void Shoot(float length)
        {
            StartCoroutine(FirShots(length));
        }
        IEnumerator FirShots(float allowed_time)
        {
            memory.is_attacking = true;
            memory.attack_timer = 0;
            yield return new WaitForSeconds(memory.delay_attack_start);
            int amount = Random.Range((int)memory.attacks.x, (int)memory.attacks.y);
            for (int i = 0; i < amount; i++)
            {
                if (memory.attack_timer < allowed_time && memory.is_attacking == true)
                {
                    GetComponent<AIVisualsLib>().GunShot();
                    CheckForHit();
                }
                else
                {
                    break;
                }
                yield return new WaitForSeconds(memory.attack_delay);
            }
            memory.is_attacking = false;
        }
        void CheckForHit()
        {
            GameObject hitObj = AIHelpers.InaccurateRaycast(memory.enemy.transform, memory.fire_point, 1.0f, memory.inaccuracy, 999, transform, memory.showVisuals);
            if (hitObj && hitObj.transform.root.GetComponent<Health>())
            {
                hitObj.transform.root.GetComponent<Health>().ApplyDamage(memory.damage_per_shot, this.gameObject);
            }
            else if (hitObj)
            {
                ParticleSystem ps = GetComponent<AIVisualsLib>().GetParticle(hitObj.tag);
                if (ps)
                {
                    RaycastHit hit = AIHelpers.GetRaycast(memory.enemy.transform, memory.fire_point, memory.inaccuracy, 999, transform, memory.showVisuals);
                    Instantiate(ps, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }

        [Task]
        public void MoveToNearestCover()
        {
            Transform cover = AIHelpers.ReturnValidCover(memory.cover_tags, memory.enemy.transform, memory.eyes, memory.visualRange, memory.showTargetCover);
            if (cover != null)
            {
                if (memory.cover != cover && memory.cover != null)
                {
                    memory.cover.tag = memory.cover_tags[0];
                }
                memory.cover = cover.gameObject;
                memory.cover.tag = "Untagged";
                agent.SetDestination(cover.position);
            }
            else
            {
                agent.SetDestination(transform.position);
            }
            Task.current.Succeed();
        }

        [Task]
        public bool WasDamaged()
        {
            if (memory.prev_health != GetComponent<Health>().GetHealth())
            {
                memory.prev_health = GetComponent<Health>().GetHealth();
                return true;
            }
            return false;
        }

        [Task]
        public bool InMeleeRange()
        {
            if (memory.enemy == null)
                return false;
            if (Vector3.Distance(transform.position, memory.enemy.transform.position) <= memory.melee_range)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Helpers
        [Task]
        public void SetStatus(AIStatus status)
        {
            memory.status = status;
            SetAnimStatus(status);
            Task.current.Succeed();
        }

        [Task]  
        public void IsStatus(AIStatus status)
        {
            if (memory.status == status)
            {
                Task.current.Succeed();
            }
            else
            {
                Task.current.Fail();
            }
        }

        [Task]
        public bool IsCombatType(CombatType type)
        {
            if (memory.combat_type == type)
            {
                return true;
            }
            return false;
        }

        void SetAnimStatus(AIStatus status)
        {
            switch (status)
            {
                case AIStatus.Calm:
                    agent.speed = memory.walk_speed;
                    memory.anim.SetBool("suspicious", false);
                    memory.anim.SetBool("hostile", false);
                    break;
                case AIStatus.Suspicious:
                    agent.speed = memory.walk_speed;
                    memory.anim.SetBool("suspicious", true);
                    memory.anim.SetBool("hostile", false);
                    break;
                case AIStatus.Hostile:
                    agent.speed = memory.run_speed;
                    memory.anim.SetBool("suspicious", true);
                    memory.anim.SetBool("hostile", true);
                    break;
            }
        }

        [Task]
        public void EnableHeadLookController(bool state)
        {
            if (GetComponent<HeadLookController>())
            {
                if ((memory.enemy == null && state == false) || state == true)
                    GetComponent<HeadLookController>().enabled = state;
            }
            Task.current.Succeed();
        }

        [Task]
        public void EnableAimAtController(bool state)
        {
            if (GetComponent<AimAt>())
            {
                if ((memory.enemy == null && state == false) || state == true)
                    GetComponent<AimAt>().enabled = state;
            }
            Task.current.Succeed();
        }
        #endregion
    }
}