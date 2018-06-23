using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CyberBullet.Weapons {
    #region Events
    [Serializable]
    public class ReloadEvents {
        public UnityEvent OnReloadStart;
        public UnityEvent OnReloadEnd;
    }
    #endregion

    #region HandPlacements
    [Serializable]
    public class ReloadAnimEvent {
        public float transitionTime = 2.0f;
        public float errorRange = 0.0001f;
        [Header("Left Hand")]
        public bool ignoreLeftHand = false;
        public Transform leftHandPosition = null;
        public Vector3 leftPos = Vector3.zero;
        public Vector3 leftRot = Vector3.zero;
        public float leftPosSpeed = 0.1f;
        public float leftRotSpeed = 300f;
        [Header("Right Hand")]
        public bool ignoreRightHand = false;
        public Transform rightHandPosition = null;
        public Vector3 rightPos = Vector3.zero;
        public Vector3 rightRot = Vector3.zero;
        public float rightPosSpeed = 0.1f;
        public float rightRotSpeed = 300f;
        public UnityEvent nodeEvent;
    }
    #endregion

    public class HandReloadAnim : MonoBehaviour {

        #region Variables
        #region Adjustables
        public Animator anim = null;
        public ReloadEvents events;
        public ReloadAnimEvent[] animEvents;
        public Transform startleft = null;
        public Transform startright = null;
        #endregion
        #region Internal Use Only
        private Vector3 diff = Vector3.zero;
        private bool next = false;
        private bool started = false;
        private bool prev = false;
        private int idx = -1;
        private float timer = 0;
        private bool eventTriggered = false;
        #endregion

        #region Body Parts
        private Transform tempLeft;
        private Vector3 tempLeftPos = Vector3.zero;
        private Vector3 leftVel = Vector3.zero;
        private Quaternion tempLeftRot;
        private Vector3 tempRightPos = Vector3.zero;
        private Vector3 rightVel = Vector3.zero;
        private Quaternion tempRightRot;
        #endregion

        [Header("Debugging")]
        public bool trigger = false;
        public bool run_index = false;
        public int test_index = 0;
        #endregion

        void Start()
        {
            anim = (anim == null) ? GetComponent<Animator>() : anim;
            if (startleft)
            {
                tempLeftPos = startleft.position;
                tempLeftRot = startleft.rotation;
            }
            if (startright)
            {
                tempRightPos = startright.position;
                tempRightRot = startright.rotation;
            }
        }
        void Update()
        {
            if (prev != started && started == true)
            {
                prev = started;
                events.OnReloadStart.Invoke();
                next = true;
            }
            if (prev != started && started == false)
            {
                prev = started;
                events.OnReloadEnd.Invoke();
                next = false;
            }
            if (next == true)
            {
                timer += Time.deltaTime;
                if (idx >= animEvents.Length)
                {
                    started = false;
                }
                else
                {
                    TriggerEvent();
                    PositionLeftHand();
                    PositionRightHand();
                }

            }
            if (trigger == true)
            {
                trigger = false;
                StartReload();
            }
            if (run_index == true)
            {
                events.OnReloadStart.Invoke();
                idx = test_index;
                TriggerEvent();
                PositionLeftHand();
                PositionRightHand();
            }
        }

        #region Positioning Functions
        void PositionLeftHand()
        {
            if (animEvents[idx].leftHandPosition)
            {
                tempLeftPos = Vector3.SmoothDamp(tempLeftPos, animEvents[idx].leftHandPosition.position, ref leftVel, animEvents[idx].leftPosSpeed);
                tempLeftRot = Quaternion.RotateTowards(tempLeftRot, animEvents[idx].leftHandPosition.rotation, animEvents[idx].leftRotSpeed * Time.deltaTime);
            }
            else
            {
                tempLeftPos = Vector3.SmoothDamp(tempLeftPos, animEvents[idx].leftPos, ref leftVel, animEvents[idx].leftPosSpeed);
                tempLeftRot = Quaternion.RotateTowards(tempLeftRot, Quaternion.Euler(animEvents[idx].leftRot), animEvents[idx].leftRotSpeed * Time.deltaTime);
            }
            diff = (animEvents[idx].leftHandPosition) ? tempLeftPos - animEvents[idx].leftHandPosition.position : tempLeftPos - animEvents[idx].leftPos;
            if (diff.sqrMagnitude < animEvents[idx].errorRange)
            {
                if (timer >= animEvents[idx].transitionTime && run_index == false)
                {
                    eventTriggered = false;
                    timer = 0;
                    idx += 1;
                }
            }
        }
        void PositionRightHand()
        {
            if (idx < animEvents.Length)
            {
                if (animEvents[idx].rightHandPosition)
                {
                    tempRightPos = Vector3.SmoothDamp(tempRightPos, animEvents[idx].rightHandPosition.position, ref rightVel, animEvents[idx].leftPosSpeed);
                    tempRightRot = Quaternion.RotateTowards(tempRightRot, animEvents[idx].rightHandPosition.rotation, animEvents[idx].leftRotSpeed * Time.deltaTime);
                }
                else
                {
                    tempRightPos = Vector3.SmoothDamp(tempRightPos, animEvents[idx].rightPos, ref rightVel, animEvents[idx].rightPosSpeed);
                    tempRightRot = Quaternion.RotateTowards(tempRightRot, Quaternion.Euler(animEvents[idx].rightRot), animEvents[idx].rightRotSpeed * Time.deltaTime);
                }
                diff = (animEvents[idx].rightHandPosition) ? tempRightPos - animEvents[idx].rightHandPosition.position : tempRightPos - animEvents[idx].leftPos;
                if (diff.sqrMagnitude < animEvents[idx].errorRange)
                {
                    if (timer >= animEvents[idx].transitionTime && run_index == false)
                    {
                        eventTriggered = false;
                        timer = 0;
                        idx += 1;
                    }
                }
            }
        }
        void OnAnimatorIK() {
            if (anim == null || (started == false && run_index == false) || idx >= animEvents.Length)
                return;
            if (animEvents[idx].ignoreLeftHand == false)
            {
                anim.SetIKPosition(AvatarIKGoal.LeftHand, tempLeftPos);
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, tempLeftRot);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            }
            if (animEvents[idx].ignoreRightHand == false)
            {
                anim.SetIKPosition(AvatarIKGoal.RightHand, tempRightPos);
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKRotation(AvatarIKGoal.RightHand, tempRightRot);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            }
        }
        #endregion

        void TriggerEvent()
        {
            if (eventTriggered == true)
            {
                return;
            }
            else
            {
                eventTriggered = true;
                animEvents[idx].nodeEvent.Invoke();
            }
        }
        public void StartReload()
        {
            started = true;
            idx = 0;
            next = false;
            timer = 0;
        }
    }
}