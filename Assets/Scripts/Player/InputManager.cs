using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{
    public delegate void OnInteractPressed();
    public event OnInteractPressed Event_OnInteract;

    [Header("Component Ref")]
    [SerializeField] PlayerManager playerMgmt = null;
    [SerializeField] CombatManager combatMgmt = null;
    [SerializeField] PlayerMovement playerMovement = null;

    public bool canRecieveAttackInput;
    public bool attackInputRecieved;
    public bool attackInputHeld;

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

        ClientInstance.OnOwnerCharacterSpawned += InitializeComponents;

        Controls.Player.Attack.started += ctx => RecieveAttackInput();
        Controls.Player.Attack.canceled += ctx => ReleaseAttackInput();

        // Player Locomotion
        Controls.Player.Jump.performed += ctx => Jump();
        Controls.Locomotion.Sprint.started += ctx => SprintPressed();
        Controls.Locomotion.Sprint.canceled += ctx => SprintReleased();

        // Player Interaction
        Controls.Player.Interact.performed += ctx => InteractPressed();
    }

    void InitializeComponents(GameObject go)
    {
        playerMgmt = go.GetComponent<PlayerManager>();
        combatMgmt = go.GetComponent<CombatManager>();
        playerMovement = go.GetComponent<PlayerMovement>();
    }

    public void RecieveAttackInput()
    {
        // If player is locked into an "interacting" state then don't let this happen.
        if (playerMgmt.isInteracting) { return; }

        if (!canRecieveAttackInput) { return; }

        if (canRecieveAttackInput)
        {
            attackInputHeld = true;
            // Tells CombatManager to determine the means of the attack
            combatMgmt.AttackPerformed();
        }
    }

    public void ReleaseAttackInput()
    {
        attackInputHeld = false;
    }

    void Jump()
    {
        playerMovement.Jump();
    }

    void SprintPressed()
    {
        playerMovement.SprintPressed();
    }

    void SprintReleased()
    {
        playerMovement.SprintReleased();
    }

    void InteractPressed()
    {
        Event_OnInteract?.Invoke();
    }
}
