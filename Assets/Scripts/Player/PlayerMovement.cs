using System.Collections;
using System.Collections.Generic;
using Mirror;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] CharacterController controller = null;
    [SerializeField] Animator myAnimator;
    [SerializeField] GameObject myCamera;
    [SerializeField] CinemachineFreeLook freeLook;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float turnSpeed = 15f;

    float yVelocity = 0;
    float gravity = -9.81f;

    bool isJumping;
    [SerializeField] float jumpVelocity = 5f;

    Vector2 previousInput;

    Controls controls;
    Controls Controls
    {
        get
        {
            if(controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    [ClientCallback]
    void OnEnable() => Controls.Enable();
    [ClientCallback]
    void OnDisable() => Controls.Disable();

    public override void OnStartAuthority()
    {
        myCamera.SetActive(true);
        freeLook.gameObject.SetActive(true);

        enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Controls.Player.Jump.performed += ctx => Jump();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        controller.enabled = base.hasAuthority;
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

        myAnimator.SetBool("isJumping", isJumping);
        myAnimator.SetBool("isGrounded", controller.isGrounded);
        myAnimator.SetFloat("yVelocity", yVelocity);
        Move();
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
        Vector3 rotationMovement = Quaternion.Euler(0,myCamera.transform.rotation.eulerAngles.y, 0)  * movement;
        Vector3 verticalMovement = Vector3.up * yVelocity;

        // MAKES THE CHARACTER MODEL TURN TOWARDS THE CAMERA'S FORWARD AXIS
        float cameraYaw = myCamera.transform.rotation.eulerAngles.y;
        // ... ONLY IF THE PLAYER IS MOVING
        if(movement.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,cameraYaw,0), turnSpeed * Time.deltaTime);
        }

        // HANDLES ANIMATIONS
        myAnimator.SetFloat("InputX", movement.x);
        myAnimator.SetFloat("InputY", movement.z);

        // MOVES THE PLAYER
        controller.Move((verticalMovement + (rotationMovement * moveSpeed)) * Time.deltaTime);
    }

    [Client]
    void Jump()
    {
        if(!isJumping)
        {
            isJumping = true;

            yVelocity += jumpVelocity;
        }
    }
}
