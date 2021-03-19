using System.Collections;
using UnityEngine;

public interface IDamageable<FloatVariable, T>
{
    void TakeDamage(FloatVariable floatVar, T damageTaken);
}
