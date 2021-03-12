using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [Header("Component Ref")]
    public CharacterController charCtrl;
    public ClientInstance ci;
    public InputManager inputMgmt;
    public EquipmentManager equipmentMgmt;
    public PlayerStats playerStats;
    public AnimationManager animMgmt;
    public CombatManager combatMgmt;
    public PlayerMovement playerMovement;

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

        charCtrl = gameObject.GetComponent<CharacterController>();
        charCtrl.enabled = true;

        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerMovement.enabled = true;

        equipmentMgmt = gameObject.GetComponent<EquipmentManager>();
        equipmentMgmt.enabled = true;

        playerStats = gameObject.GetComponent<PlayerStats>();
        playerStats.enabled = true;

        animMgmt = gameObject.GetComponent<AnimationManager>();
        animMgmt.enabled = true;

        combatMgmt = gameObject.GetComponent<CombatManager>();
        combatMgmt.enabled = true;

        myCamera.SetActive(true);
        freeLook.SetActive(true);
        sprintCamera.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
