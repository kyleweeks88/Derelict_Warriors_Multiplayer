using UnityEngine;
using Mirror;

public class NpcHealthManager : HealthManager
{
    private void Start()
    {
        InitializeVital();
        SetVital(maxVital);
    }

    public override void TakeDamage(float dmgVal)
    {
        base.TakeDamage(dmgVal);
        ModfiyVital(-dmgVal);
    }

    [Server]
    public override void SetVital(float setVal)
    {
        base.SetVital(setVal);

        synchronizedVital = setVal;
        RpcOnHealthChanged(currentVital, maxVital);
    }

    [Server]
    public override void ModfiyVital(float modVal)
    {
        base.ModfiyVital(modVal);

        synchronizedVital = currentVital;
        RpcOnHealthChanged(currentVital, maxVital);

        if (currentVital <= 0)
            Die();
    }

    public override void Die()
    {
        NpcStats npcStats = GetComponent<NpcStats>();

        if (npcStats != null)
        {
            npcStats.Death();
        }
    }

    [ClientRpc]
    public override void RpcOnHealthChanged(float curVal, float maxVal)
    {
        base.RpcOnHealthChanged(curVal, maxVal);
    }
}
