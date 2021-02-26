using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;

    [SyncVar(hook = nameof(OnNameUpdated))]
    public string synchronizedName = string.Empty;


    /// <summary>
    /// Sets the player name for owner client with authority.
    /// This function is currently called from ClientInstance when
    /// a new player character object is spawned.
    /// </summary>
    /// <param name="name"></param>
    [Client]
    public void SetName(string name)
    {
        CmdSetName(name);
    }

    /// <summary>
    /// Sets the name for this character on the server
    /// and for the rest of the joined clients.
    /// </summary>
    /// <param name="name"></param>
    [Command]
    void CmdSetName(string name)
    {
        synchronizedName = name;
    }

    /// <summary>
    /// SyncVar hook for synchronizedName
    /// </summary>
    void OnNameUpdated(string oldString, string newString)
    {
        if (hasAuthority)
            nameText.gameObject.SetActive(false);

        nameText.text = newString;
    }
}
