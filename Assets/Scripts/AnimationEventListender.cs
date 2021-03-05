using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventListender : MonoBehaviour
{
    public GameObject handCol_L = null;
    public GameObject handCol_R = null;

    [SerializeField] CombatManager combatManager;

    public void ActivateImpact(int handInt)
    {
        combatManager.ActivateImpact(handInt);

        if (handInt == 1)
            handCol_L.SetActive(true);

        if (handInt == 2)
            handCol_R.SetActive(true);
    }

    public void DeactivateImpact()
    {
        combatManager.impactActivated = false;

        handCol_L.SetActive(false);
        handCol_R.SetActive(false);
    }

    public void CheckRangedAttack()
    {
        combatManager.CheckRangedAttack();
    }
}
