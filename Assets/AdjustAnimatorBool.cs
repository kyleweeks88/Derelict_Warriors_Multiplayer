using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustAnimatorBool : StateMachineBehaviour
{
    CombatManager combatMgmgt = null;
    public bool boolStatus;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        combatMgmgt = animator.transform.GetComponentInParent<CombatManager>();

        if(combatMgmgt != null)
            combatMgmgt.canRecieveAttackInput = boolStatus;
    }
}
