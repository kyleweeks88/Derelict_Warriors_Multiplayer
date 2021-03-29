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

    [Header("Locomotion stats")]
    public Stat moveSpeed;
    [Tooltip("")]
    public float jumpVelocity = 5f;

    public StatModifier sprintMovementModifier = new StatModifier(1f, StatModType.PercentAdd);
    public StatModifier aerialMovementModifier = new StatModifier(-0.5f, StatModType.PercentMulti);
    public StatModifier combatMovementModifier = new StatModifier(-0.5f, StatModType.PercentMulti);

    [Header("Combat settings")]
    public Stat attackDamage;

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

    void Update()
    {
        Debug.Log(attackDamage.value);
    }

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
}
