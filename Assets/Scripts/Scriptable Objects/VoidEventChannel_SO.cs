using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is used for Events that have no argument parameters
/// </summary>
[CreateAssetMenu(menuName = "Events/Void Event Channel")]
public class VoidEventChannel_SO : ScriptableObject
{
    public UnityAction OnEventRaised;

    public void RaiseEvent() => OnEventRaised?.Invoke();
}