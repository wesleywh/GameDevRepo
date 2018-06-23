using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CyberBullet.Controllers;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

namespace CyberBullet.AI {
    public class AIActions : MonoBehaviour {

        #region Classes
        [System.Serializable]
        public class AIVisuals
        {
            public ParticleSystem muzzleFlash = null;
            public ParticleSystem bodyHit = null;
            public ParticleSystem heal = null;
        }
        [System.Serializable]
        public class AIAnimator 
        {
            public Animator anim = null;
            [Space(10)]
            public float[] rangeAttacks = new float[1]{1.0f};
            public float[] standAttacks = new float[9]{0.1818182f,0.2727273f,0.3636364f,0.4545455f,0.5454546f,0.7272727f,0.8181818f,0.9090909f,1.0f};
            public float[] crouchAttacks = new float[2]{0.0f,1.0f};
            public float[] standBlock = new float[2]{ 0.5f, 1.0f };
            public float[] takedownAttack = new float[12]{0.0f,0.09090f,0.18181f,0.27272f,0.363636f,0.454545f,0.545454f,0.636363f,0.727272f,0.818181f,0.909090f,1.0f};
            [Space(10)]
            public string hostile = "hostile";
            public string suspicious = "suspicious";
            public string sneak = "sneak";
            public string climbing = "climbing";
            public string rangeAttackTrigger = "rangeAttack";
            public string meleeAttackTrigger = "melee";
            [Space(5)]
            public string blockNumber = "blockNumber";
            public string climbNumber = "climbNumber";
            public string takedownNumber = "takedownNumber";
            public string meleeNumber = "meleeNumber";
            public string rangeNumber = "rangeNumber";
            public string deadNumber = "deadNumber";
            public string damagedNumber = "damagedNumber";
        }
        [System.Serializable]
        public class AISounds 
        {
            public AudioSource weaponSource = null;
            public AudioClip[] weaponSounds = null;
        }
        #endregion

        [SerializeField] AIVisuals visuals = new AIVisuals();
        [SerializeField] AIAnimator animator = new AIAnimator();
        [SerializeField] AISounds sounds = new AISounds();
        [SerializeField] private AIMemory memory = null;
        private GameObject lockedOn = null;
        private int number_of_allowed_attacks = 0;
        [SerializeField] private bool isDebugging = false;
        #region Initialization
        void Start()
        {
            memory = (memory == null) ? GetComponent<AIMemory>() : memory;
            if (sounds.weaponSource == null)
            {
                SetWeaponSoundSource();
            }
        }
        #endregion

        #region Hearbeat
        void Update()
        {
            LookAtTarget();
        }
        #endregion

        #region Plays
        public void PlayMuzzleFlash(Transform location=null)
        {
            if (visuals.muzzleFlash == null)
                return;
            if (location != null)
            {
                visuals.muzzleFlash.gameObject.transform.position = location.position;
                visuals.muzzleFlash.gameObject.transform.rotation = location.rotation;
            }
            visuals.muzzleFlash.Play();
        }
        public void PlayBodyHit(Transform location=null)
        {
            if (location != null)
            {
                visuals.bodyHit.gameObject.transform.position = location.position;
                visuals.bodyHit.gameObject.transform.rotation = location.rotation;
            }
            visuals.bodyHit.Play();
        }
        public void PlayHeal(Transform location=null)
        {
            if (location != null)
            {
                visuals.heal.gameObject.transform.position = location.position;
                visuals.heal.gameObject.transform.rotation = location.rotation;
            }
            visuals.heal.Play();
        }
        public void PlayWeaponSound()
        {
            AudioSource source = GetWeaponSoundSource();
            if (source != null && GetWeapounSoundsLength() > 0)
            {
                source.clip = GetWeaponSound();
                source.Play();
            }
        }
        #endregion

        #region Gets
        public GameObject GetNearestPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject closest = AIHelpers.FindClosest(players, transform);
            return closest;
        }
        public AudioSource GetWeaponSoundSource()
        {
            return sounds.weaponSource;
        }
        public AudioClip GetWeaponSound()
        {
            return sounds.weaponSounds[Random.Range(0, sounds.weaponSounds.Length)];
        }
        public int GetWeapounSoundsLength()
        {
            return sounds.weaponSounds.Length;
        }
        #endregion

        #region Actions
        public void SetWeaponSoundSource(AudioSource source = null)
        {
            if (source == null)
            {
                source = GetComponent<AudioSource>();
            }
            sounds.weaponSource = source;
        }
        public void SetStrafeLocation()
        {
            float distance = memory.GetStrafeDistance();
            Vector3 location = transform.position + Random.insideUnitSphere * distance;
            NavMeshHit hit;
            Vector3 retVal = transform.position;
            if (NavMesh.SamplePosition(location, out hit, distance, NavMesh.AllAreas))
            {
                retVal = hit.position;
            }
            GetComponent<BehaviorTree>().SetVariable("strafe_position", (SharedVector3)retVal);
        }
        public void SetNumberOfAttacks()
        {
            number_of_allowed_attacks = memory.GetNumberOfAttacks();
        }
        void LookAtTarget()
        {
            if (lockedOn != null)
            {
                Vector3 lookPos = lockedOn.transform.position - transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * memory.GetRotationSpeed());
            }
        }
        public void SetAnimVars(AIStates state)
        {
            switch (state)
            {
                case AIStates.Calm:
                    animator.anim.SetBool(animator.hostile, false);
                    animator.anim.SetBool(animator.suspicious, false);
                    break;
                case AIStates.Suspicious:
                    animator.anim.SetBool(animator.hostile, false);
                    animator.anim.SetBool(animator.suspicious, true);
                    break;
                case AIStates.Hostile:
                    animator.anim.SetBool(animator.hostile, true);
                    animator.anim.SetBool(animator.suspicious, true);
                    break;
            }
        }
        public void SetAnimVars(string state)
        {
            switch (state)
            {
                case "calm":
                    animator.anim.SetBool(animator.hostile, false);
                    animator.anim.SetBool(animator.suspicious, false);
                    break;
                case "suspicious":
                    animator.anim.SetBool(animator.hostile, false);
                    animator.anim.SetBool(animator.suspicious, true);
                    break;
                case "hostile":
                    animator.anim.SetBool(animator.hostile, true);
                    animator.anim.SetBool(animator.suspicious, true);
                    break;
            }
        }
        public void BroadCastTargetPosition()
        {
            AIMemory memory = GetComponent<AIMemory>();
            Vector3 position = memory.GetLastTargetPosition();
            float distance = memory.GetShoutDistance();
            string[] friendTags = memory.GetFriendTags();
            GameObject[] friends = AIHelpers.GetAllGameObjectsInRange(friendTags, distance);
            foreach (GameObject friend in friends)
            {
                friend.GetComponent<AIMemory>().SetLastTargetPosition(position);
                friend.GetComponent<AIMemory>().SetIncreasedLevel();
            }
        }
        public void Attack(GameObject overrideTarget=null)
        {
            if (number_of_allowed_attacks < 1)
                return;
            number_of_allowed_attacks -= 1;
            if (isDebugging == true)
                Debug.Log(name + " attacked");
            AIMemory memory = GetComponent<AIMemory>();
            GameObject target = null;
            if (overrideTarget == null)
            {
                target = memory.GetTarget();
            }
            else
            {
                target = overrideTarget;
            }
            if (target == null)
            {
                return;
            }
            GameObject returnTarget = AIHelpers.InaccurateRaycast(
                target.transform, 
                memory.GetEyes().transform, 
                memory.GetIgnoreLayers(),
                memory.GetOffsetRange(), 
                memory.GetInaccuracy(), 
                memory.GetFireRange(), 
                isDebugging
            );
            PlayMuzzleFlash();
            PlayWeaponSound();
            if (returnTarget != null)
            {
                if (isDebugging == true)
                {
                    Debug.Log(name + " sending damage to " + returnTarget.transform.root.name);
                }
                if (returnTarget.transform.root.GetComponent<Health>())
                {
                    returnTarget.transform.root.GetComponent<Health>().ApplyDamage(memory.GetRangeDamageAmount(), gameObject);
                }
            }
        }
        public void LockOn(GameObject target)
        {
            lockedOn = target;
        }
        #endregion

        #region Animator Parameters
        public void SetAttackAnim(bool melee=false)
        {
            if (melee == true)
            {
                if (animator.anim.GetBool("crouch") == true)
                {
                    animator.anim.SetFloat(animator.meleeNumber, animator.crouchAttacks[Random.Range(0, animator.crouchAttacks.Length)]);
                }
                else
                {
                    animator.anim.SetFloat(animator.meleeNumber, animator.standAttacks[Random.Range(0, animator.standAttacks.Length)]);
                }
                animator.anim.SetTrigger(animator.meleeAttackTrigger);
            }
            else
            {
                animator.anim.SetFloat(animator.rangeNumber, animator.rangeAttacks[Random.Range(0, animator.rangeAttacks.Length)]);
                animator.anim.SetTrigger(animator.rangeAttackTrigger);
            }
        }
        #endregion
    }
}