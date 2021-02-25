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

    [SyncVar] public float synchronizedHealth = 0;

    public float Health { get; set; }
    public float HealthMax { get; set; }

    void Awake()
    {
        HealthMax = healthMax;
    }

    public override void OnStartServer() => SetHealth(HealthMax);

    public override void OnStartClient()
    {
        if (hasAuthority)  
            CmdSetHealth(HealthMax);

        // FIGURE OUT SOME WAY TO DISPLAY THE HEALTH OF EACH SERVER OWNED 
        // OBJECT ALREADY ON THE SERVER W/ CharacterStats TO IT'S CURRENT HEALTH
        // VALUE WHEN A NEW CLIENT JOINS THE SERVER.
    }

    [Command]
    void CmdSetHealth(float value) => SetHealth(value);

    [Server]
    void SetHealth(float value)
    {
        synchronizedHealth = value;
        Health = synchronizedHealth;

        this.Event_HealthChanged?.Invoke(Health, HealthMax);
        RpcOnHealthChanged(Health, HealthMax);
    }

    [ClientRpc]
    private void RpcOnHealthChanged(float currentHealth, float maxHealth)
    {
        this.Event_HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void TakeDamage(float attackValue)
    {
        attackValue *= -1;
        ModifyHealth(attackValue); 
    }

    [Server]
    public virtual void ModifyHealth(float amount)
    {
        synchronizedHealth += amount;
        Health = synchronizedHealth;

        this.Event_HealthChanged?.Invoke(Health, HealthMax);
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

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(-10);
        }
    }
}
