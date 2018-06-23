using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using CyberBullet.Controllers;
using CyberBullet.AI;
using TeamUtility.IO;

namespace CyberBullet.AI {
    [System.Serializable]
    class SyncAnimations {
        public string[] death = null;
        public string[] hurt = null;
        public string idle = null;
        public string walk = null;
        public string run = null;
        public string fall = null;
        public float walk_speed = 0.05f;
        public float run_speed = 3.0f;
    }
    [System.Serializable]
    class SyncAnimNames {
        public string VelX = "velocityX";
        public string VelZ = "velocityZ";
        public string moveSpeed = "moveSpeed";
        public string rotation = "rotation";
        public string grounded = "grounded";
        public string damaged = "damaged";
        public string dead = "dead";
    }
    public class AIAnimSync : MonoBehaviour {
        #region Adjustables
        [SerializeField] private Animator animator = null;
        [SerializeField] private Animation animationcomp = null;
        [SerializeField] private SyncAnimations animList;
        [SerializeField] private SyncAnimNames animatorNames;
        public int updateEveryXFrames = 3;
        public NavMeshAgent agent = null;
        public Rigidbody body = null;
        public float multiplyVelocity = 0.0f;
        public Health health = null;
        public Transform groundRaycast = null;
        public Vector2 colliderHeight = new Vector2(1.3f, 2.3f);
        public Vector2 colliderCenter = new Vector2(0.65f, 1.2f);
        public bool ignoreGroundCheck = false;
        public bool syncGroundState = true;
        public bool syncColliderHeight = true;
        public bool syncMovement = true;
        public bool syncRotation = true;
        public bool syncHealth = true;

        #endregion
        #region Internal Use Only
        private int frames = 0;
        private float lastHealth = 0;
        private float angle = 0;
        private Vector3 currentFacing = Vector3.zero;
        private Vector3 lastFacing = Vector3.zero;
        private bool grounded = true;
        private Vector3 previous = Vector3.zero;
        #endregion
    	
    	void Start () 
        {
            if (animator == null && animationcomp == null)
                Debug.LogError("You must supply animator or animation to AIAnimSync");
            health = (health == null) ? GetComponent<Health>() : health;
            lastHealth = health.GetHealth();
            groundRaycast = (groundRaycast == null) ? transform : groundRaycast;
            previous = transform.position;
    	}
    	void Update () {
            frames++;
            if (frames % updateEveryXFrames == 0)
            {
                frames = 0;
                if (syncMovement)
                    UpdateMovementSpeeds();
                if (syncRotation)
                    UpdateRotationSpeeds();
                if (syncHealth)
                    UpdateHealthChanges();
            }
    	}
        void FixedUpdate()
        {
            if(syncGroundState)
                UpdateGroundedState();
            if (syncColliderHeight)
                UpdateColliderHeight();
        }

        void UpdateMovementSpeeds()
        {
            if (animator != null)
            {
                Vector3 localVelocity = Vector3.zero;
                if (agent != null)
                {
                    localVelocity = transform.InverseTransformDirection(agent.velocity);
                }
                else if (body != null)
                {
                    Debug.Log(body.velocity);
                    localVelocity = transform.InverseTransformDirection(body.velocity);
                }
                else
                {
                    float x = (transform.position.x - previous.x) / Time.deltaTime;
                    float z = (transform.position.z - previous.z) / Time.deltaTime;
                    localVelocity = new Vector3(x, 0, z);
                    previous = transform.position;
                }
                animator.SetFloat(animatorNames.VelX, localVelocity.x);
                animator.SetFloat(animatorNames.VelZ, localVelocity.z);
                float move_speed = 0;
                if (agent != null)
                {
                    move_speed = (multiplyVelocity != 0) ? agent.velocity.magnitude * multiplyVelocity : agent.velocity.magnitude;
                    animator.SetFloat(animatorNames.moveSpeed, move_speed);
                }
                else if (body != null)
                {
                    move_speed = (multiplyVelocity != 0) ? body.velocity.magnitude * multiplyVelocity : body.velocity.magnitude;
                    animator.SetFloat(animatorNames.moveSpeed, move_speed);
                }
            }
            else if (animationcomp != null && grounded == true)
            {
                if (agent != null && agent.velocity.magnitude >= animList.run_speed && string.IsNullOrEmpty(animList.run) == false)
                {
                    animationcomp.CrossFade(animList.run);
                }
                else if (agent != null && agent.velocity.magnitude < animList.run_speed && agent.velocity.magnitude > animList.walk_speed 
                    && string.IsNullOrEmpty(animList.walk) == false)
                {
                    animationcomp.CrossFade(animList.walk);
                }
                else if (string.IsNullOrEmpty(animList.idle) == false)
                {
                    animationcomp.CrossFade(animList.idle);
                }
            }
        }
        void UpdateHealthChanges()
        {
            if (animator != null)
            {
                if (health.GetHealth() <= 0)
                {
                    animator.SetBool(animatorNames.dead, true);
                }
                else if (lastHealth > health.GetHealth())
                {
                    lastHealth = health.GetHealth();
                    animator.SetTrigger(animatorNames.damaged);
                }
                else if (lastHealth < health.GetHealth())
                {
                    lastHealth = health.GetHealth();
                }
            }
            else if (animationcomp != null)
            {
                if (health.GetHealth() <= 0)
                {
                    if (animList.death.Length > 1)
                    {
                        animationcomp.CrossFade(animList.death[Random.Range(0, animList.death.Length - 1)]);
                    }
                }
                else if (lastHealth > health.GetHealth())
                {
                    lastHealth = health.GetHealth();
                    if (animList.hurt.Length > 1)
                    {
                        animationcomp.CrossFade(animList.hurt[Random.Range(0,animList.hurt.Length-1)]);
                    }
                }
                else if (lastHealth < health.GetHealth())
                {
                    lastHealth = health.GetHealth();
                }
            }
        }
        void UpdateRotationSpeeds()
        {
            if (animator == null || string.IsNullOrEmpty(animatorNames.rotation))
                return;
            currentFacing = transform.forward;
            angle = Vector3.Angle(currentFacing, lastFacing) / Time.deltaTime;
            lastFacing = currentFacing;
            animator.SetFloat(animatorNames.rotation, angle);
        }
        void UpdateGroundedState()
        {
            if (animator != null)
            {
            
                if (ignoreGroundCheck == true)
                {
                    return;
                }
                if (AIHelpers.RaycastHitting(transform, groundRaycast, Vector3.down, 0.4f))
                {
                    grounded = true;
                    animator.SetBool(animatorNames.grounded, true);
                }
                else
                {
                    grounded = false;
                    animator.SetBool(animatorNames.grounded, false);
                }
            }
            else if (animationcomp != null)
            {
                if (AIHelpers.RaycastHitting(transform, groundRaycast, Vector3.down, 0.4f))
                {
                    grounded = true;
                }
                else
                {
                    grounded = false;
                    animationcomp.CrossFade(animList.fall);
                }
            }
        }
        void UpdateColliderHeight()
        {
            if (animator == null)
                return;
            if (animator.GetBool("crouch") == true && GetComponent<CapsuleCollider>())
            {
                GetComponent<CapsuleCollider>().height = colliderHeight.x;
                Vector3 newCenter = GetComponent<CapsuleCollider>().center;
                newCenter.y = colliderCenter.x;
                GetComponent<CapsuleCollider>().center = newCenter;
            }
            else if (animator.GetBool("crouch") == false && GetComponent<CapsuleCollider>())
            {
                GetComponent<CapsuleCollider>().height = colliderHeight.y;
                Vector3 newCenter = GetComponent<CapsuleCollider>().center;
                newCenter.y = colliderCenter.y;
                GetComponent<CapsuleCollider>().center = newCenter;
            }
        }
    }
}
