using System.Collections;
using System.Collections.Generic;
using Mirror;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public bool isGrounded;
    public Transform groundColPos;
    public LayerMask whatIsWalkable;

    [Header("Component Ref")]
    [SerializeField] PlayerManager playerMgmt = null;
    [SerializeField] PlayerStaminaManager staminaMgmt = null;

    float currentMoveSpeed = 0f;
    float turnSpeed = 15f;
    [HideInInspector] public bool isSprinting = false;
    Vector3 movement;

    [HideInInspector] public bool isJumping;
    [HideInInspector] public float yVelocity = 0;
    float gravity = -9.81f;


    public override void OnStartAuthority()
    {
        staminaMgmt = GetComponent<PlayerStaminaManager>();

        currentMoveSpeed = playerMgmt.playerStats.moveSpeed;
    }

    [ClientCallback]
    void Update()
    {
        if(!hasAuthority) {return;}

        playerMgmt.inputMgmt.TickInput(Time.deltaTime);
    }

    [ClientCallback]
    private void FixedUpdate()
    {
        if (!base.hasAuthority) { return; }

        GroundCheck();

        if (isJumping && playerMgmt.myRb.velocity.y < -5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, whatIsWalkable))
            {
                isJumping = false;
            }
        }

        UpdateIsSprinting();
        Move();
    }

    void GroundCheck()
    {
        Collider[] groundCollisions = Physics.OverlapSphere(groundColPos.position, 0.25f, whatIsWalkable);

        if (groundCollisions.Length <= 0)
        {
            isGrounded = false;
            playerMgmt.myRb.velocity += -Vector3.up * 1.1f;
        }
        else
        {
            isJumping = false;
            isGrounded = true;
        }
    }

    [Client]
    public void Move()
    {
        // CONVERTS THE INPUT INTO A NORMALIZED VECTOR3
        movement = new Vector3
        {
            x = playerMgmt.inputMgmt.horizontal,
            z = playerMgmt.inputMgmt.vertical
        }.normalized;

        // MAKES THE CHARACTER'S FORWARD AXIS MATCH THE CAMERA'S FORWARD AXIS
        Vector3 rotationMovement = Quaternion.Euler(0, playerMgmt.myCamera.transform.rotation.eulerAngles.y, 0) * movement;
        Vector3 verticalMovement = Vector3.up * yVelocity;

        // MAKES THE CHARACTER MODEL TURN TOWARDS THE CAMERA'S FORWARD AXIS
        float cameraYaw = playerMgmt.myCamera.transform.rotation.eulerAngles.y;
        // ... ONLY IF THE PLAYER IS MOVING
        if (movement.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, cameraYaw, 0), turnSpeed * Time.deltaTime);
        }

        if(movement.z <= 0)
        {
            SprintReleased();
        }

        // HANDLES ANIMATIONS
        playerMgmt.animMgmt.MovementAnimation(movement.x, movement.z);

        // MOVES THE PLAYER
        playerMgmt.myRb.AddForce((verticalMovement + (rotationMovement * currentMoveSpeed)) / Time.deltaTime);
    }

    #region Sprinting
    public void SprintPressed()
    {
        if (movement.z > 0.1 && staminaMgmt.GetCurrentVital() - staminaMgmt.staminaDrainAmount > 0)
        {
            currentMoveSpeed *= playerMgmt.playerStats.sprintMultiplier;
            isSprinting = true;
            playerMgmt.isInteracting = true;

            playerMgmt.sprintCamera.GetComponent<CinemachineVirtualCameraBase>().m_Priority = 11;
        }
    }

    public void SprintReleased()
    {
        isSprinting = false;
        currentMoveSpeed = playerMgmt.playerStats.moveSpeed;

        playerMgmt.sprintCamera.GetComponent<CinemachineVirtualCameraBase>().m_Priority = 9;
    }

    void UpdateIsSprinting()
    {
        if (isSprinting)
        {
            if (staminaMgmt.GetCurrentVital() - staminaMgmt.staminaDrainAmount > 0)
            {
                staminaMgmt.StaminaDrain();
            }
            else
            {
                SprintReleased();
                return;
            }
        }
    }
    #endregion

    [Client]
    public void Jump()
    {
        if(!isJumping && isGrounded)
        {
            if (staminaMgmt.GetCurrentVital() - 10f > 0)
            {
                playerMgmt.isInteracting = true;
                isJumping = true;
                isGrounded = false;
                staminaMgmt.TakeDamage(10f);

                playerMgmt.myRb.velocity += Vector3.up * playerMgmt.playerStats.jumpVelocity;
            }
        }
    }
}
