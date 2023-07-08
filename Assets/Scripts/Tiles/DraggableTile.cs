using System;
using UnityEngine;

[RequireComponent(typeof(SnapToGrid))]
[RequireComponent(typeof(TileFinder))]
public class DraggableTile :MonoBehaviour {

    [SerializeField] private Tile tilePrefab;
    private SnapToGrid _snapToGrid;
    private GameObject _previousPreview;
    private TileFinder _tileFinder;

    private void Awake() {
        _snapToGrid = GetComponent<SnapToGrid>();
        _snapToGrid.OnCellChanged += DrawTilePreview;
        _tileFinder = GetComponent<TileFinder>();
    }

    private void DrawTilePreview() {
        DestroyPreviousPreviewIfNecessary();

        var cell = _snapToGrid.GetCurrentCellPosition();
        var connections = _tileFinder.GetConnections(cell);
        if (connections.Center == null) {
            if(connections.Down!=null && connections.Down.FollowingTile==null && connections.Down.NextTileDirection==TileDirection.UP ||
               connections.Left!=null && connections.Left.FollowingTile==null && connections.Left.NextTileDirection==TileDirection.RIGHT ||
               connections.Up!=null && connections.Up.FollowingTile==null && connections.Up.NextTileDirection==TileDirection.DOWN)
                _previousPreview = tilePrefab.MakeCorrectPreview();
            else {
                _previousPreview = tilePrefab.MakeWrongPreview();
            }
        }

        _previousPreview.transform.position = _snapToGrid.GetCurrentSnappingPosition();
    }

    private void OnDestroy() {
        DestroyPreviousPreviewIfNecessary();
    }

    private void DestroyPreviousPreviewIfNecessary() {
        if (_previousPreview != null)
            Destroy(_previousPreview);
    }
}