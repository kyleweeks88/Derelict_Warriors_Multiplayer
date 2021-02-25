using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class JoinLobbyMenu : MonoBehaviour
{
    public static string IPAddress { get; private set; }
    const string PlayerPrefsAddressKey = "IP Address";
    [SerializeField] MyNetworkManager networkManager = null;

    [Header("UI")]
    [SerializeField] GameObject landingPagePanel = null;
    [SerializeField] TMP_InputField ipAddressInputField = null;
    [SerializeField] Button joinButton = null;

    private void OnEnable()
    {
        MyNetworkManager.OnClientConnected += HandleClientConnected;
        MyNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        MyNetworkManager.OnClientConnected -= HandleClientConnected;
        MyNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    void Start() => SetUpInputField();

    void SetUpInputField()
    {
        // CHECKS PlayerPrefs FOR A ALREADY CREATED DEFAULT PLAYER NAME
        if (!PlayerPrefs.HasKey(PlayerPrefsAddressKey)) { return; }

        // SETS THE defaultName TO THE NAME KEY FOUND IN PlayerPrefs
        string lastAddress = PlayerPrefs.GetString(PlayerPrefsAddressKey);

        // CHANGES THE DISPLAY
        ipAddressInputField.text = lastAddress;

        SetIPAdress(lastAddress);
    }

    public void SetIPAdress(string name)
    {
        // IF THE NAME IS VALID AND NOT EMPTY MAKE BUTTON INTERACTIVE
        joinButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        // DEACTIVATES THE INPUT IP ADDRESS UI WINDOW ONCE CLIENT HAS CONNECTED 
        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }

    public void SaveIPAddress()
    {
        IPAddress = ipAddressInputField.text;

        PlayerPrefs.SetString(PlayerPrefsAddressKey, IPAddress);
    }
}
