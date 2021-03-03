using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StaminaManager : VitalStat, IDamageable<float>
{
    public float staminaGainAmount;
    public float staminaDrainAmount;
    float staminaDrainInterval = 0f;
    float staminaGainInterval = 0f;
    public bool drainingStamina;

    public delegate void OnStaminaChanged(float curVal, float maxVal);
    public event OnStaminaChanged Event_StaminaChanged;

    [Client]
    public override void SetVital(float setVal)
    {
        InitializeVital();
        this.Event_StaminaChanged?.Invoke(currentVital, maxVital);
        CmdSetVital(setVal);
    }

    [Command]
    public override void CmdSetVital(float setVal)
    {
        InitializeVital();
        base.CmdSetVital(setVal);
        
        RpcOnStaminaChanged(currentVital, maxVital);
    }

    [Client]
    public override void ModfiyVital(float modVal)
    {
        base.ModfiyVital(modVal);
        this.Event_StaminaChanged?.Invoke(currentVital, maxVital);

        CmdModifyVital(modVal);
    }

    [Command]
    public override void CmdModifyVital(float modVal)
    {
        base.CmdModifyVital(modVal);
        RpcOnStaminaChanged(currentVital, maxVital);
    }

    [ClientRpc]
    private void RpcOnStaminaChanged(float curVal, float maxVal)
    {
        if(base.hasAuthority){return;}

        this.Event_StaminaChanged?.Invoke(curVal, maxVal);
    }

    #region Drain
    public void TakeDamage(float dmgVal)
    {
        dmgVal *= -1f;
        ModfiyVital(dmgVal);
        drainingStamina = true;
        StartCoroutine(StaminaGainDelay(currentVital));
    }

    public void StaminaDrain()
    {
        if (ShouldDrainStamina())
        {
            drainingStamina = true;
            TakeDamage(staminaDrainAmount);
            staminaDrainInterval = Time.time + 0.1f;
            StartCoroutine(StaminaGainDelay(currentVital));
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
        if (currentVital + staminaGain <= maxVital)
        {
            ModfiyVital(staminaGain);
            return;
        }
    }

    public void StaminaGain()
    {
        if (ShouldGainStamina())
        {
            GainStamina(staminaGainAmount);
            staminaGainInterval = Time.time + 100f / 1000f;
        }
    }

    bool ShouldGainStamina()
    {
        bool result = (Time.time >= staminaGainInterval);

        return result;
    }

    IEnumerator StaminaGainDelay(float oldValue)
    {
        yield return new WaitForSeconds(1f);

        if(oldValue == currentVital)
        {
            drainingStamina = false;

            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            while (currentVital < maxVital && !drainingStamina)
            {
                StaminaGain();
                yield return wait;
            }
        }
        else
        {
            yield return null;
        }
    }
    #endregion
}
