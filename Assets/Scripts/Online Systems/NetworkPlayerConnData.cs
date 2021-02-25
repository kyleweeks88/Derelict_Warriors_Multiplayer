using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class NetworkPlayerConnData : NetworkBehaviour
{
    #region MyNetworkManager Singleton Ref
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

    // SyncVar: VARIABLES THAT CAN ONLY BE CHANGED BY THE SERVER
    // IF THERE IS A hook FUNCTION THEN IT RUNS THE ASSOCIATED FUNCTION
    // WHENEVER THE SyncVar IS UPDATED
    [SyncVar]
    private string displayName = "Loading...";
    //[SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);
    }


    public override void OnStopClient()
    {
        myNetworkManager.playerDataList.Remove(this);
    }

    //public void HandleDisplayNameChanged(string oldValue, string newValue) => NetworkLobbySystem.Instance.UpdateDisplay();

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    [Command]
    void CmdSetDisplayName(string displayName)
    {
        // VALIDATE NAME HERE FOR PROFANITY OR LENGTH ETC.
        DisplayName = displayName;
    }
}
