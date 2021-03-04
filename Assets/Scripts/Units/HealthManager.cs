using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class HealthManager : VitalStat, IDamageable<float>
{
    [SerializeField] GameObject myUI;
    [SerializeField] GameObject worldUI;
    public delegate void OnHealthChanged(float curVal, float maxVal);
    public event OnHealthChanged Event_HealthChanged;

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

    [Client]
    public override void SetVital(float setVal)
    {
        if(myUI != null)
            myUI.SetActive(true);

        InitializeVital();
        this.Event_HealthChanged?.Invoke(currentVital, maxVital);
        CmdSetVital(setVal);
    }

    [Command]
    public override void CmdSetVital(float setVal)
    {
        InitializeVital();
        base.CmdSetVital(setVal);
        
        RpcOnHealthChanged(currentVital, maxVital);
    }

    [ClientRpc]
    private void RpcOnHealthChanged(float curVal, float maxVal)
    {
        if(base.hasAuthority){return;}

        this.Event_HealthChanged?.Invoke(curVal, maxVal);
    }

    public void TakeDamage(float dmgVal)
    {
        dmgVal *= -1f;
        ModfiyVital(dmgVal);
    }

    [Client]
    public override void ModfiyVital(float modVal)
    {
        base.ModfiyVital(modVal);
        this.Event_HealthChanged?.Invoke(currentVital, maxVital);

        CmdModifyVital(modVal);

        if(currentVital <= 0)
            Die();
    }

    [Command]
    public override void CmdModifyVital(float modVal)
    {
        base.CmdModifyVital(modVal);
        RpcOnHealthChanged(currentVital, maxVital);
    }

    public void Die()
    {
        CharacterStats characterStats = GetComponent<CharacterStats>();

        if(characterStats != null)
        {
            characterStats.Death();
        }
    }
}
