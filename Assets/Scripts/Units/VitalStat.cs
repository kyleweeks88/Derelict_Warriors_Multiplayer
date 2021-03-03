using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class VitalStat : NetworkBehaviour
{
    protected float currentVital;
    public float maxVital;
    [SyncVar] float synchronizedVital = 0f;

    public void InitializeVital()
    {
        currentVital = maxVital;
    }

    [Client]
    public virtual void SetVital(float setVal)
    {
        //Generic code here
    }

    [Command]
    public virtual void CmdSetVital(float setVal)
    {
        synchronizedVital = setVal;
    }

    [Client]
    public virtual void ModfiyVital(float modVal)
    {
        currentVital = Mathf.Clamp((currentVital + modVal), 0 , maxVital);
    }

    [Command]
    public virtual void CmdModifyVital(float modVal)
    {
        currentVital = Mathf.Clamp((synchronizedVital + modVal), 0 , maxVital);
        synchronizedVital = currentVital;
    }
}
