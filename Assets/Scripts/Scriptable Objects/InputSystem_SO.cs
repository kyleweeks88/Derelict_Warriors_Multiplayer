using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputSystem_SO : ScriptableObject, Controls.IPlayerActions, Controls.IUserInterfaceActions
{
    private Controls controls;
    public Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    public event UnityAction<Vector2> moveEvent;
    public event UnityAction sprintEventStarted;
    public event UnityAction sprintEventCancelled;
    public event UnityAction jumpEvent;
    public event UnityAction dodgeEvent;
    public event UnityAction attackEventStarted;
    public event UnityAction attackEventCancelled;
    public event UnityAction rangedAttackEventStarted;
    public event UnityAction rangedAttackEventCancelled;
    // Used when the player interacts with contextual objects in the environment
    public event UnityAction interactEvent;
    public event UnityAction userInterfaceEvent;

    private void OnEnable()
    {
        Controls.Enable();
        Controls.Player.SetCallbacks(this);
        EnableGameplayInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (attackEventStarted != null &&
            context.phase == InputActionPhase.Started)
            attackEventStarted.Invoke();

        if (attackEventCancelled != null &&
            context.phase == InputActionPhase.Canceled)
            attackEventCancelled.Invoke();
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (rangedAttackEventStarted != null &&
            context.phase == InputActionPhase.Started)
            rangedAttackEventStarted.Invoke();

        if (rangedAttackEventCancelled != null &&
            context.phase == InputActionPhase.Canceled)
            rangedAttackEventCancelled.Invoke();
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (dodgeEvent != null &&
            context.phase == InputActionPhase.Performed)
            dodgeEvent.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (interactEvent != null &&
            context.phase == InputActionPhase.Performed)
            interactEvent.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (jumpEvent != null && 
            context.phase == InputActionPhase.Performed)
            jumpEvent.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // USE FOR DIFFERENT FORMS OF MOUSE MOVEMENT EVENTUALLY...
        // I.E. UI MOVEMENT etc.
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(moveEvent != null)
        {
            moveEvent.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (sprintEventStarted != null &&
            context.phase == InputActionPhase.Started)
            sprintEventStarted.Invoke();

        if (sprintEventCancelled != null &&
            context.phase == InputActionPhase.Canceled)
            sprintEventCancelled.Invoke();
    }

    public void OnUserInterface(InputAction.CallbackContext context)
    {
        if (userInterfaceEvent != null &&
            context.phase == InputActionPhase.Started)
            userInterfaceEvent.Invoke();       
    }

    public void OnExitUserInterface(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
        //EnableGameplayInput();
    }

    public void EnableGameplayInput()
    {
        controls.Player.Enable();
        controls.UserInterface.Disable();
    }

    public void EnableUserInterfaceInput()
    {
        controls.UserInterface.Enable();
        controls.Player.Disable();
    }

    public void DisableAllInput()
    {
        controls.Player.Disable();
        controls.UserInterface.Disable();
    }
}
