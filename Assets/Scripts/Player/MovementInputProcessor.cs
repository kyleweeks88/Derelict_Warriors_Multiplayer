using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MovementInputProcessor : NetworkBehaviour, IMovementModifier
{
    [Header("References")]
    [SerializeField] private CharacterController charCtrl = null;
    [SerializeField] private MovementHandler movementHandler = null;
    [SerializeField] private ForceReceiver forceReceiver = null;
    [SerializeField] private Camera playerCam;
    Controls controls;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float acceleration = 0.5f;

    private float currentSpeed;

    private Vector3 previousVelocity;
    private Vector2 previousInputDirection;
    private Vector2 move;

    private Transform mainCameraTransform;

    public Vector3 Value { get; private set; }

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Enable();
        movementHandler.AddModifier(this);
    }

    private void OnDisable()
    {
        controls.Disable();
        movementHandler.RemoveModifier(this);
    }

    private void Start() => mainCameraTransform = playerCam.transform;

    private void Update()
    {
        move = controls.Player.Move.ReadValue<Vector2>();
        SetMovementInput(move);
    }

    public void SetMovementInput(Vector2 inputDir)
    {
        previousInputDirection = inputDir;
        Move();
    }

    private void Move()
    {
        float targetSpeed = movementSpeed * previousInputDirection.magnitude;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        Vector3 forward = gameObject.transform.forward;
        Vector3 right = gameObject.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 movementDirection;

        if(targetSpeed != 0f)
        {
            movementDirection = forward * previousInputDirection.y + right * previousInputDirection.x;
        }
        else
        {
            movementDirection = previousVelocity.normalized;
        }

        Value = movementDirection * currentSpeed;

        previousVelocity = new Vector3(charCtrl.velocity.x, 0f, charCtrl.velocity.z);

        currentSpeed = previousVelocity.magnitude;
    }
}
