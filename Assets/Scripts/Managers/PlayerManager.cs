using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [Header("Component Ref")]
    public InputManager inputMgmt;
    public Rigidbody myRb;
    public EquipmentManager equipmentMgmt;
    public PlayerStats playerStats;
    public AnimationManager animMgmt;
    public CombatManager combatMgmt;
    public PlayerMovement playerMovement;
    public StaminaManager staminaMgmt;
    public DodgeControl dodgeCtrl;
    public VitalsManager vitalsMgmt;

    [Header("Camera Ref")]
    public GameObject myCamera = null;
    public GameObject freeLook;
    public GameObject sprintCamera;

    public bool isInteracting = false;

    public override void OnStartAuthority()
    {
        myRb = gameObject.GetComponent<Rigidbody>();

        inputMgmt = gameObject.GetComponent<InputManager>();
        inputMgmt.enabled = true;

        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerMovement.enabled = true;

        vitalsMgmt = gameObject.GetComponent<VitalsManager>();
        vitalsMgmt.enabled = true;

        equipmentMgmt = gameObject.GetComponent<EquipmentManager>();
        equipmentMgmt.enabled = true;

        playerStats = gameObject.GetComponent<PlayerStats>();
        playerStats.enabled = true;

        animMgmt = gameObject.GetComponent<AnimationManager>();
        animMgmt.enabled = true;

        combatMgmt = gameObject.GetComponent<CombatManager>();
        combatMgmt.enabled = true;

        dodgeCtrl = gameObject.GetComponent<DodgeControl>();
        dodgeCtrl.enabled = true;

        myCamera.SetActive(true);
        freeLook.SetActive(true);
        sprintCamera.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
