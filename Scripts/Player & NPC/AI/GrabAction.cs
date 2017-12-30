using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pandora.Helpers;
using Pandora.Controllers;
using Pandora.Cameras;
using Pandora.GameManager;

using TeamUtility.IO;

namespace Pandora.AI {
    [Serializable]
    public class GrabOffsets {
        public Vector3 standPosition = Vector3.zero;
        public Vector3 standRotation = Vector3.zero;
        public Vector3 leftHandPosition = Vector3.zero;
        public Vector3 leftHandRotation = Vector3.zero;
        public Vector3 rightHandPosition = Vector3.zero;
        public Vector3 rightHandRotation = Vector3.zero;
        public Vector3 lookOffset = Vector3.zero;
    }
    [Serializable]
    public class GrabEvents {
        public UnityEvent OnEnable = null;
        public UnityEvent OnDisable = null;
    }
    [Serializable]
    public class GrabTransforms {
        public string handPositionHolderTag = "PlayerHands";
        public string leftHandChild = "LeftHand";
        public string rightHandChild = "RightHand";
        public string standChild = "StandPosition";
        public bool disableLeftHand = false;
        public bool disableRightHand = false;
    }
    [Serializable]
    public class GrabText {
        public string struggleText = "Press <ACTION> to break free.";
        public int minSize = 20;
        public int maxSize = 30;
        public float textTransitionTime = 30f;
        public float textBottomOffset = 100f;
        public GUIStyle style = null;
    }
    [Serializable]
    public class GrabCamera {
        public float shakeAmount = 3.0f;
        public float shakeDuration = 4.0f;
    }
    public class GrabAction : MonoBehaviour {
        #region Modifiable Variables
        public Animator anim;
        public GrabTransforms transforms;
        public GrabText text;
        public GrabEvents events;
        public GrabOffsets offsets;
        public GrabCamera cam;
        public float breakFreeSpeed = 5;
        public float breakTimeout = 5;
        #endregion
        #region Internal Use
        private Transform leftHand;
        private Transform rightHand;
        private bool isEnabled = false;
        private bool displayActionText = false;
        private float weight = 0;
        private Transform stand = null;
       
        private Quaternion lhRotaion;
        private Quaternion rhRotation;
        private Vector3 lhPosition;
        private Vector3 rhPosition;

        private MovementController mc;
        private float textSize = 0;
        private bool dir = false;
        private int breakCounter = 0;
        private float timeout = 0;
        #endregion
        [Header("Debugging")]
        [SerializeField] private bool enableGrab = false;    
        [SerializeField] private bool debugLookOffset = false;
        [SerializeField] private bool endGrab = false;

        private PlayerManager playerManager;
        void Start()
        {
            textSize = text.minSize;
            playerManager = dontDestroy.currentGameManager.GetComponent<PlayerManager>();
        }
    	void Update () {
            if (enableGrab == true)
            {
                enableGrab = false;
                TriggerGrab();
            }
            if (isEnabled == true)
            {
                if (dir == false && textSize > text.minSize)
                {
                    textSize -= Time.deltaTime * text.textTransitionTime;
                }
                else if (dir == true && textSize <= text.minSize)
                {
                    textSize += Time.deltaTime * text.textTransitionTime;
                }
                else
                {
                    dir = !dir;
                }
                MoveToStand();
                if (InputManager.GetButtonDown("Action"))
                {
                    breakCounter += 1;
                    if (breakCounter >= breakFreeSpeed)
                    {
                        EndGrab();
                    }
                }
                timeout += Time.deltaTime;
                if (timeout >= breakTimeout)
                {
                    EndGrab();
                }
            }
            if (debugLookOffset == true)
            {
                GameObject.FindGameObjectWithTag("CameraHolder").transform.LookAt(this.transform.position + offsets.lookOffset);
            }
            if (endGrab == true)
            {
                endGrab = false;
                EndGrab();
            }
    	}

        public void TriggerGrab()
        {
            breakCounter = 0;
            timeout = 0;
            mc = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementController>();
            mc.enabled = false;
            GameObject.FindGameObjectWithTag("Player").GetComponent<MouseLook>().enable = false;
            GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<CameraShake>().ShakeCamera(cam.shakeAmount,cam.shakeDuration);

            playerManager.EnableCameraControl(false);

            weight = 1;
            leftHand = GameObject.FindGameObjectWithTag(transforms.handPositionHolderTag).transform.Find(transforms.leftHandChild);
            rightHand = GameObject.FindGameObjectWithTag(transforms.handPositionHolderTag).transform.Find(transforms.rightHandChild);
            isEnabled = true;
            displayActionText = true;
            text.struggleText = Helpers.Helpers.ModifiedText(Helpers.ButtonOptions.Action, text.struggleText);
            events.OnEnable.Invoke();
        }

        void MoveToStand()
        {
            stand = (stand == null) ? GameObject.FindGameObjectWithTag(transforms.handPositionHolderTag).transform.Find(transforms.standChild) : stand;
            transform.position = stand.position + offsets.standPosition;
            transform.rotation = stand.rotation * Quaternion.Euler(offsets.standRotation);
        }

        public void EndGrab()
        {
            isEnabled = false;
            displayActionText = false;
            events.OnDisable.Invoke();
            mc.enabled = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<MouseLook>().enable = true;
            playerManager.EnableCameraControl(true);
        }

        void OnAnimatorIK() {
            if (anim == null || isEnabled == false)
                return;
            if (leftHand != null)
            {
                lhPosition = leftHand.position + offsets.leftHandPosition;
                anim.SetIKPosition(AvatarIKGoal.LeftHand, lhPosition);
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
                lhRotaion = leftHand.rotation * Quaternion.Euler(offsets.leftHandRotation);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, lhRotaion);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
            }
            if (rightHand != null)
            {
                rhPosition = rightHand.position + offsets.rightHandPosition;
                anim.SetIKPosition(AvatarIKGoal.RightHand, rhPosition);
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
                rhRotation = rightHand.rotation * Quaternion.Euler(offsets.rightHandRotation);
                anim.SetIKRotation(AvatarIKGoal.RightHand, rhRotation);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
            }
        }
    
        void OnGUI()
        {
            if (displayActionText == true)
            {
                text.style.fontSize = Mathf.RoundToInt(textSize);
                GUI.TextArea(new Rect(Screen.width/2, (Screen.height - text.textBottomOffset), 0, 0), text.struggleText, 1000, text.style);
            }
        }
    }
}