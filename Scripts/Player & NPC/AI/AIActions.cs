using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CyberBullet.Controllers;
using CyberBullet.GameManager;
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
            public AudioSource defaultSource = null;
            public AIWeaponSounds weapons = null;
            public AIVoiceSounds voice = null;
            public AIPhysicalSounds physicals = null;
        }
        [System.Serializable]
        public class AIGrabThrow
        {
            public Transform grabPoint = null;
            public float resetTime = 4.0f;
            public float throwDistance = 5.0f;
            public float throwHeight = 2.0f;
            public float throwMoveTime = 1.0f;
            public float grabMoveSpeed = 4.0f;
            public float grabRotateSpeed = 4.0f;
            public float layingTime = 1.0f;
            public float standTime = 2.0f;
            [Space(10)]
            [Header("Debugging")]
            public GameObject target = null;
            public bool triggerGrab = false;
            public bool triggerThrow = false;

            //Non-changable items
            [HideInInspector] public GameObject curTarget = null;
            [HideInInspector] public bool throwing = false;
            [HideInInspector] public bool stand_up = false;
            [HideInInspector] public bool grabbing = false;
            [HideInInspector] public bool grabbed = false;
            [HideInInspector] public float timer = 0.0f;
            [HideInInspector] public bool can_stand = false;
            [HideInInspector] public Quaternion targetRotation;
            [HideInInspector] public Vector3 cur_pos = Vector3.zero;
            [HideInInspector] public Vector3 final_pos = Vector3.zero;
            [HideInInspector] public Vector3 start_pos = Vector3.zero;
            [HideInInspector] public Quaternion start_rot = Quaternion.identity;
            [HideInInspector] public PlayerManager pm = null;
            [HideInInspector] public bool p_control = true;
        }
        [System.Serializable]
        public class AIWeaponSounds 
        {
            public AudioSource weaponSource = null;
            public AudioClip[] weaponSounds = null;
        }
        [System.Serializable]
        public class AIVoiceSounds
        {
            public AudioSource voiceSource = null;
            public AudioClip[] effortSounds = null;
            public AudioClip[] hurt = null;
        }
        [System.Serializable]
        public class AIPhysicalSounds
        {
            public AudioSource physicalSource = null;
            public AudioClip[] swings = null;
            public AudioClip[] punchHits = null;
        }
        #endregion

        #region Parameters
        [SerializeField] AIVisuals visuals = new AIVisuals();
        [SerializeField] AIAnimator animator = new AIAnimator();
        [SerializeField] AISounds sounds = new AISounds();
        [SerializeField] AIGrabThrow grabthrowSettings = new AIGrabThrow();
        [SerializeField] private AIMemory memory = null;
        private GameObject lockedOn = null;
        private int number_of_allowed_attacks = 0;
        [SerializeField] private bool isDebugging = false;
        #endregion

        #region Initialization
        void Start()
        {
            memory = (memory == null) ? GetComponent<AIMemory>() : memory;
            grabthrowSettings.pm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayerManager>();
        }
        #endregion

        #region Hearbeat
        void Update()
        {
            LookAtTarget();
            if (grabthrowSettings.triggerGrab == true)
            {
                grabthrowSettings.triggerGrab = false;
                Grab(grabthrowSettings.target);
            }
            if (grabthrowSettings.triggerThrow == true)
            {
                grabthrowSettings.triggerThrow = false;
                Throw();
            }
            if (grabthrowSettings.throwing == true)
            {
                ParabolaThrow();
            }
            if (grabthrowSettings.grabbing == true)
            {
                GrabbingMove();
            }
            if (grabthrowSettings.grabbed == true)
            {
                grabthrowSettings.curTarget.transform.position = grabthrowSettings.grabPoint.position;
                grabthrowSettings.curTarget.transform.rotation = grabthrowSettings.grabPoint.rotation;
            }
            if (grabthrowSettings.stand_up == true)
            {
                StandUp();
            }
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
            return sounds.weapons.weaponSource;
        }
        public AudioClip GetWeaponSound()
        {
            return sounds.weapons.weaponSounds[Random.Range(0, sounds.weapons.weaponSounds.Length)];
        }
        public int GetWeapounSoundsLength()
        {
            return sounds.weapons.weaponSounds.Length;
        }
        #endregion

        #region Sets
        public void SetWeaponSoundSource(AudioSource source)
        {
            sounds.weapons.weaponSource = source;
        }
        public void SetVoiceSoundSource(AudioSource source)
        {
            sounds.voice.voiceSource = source;
        }
        public void SetPhysicalSoundSource(AudioSource source)
        {
            sounds.physicals.physicalSource = source;
        }
        #endregion

        #region Actions
        public void ApplyDamageToPlayer(float damage)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Health>().ApplyDamage(damage);
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
        public void PlayRandomSwingSound()
        {
            if (sounds.physicals.swings.Length < 1)
                return;
            if (sounds.physicals.physicalSource != null)
            {
                sounds.physicals.physicalSource.clip = sounds.physicals.swings[Random.Range(0, sounds.physicals.swings.Length)];
                sounds.physicals.physicalSource.Play();
            }
            else if (sounds.defaultSource != null)
            {
                sounds.defaultSource.clip = sounds.physicals.swings[Random.Range(0, sounds.physicals.swings.Length)];
                sounds.defaultSource.Play();
            }
        }
        public void PlayRandomPunchHitSound()
        {
            if (sounds.physicals.punchHits.Length < 1)
                return;
            if (sounds.physicals.physicalSource != null)
            {
                sounds.physicals.physicalSource.clip = sounds.physicals.punchHits[Random.Range(0, sounds.physicals.punchHits.Length)];
                sounds.physicals.physicalSource.Play();
            }
            else if (sounds.defaultSource != null)
            {
                sounds.defaultSource.clip = sounds.physicals.punchHits[Random.Range(0, sounds.physicals.punchHits.Length)];
                sounds.defaultSource.Play();
            }
        }
        public void PlayRandomWeaponSound()
        {
            if (sounds.weapons.weaponSounds.Length < 1)
                return;
            if (sounds.weapons.weaponSource != null)
            {
                sounds.weapons.weaponSource.clip = sounds.weapons.weaponSounds[Random.Range(0, sounds.weapons.weaponSounds.Length)];
                sounds.weapons.weaponSource.Play();
            }
            else if (sounds.defaultSource != null)
            {
                sounds.defaultSource.clip = sounds.weapons.weaponSounds[Random.Range(0, sounds.weapons.weaponSounds.Length)];
                sounds.defaultSource.Play();
            }
        }
        public void PlayRandomVoiceEffortSound()
        {
            if (sounds.voice.effortSounds.Length < 1)
                return;
            if (sounds.voice.voiceSource != null)
            {
                sounds.voice.voiceSource.clip = sounds.voice.effortSounds[Random.Range(0, sounds.voice.effortSounds.Length)];
                sounds.voice.voiceSource.Play();
            }
            else if (sounds.defaultSource != null)
            {
                sounds.defaultSource.clip = sounds.voice.effortSounds[Random.Range(0, sounds.voice.effortSounds.Length)];
                sounds.defaultSource.Play();
            }
        }
        public void PlayRandomVoiceHurtSound()
        {
            if (sounds.voice.hurt.Length < 1)
                return;
            if (sounds.voice.voiceSource != null)
            {
                sounds.voice.voiceSource.clip = sounds.voice.hurt[Random.Range(0, sounds.voice.hurt.Length)];
                sounds.voice.voiceSource.Play();
            }
            else if (sounds.defaultSource != null)
            {
                sounds.defaultSource.clip = sounds.voice.hurt[Random.Range(0, sounds.voice.hurt.Length)];
                sounds.defaultSource.Play();
            }
        }
        #endregion

        #region Grab/Throw Actions 
        public void Grab(GameObject target)
        {
            if (grabthrowSettings.curTarget != null || grabthrowSettings.grabbing == true)
                return;
            GameObject rootObj = target.transform.root.gameObject;
            grabthrowSettings.curTarget = rootObj;
            grabthrowSettings.grabbing = true;
        }
        public void GrabPlayer()
        {
            if (grabthrowSettings.curTarget != null || grabthrowSettings.grabbing == true)
                return;
            GameObject rootObj = grabthrowSettings.target.transform.root.gameObject;
            grabthrowSettings.curTarget = GameObject.FindGameObjectWithTag("Player");
            grabthrowSettings.grabbing = true;
        }
        public void Throw()
        {
            if (grabthrowSettings.grabbing == true)
            {
                grabthrowSettings.grabbing = false;
            }
            grabthrowSettings.final_pos = GetLandPos();
            grabthrowSettings.start_pos = grabthrowSettings.curTarget.transform.position;
            grabthrowSettings.start_rot = grabthrowSettings.curTarget.transform.rotation;
            grabthrowSettings.timer = 0;
            grabthrowSettings.throwing = true;
        }
        IEnumerator RemoveCurTarget()
        {
            yield return new WaitForSeconds(grabthrowSettings.resetTime);
            grabthrowSettings.curTarget = null;
        }
        void ParabolaThrow()
        {
            if (grabthrowSettings.curTarget == null)
                return;
            grabthrowSettings.grabbed = false;
            grabthrowSettings.timer += Time.deltaTime;
            grabthrowSettings.cur_pos = Vector3.Lerp(grabthrowSettings.start_pos, grabthrowSettings.final_pos, grabthrowSettings.timer / grabthrowSettings.throwMoveTime);
            grabthrowSettings.cur_pos.y += grabthrowSettings.throwHeight * Mathf.Sin(Mathf.Clamp01(grabthrowSettings.timer / grabthrowSettings.throwMoveTime) * Mathf.PI);
            grabthrowSettings.curTarget.transform.position = grabthrowSettings.cur_pos;

            Quaternion targetRotation = Quaternion.LookRotation(Vector3.up);
            grabthrowSettings.curTarget.transform.rotation = Quaternion.Slerp(grabthrowSettings.start_rot, targetRotation, grabthrowSettings.timer / grabthrowSettings.throwMoveTime);

            if (Vector3.Distance(grabthrowSettings.curTarget.transform.position, grabthrowSettings.final_pos) < 0.05f)
            {
                grabthrowSettings.start_rot = grabthrowSettings.curTarget.transform.rotation;
                grabthrowSettings.timer = 0;
                grabthrowSettings.throwing = false;
                grabthrowSettings.stand_up = true;
                grabthrowSettings.final_pos = Vector3.zero;
            }
        }
        Vector3 GetLandPos()
        {
            if (grabthrowSettings.curTarget == null)
                return Vector3.zero;

            Vector3 target_pos = this.transform.position + this.transform.forward * grabthrowSettings.throwDistance;

            NavMeshHit hit;
            bool valid = false;
            float modThrow = grabthrowSettings.throwDistance;
            while (valid == false)
            {
                if (NavMesh.SamplePosition(target_pos, out hit, grabthrowSettings.throwDistance, -1))
                {
                    valid = true;
                }
                else
                {
                    modThrow -= 0.2f;
                    target_pos -= this.transform.forward * modThrow;
                }
            }

            return target_pos;
        }
        void GrabbingMove()
        {
            if (grabthrowSettings.curTarget == null)
                return;
            SetControllableState(false);
            grabthrowSettings.timer += Time.deltaTime;
            grabthrowSettings.targetRotation = Quaternion.LookRotation(grabthrowSettings.grabPoint.transform.position - grabthrowSettings.curTarget.transform.position);
            grabthrowSettings.curTarget.transform.rotation = Quaternion.Slerp(grabthrowSettings.curTarget.transform.rotation, grabthrowSettings.grabPoint.rotation, grabthrowSettings.timer / grabthrowSettings.grabRotateSpeed);
            grabthrowSettings.curTarget.transform.rotation = Quaternion.Slerp(grabthrowSettings.curTarget.transform.rotation, grabthrowSettings.grabPoint.rotation, grabthrowSettings.timer / grabthrowSettings.grabRotateSpeed);
            if (Vector3.Distance(grabthrowSettings.curTarget.transform.position, grabthrowSettings.grabPoint.position) > 0.01f)
            {
                grabthrowSettings.curTarget.transform.position = Vector3.Lerp(grabthrowSettings.curTarget.transform.position, grabthrowSettings.grabPoint.position, grabthrowSettings.timer / grabthrowSettings.grabMoveSpeed);
            }
            else
            {
                grabthrowSettings.curTarget.transform.position = grabthrowSettings.grabPoint.position;
                grabthrowSettings.curTarget.transform.rotation = grabthrowSettings.grabPoint.rotation;
                grabthrowSettings.grabbing = false;
                grabthrowSettings.timer = 0;
                grabthrowSettings.grabbed = true;
            }
        }
        void SetControllableState(bool state)
        {
            if (state != grabthrowSettings.p_control)
            {
                grabthrowSettings.p_control = state;
                grabthrowSettings.pm.SetPlayerControllable(state);
                grabthrowSettings.pm.EnableCameraControl(state);
                grabthrowSettings.pm.EnablePlayerColliders(state);
            }
        }
        void StandUp()
        {
            grabthrowSettings.timer += Time.deltaTime;
            if (grabthrowSettings.timer >= grabthrowSettings.layingTime && grabthrowSettings.can_stand == false)
            {
                grabthrowSettings.can_stand = true;
                grabthrowSettings.timer = 0;
            }
            if (grabthrowSettings.can_stand == true)
            {
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
                grabthrowSettings.curTarget.transform.rotation = Quaternion.Slerp(grabthrowSettings.start_rot, targetRotation, grabthrowSettings.timer / grabthrowSettings.standTime);
                if (Quaternion.Angle(grabthrowSettings.curTarget.transform.rotation, targetRotation) <= 0.01f)
                {
                    grabthrowSettings.stand_up = false;
                    grabthrowSettings.can_stand = false;
                    grabthrowSettings.curTarget = null;
                    SetControllableState(true);
                }
            }
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