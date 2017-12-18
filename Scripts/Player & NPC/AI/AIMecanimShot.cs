using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pandora.AI;
using Panda.AI;

namespace Pandora.AI {
    public class AIMecanimShot : StateMachineBehaviour {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetComponent<AIMemory>().is_attacking != true)
            {
                animator.GetComponent<AIMemory>().attack_timer = 0;
                animator.GetComponent<AIMemory>().is_attacking = true;
                animator.GetComponent<AITasks>().Shoot(stateInfo.length);
            }
        }
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {   
            animator.GetComponent<AIMemory>().attack_timer += Time.deltaTime;
        }
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<AIMemory>().is_attacking = false;
        }
    }
}