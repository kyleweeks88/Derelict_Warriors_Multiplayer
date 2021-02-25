using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class NetworkLobbySystem : NetworkBehaviour
{
    #region Variables
    public GameObject lobbyUI = null;
    [SerializeField] TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] Button startGameButton = null;

    // SyncVar: VARIABLES THAT CAN ONLY BE CHANGED BY THE SERVER
    // IF THERE IS A hook FUNCTION THEN IT RUNS THE ASSOCIATED FUNCTION
    // WHENEVER THE SyncVar IS UPDATED
    //[SyncVar(hook = nameof(HandleDisplayNameChanged))]
    //public string DisplayName = "Loading...";
    //[SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }
    #endregion

    #region MyNetworkManager Ref
    private MyNetworkManager l_myNetworkManager;
    private MyNetworkManager myNetworkManager
    {
        get
        {
            if (l_myNetworkManager != null) { return l_myNetworkManager; }
            return l_myNetworkManager = NetworkManager.singleton as MyNetworkManager;
        }
    }
    #endregion

    #region Singleton
    private static NetworkLobbySystem _instance;

    public static NetworkLobbySystem Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion


    public override void OnStartServer()
    {
        Debug.Log("OnPlayerAdded Subscribed");
        base.OnStartServer();
    }


    public override void OnStartAuthority()
    {
        lobbyUI.SetActive(true);
    }


    [ClientRpc]
    public void RpcAddPlayerToLobby(NetworkPlayerConnData playerData)
    {
        // ADD PLAYER TO A LIST OF PLAYERS IN THE LOBBY

        Debug.Log("PLAYER "+playerData.connectionToClient+" ADDED TO LOBBY");
    }


    public void UpdateDisplay()
    {
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < myNetworkManager.playerDataList.Count; i++)
        {
            playerNameTexts[i].text = myNetworkManager.playerDataList[i].DisplayName;
        }
        Debug.Log("UpdateDisplay");
    }


    // IS THIS NEEDED???
    public override void OnStopClient()
    {
        UpdateDisplay();
    }


    //[Command]
    //public void CmdStartGame()
    //{
    //    if (myNetworkManager.playerDataList[0].connectionToClient != connectionToClient) { return; }
    //    lobbyUI.SetActive(false);
    //    myNetworkManager.StartGame();
    //}
}
