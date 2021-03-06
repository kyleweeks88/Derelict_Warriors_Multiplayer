using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [Header("Component Ref")]
    public ClientInstance ci;
    public InputManager inputMgmt;

    [Header("Camera Ref")]
    public GameObject myCamera = null;
    public GameObject freeLook;
    public GameObject sprintCamera;

    public bool isInteracting = false;

    public override void OnStartAuthority()
    {
        ci = ClientInstance.ReturnClientInstance();
        if (ci != null)
        {
            inputMgmt = ci.GetComponent<InputManager>();
        }

        myCamera.SetActive(true);
        freeLook.SetActive(true);
        sprintCamera.SetActive(true);
    }
}
