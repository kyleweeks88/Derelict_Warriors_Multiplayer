using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerUserInterface : MonoBehaviour
{
    InputManager inputMgmt;
    public GameObject playerUI;

    private void OnEnable()
    {
        ClientInstance.OnOwnerCharacterSpawned += Initialize;    
    }

    void Initialize(GameObject go)
    {
        inputMgmt = go.GetComponent<InputManager>();
        inputMgmt.userInterfaceEvent += EnableInterface;
    }

    public void EnableInterface()
    {
        if (!playerUI.activeInHierarchy)
        {
            playerUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            inputMgmt.EnableUserInterfaceInput();
        }
        else
        {
            playerUI.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            inputMgmt.EnableGameplayInput();
        }
    }
}
