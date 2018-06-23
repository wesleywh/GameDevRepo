using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideLimbPositions : MonoBehaviour {

    public Animator anim = null;
    public Transform leftHand = null;
    public Transform rightHand = null;
    public Transform leftFoot = null;
    public Transform rightFoot = null;
    public string[] disableOnAnimStates = null;
    public bool onDisableLefthand = true;
    public bool onDisableRightHand = true;
    public bool onDisableLeftFoot = true;
    public bool onDisableRightFoot = true;
    private bool isEnabled = true;
    private bool check = true;
    private float tempWeight = 1;

    void Update() {
        check = true;
        foreach (string name in disableOnAnimStates)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag(name))
            {
                isEnabled = false;
                check = false;
            }
            break;
        }
        if (check == true)
        {
            isEnabled = true;
        }
    }

    void OnAnimatorIK() {
        if (anim == null)
            return;
        if (leftHand != null)
        {
            if (isEnabled == false && onDisableLefthand == true)
                tempWeight = 0;
            else
                tempWeight = 1;
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, tempWeight);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, tempWeight);
        }
        if (rightHand != null)
        {
            if (isEnabled == false && onDisableRightHand == true)
                tempWeight = 0;
            else
                tempWeight = 1;
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, tempWeight);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, tempWeight);
        }
        if (leftFoot != null)
        {
            if (isEnabled == false && onDisableLeftFoot == true)
                tempWeight = 0;
            else
                tempWeight = 1;
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFoot.position);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, tempWeight);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFoot.rotation);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, tempWeight);
        }
        if (rightFoot != null)
        {
            if (isEnabled == false && onDisableRightFoot == true)
                tempWeight = 0;
            else
                tempWeight = 1;
            anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot.position);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, tempWeight);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFoot.rotation);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, tempWeight);
        }
    }
}
