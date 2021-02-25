using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHaveHealth
{
    float Health { get; set; }
    float HealthMax { get; set; }
    void ModifyHealth(float amount);
    void TakeDamage(float attackValue);
    void Death();
}
