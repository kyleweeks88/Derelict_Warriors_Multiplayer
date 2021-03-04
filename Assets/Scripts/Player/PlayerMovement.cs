using System.Collections;
using System.Collections.Generic;
using Mirror;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] StaminaManager staminaMgmt = null;
    [SerializeField] CharacterController controller = null;
    [SerializeField] Animator myAnimator;

    [Header("Camera Ref")]
    [SerializeField] GameObject myCamera;
    [SerializeField] GameObject freeLook;
    [SerializeField] GameObject sprintCamera;

    [Header("Movement settings")]
    [SerializeField] float moveSpeed = 5f;
    float currentMoveSpeed = 0f;
    [SerializeField] float sprintMultiplier = 2f;
    [SerializeField] float turnSpeed = 15f;
    bool isSprinting = false;

    [Header("Jump settings")]
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

    #region Initialize Input
    Controls controls;
    Controls Controls
    {
        get
        {
            if(controls != null) { return controls; }
            return controls = new Controls();
        }
    }
    #endregion

    [ClientCallback]
    void OnEnable() => Controls.Enable();
    [ClientCallback]
    void OnDisable() => Controls.Disable();

    public override void OnStartAuthority()
    {
        myCamera.SetActive(true);
        freeLook.gameObject.SetActive(true);

        enabled = true;
        controller.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Controls.Player.Jump.performed += ctx => Jump();
        Controls.Locomotion.Sprint.started += ctx => SprintPressed();
        Controls.Locomotion.Sprint.canceled += ctx => SprintReleased();

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
            if(Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f, LayerMask.GetMask("Default")))
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
        var movementInput = Controls.Player.Move.ReadValue<Vector2>();

        // CONVERTS THE INPUT INTO A NORMALIZED VECTOR3
        var movement = new Vector3
        {
            x = movementInput.x,
            z = movementInput.y
        }.normalized;

        // MAKES THE CHARACTER'S FORWARD AXIS MATCH THE CAMERA'S FORWARD AXIS
        Vector3 rotationMovement = Quaternion.Euler(0, myCamera.transform.rotation.eulerAngles.y, 0) * movement;
        Vector3 verticalMovement = Vector3.up * yVelocity;

        // MAKES THE CHARACTER MODEL TURN TOWARDS THE CAMERA'S FORWARD AXIS
        float cameraYaw = myCamera.transform.rotation.eulerAngles.y;
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
    void SprintPressed()
    {
        if (staminaMgmt.GetCurrentVital() - staminaMgmt.staminaDrainAmount > 0)
        {
            currentMoveSpeed *= sprintMultiplier;
            isSprinting = true;
            freeLook.SetActive(false);
            sprintCamera.SetActive(true);
        }
    }

    void SprintReleased()
    {
        isSprinting = false;
        currentMoveSpeed = moveSpeed;
        freeLook.SetActive(true);
        sprintCamera.SetActive(false);
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
    void Jump()
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
