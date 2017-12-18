using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pandora.Controllers;
using Pandora.AI;

namespace Pandora.AI {
    public class AIAnimSync : MonoBehaviour {
        #region Adjustables
        public Animator anim = null;
        public int updateEveryXFrames = 3;
        public NavMeshAgent agent = null;
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
        #endregion
    	
    	void Start () 
        {
            anim = (anim == null) ? GetComponent<Animator>() : anim;
            agent = (agent == null) ? GetComponent<NavMeshAgent>() : agent;
            health = (health == null) ? GetComponent<Health>() : health;
            lastHealth = health.GetHealth();
            groundRaycast = (groundRaycast == null) ? transform : groundRaycast;
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
            Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
            anim.SetFloat("velocityX", localVelocity.x);
            anim.SetFloat("velocityZ", localVelocity.z);
            anim.SetFloat("moveSpeed", agent.velocity.magnitude);
        }
        void UpdateHealthChanges()
        {
            if (health.GetHealth() <= 0)
            {
                anim.SetBool("dead", true);
            }
            else if (lastHealth > health.GetHealth())
            {
                lastHealth = health.GetHealth();
                anim.SetTrigger("damaged");
            }
            else if (lastHealth < health.GetHealth())
            {
                lastHealth = health.GetHealth();
            }
        }
        void UpdateRotationSpeeds()
        {
            currentFacing = transform.forward;
            angle = Vector3.Angle(currentFacing, lastFacing) / Time.deltaTime;
            lastFacing = currentFacing;
            anim.SetFloat("rotation", angle);
        }
        void UpdateGroundedState()
        {
            if (ignoreGroundCheck == true)
            {
                return;
            }
            if (AIHelpers.RaycastHitting(transform, groundRaycast, Vector3.down, 0.4f))
            {
                anim.SetBool("grounded", true);
            }
            else
            {
                anim.SetBool("grounded", false);
            }
        }
        void UpdateColliderHeight()
        {
            if (anim.GetBool("crouch") == true && GetComponent<CapsuleCollider>())
            {
                GetComponent<CapsuleCollider>().height = colliderHeight.x;
                Vector3 newCenter = GetComponent<CapsuleCollider>().center;
                newCenter.y = colliderCenter.x;
                GetComponent<CapsuleCollider>().center = newCenter;
            }
            else if (anim.GetBool("crouch") == false && GetComponent<CapsuleCollider>())
            {
                GetComponent<CapsuleCollider>().height = colliderHeight.y;
                Vector3 newCenter = GetComponent<CapsuleCollider>().center;
                newCenter.y = colliderCenter.y;
                GetComponent<CapsuleCollider>().center = newCenter;
            }
        }
    }
}
