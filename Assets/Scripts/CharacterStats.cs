using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CharacterStats : NetworkBehaviour, IKillable
{
    [SerializeField] HealthManager healthMgmt;
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
    float staminaDrainInterval = 0f;
    float staminaGainInterval = 0f;
    public float staminaDrainAmount = 0f;
    public float staminaGainAmount = 0f;
    public bool drainingStamina = false;

    public delegate void OnStatChanged(string key, float currentValue, float maxValue);
    public event OnStatChanged Event_StatChanged;


    // public override void OnStartServer()
    // {
    //     //healthStat.InitializeStat(this, healthMax);
    //     //staminaStat.InitializeStat(this, maxStamina);

    //     //SetStat(healthStat.statName, healthStat.GetCurrentValue(), healthStat.GetMaxValue());
    //     //SetStat(staminaStat.statName, staminaStat.GetMaxValue(), staminaStat.GetMaxValue());
    // }

    // public override void OnStartClient()
    // {
    //     //if (!base.hasAuthority) { return; }

    //     //healthStat.InitializeStat(this, healthMax);
    //     //staminaStat.InitializeStat(this, maxStamina);

    //     //CmdSetStat(healthStat.statName, healthStat.GetCurrentValue(), healthStat.GetMaxValue());
    //     //CmdSetStat(staminaStat.statName, staminaStat.GetMaxValue(), staminaStat.GetMaxValue());


    //     // figure out some way to display the health of each server owned 
    //     // object already on the server w/ characterstats to it's current health
    //     // value when a new client joins the server.
    // }

    // Currently using this for testing only
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            healthMgmt.TakeDamage(baseAttackDamage);
        }
    }

    // [Command]
    // void CmdSetStat(string key, float currentValue, float maxValue)
    // {
    //     SetStat(key, currentValue, maxValue);
    // }

    // [Server]
    // void SetStat(string key, float currentValue, float maxValue)
    // {
    //     if (key == "Health")
    //         synchronizedHealth = currentValue;

    //     if(key == "Stamina")
    //         synchronizedStamina = currentValue;

    //     this.Event_StatChanged?.Invoke(key, currentValue, maxValue);
    //     RpcOnStatChanged(key, currentValue, maxValue);
    // }

    // [ClientRpc]
    // private void RpcOnStatChanged(string key, float currentValue, float maxValue)
    // {
    //     this.Event_StatChanged?.Invoke(key, currentValue, maxValue);
    // }

    // [Command]
    // public virtual void ModifyStat(string key, float value)
    // {
    //     if (key == "Stamina")
    //     {
    //         synchronizedStamina += value;
    //         float modValue = staminaStat.ModifyStat(value);
    //         this.Event_StatChanged(key, staminaStat.GetCurrentValue(), staminaStat.GetMaxValue());
    //         RpcOnStatChanged(key, staminaStat.GetCurrentValue(), staminaStat.GetMaxValue());
    //     }
    // }

    #region Death!!!
    [Client]
    public virtual void Death()
    {
        Debug.Log(charName + " has died!");
        //this.transform.position = Vector3.zero;
        CmdDeath();
    }

    [Command]
    public virtual void CmdDeath()
    {
        Debug.Log(charName + " has died!");
        this.transform.position = Vector3.zero;
        RpcDeath();
    }

    [ClientRpc]
    void RpcDeath()
    {
        //if(base.hasAuthority){return;}

        Debug.Log(charName + " has died!");
        this.transform.position = Vector3.zero;
    }
    #endregion

    #region Stamina Functions

    #region Drain
    // void UseStamina(float staminaDrain)
    // {
    //     if (staminaStat.GetCurrentValue() - staminaDrain >= 0)
    //     {
    //         ModifyStat(staminaStat.statName, staminaDrain * -1f);
    //     }
    // }

    // public void StaminaDrain()
    // {
    //     if (ShouldDrainStamina())
    //     {
    //         drainingStamina = true;
    //         UseStamina(staminaDrainAmount);
    //         staminaDrainInterval = Time.time + 0.1f;
    //         StartCoroutine(StaminaGainDelay(staminaStat.GetCurrentValue()));
    //     }
    // }

    // bool ShouldDrainStamina()
    // {
    //     bool result = (Time.time >= staminaDrainInterval);

    //     return result;
    // }
    #endregion

    #region Gain
    // void GainStamina(float staminaGain)
    // {
    //     if (staminaStat.GetCurrentValue() + staminaGain <= maxStamina)
    //     {
    //         ModifyStat(staminaStat.statName, staminaGain);
    //         return;
    //     }
    // }

    // public void StaminaGain()
    // {
    //     if (ShouldGainStamina())
    //     {
    //         GainStamina(staminaGainAmount);
    //         staminaGainInterval = Time.time + 100f / 1000f;
    //     }
    // }

    // bool ShouldGainStamina()
    // {
    //     bool result = (Time.time >= staminaGainInterval);

    //     return result;
    // }

    // IEnumerator StaminaGainDelay(float oldValue)
    // {
    //     yield return new WaitForSeconds(1f);

    //     if(oldValue == staminaStat.GetCurrentValue())
    //     {
    //         drainingStamina = false;

    //         WaitForEndOfFrame wait = new WaitForEndOfFrame();
    //         while (staminaStat.GetCurrentValue() < staminaStat.GetMaxValue() && !drainingStamina)
    //         {
    //             StaminaGain();
    //             yield return wait;
    //         }
    //     }
    //     else
    //     {
    //         yield return null;
    //     }
    // }
    #endregion

    #endregion

}
