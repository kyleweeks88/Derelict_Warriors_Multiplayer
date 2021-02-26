using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerStats : CharacterStats
{
    PlayerName playerName;


    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        playerName = GetComponent<PlayerName>();
        base.charName = playerName.synchronizedName;
    }
}
