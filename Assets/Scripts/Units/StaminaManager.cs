using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StaminaManager : VitalStat, IDamageable<float>
{
    public float staminaGainAmount;
    public float staminaDrainAmount;
    protected float staminaDrainInterval = 0f;
    protected float staminaGainInterval = 0f;
    public bool drainingStamina;

    public delegate void OnStaminaChanged(float curVal, float maxVal);
    public event OnStaminaChanged Event_StaminaChanged;

    public override void SetVital(float setVal)
    {
        InitializeVital();
        this.Event_StaminaChanged?.Invoke(currentVital, maxVital);
    }

    [Client]
    public override void ModfiyVital(float modVal)
    {
        base.ModfiyVital(modVal);
        this.Event_StaminaChanged?.Invoke(currentVital, maxVital);
    }

    [ClientRpc]
    public virtual void RpcOnStaminaChanged(float curVal, float maxVal)
    {
        this.Event_StaminaChanged?.Invoke(curVal, maxVal);
    }

    #region Drain
    public virtual void TakeDamage(float dmgVal)
    {
        drainingStamina = true;
    }

    protected bool ShouldDrainStamina()
    {
        bool result = (Time.time >= staminaDrainInterval);

        return result;
    }
    #endregion

    #region Gain

    protected bool ShouldGainStamina()
    {
        bool result = (Time.time >= staminaGainInterval);

        return result;
    }

    #endregion
}
