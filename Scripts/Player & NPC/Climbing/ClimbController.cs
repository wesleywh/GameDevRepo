using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;
using Pandora.Climbing;
using Pandora.Cameras;

namespace Pandora.Controllers {
	public class ClimbController : MonoBehaviour {
        #region Variabes
        #region Enable/Disables
        [Header("Initializations")]
        public GameObject[] enableWhenClimbing = null;
        public GameObject[] disableWhenClimbing = null;
        public MovementController moveController;
        public SwimController swimController;
        public WeaponManagerNew weaponManager;
        #endregion
        #region Movement
        [Header("Movement")]
        public Camera EnableCamera;
        public float reachDistance = 2.0f;
        public LayerMask climbLayer;
        float inputX = 0.0f;
        float inputY = 0.0f;
        float initalizeTime = 1.0f;
        float waitPointTime = 0.1f;
        Transform targetPoint = null;
        bool isMoving = false;
        ClimbPoint current = null;

        Transform startMarker = null;
        Transform endMarker = null;
        public float climbSpeed = 1.0F;
        private float orgClimbSpeed = 1.0f;
        private float initClimbSpeed = 1.0f;
        float startTime = 0.0f;
        private float journeyLength = 0.0f;
        private bool autoDismount = false;
        public float dismountWaitTime = 2.0f;
        private float dismountWait = 0.0f;
        #endregion

        #region IK
        #region IK Management
        [Header("IK")]
        public Animator[] anims;
        bool enableIK = false;
//            bool[] movingLeft;
//            bool[] movingRight;
        public float handGrabSpeed = 2.0f;
        #endregion
        #region IK Lerping Movement
        public float moveHandsWait = 0.5f;
        //Actual IKPositions
        //Hands & Feet IK
        Transform lhTarget = null;
        Transform rhTarget = null;
        Transform lfTarget = null;
        Transform rfTarget = null;
        //Hints IK
        Transform leTarget = null;
        Transform reTarget = null;
        Transform lkTarget = null;
        Transform rkTarget = null;
        //Lerping - Starts
        //Hands & Feet IK
        Transform lhTarget_start;
        Transform rhTarget_start;
        Transform lfTarget_start;
        Transform rfTarget_start;
        //Hints IK
        Transform leTarget_start;
        Transform reTarget_start;
        Transform lkTarget_start;
        Transform rkTarget_start;
        //Lerping - Ends
        //Hands & Feet IK
        Transform lhTarget_final;
        Transform rhTarget_final;
        Transform lfTarget_final;
        Transform rfTarget_final;
        //Hints IK
        Transform leTarget_final;
        Transform reTarget_final;
        Transform lkTarget_final;
        Transform rkTarget_final;
       
        //Lerping - Lengths
        //IKs
        float lhLength;
        float rhLength;
        float lfLength;
        float rfLength;
        //Hints
        float leLength;
        float reLength;
        float lkLength;
        float rkLength;
        //Lerping - Unversal
        float left_ikstartTime;
        float right_ikstartTime;
        #endregion
        #endregion
        #region For Debugging Purposes
        [HideInInspector] public bool canMove = false;
        public bool debugLeftHand = false;
        public bool debugRightHand = false;
        #endregion
        #endregion

        void Start() {
            orgClimbSpeed = climbSpeed;
            moveController = (moveController == null) ? GetComponent<MovementController>() : moveController;
            swimController = (swimController == null) ? GetComponent<SwimController>() : swimController;
            weaponManager = (weaponManager == null) ? GetComponentInChildren<WeaponManagerNew>() : weaponManager;
        }

        #region Main Decisions
        void Update() 
        {
            if (autoDismount == true)
            {
                dismountWait += Time.deltaTime;
                if (dismountWait >= dismountWaitTime)
                {
                    Dismount();
                }
            }
            if (canMove == true)
            {
                if (InputManager.GetButton("Dismount"))
                {
                    Dismount();
                    return;
                }
                inputX = InputManager.GetAxis("Horizontal");
                inputY = InputManager.GetAxis("Vertical");
                SetMovement(inputX, inputY);
                if (inputY > 0)
                {
                    AssignTarget(ClimbDirection.Up);
                }
                else if (inputY < 0)
                {
                    AssignTarget(ClimbDirection.Down);
                }
                else if (inputX > 0)
                {
                    AssignTarget(ClimbDirection.Right);
                }
                else if (inputX < 0)
                {
                    AssignTarget(ClimbDirection.Left);
                }
            }
            else
            {
                SetMovement(0, 0);
            }
            if (enableIK == true)
            {
                SetLerp(lhTarget_start, lhTarget_final, lhLength, handGrabSpeed, left_ikstartTime, lhTarget);
                SetLerp(lfTarget_start, lfTarget_final, lfLength, handGrabSpeed, left_ikstartTime, lfTarget);
                SetLerp(leTarget_start, leTarget_final, leLength, handGrabSpeed, left_ikstartTime, leTarget);
                SetLerp(lkTarget_start, lkTarget_final, lkLength, handGrabSpeed, left_ikstartTime, lkTarget);

                SetLerp(rhTarget_start, rhTarget_final, rhLength, handGrabSpeed, right_ikstartTime, rhTarget);
                SetLerp(rfTarget_start, rfTarget_final, rfLength, handGrabSpeed, right_ikstartTime, rfTarget);
                SetLerp(reTarget_start, reTarget_final, reLength, handGrabSpeed, right_ikstartTime, reTarget);
                SetLerp(rkTarget_start, rkTarget_final, rkLength, handGrabSpeed, right_ikstartTime, rkTarget);
            }
            if (isMoving == true)
            {
                if (SetLerp(startMarker,endMarker,journeyLength,climbSpeed,startTime,transform))
                {
                    climbSpeed = orgClimbSpeed;             //for reseting after init function
                    transform.position = endMarker.position;
                    isMoving = false;
                    StartCoroutine(EnablePoint());
                }
            }
        }
        #endregion

        #region Enabling/Disable Climbing
        public void InitClimbing() {
            if (enableWhenClimbing != null)
            {
                foreach (GameObject targetObj in enableWhenClimbing)
                {
                    targetObj.SetActive(true);
                }
            }
            if (disableWhenClimbing != null)
            {
                foreach (GameObject targetObj in disableWhenClimbing)
                {
                    targetObj.SetActive(false);
                }
            }
            current = GetClosestClimbPoint();
            if (current != null)
            {
                climbSpeed = initClimbSpeed;
                AssignInitialIKTarget(current.GetComponent<ClimbPoint>());
                AssignMovePoints(this.transform, current.transform);
                SetCameraRotationPoints(true, current.transform);
                StartCoroutine(EnableClimbing());
                moveController.enabled = false;
                swimController.enabled = false;
                SetClimbAnimation(true);
                enableIK = true;
                weaponManager.canEquipWeapons = false;
                weaponManager.SelectWeapon(0);
            }
        }
        public void Dismount() {
            canMove = false;
            dismountWait = 0.0f;
            autoDismount = false;
            SetCameraRotationPoints(false);
            SetClimbAnimation(false);
            current = null;
            enableIK = false;
            DestroyAllTempObjects();
            moveController.enabled = true;
            swimController.enabled = true;
            weaponManager.canEquipWeapons = true;
        }
        void DestroyAllTempObjects() 
        {
            //Destroy Lerp Points
            Destroy(lhTarget.gameObject);
            Destroy(leTarget.gameObject);
            Destroy(lkTarget.gameObject);
            Destroy(lfTarget.gameObject);
            Destroy(rhTarget.gameObject);
            Destroy(rfTarget.gameObject);
            Destroy(reTarget.gameObject);
            Destroy(rkTarget.gameObject);

            //Destroy Start Points
            lhTarget_start = null;
            leTarget_start = null;
            lkTarget_start = null;
            lfTarget_start = null;
            rhTarget_start = null;
            reTarget_start = null;
            rkTarget_start = null;
            rfTarget_start = null;

            //Destroy Final Points
            rhTarget_final = null;
            rfTarget_final = null;
            reTarget_final = null;
            rkTarget_final = null;
            lhTarget_final = null;
            lfTarget_final = null;
            leTarget_final = null;
            lkTarget_final = null;
        }
        void SetCameraRotationPoints(bool climbing, Transform target=null) {
            if (climbing == true)
            {
                if (transform.GetComponent<MouseLook>())
                    transform.GetComponent<MouseLook>().enable = false;
                foreach (MouseLook script in transform.GetComponentsInChildren<MouseLook>())
                {
                    script.enable = false;
                }

                Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = targetRotation;

                if (transform.GetComponent<MouseLook>())
                    transform.GetComponent<MouseLook>().enable = true;
                foreach (MouseLook script in transform.GetComponentsInChildren<MouseLook>())
                {
                    script.enable = true;
                }
//                    transform.GetComponent<MouseLook>().minimumX = angle-50;
//                    transform.GetComponent<MouseLook>().maximumX = angle+50;
            }
            else
            {
                transform.GetComponent<MouseLook>().minimumX = -360;
                transform.GetComponent<MouseLook>().maximumX = 360;
            }
        }
        ClimbPoint GetClosestClimbPoint() {
            Climbing.ClimbPoint retVal = null;
            Ray ray = EnableCamera.ScreenPointToRay(InputManager.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, reachDistance, climbLayer))
            {
                Climbing.ClimbPoint[] points = hit.transform.GetComponentsInChildren<Climbing.ClimbPoint>();
                Climbing.ClimbPoint closest = null;
                float closetDistance = Mathf.Infinity;
                foreach (Climbing.ClimbPoint point in points)
                {
                    if (Vector3.Distance(transform.position, point.transform.position) < closetDistance)
                    {
                        closest = point;
                        closetDistance = Vector3.Distance(transform.position, point.transform.position);
                        transform.LookAt(point.transform.position);
                    }
                    retVal = closest;
                }
            }
            return retVal;
        }
        IEnumerator EnablePoint() {
            yield return new WaitForSeconds(waitPointTime);
            canMove = true;
        }
        IEnumerator EnableClimbing() {
            yield return new WaitForSeconds(initalizeTime);
            canMove = true;
        }
        #endregion

        #region Target Assignment
        void AssignTarget(ClimbDirection direction) {
            canMove = false;
            targetPoint = GetDestination(direction);
            if (targetPoint != null)
            {
                //General Movement
                AssignMovePoints(this.transform, targetPoint);
                //IK
                if (direction == ClimbDirection.Down || direction == ClimbDirection.Left)
                    StartCoroutine(MovingHands(true, targetPoint.GetComponent<ClimbPoint>()));
                else
                    StartCoroutine(MovingHands(false, targetPoint.GetComponent<ClimbPoint>()));
                if (GetTransition(direction) == ClimbTransitionType.dismount)
                {
                    autoDismount = true;
                }
                current = targetPoint.GetComponent<ClimbPoint>();
            }
            else
            {
                canMove = true;
            }
        }
        void AssignMovePoints(Transform start, Transform end) {
            startMarker = start;
            endMarker = end;
            startTime = Time.time;
            journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
            isMoving = true;
        }
        ClimbTransitionType GetTransition(ClimbDirection direction) {
            ClimbTransitionType retVal = ClimbTransitionType.step;
            foreach (Climbing.ClimbNeighbor point in current.neighbors)
            {
                if (point.direction == direction)
                {
                    retVal = point.type;
                    break;
                }
            }
            return retVal;
        }
        Transform GetDestination(ClimbDirection direction) 
        {
            Transform retVal = null;
            foreach (Climbing.ClimbNeighbor point in current.neighbors)
            {
                if (point.direction == direction)
                {
                    retVal = point.target.transform;
                    break;
                }
            }
            return retVal;
        }
        bool SetLerp (Transform start, Transform end, float inputLength, float speed, float inputTime, Transform target) {
            if (Vector3.Distance(target.position, end.position) <= 0.05f)
            {
                target.position = end.position;
                target.rotation = end.rotation;
                return true;
            }
            float distCovered = (Time.time - inputTime) * speed;
            float journey = distCovered / inputLength;
            target.position = Vector3.Lerp(start.position, end.position, journey);
            return false;
        }
        #endregion

        #region IK
        void AssignInitialTransforms() {
            //Assign Body Parts
            //-- lefts
            lhTarget = (lhTarget == null) ? new GameObject("climb_lhTarget").transform : lhTarget;
            leTarget = (leTarget == null) ? new GameObject("climb_lhTarget").transform : leTarget;
            lkTarget = (lkTarget == null) ? new GameObject("climb_lhTarget").transform : lkTarget;
            lfTarget = (lfTarget == null) ? new GameObject("climb_lhTarget").transform : lfTarget;
            //-- rights
            rhTarget = (rhTarget == null) ? new GameObject("climb_lhTarget").transform : rhTarget;
            rfTarget = (rfTarget == null) ? new GameObject("climb_lhTarget").transform : rfTarget;
            reTarget = (reTarget == null) ? new GameObject("climb_lhTarget").transform : reTarget;
            rkTarget = (rkTarget == null) ? new GameObject("climb_lhTarget").transform : rkTarget;
        }
        void ClearIKTargets() {
            lhTarget = null;
            leTarget = null;
            lkTarget = null;
            lfTarget = null;

            rhTarget = null;
            rfTarget = null;
            reTarget = null;
            rkTarget = null;
        }
        void ClearIKStarts() 
        {
            lhTarget_start = null;
            leTarget_start = null;
            lkTarget_start = null;
            lfTarget_start = null;
            rhTarget_start = null;
            reTarget_start = null;
            rkTarget_start = null;
            rfTarget_start = null;
        }
        void ClearIKFinals() 
        {
            rhTarget_final = null;
            rfTarget_final = null;
            reTarget_final = null;
            rkTarget_final = null;
            lhTarget_final = null;
            lfTarget_final = null;
            leTarget_final = null;
            lkTarget_final = null;
        }
        void ClearIKLengths() {
            lhLength = 0.0f;
            lfLength = 0.0f;
            leLength = 0.0f;
            lkLength = 0.0f;

            rhLength = 0.0f;
            rfLength = 0.0f;
            reLength = 0.0f;
            rkLength = 0.0f;
        }
        void ClearIKStartTimes() {
            left_ikstartTime = 0.0f;
            right_ikstartTime = 0.0f;
        }
        void OnAnimatorIK(){
            if (enableIK == true)
            {
                for (int i = 0; i < anims.Length; i++)
                {
                    //--- IKs
                    SetIK(AvatarIKGoal.LeftHand, lhTarget, anims[i]);
                    SetIK(AvatarIKGoal.LeftFoot, lfTarget, anims[i]);
                    SetHintIK(AvatarIKHint.LeftElbow, leTarget, anims[i]);
                    SetHintIK(AvatarIKHint.LeftKnee, lkTarget, anims[i]);
//                        //-- Hints
                    SetIK(AvatarIKGoal.RightFoot, rfTarget, anims[i]);
                    SetIK(AvatarIKGoal.RightHand, rhTarget, anims[i]);
                    SetHintIK(AvatarIKHint.RightElbow, reTarget, anims[i]);
                    SetHintIK(AvatarIKHint.RightKnee, rkTarget, anims[i]);
                }
            }
        }
        void SetIK(AvatarIKGoal ik, Transform target, Animator anim)
        {
            anim.SetIKPosition (ik, target.position);
            anim.SetIKPositionWeight (ik, 1f);
            anim.SetIKRotation (ik, target.rotation);
            anim.SetIKRotationWeight (ik, 1f);
        }
        void SetHintIK(AvatarIKHint ik, Transform target, Animator anim)
        {
            anim.SetIKHintPosition(ik, target.position);
            anim.SetIKHintPositionWeight(ik, 1f);
        }
        void AssignInitialIKTarget(ClimbPoint destination) {
            ClearIKTargets();
            ClearIKStarts();
            ClearIKFinals();
            ClearIKLengths();
            ClearIKStartTimes();
            AssignInitialTransforms();

            //Assign Starts
            lhTarget_start = current.transform.Find("Hands").transform;
            leTarget_start = current.transform.Find("Hands").transform;
            lkTarget_start = current.transform.Find("Feet").transform;
            lfTarget_start = current.transform.Find("Feet").transform;

            rhTarget_start = current.transform.Find("Hands").transform;
            reTarget_start = current.transform.Find("Hands").transform;
            rkTarget_start = current.transform.Find("Feet").transform;
            rfTarget_start = current.transform.Find("Feet").transform;

            //Assign the start value at the start point
            lhTarget.position = lhTarget_start.position;
            leTarget.position = leTarget_start.position;
            lkTarget.position = lkTarget_start.position;
            lfTarget.position = lfTarget_start.position;

            rhTarget.position = rhTarget_start.position;
            reTarget.position = reTarget_start.position;
            rkTarget.position = rkTarget_start.position;
            rfTarget.position = rfTarget_start.position;

            //Assign Finals
            lhTarget_final = GetIKGoalTransform(AvatarIKGoal.LeftHand, destination);
            lfTarget_final = GetIKGoalTransform(AvatarIKGoal.LeftFoot, destination);
            leTarget_final = GetIKHintTransform(AvatarIKHint.LeftElbow, destination);
            lkTarget_final = GetIKHintTransform(AvatarIKHint.LeftKnee, destination);

            rhTarget_final = GetIKGoalTransform(AvatarIKGoal.RightHand, destination);
            rfTarget_final = GetIKGoalTransform(AvatarIKGoal.RightFoot, destination);
            reTarget_final = GetIKHintTransform(AvatarIKHint.RightElbow, destination);
            rkTarget_final = GetIKHintTransform(AvatarIKHint.RightKnee, destination);

            //Assign Lengths
            lhLength = Vector3.Distance(lhTarget_start.position, lhTarget_final.position);
            rhLength = Vector3.Distance(rhTarget_start.position, rhTarget_final.position);
            lfLength = Vector3.Distance(lfTarget_start.position, lfTarget_final.position);
            rfLength = Vector3.Distance(rfTarget_start.position, rfTarget_final.position);

            leLength = Vector3.Distance(leTarget_start.position, leTarget_final.position);
            reLength = Vector3.Distance(reTarget_start.position, reTarget_final.position);
            lkLength = Vector3.Distance(lkTarget_start.position, lkTarget_final.position);
            rkLength = Vector3.Distance(rkTarget_start.position, rkTarget_final.position);

            //Set Start Times
            left_ikstartTime = Time.time;
            right_ikstartTime = Time.time;

        }
        IEnumerator MovingHands(bool left, ClimbPoint destination) {
            AssignIKTarget(left, destination);
            yield return new WaitForSeconds(moveHandsWait);
            AssignIKTarget(!left, destination);
        }
        void AssignIKTarget(bool left, ClimbPoint destination) 
        {
            if (left == true)
            {
                //Assign Left Initial Positions
                lhTarget_start = current.transform.Find("Hands").transform;//lhTarget[i];
                leTarget_start = current.transform.Find("Hands").transform;//leTarget[i];
                lfTarget_start = current.transform.Find("Feet").transform;//lfTarget[i];
                lkTarget_start = current.transform.Find("Feet").transform;//lkTarget[i];
                //Assign Left Final Positions
                lhTarget_final = GetIKGoalTransform(AvatarIKGoal.LeftHand, destination);
                lfTarget_final = GetIKGoalTransform(AvatarIKGoal.LeftFoot, destination);
                leTarget_final = GetIKHintTransform(AvatarIKHint.LeftElbow, destination);
                lkTarget_final = GetIKHintTransform(AvatarIKHint.LeftKnee, destination);
                //Assign travel length
                lhLength = Vector3.Distance(lhTarget_start.position, lhTarget_final.position);
                lfLength = Vector3.Distance(lfTarget_start.position, lfTarget_final.position);
                leLength = Vector3.Distance(leTarget_start.position, leTarget_final.position);
                lkLength = Vector3.Distance(lkTarget_start.position, lkTarget_final.position);
                //Set the start times
                left_ikstartTime = Time.time;
            }
            else
            {
                //Assign Right Initial Positions
                rhTarget_start = rhTarget;
                rfTarget_start = rfTarget;
                reTarget_start = reTarget;
                rkTarget_start = rkTarget;
                //Assign Right Final Positions
                rhTarget_final = GetIKGoalTransform(AvatarIKGoal.RightHand, destination);
                rfTarget_final = GetIKGoalTransform(AvatarIKGoal.RightFoot, destination);
                reTarget_final = GetIKHintTransform(AvatarIKHint.RightElbow, destination);
                rkTarget_final = GetIKHintTransform(AvatarIKHint.RightKnee, destination);
                //Assign travel length
                rhLength = Vector3.Distance(rhTarget_start.position, rhTarget_final.position);
                rfLength = Vector3.Distance(rfTarget_start.position, rfTarget_final.position);
                reLength = Vector3.Distance(reTarget_start.position, reTarget_final.position);
                rkLength = Vector3.Distance(rkTarget_start.position, rkTarget_final.position);
                //Set the start times
                right_ikstartTime = Time.time;
            }
        }
        Transform GetIKGoalTransform(AvatarIKGoal ik,  ClimbPoint target) 
        {
            Transform retVal = null;
            foreach (ClimbIKPositions point in target.iks)
            {
                if (point.ikType == ik)
                {
                    retVal = point.target;
                    break;
                }
            }
            return retVal;
        }
        Transform GetIKHintTransform(AvatarIKHint ik,  ClimbPoint target) 
        {
            Transform retVal = null;
            foreach (ClimbHintIKPositions point in target.hints)
            {
                if (point.hintType == ik)
                {
                    retVal = point.target;
                    break;
                }
            }
            return retVal;
        }
        #endregion

        #region Animations
        void SetClimbAnimation(bool value) {
            foreach (Animator anim in anims)
            {
                anim.SetBool("climb", value);
            }
        }
        void SetMovement(float x, float y) {
            float value = Mathf.Abs(x) + Mathf.Abs(y);
            value = Mathf.Clamp01(value);
            foreach (Animator anim in anims)
            {
                if (anim.transform.gameObject.activeSelf == false)
                    continue;
                anim.SetFloat("speed", y);
                anim.SetFloat("direction", x);
                anim.SetFloat("Movement", value);
            }
        }
        #endregion

        #region Debugging
        void OnDrawGizmosSelected() 
        {
            if (debugLeftHand == true)
            {
                Gizmos.color = new Color(1, 1, 0, 0.5F);
                Gizmos.DrawCube(lhTarget.position, new Vector3(0.25f, 0.25f, 0.25f));
                Gizmos.color = new Color(0, 1, 0, 0.5F);
                Gizmos.DrawCube(lhTarget_start.position, new Vector3(0.25f, 0.25f, 0.25f));
                Gizmos.color = new Color(1, 0, 0, 0.5F);
                Gizmos.DrawCube(lhTarget_final.position, new Vector3(0.25f, 0.25f, 0.25f));
            } 
            if (debugRightHand == true)
            {
                Gizmos.color = new Color(1, 1, 0, 0.5F);
                Gizmos.DrawCube(rhTarget.position, new Vector3(0.25f, 0.25f, 0.25f));
                Gizmos.color = new Color(0, 1, 0, 0.5F);
                Gizmos.DrawCube(rhTarget_start.position, new Vector3(0.25f, 0.25f, 0.25f));
                Gizmos.color = new Color(1, 0, 0, 0.5F);
                Gizmos.DrawCube(rhTarget_final.position, new Vector3(0.25f, 0.25f, 0.25f));
            } 
        }
        #endregion
	}
}
