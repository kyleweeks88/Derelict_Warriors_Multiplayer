using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VitalsManager : NetworkBehaviour, IDamageable<FloatVariable, float>
{
    public FloatVariable health;
    public FloatVariable stamina;
    protected float gainInterval = 0f;
    protected float drainInterval = 0f;

    [SyncVar] protected float syncHealth;
    [SyncVar] protected float syncStamina;

    #region Initialize Vital
    public virtual void InitializeVitals()
    {
        health.InitializeValue();
        stamina.InitializeValue();

        if (isServer)
            RpcInitializeVitals();
    }

    public float GetCurrentVital(FloatVariable vital)
    {
        return vital.RuntimeValue;
    }

    [ClientRpc]
    public virtual void RpcInitializeVitals()
    {
        health.InitializeValue();
        stamina.InitializeValue();
    }
    #endregion

    #region Drain Vital
    public virtual void TakeDamage(FloatVariable vital, float dmgVal)
    {
        vital.ModfiyValue(-dmgVal);
        vital.drainingVital = true;

        // CHECK IF vital IS HEALTH OR STAMINA
        // CHECK TO SEE IF VITAL CAN BE REGENED

        StartCoroutine(VitalGainDelay(vital, 1f, 10f, vital.RuntimeValue));
    }

    public void VitalDrainOverTime(FloatVariable vital, float drainAmount, float drainDelay)
    {
        if (ShouldAffectVital(drainInterval))
        {
            vital.drainingVital = true;
            TakeDamage(stamina, drainAmount);
            drainInterval = Time.time + drainDelay / 1000f;
            StartCoroutine(VitalGainDelay(vital, 1f, 10f, vital.RuntimeValue));
        }
    }
    #endregion

    #region Gain Vital
    public void GainVital(FloatVariable vital, float gainAmount)
    {
        vital.ModfiyValue(gainAmount);
    }

    public void VitalGainOverTime(FloatVariable vital, float gainAmount, float gainDelay)
    {
        if (ShouldAffectVital(gainInterval))
        {
            GainVital(vital, gainAmount);
            gainInterval = Time.time + gainDelay / 1000f;
        }
    }

    IEnumerator VitalGainDelay(FloatVariable vital, float gainAmount, float gainDelay, float oldValue)
    {
        yield return new WaitForSeconds(2f);

        if (oldValue == vital.RuntimeValue)
        {
            vital.drainingVital = false;

            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            while (vital.RuntimeValue < vital.maxValue && !vital.drainingVital)
            {
                VitalGainOverTime(vital, gainAmount, gainDelay);
                yield return wait;
            }
        }
        else
        {
            yield return null;
        }
    }
    #endregion

    protected bool ShouldAffectVital(float interval)
    {
        bool result = (Time.time >= interval);

        return result;
    }

    public virtual void SetVital(FloatVariable vital, float setVal)
    {
        vital.SetValue(setVal);
    }
}
