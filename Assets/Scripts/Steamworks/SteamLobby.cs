using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    [SerializeField] private GameObject buttons = null;

    // TRIGGERED BY SteamMatchmaking.CreateLobby TO CALL OnLobbyCreated FUNCTION
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string HostAddressKey = "HostAddress";

    private NetworkManager networkManager;

    private void Start()
    {
        // CACHE NetworkManager FOR EASE OF USE
        networkManager = GetComponent<NetworkManager>();

        // PREVENTS STEAM ERRORS IF SteamManager ISN'T INITIALIZED
        if (!SteamManager.Initialized) { return; }

        // INITIALIZES A Callback THAT WILL BE TRIGGERED WHEN SteamMatchmaking.CreateLobby IS CALLED
        // Callback WILL THEN TRIGGER OnLobbyCreated FUNCTION
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);

        // USED FOR PROCESSING JOIN REQUESTS
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);

        // 
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    // LOBBY IS CREATED HERE ON THE STEAM SERVERS
    public void HostLobby()
    {
        // TURNS OFF BUTTONS ONCE LOBBY IS CREATED
        buttons.SetActive(false);

        // WHEN STEAM CREATES A LOBBY HERE IT WILL DO A CALLBACK 
        // THAT WILL BE CALLED BY OnLobbyCreated
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        // lobbyCreated Callback WILL STILL BE TRIGGERED EVEN IF LOBBY WASN'T ACTUALLY ABLE TO START
        // IF THE LOBBY DIDN'T START CORRECTLY TURN BUTTONS BACK TO ACTIVE
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            buttons.SetActive(true);
            return;
        }

        // START THE MIRROR SERVER AS HOST
        networkManager.StartHost();

        // THIS SETS THE DATA IN THE STEAM LOBBY TO BE OUR STEAM ID
        // BECAUSE WE HOSTED THE LOBBY
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HostAddressKey, 
            SteamUser.GetSteamID().ToString()
            );
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        // THIS IS WHERE WE COULD ADD EXTRA LOGIC FOR PLAYERS TRYING TO JOIN OUR LOBBY

        // RIGHT NOW THIS WILL JUST LET PLAYERS JOIN WITH NO VALIDATION
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        // IF THE SERVER IS ACTIVE ON THIS CLIENT, 
        // THEN THIS CLIENT IS THE HOST AND SHOULD RETURN
        if(NetworkServer.active) { return; }

        // IF THIS CLIENT IS A JOINER THEN ACCESS LOBBY DATA AND GET 
        // THE HOSTS HostAddressKey AND SAVE AS hostAddress
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);

        // WITH hostAddress SET AS THE APPRIOPRIATE STEAM ID HostAddressKey
        // SET IS AS MIRRORS' networkAddress AND JOIN AS A CLIENT
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        buttons.SetActive(false);
    }
}
