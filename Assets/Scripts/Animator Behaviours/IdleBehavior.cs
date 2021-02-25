using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class IdleBehavior : StateMachineBehaviour
{
    NetworkAnimator myNetworkAnimator = null;
    CombatManager combatManager = null;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        combatManager = animator.transform.GetComponentInParent<CombatManager>();
        myNetworkAnimator = animator.transform.GetComponentInParent<NetworkAnimator>() != null
            ? animator.transform.GetComponentInParent<NetworkAnimator>()
            : myNetworkAnimator = null;
        combatManager.canRecieveInput = true;
        combatManager.currentCombatTimer = combatManager.combatTimer;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(combatManager.inputRecieved)
        {
            if(myNetworkAnimator != null)
            {
                myNetworkAnimator.SetTrigger("attackOne");
            }
            else
            {
                animator.SetTrigger("attackOne");
            }

            combatManager.InputManager();
            combatManager.inputRecieved = false;
        }

        // TIMER FOR COMBAT IDLE ANIMATION
        if (combatManager.inCombat)
        {
            combatManager.currentCombatTimer -= Time.deltaTime;

            if (combatManager.currentCombatTimer <= 0)
            {
                combatManager.currentCombatTimer = combatManager.combatTimer;
                combatManager.inCombat = false;
            }
        }
        else
        {
            return;
        }
    }

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
