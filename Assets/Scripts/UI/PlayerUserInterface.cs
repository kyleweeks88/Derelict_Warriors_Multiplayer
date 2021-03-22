using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerUserInterface : MonoBehaviour
{
    public InputSystem_SO inputSystem;
    public GameObject playerUI;

    private void Awake()
    {
        inputSystem.userInterfaceEvent += EnableInterface;
    }

    public void EnableInterface()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (!playerUI.activeInHierarchy)
        {
            playerUI.SetActive(true);
            inputSystem.EnableUserInterfaceInput();
        }
        else
        {
            playerUI.SetActive(false);
            inputSystem.EnableGameplayInput();
        }
    }
}
