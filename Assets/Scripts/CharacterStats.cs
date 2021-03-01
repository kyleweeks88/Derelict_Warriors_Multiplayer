using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CharacterStats : NetworkBehaviour
{
    public Stat healthStat;
    public Stat staminaStat;

    [Header("Settings")]
    public string charName;
    public float baseAttackDamage;

    [Header("Health Settings")]
    float synchronizedHealth = 0f;
    public float healthMax;

    [Header("Stamina Settings")]
    float synchronizedStamina = 0f;
    public float maxStamina = 100f;
    public float staminaDrainInterval = 0f;
    public float staminaDrainAmount = 0f;

    public delegate void OnStatChanged(string key, float currentValue, float maxValue);
    public event OnStatChanged Event_StatChanged;


    public override void OnStartServer()
    {
        //HealthMax = healthMax;
        //SetHealth(healthStat.GetMaxValue());

        healthStat.InitializeStat(this, healthMax);
        staminaStat.InitializeStat(this, maxStamina);

        SetStat(healthStat.statName, healthStat.GetCurrentValue(), healthStat.GetMaxValue());
        SetStat(staminaStat.statName, staminaStat.GetMaxValue(), staminaStat.GetMaxValue());
    }

    public override void OnStartClient()
    {
        if (!base.hasAuthority) { return; }

        healthStat.InitializeStat(this, healthMax);
        staminaStat.InitializeStat(this, maxStamina);

        CmdSetStat(healthStat.statName, healthStat.GetCurrentValue(), healthStat.GetMaxValue());
        CmdSetStat(staminaStat.statName, staminaStat.GetMaxValue(), staminaStat.GetMaxValue());


        // figure out some way to display the health of each server owned 
        // object already on the server w/ characterstats to it's current health
        // value when a new client joins the server.
    }

    // Currently using this for testing only
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(10);
        }

        ShouldGainStamina();
    }

    [Command]
    void CmdSetStat(string key, float currentValue, float maxValue)
    {
        SetStat(key, currentValue, maxValue);
    }

    [Server]
    void SetStat(string key, float currentValue, float maxValue)
    {
        if (key == "Health")
            synchronizedHealth = currentValue;

        if(key == "Stamina")
            synchronizedStamina = currentValue;

        this.Event_StatChanged?.Invoke(key, currentValue, maxValue);
        RpcOnStatChanged(key, currentValue, maxValue);
    }

    [ClientRpc]
    private void RpcOnStatChanged(string key, float currentValue, float maxValue)
    {
        this.Event_StatChanged?.Invoke(key, currentValue, maxValue);
    }

    public virtual void TakeDamage(float attackValue)
    {
        attackValue *= -1;

        ModifyStat(healthStat.statName, attackValue);
    }

    [Command]
    public virtual void ModifyStat(string key, float value)
    {
        if(key == "Health")
        {
            synchronizedHealth += value;
            float modValue = healthStat.ModifyStat(value);
            this.Event_StatChanged(key, healthStat.GetCurrentValue(), healthStat.GetMaxValue());
            RpcOnStatChanged(key, healthStat.GetCurrentValue(), healthStat.GetMaxValue());
        }

        if(key == "Stamina")
        {
            synchronizedStamina += value;
            float modValue = staminaStat.ModifyStat(value);
            this.Event_StatChanged(key, staminaStat.GetCurrentValue(), staminaStat.GetMaxValue());
            RpcOnStatChanged(key, staminaStat.GetCurrentValue(), staminaStat.GetMaxValue());
        }
    }

    public virtual void Death()
    {
        Debug.Log(charName + " has died!");
    }

    #region Stamina Functions

    #region Drain
    void UseStamina(float staminaDrain)
    {
        if (staminaStat.GetCurrentValue() - staminaDrain >= 0)
        {
            ModifyStat(staminaStat.statName, staminaDrain * -1f);
            Debug.Log("TEST");
        }
    }

    public void StaminaDrain()
    {
        if (ShouldDrainStamina())
        {
            UseStamina(staminaDrainAmount);
            staminaDrainInterval = Time.time + 0.1f;
        }
    }

    bool ShouldDrainStamina()
    {
        bool result = (Time.time >= staminaDrainInterval);

        return result;
    }
    #endregion

    #region Gain
    void GainStamina(float staminaGain)
    {
        if (staminaStat.GetCurrentValue() + staminaGain <= maxStamina)
        {
            ModifyStat(staminaStat.statName, staminaGain);
        }
    }

    bool ShouldGainStamina()
    {
        float currentStam = staminaStat.GetCurrentValue();
        if (currentStam < staminaStat.GetMaxValue())
        {
            StartCoroutine(StaminaGainDelay(currentStam));
        }
        return false;
    }

    IEnumerator StaminaGainDelay(float oldValue)
    {
        yield return new WaitForSeconds(2f);

        if(oldValue != staminaStat.GetCurrentValue())
        {
            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            while (staminaStat.GetCurrentValue() < staminaStat.GetMaxValue())
            {
                GainStamina(1f);
                yield return wait;
            }
        }
    }
    #endregion

    #endregion

}
