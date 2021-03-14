using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float jumpVelocity = 10f;

    CharacterController charCtrl;
    ForceReceiver forceReceiver;
    Controls controls;

    private void Awake()
    {
        if (controls == null)
            controls = new Controls();

        controls.Player.Jump.performed += ctx => Jump();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();


    private void Start()
    {
        charCtrl = GetComponent<CharacterController>();
        forceReceiver = GetComponent<ForceReceiver>();
    }

    private void Jump()
    {
        if (charCtrl.isGrounded)
            forceReceiver.AddForce(Vector3.up * jumpVelocity);
    }
}
