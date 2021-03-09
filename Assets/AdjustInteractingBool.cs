using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustInteractingBool : StateMachineBehaviour
{
    PlayerManager playerMgmt;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerMgmt = animator.transform.GetComponentInParent<PlayerManager>();

        if (playerMgmt != null)
        {
            playerMgmt.isInteracting = false;
            animator.SetBool("isInteracting", false);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerMgmt != null)
        {
            playerMgmt.isInteracting = true;
            animator.SetBool("isInteracting", true);
        }
    }
}
