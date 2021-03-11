using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerHealthManager : HealthManager
{
    [SerializeField] GameObject myUI;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    // Currently using this for testing only
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(10f);
        }
    }

    public override void TakeDamage(float dmgVal)
    {
        base.TakeDamage(dmgVal);
        ModfiyVital(dmgVal);
    }

    [Client]
    public override void SetVital(float setVal)
    {
        if (myUI != null)
            myUI.SetActive(true);

        base.SetVital(setVal);
        CmdSetVital(setVal);
    }

    [Command]
    void CmdSetVital(float setVal)
    {
        InitializeVital();
        synchronizedVital = setVal;

        RpcOnHealthChanged(currentVital, maxVital);
    }

    public override void ModfiyVital(float modVal)
    {
        base.ModfiyVital(modVal);

        CmdModifyVital(modVal);

        if (currentVital <= 0)
            Die();
    }

    [Command]
    void CmdModifyVital(float modVal)
    {
        currentVital = Mathf.Clamp((synchronizedVital + modVal), 0, maxVital);
        synchronizedVital = currentVital;
        RpcOnHealthChanged(currentVital, maxVital);
    }

    [ClientRpc]
    public override void RpcOnHealthChanged(float curVal, float maxVal)
    {
        if (base.hasAuthority) { return; }

        base.RpcOnHealthChanged(curVal,maxVal);
    }

    public override void Die()
    {
        PlayerStats playerStats = GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.Death();
        }
    }
}
