using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{

    public delegate void OnInteractPressed();
    public event OnInteractPressed Event_OnInteract;

    [Header("Component Ref")]
    [SerializeField] PlayerManager playerMgmt = null;

    public bool canRecieveAttackInput;
    public bool attackInputRecieved;
    public bool attackInputHeld;
    public bool rangedAttackHeld;

    public float horizontal;
    public float vertical;
    float moveAmount;

    Vector2 moveInput;

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

        Controls.Player.RangedAttack.started += ctx => RecieveRangedAttackInput();
        Controls.Player.RangedAttack.canceled += ctx => ReleaseRangedAttackInput();

        // Player Locomotion
        Controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        Controls.Player.Jump.performed += ctx => Jump();
        Controls.Locomotion.Sprint.started += ctx => SprintPressed();
        Controls.Locomotion.Sprint.canceled += ctx => SprintReleased();

        // Player Interaction
        Controls.Player.Interact.performed += ctx => InteractPressed();

        Controls.Player.Dodge.performed += ctx => DodgeInputRecieved();
    }

    void InitializeComponents(GameObject go)
    {
        playerMgmt = go.GetComponent<PlayerManager>();
    }

    public void TickInput(float delta)
    {
        MovementInput(delta);
    }

    void MovementInput(float delta)
    {
        horizontal = moveInput.x;
        vertical = moveInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
    }

    #region Ranged
    public void RecieveRangedAttackInput()
    {
        // If player is locked into an "interacting" state then don't let this happen.
        if (playerMgmt.isInteracting) { return; }

        if (!canRecieveAttackInput) { return; }

        if (canRecieveAttackInput)
        {
            rangedAttackHeld = true;
            // Tells CombatManager to determine the means of the attack
            playerMgmt.combatMgmt.RangedAttackPerformed();
            playerMgmt.animMgmt.HandleRangedAttackAnimation(rangedAttackHeld);
        }
    }

    public void ReleaseRangedAttackInput()
    {
        rangedAttackHeld = false;
        playerMgmt.animMgmt.HandleRangedAttackAnimation(rangedAttackHeld);
    }
    #endregion

    #region Melee
    public void RecieveAttackInput()
    {
        // If player is locked into an "interacting" state then don't let this happen.
        if (playerMgmt.isInteracting) { return; }

        if (!canRecieveAttackInput) { return; }

        if (canRecieveAttackInput)
        {
            attackInputHeld = true;
            // Tells CombatManager to determine the means of the attack
            playerMgmt.combatMgmt.AttackPerformed();
        }
    }

    public void ReleaseAttackInput()
    {
        attackInputHeld = false;
        playerMgmt.animMgmt.HandleMeleeAttackAnimation(attackInputHeld);
    }
    #endregion

    void DodgeInputRecieved()
    {
        playerMgmt.dodgeCtrl.Dodge(Controls.Player.Move.ReadValue<Vector2>());
    }

    void Jump()
    {
        if (playerMgmt.isInteracting) { return; }

        playerMgmt.playerMovement.Jump();
    }

    void SprintPressed()
    {
        playerMgmt.playerMovement.SprintPressed();
    }

    void SprintReleased()
    {
        playerMgmt.isInteracting = false;
        playerMgmt.playerMovement.SprintReleased();
    }

    void InteractPressed()
    {
        Event_OnInteract?.Invoke();
    }
}
