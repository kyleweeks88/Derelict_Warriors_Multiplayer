using UnityEngine;
using UnityEngine.Events;
using Mirror;

/// <summary>
/// This class is used for Events that take in NetworkConnections like 
/// disconnecting a client from the server.
/// </summary>
[CreateAssetMenu(menuName = "Events/NetworkConnection Event Channel")]
public class NetworkConnectionEventChannel_SO : ScriptableObject
{
    public UnityAction<NetworkConnection> OnEventRaised;

    public void RaiseEvent(NetworkConnection conn)
    {
        OnEventRaised?.Invoke(conn);
    }
}