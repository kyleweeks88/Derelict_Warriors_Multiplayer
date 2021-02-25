using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterStats
{
    public override void Death()
    {
        base.Death();
        Destroy(gameObject);
    }
}
