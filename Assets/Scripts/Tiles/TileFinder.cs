using System;
using UnityEngine;

public class TileFinder : MonoBehaviour {

    private Grid _mainGrid;

    private void Awake() {
        _mainGrid = FindObjectOfType<Grid>();
    }

    public TileConnections GetConnections(Vector2Int cell) {
        return new TileConnections(GetTile(cell),
                                    GetTile(cell+Vector2Int.up),
                                    GetTile(cell+Vector2Int.left),
                                    GetTile(cell+Vector2Int.right),
                                    GetTile(cell+Vector2Int.down));
    }

    private Tile GetTile(Vector2Int cell) {
        var overlappingCollider = Physics2D.OverlapPoint(_mainGrid.GetCellCenterWorld((Vector3Int) cell));
        Tile tileFound;
        if (overlappingCollider != null && (tileFound = overlappingCollider.GetComponent<Tile>()) != null)
            return tileFound;
        return null;
    }

}