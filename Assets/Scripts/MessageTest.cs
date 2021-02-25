using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;


public struct Notification : NetworkMessage
{
    public string content;
}


public class MessageTest : MonoBehaviour
{
    [SerializeField] private TMP_Text notificationsText = null;

    private void Start()
    {
        if(!NetworkClient.active) { return; }

        NetworkClient.RegisterHandler<Notification>(OnNotification); 
    }

    private void OnNotification(NetworkConnection conn, Notification msg)
    {
        notificationsText.text += $"\n{msg.content}";
    }
}
