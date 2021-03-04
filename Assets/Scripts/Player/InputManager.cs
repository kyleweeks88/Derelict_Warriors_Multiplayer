using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{
    [Header("Component Ref")]
    [SerializeField] PlayerManager playerMgmt = null;
    [SerializeField] CombatManager combatMgmt = null;

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

        Controls.Player.Attack.performed += ctx => RecieveAttackInput();
    }

    public void RecieveAttackInput()
    {
        // If player is locked into an "interacting" state then don't let this happen.
        if (playerMgmt.isInteracting) { return; }

        if (!canRecieveAttackInput) { return; }

        if (canRecieveAttackInput)
        {
            // Tells CombatManager to determine the means of the attack
            combatMgmt.CheckAttack();
        }
    }
}
