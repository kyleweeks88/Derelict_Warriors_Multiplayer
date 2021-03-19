using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class HealthManager : VitalStat, IDamageable<float>
{
    [SerializeField] protected GameObject playerUI;
    [SerializeField] protected GameObject worldspaceUI;

    public delegate void OnHealthChanged(float curVal, float maxVal);
    public event OnHealthChanged Event_HealthChanged;

    public override void InitializeVital()
    {
        base.InitializeVital();
        this.Event_HealthChanged?.Invoke(currentVital, maxVital);
    }

    public override void SetVital(float setVal)
    {
        base.SetVital(setVal);
        this.Event_HealthChanged?.Invoke(currentVital, maxVital);
    }

    public override void ModfiyVital(float modVal)
    {
        base.ModfiyVital(modVal);
        this.Event_HealthChanged?.Invoke(currentVital, maxVital);
    }

    public virtual void TakeDamage(float dmgVal)
    {
        Debug.Log(gameObject.name + " took: " + dmgVal + "!");
    }

    public virtual void Die()
    {
        // GENERIC DIE CODE HERE
    }

    [ClientRpc]
    public virtual void RpcOnHealthChanged(float curVal, float maxVal)
    {
        this.Event_HealthChanged?.Invoke(curVal, maxVal);
        Debug.Log("Manager: curVal: " + curVal + ", maxVal: " + maxVal);
    }
}
