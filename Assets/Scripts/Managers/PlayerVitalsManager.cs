using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerVitalsManager : VitalsManager
{
    [SerializeField] GameObject myUI;

    public override void OnStartAuthority()
    {
        health.Event_ValueChanged += Die;
    }

    // Currently using this for testing only
    [ClientCallback]
    private void Update()
    {
        if (!base.hasAuthority) { return; }

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(health, 10f);
        }
    }

    #region Initialize Vitals
    public override void InitializeVitals()
    {
        if (myUI != null)
            myUI.SetActive(true);

        base.InitializeVitals();

        CmdInitializeVitals();
    }

    [Command]
    void CmdInitializeVitals()
    {
        syncHealth = health.maxValue;
        syncStamina = stamina.maxValue;
        RpcInitializeVitals();
    }

    [ClientRpc]
    public override void RpcInitializeVitals()
    {
        if (base.hasAuthority) { return; }

        base.RpcInitializeVitals();
    }
    #endregion

    #region Drain Vital
    [Client]
    public override void TakeDamage(FloatVariable vital, float dmgVal)
    {
        base.TakeDamage(vital, dmgVal);
        CmdTakeDamage(vital.varName, dmgVal);
    }

    [Command]
    void CmdTakeDamage(string vitalName, float dmgVal)
    {
        if (base.isClient) { return; }

        if (vitalName == "Player Health")
        {
            health.ModfiyValue(-dmgVal);
            syncHealth = health.GetCurrentValue();
        }

        if (vitalName == "Player Stamina")
        {
            stamina.ModfiyValue(-dmgVal);
            syncStamina = stamina.GetCurrentValue();
        }

        RpcTakeDamage(vitalName, dmgVal);
    }

    [ClientRpc]
    void RpcTakeDamage(string vitalName, float dmgVal)
    {
        if (base.hasAuthority) { return; }

        if (vitalName == "Player Health")
            health.ModfiyValue(-dmgVal);

        if (vitalName == "Player Stamina")
            stamina.ModfiyValue(-dmgVal);
    }
    #endregion

    #region Set Vital
    public override void SetVital(FloatVariable vital, float setVal)
    {
        base.SetVital(vital, setVal);
        CmdSetVital(vital.varName, setVal);
    }

    [Command]
    void CmdSetVital(string vitalName, float setVal)
    {
        if (vitalName == "Player Health")
            syncHealth = setVal;

        if (vitalName == "Player Stamina")
            syncStamina = setVal;

        RpcSetVital(vitalName, setVal);
    }

    [ClientRpc]
    void RpcSetVital(string vitalName, float setVal)
    {
        if (base.hasAuthority) { return; }

        if (vitalName == "Player Health")
            health.SetValue(setVal);

        if (vitalName == "Player Stamina")
            stamina.SetValue(setVal);
    }
    #endregion

    void Die(float curVal, float maxVal)
    {
        if (curVal <= 0)
        {
            PlayerStats playerStats = GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.Death();
                SetVital(health, health.maxValue);
            }
        }
    }
}
