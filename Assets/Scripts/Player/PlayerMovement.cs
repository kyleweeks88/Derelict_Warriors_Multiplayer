using System.Collections;
using System.Collections.Generic;
using Mirror;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Component Ref")]
    [SerializeField] PlayerManager playerMgmt = null;
    [SerializeField] StaminaManager staminaMgmt = null;
    [SerializeField] CharacterController controller = null;
    [SerializeField] Animator myAnimator;

    [Header("Movement settings")]
    [SerializeField] float moveSpeed = 5f;
    float currentMoveSpeed = 0f;
    [SerializeField] float sprintMultiplier = 2f;
    [SerializeField] float turnSpeed = 15f;
    bool isSprinting = false;

    [Header("Jump settings")]
    public LayerMask whatIsWalkable;
    [SerializeField] float jumpVelocity = 5f;
    bool isJumping;
    float yVelocity = 0;
    float gravity = -9.81f;

    #region Animator Parameters
    // My Animator parameters turned from costly Strings to cheap Ints
    int isSprintingParam = Animator.StringToHash("isSprinting");
    int isJumpingParam = Animator.StringToHash("isJumping");
    int isGroundedParam = Animator.StringToHash("isGrounded");
    int yVelocityParam = Animator.StringToHash("yVelocity");
    int inputXParam = Animator.StringToHash("InputX");
    int inputYParam = Animator.StringToHash("InputY");
    #endregion


    public override void OnStartAuthority()
    {
        enabled = true;
        controller.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        currentMoveSpeed = moveSpeed;
    }

    [ClientCallback]
    void Update()
    {
        if(!hasAuthority) {return;}

        // APPLIES GRAVITY TO CHARACTER IF NOT GROUNDED
        if(!controller.isGrounded)
        {
            yVelocity += gravity * Time.deltaTime;
        }
        else if(yVelocity < 0)
        {
            yVelocity = 0f;
        }

        if(isJumping && yVelocity < 0)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, whatIsWalkable))
            {
                isJumping = false;
            }
        }

        myAnimator.SetBool(isJumpingParam, isJumping);
        myAnimator.SetBool(isGroundedParam, controller.isGrounded);
        myAnimator.SetFloat(yVelocityParam, yVelocity);

        Move();
        UpdateIsSprinting();
    } 


    [Client]
    void Move()
    {
        // READS THE INPUT SYSTEMS ACTION
        var movementInput = playerMgmt.inputMgmt.Controls.Player.Move.ReadValue<Vector2>();

        // CONVERTS THE INPUT INTO A NORMALIZED VECTOR3
        var movement = new Vector3
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

        // HANDLES ANIMATIONS
        myAnimator.SetFloat(inputXParam, movement.x);
        myAnimator.SetFloat(inputYParam, movement.z);

        // MOVES THE PLAYER
        controller.Move((verticalMovement + (rotationMovement * currentMoveSpeed)) * Time.deltaTime);
    }

    #region Sprinting
    public void SprintPressed()
    {
        if (staminaMgmt.GetCurrentVital() - staminaMgmt.staminaDrainAmount > 0)
        {
            currentMoveSpeed *= sprintMultiplier;
            isSprinting = true;

            playerMgmt.sprintCamera.GetComponent<CinemachineVirtualCameraBase>().m_Priority = 11;
        }
    }

    public void SprintReleased()
    {
        isSprinting = false;
        currentMoveSpeed = moveSpeed;

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

        myAnimator.SetBool(isSprintingParam, isSprinting);
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

                yVelocity += jumpVelocity;
            }
        }
    }
}
