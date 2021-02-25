using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler
{
    // THIS LIST IS NEVER MODIFIED IT JUST GETS SET IN THE Constructor AND STAYS THERE 
    private readonly IReadOnlyCollection<string> maps;
    private readonly int numberOfRounds;

    private int currentRound;
    // THIS LIST IS DYNAMIC AND REMOVES MAPS AS WE PLAY THROUGH ROUNDS
    // WILL BE USED TO DETERMINE WHEN THE GAME IS DONE OR WHEN TO GET A NEW MAPSET
    private List<string> remainingMaps;

    public MapHandler(MapSet mapSet, int numberOfRounds)
    {
        // GRABS THE MAP FROM THE mapSet AND "STORES" THEM
        // THIS IS STILL UNCLEAR TO ME...
        maps = mapSet.Maps;
        this.numberOfRounds = numberOfRounds;

        ResetMaps();
    }

    // CHECKS IF THE GAME IS COMPLETE BY COMPARING currentRound TO numberOfRounds
    public bool IsComplete => currentRound == numberOfRounds;

    public string NextMap()
    {
        if(IsComplete) { return null; }

        currentRound++;

        if(remainingMaps.Count == 0) { ResetMaps(); }

        string map = remainingMaps[UnityEngine.Random.Range(0, remainingMaps.Count)];

        remainingMaps.Remove(map);

        return map;
    }

    private void ResetMaps() => remainingMaps = maps.ToList();
}
