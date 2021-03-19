using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerStaminaManager : StaminaManager
{
    public override void OnStartAuthority()
    {
        enabled = true;
    }

    public override void SetVital(float setVal)
    {
        base.SetVital(setVal);

        CmdSetVital(setVal);
    }

    [Command]
    void CmdSetVital(float setVal)
    {
        InitializeVital();
        synchronizedVital = setVal;

        RpcOnStaminaChanged(currentVital, maxVital);
    }

    [ClientRpc]
    public override void RpcOnStaminaChanged(float curVal, float maxVal)
    {
        if (base.hasAuthority) { return; }

        base.RpcOnStaminaChanged(curVal, maxVal);
    }

    [Client]
    public override void ModfiyVital(float modVal)
    {
        base.ModfiyVital(modVal);
        CmdModifyVital(modVal);
    }

    [Command]
    void CmdModifyVital(float modVal)
    {
        currentVital = Mathf.Clamp((synchronizedVital + modVal), 0, maxVital);
        synchronizedVital = currentVital;
        RpcOnStaminaChanged(currentVital, maxVital);
    }

    #region Drain
    public override void TakeDamage(float dmgVal)
    {
        // Turns takingDamage bool to true
        base.TakeDamage(dmgVal);

        ModfiyVital(-dmgVal);
        StartCoroutine(StaminaGainDelay(currentVital));
    }

    public void StaminaDrain()
    {
        if (ShouldDrainStamina())
        {
            drainingStamina = true;
            //TakeDamage(staminaDrainAmount);
            vitalDrainInterval = Time.time + 0.1f;
            StartCoroutine(StaminaGainDelay(currentVital));
        }
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
            GainStamina(vitalGainAmount);
            vitalGainInterval = Time.time + 100f / 1000f;
        }
    }

    IEnumerator StaminaGainDelay(float oldValue)
    {
        yield return new WaitForSeconds(2f);

        if (oldValue == currentVital)
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
