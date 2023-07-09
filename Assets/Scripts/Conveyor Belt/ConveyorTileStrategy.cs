using System.Collections.Generic;
using UnityEngine;

public class ConveyorTileStrategy : MonoBehaviour {
    public Tile CalculateTile(List<Tile> possibleTiles) {
        return possibleTiles[Random.Range(0, possibleTiles.Count - 1)];
    }
}