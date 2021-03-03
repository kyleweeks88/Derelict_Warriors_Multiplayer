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

    [Command]
    public virtual void CmdSetVital(float setVal)
    {
        //SetVital(setVal);
        synchronizedVital = setVal;
    }

    //[Server]
    public virtual void SetVital(float setVal)
    {
        //synchronizedVital = setVal;
    }

    [Client]
    public virtual void ModfiyVital(float modVal)
    {
        currentVital = Mathf.Clamp((currentVital + modVal), 0 , maxVital);
    }

    [Command]
    public virtual void CmdModifyVital(float modVal)
    {
        currentVital = Mathf.Clamp((currentVital + modVal), 0 , maxVital);
        synchronizedVital = currentVital;
    }
}
