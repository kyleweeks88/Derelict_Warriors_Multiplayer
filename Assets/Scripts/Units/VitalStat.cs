using Mirror;
using UnityEngine;

public class VitalStat : NetworkBehaviour
{
    public float vitalGainAmount;
    public float vitalDrainAmount;
    protected float vitalDrainInterval = 0f;
    protected float vitalGainInterval = 0f;

    protected float currentVital;
    public float maxVital;
    [SyncVar] protected float synchronizedVital = 0f;

    public virtual void InitializeVital()
    {
        currentVital = maxVital;
    }

    public float GetCurrentVital()
    {
        return currentVital;
    }

    public virtual void SetVital(float setVal)
    {
        currentVital = setVal;
    }

    public virtual void ModfiyVital(float modVal)
    {
        currentVital = Mathf.Clamp((currentVital + modVal), 0 , maxVital);
    }
}
