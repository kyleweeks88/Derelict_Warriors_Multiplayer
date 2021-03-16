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
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;
    public float jumpVelocity = 5f;

    [Header("Combat settings")]
    public float baseAttackDamage = 1f;

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
