using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustAnimatorBool : StateMachineBehaviour
{
    InputManager inputMgmt = null;
    public string animatorBool;
    public bool boolStatus;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        inputMgmt = animator.transform.GetComponentInParent<InputManager>();

        inputMgmt.canRecieveAttackInput = boolStatus;
    }
}
