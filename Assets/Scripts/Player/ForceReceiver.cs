using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceReceiver : MonoBehaviour, IMovementModifier
{
    [Header("References")]
    [SerializeField] private CharacterController charCtrl = null;
    [SerializeField] private MovementHandler movementHandler = null;

    [Header("Settings")]
    [SerializeField] private float mass = 1f;
    [SerializeField] private float drag = 5f;

    private bool wasGroundedLastFrame;

    public Vector3 Value { get; private set; }

    private void OnEnable() => movementHandler.AddModifier(this);
    private void OnDisable() => movementHandler.RemoveModifier(this);

    private void Update()
    {
        if(!wasGroundedLastFrame && charCtrl.isGrounded)
        {
            // IF PLAYER IS GROUNDED TURN Y-AXIS GRAVITY FORCE TO 0
            Value = new Vector3(Value.x, 0f, Value.z);
        }

        wasGroundedLastFrame = charCtrl.isGrounded;

        // IF THE VALUE OF FORCE IS SMALLER THAN 0.2 COUNT AS 0 FORCE
        if(Value.magnitude < 0.2)
        {
            Value = Vector3.zero;
        }

        // Value BEING INITIAL APPLIED FORCE - INTERPOLATE FROM Value TO 0 OVER TIME AND drag
        Value = Vector3.Lerp(Value, Vector3.zero, drag * Time.deltaTime);
    }

    // SETS Value TO WHATEVER FORCE CALLED THIS METHOD DIVIDED BY mass
    public void AddForce(Vector3 force) => Value += force / mass;
}
