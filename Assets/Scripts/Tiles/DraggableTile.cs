using System;
using UnityEngine;

[RequireComponent(typeof(SnapToGrid))]
[RequireComponent(typeof(TileFinder))]
public class DraggableTile :MonoBehaviour {

    public event Action ReturnToInitialPosition = () => {};
    
    [SerializeField] private Tile tilePrefab;
    private SnapToGrid _snapToGrid;
    private GameObject _previousPreview;
    private TileFinder _tileFinder;

    private void Awake() {
        _snapToGrid = GetComponent<SnapToGrid>();
        _snapToGrid.OnCellChanged += DrawTilePreview;
        _snapToGrid.OnSnap += TileDropped;
        _tileFinder = GetComponent<TileFinder>();

        var startingPosition = transform.position;
        ReturnToInitialPosition += () => {
            transform.position = startingPosition;
        };
    }

    private void DrawTilePreview() {
        DestroyPreviousPreviewIfNecessary();

        var cell = _snapToGrid.GetCurrentCellPosition();
        var connections = _tileFinder.GetConnections(cell);
        if (connections.Center == null) {
            if (connections.GetCorrectConnection()!=null) {
                _previousPreview = tilePrefab.MakeCorrectPreview();
            } else {
                _previousPreview = tilePrefab.MakeWrongPreview();
            }
        }

        _previousPreview.transform.position = _snapToGrid.GetCurrentSnappingPosition();
    }

    private void TileDropped() {
        DestroyPreviousPreviewIfNecessary();

        var cell = _snapToGrid.GetCurrentCellPosition();
        var connections = _tileFinder.GetConnections(cell);
        var correctConnection = connections.GetCorrectConnection();
        
        if (correctConnection!=null) {
            var grid = FindObjectOfType<Grid>();
            Tile newTile = Instantiate(tilePrefab, grid.transform);
            newTile.transform.position = _snapToGrid.GetCurrentSnappingPosition();
            correctConnection.SetFollowingTile(newTile);
            Destroy(gameObject);
        } else {
            ReturnToInitialPosition?.Invoke();
        }
    }

    private void DestroyPreviousPreviewIfNecessary() {
        if (_previousPreview != null)
            Destroy(_previousPreview);
    }

    private void OnDestroy() {
        DestroyPreviousPreviewIfNecessary();
    }
}