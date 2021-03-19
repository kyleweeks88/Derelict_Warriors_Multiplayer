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

    public override void OnStartServer()
    {
        // 
        if (!base.hasAuthority)
        {
            worldUI.SetActive(true);
            SetVital(GetCurrentVital());
        }
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
        // Just runs generic debug damage code for now...
        base.TakeDamage(dmgVal);
        ModfiyVital(-dmgVal);
    }

    #region Initialize Vital
    [Client]
    public override void InitializeVital()
    {
        if (myUI != null)
            myUI.SetActive(true);

        //if (worldUI != null)
        //    worldUI.SetActive(false);

        // Sets currentVital to maxVital and Invokes Event_OnHealthChanged
        base.InitializeVital();

        CmdInitializeVital();
    }

    [Command]
    void CmdInitializeVital()
    {
        // Sets the vitals for the server
        currentVital = maxVital;
        synchronizedVital = currentVital;

        RpcOnHealthChanged(currentVital, maxVital);
    }
    #endregion

    #region Set Vital
    [Client]
    public override void SetVital(float setVal)
    {
        // Sets currentVital to setVal and Invokes Event_OnHealthChanged
        base.SetVital(setVal);

        CmdSetVital(setVal);
    }

    [Command]
    void CmdSetVital(float setVal)
    {
        //InitializeVital();
        synchronizedVital = setVal;

        RpcOnHealthChanged(currentVital, maxVital);
    }
    #endregion

    #region Modify Vital
    [Client]
    public override void ModfiyVital(float modVal)
    {
        // Calculates currentVal+modVal and clamps between 0 and maxVital
        // also invokes Event_HealthChanged
        base.ModfiyVital(modVal);

        // For the server: Calculates currentVal+modVal and clamps between 0 and maxVital
        // also sets the synchronizedVital to the calculated value
        CmdModifyVital(modVal);

        if (currentVital <= 0)
            Die();
    }

    [Command]
    void CmdModifyVital(float modVal)
    {
        //currentVital = Mathf.Clamp((currentVital + modVal), 0, maxVital);
        synchronizedVital = currentVital;
        RpcOnHealthChanged(currentVital, maxVital);
    }
    #endregion

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
            SetVital(maxVital);
        }
    }
}
