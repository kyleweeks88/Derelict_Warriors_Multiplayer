using System.Collections;
using System.Collections.Generic;
using Mirror;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public Transform groundColPos;
    public LayerMask whatIsWalkable;

    [Header("Component Ref")]
    [SerializeField] PlayerManager playerMgmt = null;

    Vector3 movement;
    Vector2 _previousMovementInput;

    float playerGravity = -9.81f;
    float currentMoveSpeed = 0f;
    float turnSpeed = 15f;
    [HideInInspector] public float yVelocity = 0;

    [HideInInspector] public bool isSprinting = false;
    [HideInInspector] public bool isJumping;
    public bool isGrounded;

    FloatVariable stamina;
    FloatVariable health;
    PhysicMaterial physMat;

    public override void OnStartAuthority()
    {
        physMat = gameObject.GetComponent<CapsuleCollider>().material;

        playerMgmt.inputMgmt.jumpEvent += Jump;
        playerMgmt.inputMgmt.sprintEventStarted += SprintPressed;
        playerMgmt.inputMgmt.sprintEventCancelled += SprintReleased;
        playerMgmt.inputMgmt.moveEvent += OnMove;

        stamina = playerMgmt.vitalsMgmt.stamina;
        health = playerMgmt.vitalsMgmt.health;

        currentMoveSpeed = playerMgmt.playerStats.moveSpeed;
    }

    [ClientCallback]
    private void FixedUpdate()
    {
        if (!base.hasAuthority) { return; }

        // Handles the player's PhysicMaterial to prevent slow-sliding down shallow slopes when standing still.
        // but turns friction to zero when the player is moving.
        if (_previousMovementInput.sqrMagnitude != 0)
        {
            physMat.dynamicFriction = 0f;
        }
        else
        {
            physMat.dynamicFriction = 1f;
        }

        GroundCheck();

        if (isJumping && playerMgmt.myRb.velocity.y < -5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, whatIsWalkable))
            {
                isJumping = false;
            }
        }

        if(movement.sqrMagnitude > 0)
        {
            Debug.DrawRay(transform.position, transform.position - movement, Color.green);
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
            playerMgmt.myRb.velocity += -Vector3.up * playerMgmt.playerStats.playerGravity;
        }
        else
        {
            isJumping = false;
            isGrounded = true;
        }
    }

    [Client]
    void OnMove(Vector2 movement)
    {
        if (!base.hasAuthority) { return; }

        _previousMovementInput = movement;
    }

    [Client]
    public void Move()
    {
        // CONVERTS THE INPUT INTO A NORMALIZED VECTOR3
        movement = new Vector3
        {
            x = _previousMovementInput.x,
            z = _previousMovementInput.y
        }.normalized;

        // MAKES THE CHARACTER'S FORWARD AXIS MATCH THE CAMERA'S FORWARD AXIS
        Vector3 rotationMovement = Quaternion.Euler(0, playerMgmt.myCamera.transform.rotation.eulerAngles.y, 0) * movement;
        //Vector3 verticalMovement = Vector3.up * yVelocity;
        //if (isGrounded && playerMgmt.myRb.velocity.y > 0.75f)
        //{
        //    RaycastHit slopeHit;
        //    if (Physics.Raycast(new Vector3(transform.position.x,
        //        transform.position.y + 0.5f,
        //        transform.position.z),
        //        transform.position - movement, out slopeHit, 2f, whatIsWalkable))
        //    {
        //        Debug.Log("Hit Slope!");
        //    }
        //}

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
        playerMgmt.myRb.AddForce((rotationMovement * currentMoveSpeed) / Time.deltaTime);
    }

    #region Sprinting
    public void SprintPressed()
    {
        if (movement.z > 0.1 && stamina.GetCurrentValue() 
            - playerMgmt.playerStats.staminaDrainAmount > 0)
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
            //vitalsMgmt.staminaVal.GetCurrentValue() - playerMgmt.playerStats.staminaDrainAmount > 0
            if (stamina.GetCurrentValue() 
                - playerMgmt.playerStats.staminaDrainAmount > 0)
            {
                playerMgmt.vitalsMgmt.VitalDrainOverTime(stamina, 
                    playerMgmt.playerStats.staminaDrainAmount,
                    playerMgmt.playerStats.staminaDrainDelay);
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
        if (playerMgmt.isInteracting) { return; }

        if(!isJumping && isGrounded)
        {
            if (stamina.GetCurrentValue() - 10f > 0)
            {
                playerMgmt.isInteracting = true;
                isJumping = true;
                isGrounded = false;
                playerMgmt.vitalsMgmt.TakeDamage(stamina, 10f);

                playerMgmt.myRb.velocity += Vector3.up * playerMgmt.playerStats.jumpVelocity;
            }
        }
    }
}
