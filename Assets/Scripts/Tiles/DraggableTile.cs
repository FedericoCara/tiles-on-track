﻿using System;
using UnityEngine;

[RequireComponent(typeof(Draggable))]
[RequireComponent(typeof(SnapToGrid))]
[RequireComponent(typeof(TileFinder))]
public class DraggableTile :MonoBehaviour {

    public bool IsDragging => _draggable.Dragging;
    public event Action OnReturnToInitialPosition = () => {};
    public event Action OnTileDropped = () => {};

    [SerializeField] private Tile tilePrefab;
    private Draggable _draggable;
    private SnapToGrid _snapToGrid;
    private GameObject _previousPreview;
    private TileFinder _tileFinder;

    private void Awake() {
        _draggable = GetComponent<Draggable>();
        _snapToGrid = GetComponent<SnapToGrid>();
        _snapToGrid.OnCellChanged += DrawTilePreview;
        _snapToGrid.OnSnap += TileDropped;
        _tileFinder = GetComponent<TileFinder>();

        var startingPosition = transform.position;
        OnReturnToInitialPosition += () => {
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

        if(_previousPreview!=null)
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
            OnTileDropped();
        } else {
            OnReturnToInitialPosition();
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