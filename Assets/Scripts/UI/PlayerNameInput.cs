using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_InputField nameInputField = null;
    [SerializeField] Button continueButton = null;

    // THIS IS A STATIC SO IT CAN BE READ FROM OTHER SCRIPTS ACCESSING IT
    public static string DisplayName { get; private set; }

    const string PlayerPrefsNameKey = "PlayerName";

    void Start() => SetUpInputField();

    void SetUpInputField()
    {
        // CHECKS PlayerPrefs FOR A ALREADY CREATED DEFAULT PLAYER NAME
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        // SETS THE defaultName TO THE NAME KEY FOUND IN PlayerPrefs
        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        // CHANGES THE DISPLAY
        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    // TURNS ON THE CONTINUE BUTTON UI 
    public void SetPlayerName(string name)
    {
        // IF THE NAME IS VALID AND NOT EMPTY MAKE BUTTON INTERACTIVE
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    // SAVES THE PLAYER'S NAME, WHICH IS CALLED BY THE BUTTON IN THE UI
    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }
}
