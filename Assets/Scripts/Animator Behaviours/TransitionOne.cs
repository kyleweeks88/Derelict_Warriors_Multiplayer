using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TransitionOne : StateMachineBehaviour
{
    NetworkAnimator myNetworkAnimator = null;
    InputManager inputMgmt = null;
    [SerializeField] string attackName = string.Empty;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        myNetworkAnimator = animator.transform.GetComponentInParent<NetworkAnimator>();
        inputMgmt = animator.transform.GetComponentInParent<InputManager>();

        inputMgmt.canRecieveAttackInput = true;
    }


    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if(inputMgmt.attackInputRecieved)
    //    {
    //        inputMgmt.InvertAttackBool();
    //        inputMgmt.attackInputRecieved = false;

    //        myNetworkAnimator.SetTrigger(attackName);
    //    }
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
