using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimationManager : NetworkBehaviour
{
    [SerializeField] Animator myAnim;
    [SerializeField] CombatManager combatMgmt;
    [SerializeField] PlayerManager playerMgmt;

    public override void OnStartAuthority()
    {
        enabled = true;
        playerMgmt = GetComponent<PlayerManager>();
        combatMgmt = GetComponent<CombatManager>();
    }

    public void SetAnimation(AnimatorOverrideController overrideCtrl)
    {
        myAnim.runtimeAnimatorController = overrideCtrl;
    }

    [ClientCallback]
    void Update()
    {
        myAnim.SetBool("inCombat", combatMgmt.inCombat);

        if (combatMgmt.attackAnim != null)
            myAnim.SetBool(combatMgmt.attackAnim, playerMgmt.inputMgmt.attackInputHeld);
    }
}
