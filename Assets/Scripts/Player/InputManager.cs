using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{
    public bool canRecieveAttackInput;
    public bool attackInputRecieved;

    Controls controls;
    public Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    [ClientCallback]
    void OnEnable() => Controls.Enable();
    [ClientCallback]
    void OnDisable() => Controls.Disable();

    public override void OnStartAuthority()
    {
        enabled = true;
        canRecieveAttackInput = true;
    }

    public void RecieveAttackInput()
    {
        if (!canRecieveAttackInput) { return; }

        if (canRecieveAttackInput)
        {
            attackInputRecieved = true;
            canRecieveAttackInput = false;
        }
    }

    public void InvertAttackBool()
    {
        canRecieveAttackInput = !canRecieveAttackInput;

        //if(!canRecieveInput)
        //{
        //    canRecieveInput = true;
        //}
        //else
        //{
        //    canRecieveInput = false;
        //}
    }
}
