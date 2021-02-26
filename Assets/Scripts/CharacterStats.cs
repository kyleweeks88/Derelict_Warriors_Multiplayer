using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CharacterStats : NetworkBehaviour, IHaveHealth
{
    [Header("Settings")]
    public string charName;
    public float healthMax;
    public float baseAttackDamage;

    public delegate void OnHealthChanged(float currentHealth, float maxHealth);
    public event OnHealthChanged Event_HealthChanged;

    public delegate void OnStaminaChanged(float currentStam, float maxStam);
    public event OnStaminaChanged Event_StaminaChanged;

    //[SyncVar] public float synchronizedHealth = 0;
    [SyncVar(hook = nameof(OnHealthUpdated))]
    float synchronizedHealth = 0f;

    public float Health { get; set; }
    public float HealthMax { get; set; }


    public override void OnStartServer()
    {
        HealthMax = healthMax;
        SetHealth(HealthMax);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        
        HealthMax = healthMax;
        Health = HealthMax;

        SetHealth(HealthMax);
    }

    //public override void OnStartClient()
    //{
    //    if (!base.hasAuthority) { return; }

    //    SetHealth(HealthMax);

    //    // figure out some way to display the health of each server owned 
    //    // object already on the server w/ characterstats to it's current health
    //    // value when a new client joins the server.
    //}

    // Currently using this for testing only
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(10);
        }
    }

    #region Health Functions
    [Command]
    void CmdSetHealth(float value)
    {
        synchronizedHealth = value;
        RpcOnHealthChanged(Health, HealthMax);
    }
    
    void SetHealth(float value)
    {
        Health = value;
        CmdSetHealth(value);
        
        //this.Event_HealthChanged?.Invoke(Health, HealthMax);
        //RpcOnHealthChanged(Health, HealthMax);
    }

    void OnHealthUpdated(float oldVal, float newVal)
    {
        this.Event_HealthChanged?.Invoke(Health, HealthMax);
    }

    [ClientRpc]
    private void RpcOnHealthChanged(float currentHealth, float maxHealth)
    {
        this.Event_HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void TakeDamage(float attackValue)
    {
        Debug.Log("TEST");
        attackValue *= -1;
        ModifyHealth(attackValue); 
    }

    [Command]
    public virtual void ModifyHealth(float amount)
    {
        synchronizedHealth += amount;
        Health = synchronizedHealth;

        RpcOnHealthChanged(Health, HealthMax);
        if (Health <= 0)
        {
            Death();
        }
    }

    public virtual void Death()
    {
        Debug.Log(charName + " has died!");
    }
    #endregion

    #region Stamina Functions

    public virtual void ModifyStamina(float value)
    {
        //currentStamina += value;

        //this.Event_StaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    #endregion
}
