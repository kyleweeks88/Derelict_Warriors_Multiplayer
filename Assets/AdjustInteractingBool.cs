using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustInteractingBool : StateMachineBehaviour
{
    PlayerManager playerMgmt;
    public bool boolStatus;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerMgmt = animator.transform.GetComponentInParent<PlayerManager>();

        if (playerMgmt != null)
        {
            playerMgmt.isInteracting = boolStatus;
            animator.SetBool("isInteracting", boolStatus);
        }
    }
}
