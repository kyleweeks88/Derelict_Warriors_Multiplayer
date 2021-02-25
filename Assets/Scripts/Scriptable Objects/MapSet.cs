using Mirror;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Set", menuName = "Map Sets")]
public class MapSet : ScriptableObject
{
    [Scene]
    [SerializeField] private List<string> maps = new List<string>();

    // THIS MAKES THE LIST READONLY SO WE CAN'T ACCIDENTALLY MODIFY THE LIST...???
    // READ MORE ABOUT THIS BECAUSE I DONT UNDERSTAND!!!
    public IReadOnlyCollection<string> Maps => maps.AsReadOnly();
}
