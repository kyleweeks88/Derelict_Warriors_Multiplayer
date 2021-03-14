using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour, IMovementModifier
{
    [Header("References")]
    [SerializeField] private CharacterController charCtrl = null;
    [SerializeField] private MovementHandler movementHandler = null;

    [Header("Settings")]
    [SerializeField] private float groundedPullMagnitude = 5f;

    private readonly float gravityMagnitude = Physics.gravity.y;

    private bool wasGroundedLastFrame;

    public Vector3 Value { get; private set; }

    private void OnEnable() => movementHandler.AddModifier(this);
    private void OnDisable() => movementHandler.RemoveModifier(this);

    private void Update() => ProcessGravity();

    private void ProcessGravity()
    {
        if(charCtrl.isGrounded)
        {
            Value = new Vector3(Value.x, -groundedPullMagnitude, Value.z);
        }
        else if(wasGroundedLastFrame)
        {
            Value = Vector3.zero;
        }
        else
        {
            Value = new Vector3(Value.x, Value.y + gravityMagnitude * Time.deltaTime, Value.z);
        }

        wasGroundedLastFrame = charCtrl.isGrounded;
    }
}
