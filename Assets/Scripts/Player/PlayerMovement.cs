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

        // APPLIES GRAVITY TO CHARACTER IF NOT GROUNDED
        //if(!GroundCheck())
        //{
        //    yVelocity += gravity * Time.deltaTime;
        //}
        //else if(yVelocity < 0)
        //{
        //    yVelocity = 0f;
        //}

        if(isJumping && yVelocity < 0)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, whatIsWalkable))
            {
                isJumping = false;
            }
        }

        UpdateIsSprinting();
    }

    [ClientCallback]
    private void FixedUpdate()
    {
        if (!base.hasAuthority) { return; }

        GroundCheck();
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
            //isFalling = false;
            isGrounded = true;
        }
    }

    [Client]
    public void Dodge()
    {
        // I DUNNO...
    }

    [Client]
    public void Move()
    {
        // READS THE INPUT SYSTEMS ACTION
        var movementInput = playerMgmt.inputMgmt.Controls.Player.Move.ReadValue<Vector2>();

        // CONVERTS THE INPUT INTO A NORMALIZED VECTOR3
        movement = new Vector3
        {
            x = movementInput.x,
            z = movementInput.y
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
        playerMgmt.isInteracting = false;
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
        if(!isJumping)
        {
            if (staminaMgmt.GetCurrentVital() - 10f > 0)
            {
                staminaMgmt.TakeDamage(10f);
                isJumping = true;
                isGrounded = false;

                playerMgmt.myRb.velocity += Vector3.up * playerMgmt.playerStats.jumpVelocity;
            }
        }
    }
}
