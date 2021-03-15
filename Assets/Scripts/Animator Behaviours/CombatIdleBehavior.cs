using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CombatIdleBehavior : StateMachineBehaviour
{
    CombatManager combatManager = null;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        combatManager = animator.transform.GetComponentInParent<CombatManager>();
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // TIMER FOR COMBAT IDLE ANIMATION
        if (combatManager.inCombat)
        {
            combatManager.HandleCombatTimer();
        }
        else
        {
            return;
        }
    }
}
