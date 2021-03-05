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

        //combatManager.currentCombatTimer = combatManager.combatTimer;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
}
