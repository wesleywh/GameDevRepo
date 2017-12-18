using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using Pandora.Controllers;
using Pandora.Cameras;

namespace Pandora.Interactables {
    public class Moveable : MonoBehaviour {
        #region Adjustables
        public Transform left_hand = null;
        public Transform right_hand = null;
        [Header("Left Hand Placement")]
        public Vector3 left_hand_offset = Vector3.zero;
        public Vector3 left_lower_arm_offset = Vector3.zero;
        public Vector3 left_upper_arm_offset = Vector3.zero;
        [Header("Right Hand Placement")]
        public Vector3 right_hand_offset = Vector3.zero;
        public Vector3 right_lower_arm_offset = Vector3.zero;
        public Vector3 right_upper_arm_offset = Vector3.zero;
        public float move_speed = 1.0f;
        public float mouse_sensitivity = 1.0f;
        public float forward_placement = 2.0f;
        public float interact_distance = 2.0f;
        public GameObject moveable = null;
        public bool can_move = true;
        private GameObject player = null;
        public AudioSource soundSource;
        public AudioClip[] soundsWhileMoving;
        #endregion

        #region Internal Use
        private bool isMoving = false;
        private Animator shadow = null;
        private Animator hands = null;
        //arms parts
        private Transform h_left_hand = null;
        private Transform h_left_lower_arm = null;
        private Transform h_left_upper_arm = null;
        private Transform h_right_hand = null;
        private Transform h_right_lower_arm = null;
        private Transform h_right_upper_arm = null;
        private Transform s_left_hand = null;
        private Transform s_left_lower_arm = null;
        private Transform s_left_upper_arm = null;
        private Transform s_right_hand = null;
        private Transform s_right_lower_arm = null;
        private Transform s_right_upper_arm = null;
        private float stored_walk = 0;
        private float stored_run = 0;
        private float stored_mouse = 0;
        private Vector3 placement = Vector3.zero;
        private bool use_right_hand = true;
        private bool weight_set = false;
        private float weight = 0.0f;
        private Quaternion rotation;
        private bool can_select = false;
        private bool has_moved = false;
        private Vector3 last_position = Vector3.zero;
        private bool init = true;
        #endregion

        #region Assigning
        void Start()
        {
            rotation = moveable.transform.rotation;
            player = GameObject.FindGameObjectWithTag("Player");
            if (moveable == null)
                moveable = this.transform.gameObject;
        }
        void FixedUpdate()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
        }
        #endregion

        #region Hand Placement
        void AssignBoneTransforms()
        {
            //Hands - Left hand
            h_left_hand = hands.GetBoneTransform(HumanBodyBones.LeftHand);
            h_left_lower_arm = hands.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            h_left_upper_arm = hands.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            //Hands - right hand
            h_right_lower_arm = hands.GetBoneTransform(HumanBodyBones.RightLowerArm);
            h_right_upper_arm = hands.GetBoneTransform(HumanBodyBones.RightUpperArm);
            h_right_hand = hands.GetBoneTransform(HumanBodyBones.RightHand);
            //Shadow - Left hand
            s_left_hand = shadow.GetBoneTransform(HumanBodyBones.LeftHand);
            s_left_lower_arm = shadow.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            s_left_upper_arm = shadow.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            //Shadow - right hand
            s_right_lower_arm = shadow.GetBoneTransform(HumanBodyBones.RightLowerArm);
            s_right_upper_arm = shadow.GetBoneTransform(HumanBodyBones.RightUpperArm);
            s_right_hand = shadow.GetBoneTransform(HumanBodyBones.RightHand);
        }
        void SetHandPositions()
        {
            //Hands
            //--left hand
            h_left_hand.LookAt(left_hand.position);
            h_left_hand.rotation = h_left_hand.rotation * Quaternion.Euler(left_hand_offset);
            h_left_lower_arm.LookAt(left_hand.position);
            h_left_lower_arm.rotation = h_left_lower_arm.rotation * Quaternion.Euler(left_lower_arm_offset);
            h_left_upper_arm.LookAt(left_hand.position);
            h_left_upper_arm.rotation = h_left_upper_arm.rotation * Quaternion.Euler(left_upper_arm_offset);

            //Shadow
            //--left hand
            s_left_hand.LookAt(left_hand.position);
            s_left_hand.rotation = h_left_hand.rotation * Quaternion.Euler(left_hand_offset);
            s_left_lower_arm.LookAt(left_hand.position);
            s_left_lower_arm.rotation = h_left_lower_arm.rotation * Quaternion.Euler(left_lower_arm_offset);
            s_left_upper_arm.LookAt(left_hand.position);
            s_left_upper_arm.rotation = h_left_upper_arm.rotation * Quaternion.Euler(left_upper_arm_offset);

            if (use_right_hand == true)
            {
                //--right hand
                h_right_hand.LookAt(-right_hand.position);
                h_right_hand.rotation = h_right_hand.rotation * Quaternion.Euler(right_hand_offset);
                h_right_lower_arm.LookAt(-right_hand.position);
                h_right_lower_arm.rotation = h_right_lower_arm.rotation * Quaternion.Euler(right_lower_arm_offset);
                h_right_upper_arm.LookAt(-right_hand.position);
                h_right_upper_arm.rotation = h_right_upper_arm.rotation * Quaternion.Euler(right_upper_arm_offset);
                //--right hand
                s_right_hand.LookAt(right_hand.position);
                s_right_hand.rotation = h_right_hand.rotation * Quaternion.Euler(right_hand_offset);
                s_right_lower_arm.LookAt(right_hand.position);
                s_right_lower_arm.rotation = h_right_lower_arm.rotation * Quaternion.Euler(right_lower_arm_offset);
                s_right_upper_arm.LookAt(right_hand.position);
                s_right_upper_arm.rotation = h_right_upper_arm.rotation * Quaternion.Euler(right_upper_arm_offset);
            }
        }
        void SetAnimators()
        {
            shadow = player.transform.Find("PlayerShadow").GetComponent<Animator>();
            GameObject holder = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<InvWeaponManager>().GetHoldingObject();
            if (holder.GetComponent<Animator>())
            {
                use_right_hand = true;
                hands = holder.GetComponent<Animator>();
            }
            else
            {
                use_right_hand = false;
                hands = holder.GetComponentInChildren<Animator>();
            }
        }
        void LateUpdate()
        {
            if (weight_set == true)
            {
                AssignBoneTransforms();
                weight_set = false;
            }
            if (isMoving == true)
            {
                SetHandPositions();
            }
        }

        #region Legacy
        //        void SetIKWeights(float amount)
        //        {
        //            shadow.GetComponent<Animator>().SetIKPositionWeight(AvatarIKGoal.LeftHand, amount);
        //            shadow.GetComponent<Animator>().SetIKRotationWeight(AvatarIKGoal.LeftHand, amount);
        //            hands.GetComponent<Animator>().SetIKPositionWeight(AvatarIKGoal.LeftHand, amount);
        //            hands.GetComponent<Animator>().SetIKRotationWeight(AvatarIKGoal.LeftHand, amount);
        //            if (use_right_hand == true || amount == 0)
        //            {
        //                shadow.GetComponent<Animator>().SetIKPositionWeight(AvatarIKGoal.RightHand, amount);
        //                shadow.GetComponent<Animator>().SetIKRotationWeight(AvatarIKGoal.RightHand, amount);
        //                hands.GetComponent<Animator>().SetIKPositionWeight(AvatarIKGoal.RightHand, amount);
        //                hands.GetComponent<Animator>().SetIKRotationWeight(AvatarIKGoal.RightHand, amount);
        //            }
        //        }

        //        void SetIKPositions()
        //        {
        //            shadow.SetIKPosition(AvatarIKGoal.LeftHand, left_hand.position);
        //            shadow.SetIKRotation(AvatarIKGoal.LeftHand, left_hand.rotation);
        //            hands.SetIKPosition(AvatarIKGoal.LeftHand, left_hand.position);
        //            hands.SetIKRotation(AvatarIKGoal.LeftHand, left_hand.rotation);
        //            if (use_right_hand == true)
        //            {
        //                shadow.SetIKPosition(AvatarIKGoal.RightHand, right_hand.position);
        //                shadow.SetIKRotation(AvatarIKGoal.RightHand, right_hand.rotation);
        //                hands.SetIKPosition(AvatarIKGoal.RightHand, right_hand.position);
        //                hands.SetIKRotation(AvatarIKGoal.RightHand, right_hand.rotation);
        //            }
        //        }
        #endregion
        #endregion

        #region Box Movement
        void Update()
        {
            if (can_select && InputManager.GetButtonUp("Action"))
            {
                SetMoving(!isMoving);
            }
            else if (isMoving == true && InputManager.GetButtonUp("Action"))
            {
                SetMoving(false);
            }
            if (isMoving == true)
            {
                player.GetComponent<MovementController>().walkSpeed = move_speed;
                player.GetComponent<MovementController>().runSpeed = move_speed;
                player.GetComponent<MouseLook>().sensitivityX = mouse_sensitivity;
                placement = player.transform.position + player.transform.forward * forward_placement;
                placement.y = transform.position.y;
                moveable.transform.position = placement;
                moveable.transform.rotation = rotation;
            }
            if (transform.position != last_position)
            {
                last_position = transform.position;
                if (init == false)
                {
                    StartCoroutine(PlayMovingSounds());
                }
            }
            else
            {
                init = false;
                StartCoroutine(StopSounds());
            }
        }
        #endregion

        #region Enable/Disable Movement
        void OnTriggerStay(Collider col)
        {
            if (col.transform.root.tag == "Player")
            {
                can_select = true;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.transform.root.tag == "Player")
            {
                can_select = false;
            }
        }
        void SetMoving(bool state)
        {
            if (can_move == false)
            {
                isMoving = false;
                return;
            }
            if (state == true)
            {
                weight = 1;
                weight_set = true;
                stored_walk = player.GetComponent<MovementController>().walkSpeed;
                stored_run = player.GetComponent<MovementController>().runSpeed;
                stored_mouse = player.GetComponent<MouseLook>().sensitivityX;
                GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<InvWeaponManager>().EnableHands();
                SetAnimators();
            }
            else if (stored_run > 0 && stored_run > 0)
            {
                weight = 0;
                weight_set = true;
                player.GetComponent<MovementController>().walkSpeed = stored_walk;
                player.GetComponent<MovementController>().runSpeed = stored_run;
                player.GetComponent<MouseLook>().sensitivityX = stored_mouse;
            }
            isMoving = state;
        }
        #endregion
    
        #region Sounds
        IEnumerator PlayMovingSounds()
        {
            if (has_moved == false)
            {
                has_moved = true;
                AudioClip chosen = soundsWhileMoving[Random.Range(0, soundsWhileMoving.Length)];
                soundSource.clip = chosen;
                soundSource.Play();
                yield return new WaitForSeconds(chosen.length);
                has_moved = false;
            }
            else
            {
                yield break;
            }
        }
        IEnumerator StopSounds()
        {
            if (has_moved == true)
            {
                yield return new WaitForSeconds(0.4f);
                if (transform.position == last_position)
                {
                    has_moved = false;
                    soundSource.Stop();
                }
            }
            else
            {
                yield break;
            }
        }
        #endregion
    }
}