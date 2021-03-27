using Mirror;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Component Ref")]
    [SerializeField] PlayerManager playerMgmt = null;

    [Header("Ground detection")]
    public LayerMask whatIsWalkable;
    public Transform groundColPos;
    public bool isGrounded;

    [Header("Slide settings")]
    public float slideVelocity;
    float currentSlideVelocity;
    public bool isSliding;

    Vector3 movement;
    Vector3 rotationMovement;
    Vector2 _previousMovementInput;

    [HideInInspector] public float currentMoveSpeed = 0f;
    float turnSpeed = 15f;

    [HideInInspector] public bool isSprinting = false;
    [HideInInspector] public bool isJumping = false;
    bool jumpInputHeld = false;

    FloatVariable stamina;
    PhysicMaterial physMat;
    

    public override void OnStartAuthority()
    {
        playerMgmt.inputMgmt.jumpEventStarted += Jump;
        playerMgmt.inputMgmt.jumpEventCancelled += JumpReleased;
        playerMgmt.inputMgmt.sprintEventStarted += SprintPressed;
        playerMgmt.inputMgmt.sprintEventCancelled += SprintReleased;
        playerMgmt.inputMgmt.moveEvent += OnMove;

        stamina = playerMgmt.vitalsMgmt.stamina;
        physMat = gameObject.GetComponent<CapsuleCollider>().material;

        currentMoveSpeed = playerMgmt.playerStats.moveSpeed;
        currentSlideVelocity = slideVelocity;
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
        else if(_previousMovementInput.sqrMagnitude == 0 && !isSliding)
        {
            physMat.dynamicFriction = 1f;
        }

        HandleSliding();
        GroundCheck();
        UpdateIsSprinting();
        Move();
    }

    void HandleSliding()
    {
        Vector3 adjustedPos = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
        if (!CheckSlope(adjustedPos, Vector3.down, 10f))
        {
            isSliding = false;
            currentSlideVelocity = 0f;
        }
        else
        {
            isSliding = true;
            currentSlideVelocity = slideVelocity;
            physMat.dynamicFriction = 0f;
        }
    }

    void GroundCheck()
    {
        Collider[] groundCollisions = Physics.OverlapSphere(groundColPos.position, 0.25f, whatIsWalkable);

        if (groundCollisions.Length <= 0)
        {
            isGrounded = false;
            currentMoveSpeed = playerMgmt.playerStats.AdjustMoveSpeed(2f);

            // Makes jumping and falling feel better
            if (playerMgmt.myRb.velocity.y < 0f)
            {
                playerMgmt.myRb.velocity += Vector3.up * Physics.gravity.y * (10f - 1f) * Time.deltaTime;
            }
            else if(playerMgmt.myRb.velocity.y > 0f && !jumpInputHeld)
            {
                playerMgmt.myRb.velocity += Vector3.up * Physics.gravity.y * (8f - 1f) * Time.deltaTime;
            }

            // If the player has jumped and is now falling downwards, cast a ray 
            // to check for ground and turn isJumping false if hit.
            if (isJumping && playerMgmt.myRb.velocity.y < 0f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, whatIsWalkable))
                {
                    isJumping = false;
                }
            }
        }
        else
        {
            isGrounded = true;
            currentMoveSpeed = playerMgmt.playerStats.AdjustMoveSpeed(1f);

            // This stops the ground collision from premptively turning isJumping to false when the player jumps.
            if (Mathf.Abs(playerMgmt.myRb.velocity.y) < 0.01f && Mathf.Abs(playerMgmt.myRb.velocity.y) > -0.01f)
                isJumping = false;
        }
    }

    bool CheckSlope(Vector3 position, Vector3 desiredDirection, float distance)
    {
        Debug.DrawRay(position, desiredDirection, Color.green);

        Ray myRay = new Ray(position, desiredDirection); // cast a Ray from the position of our gameObject into our desired direction. Add the slopeRayHeight to the Y parameter.
        RaycastHit hit;

        if (Physics.Raycast(myRay, out hit, distance, whatIsWalkable))
        {
            float slopeAngle = Mathf.Deg2Rad * Vector3.Angle(Vector3.up, hit.normal); // Here we get the angle between the Up Vector and the normal of the wall we are checking against: 90 for straight up walls, 0 for flat ground.

            if (slopeAngle >= 45f * Mathf.Deg2Rad) //You can set "steepSlopeAngle" to any angle you wish.
            {
                return true; // return false if we are very near / on the slope && the slope is steep
            }

            return false; // return true if the slope is not steep
        }

        return false;
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
            y = -currentSlideVelocity,
            z = _previousMovementInput.y
        }.normalized;

        // MAKES THE CHARACTER'S FORWARD AXIS MATCH THE CAMERA'S FORWARD AXIS
        rotationMovement = Quaternion.Euler(0, playerMgmt.myCamera.transform.rotation.eulerAngles.y, 0) * movement;

        // MAKES THE CHARACTER MODEL TURN TOWARDS THE CAMERA'S FORWARD AXIS
        float cameraYaw = playerMgmt.myCamera.transform.rotation.eulerAngles.y;
        // ... ONLY IF THE PLAYER IS MOVING
        if (movement.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, cameraYaw, 0), turnSpeed * Time.deltaTime);
        }

        // Only allows the player to sprint forwards
        if(isSprinting && movement.z <= 0)
        {
            SprintReleased();
        }

        // HANDLES ANIMATIONS
        playerMgmt.animMgmt.MovementAnimation(movement.x, movement.z);

        // MOVES THE PLAYER
        playerMgmt.myRb.velocity += rotationMovement * currentMoveSpeed;
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

    public void Jump()
    {
        if (playerMgmt.isInteracting) { return; }
        if (isSliding) { return; }

        jumpInputHeld = true;

        if(!isJumping && isGrounded)
        {
            if (stamina.GetCurrentValue() - 10f > 0)
            {
                isJumping = true;
                playerMgmt.isInteracting = true;
                playerMgmt.vitalsMgmt.TakeDamage(stamina, 10f);
                playerMgmt.myRb.velocity += Vector3.up * playerMgmt.playerStats.jumpVelocity;
                playerMgmt.myRb.velocity += rotationMovement * playerMgmt.playerStats.jumpVelocity/2f;
            }
        }
    }

    void JumpReleased()
    {
        jumpInputHeld = false;
    }
}
