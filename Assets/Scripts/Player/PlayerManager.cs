using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [Header("Camera Ref")]
    public GameObject myCamera = null;
    public GameObject freeLook;
    public GameObject sprintCamera;

    public bool isInteracting = false;

    public override void OnStartAuthority()
    {
        myCamera.SetActive(true);
        freeLook.SetActive(true);
        sprintCamera.SetActive(true);
    }
}
