using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHealthManager : VitalStat, IDamageable<float>
{
    public override void OnStartServer()
    {
        base.OnStartServer();

    }

    public void TakeDamage(float dmgVal)
    {

    }
}
