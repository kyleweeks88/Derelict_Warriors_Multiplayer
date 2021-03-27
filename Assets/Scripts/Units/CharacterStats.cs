using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// I think this script will handle character leveling, dying, all the attributes/stats
// and the way they modify or mitigate incoming/outgoing damage.
public class CharacterStats : NetworkBehaviour, IKillable
{
    [Header("General settings")]
    public string charName;

    [Header("Locomotion settings")]
    [Range(0,3)] public float moveSpeed = 1f;
    [Tooltip("Affects movement speed while attacking. Higher number moves slower!")]
    [Range(1,3)] public float attackingMoveSpeedModifier = 1.5f;
    [Tooltip("Higher number makes sprinting speed faster")]
    [Range(1,4)] public float sprintMultiplier = 1.5f;
    public float jumpVelocity = 5f;

    [Header("Combat settings")]
    public float baseAttackDamage = 1f;

    [Header("Stamina vital")]
    public float staminaGainAmount;
    public float staminaGainDelay;
    public float staminaDrainAmount;
    public float staminaDrainDelay;
    [Header("Health vital")]
    public float healthGainAmount;
    public float healthGainDelay;
    public float healthDrainAmount;
    public float healthDrainDelay;

    public void Invulnerability()
    {
        StartCoroutine(Invulnerable(0.25f));
    }

    #region Death!!!
    [Client]
    public virtual void Death()
    {
        Debug.Log(charName + " has died!");
    }
    #endregion

    IEnumerator Invulnerable(float timer)
    {
        string originalTag = gameObject.tag;

        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while(timer > 0)
        {
            this.gameObject.tag = "Invulnerable";
            timer -= Time.deltaTime;
            yield return wait;
        }

        gameObject.tag = originalTag;
    }

    public float AdjustMoveSpeed(float modifier)
    {
        return moveSpeed / modifier;
    }
}
