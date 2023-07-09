using System.Collections.Generic;
using Mimic;
using UnityEngine;

public class ConveyorTileStrategy : MonoBehaviour {

    private List<Tile> tilesToGive = new();
    public Tile CalculateTile(List<Tile> possibleTiles) {
        if (tilesToGive.IsEmpty()) {
            tilesToGive.AddRange(possibleTiles);
        }
        
        return tilesToGive.RemoveElementAtRandom();
    }
}