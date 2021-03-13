using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VitalStat : NetworkBehaviour
{
    protected float currentVital;
    public float maxVital;
    [SyncVar] protected float synchronizedVital = 0f;

    public void InitializeVital()
    {
        currentVital = maxVital;
    }

    public float GetCurrentVital()
    {
        return currentVital;
    }

    public virtual void SetVital(float setVal)
    {
        //Generic code here
    }

    public virtual void ModfiyVital(float modVal)
    {
        currentVital = Mathf.Clamp((currentVital + modVal), 0 , maxVital);
    }
}
