using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventListender : MonoBehaviour
{
    [SerializeField] CombatManager combatManager;

    public void ActivateImpact(int handInt)
    {
        combatManager.ActivateImpact(handInt);
    }

    public void DeactivateImpact()
    {
        combatManager.impactActivated = false;
    }

    public void CheckRangedAttack()
    {
        combatManager.CheckRangedAttack();
    }
}
