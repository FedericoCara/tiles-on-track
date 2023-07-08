using System.Collections.Generic;
using UnityEngine;

public class ConveyorTileStrategy : MonoBehaviour {
    public DraggableTile CalculateDraggable(List<DraggableTile> possibleDraggables) {
        return possibleDraggables[Random.Range(0, possibleDraggables.Count - 1)];
    }
}