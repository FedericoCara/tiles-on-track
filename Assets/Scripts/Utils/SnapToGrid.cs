using System;
using UnityEngine;

[RequireComponent(typeof(Draggable))]
public class SnapToGrid : MonoBehaviour {

    [SerializeField] private LevelConfiguration levelConfig;
    private Draggable _draggable;
    private Grid _mainGrid;

    private void Awake() {
        _draggable = GetComponent<Draggable>();
        _draggable.OnDraggableFinished += Snap;
        _mainGrid = FindObjectOfType<Grid>();
    }

    private void Snap() {
        var cell = _mainGrid.WorldToCell(transform.position);
        var newPosition = _mainGrid.GetCellCenterWorld(cell) + levelConfig.GridOffset;
        transform.position = newPosition;
        Debug.Log($"Snapping {name} to cell {cell} which is in world position {newPosition}");
    }
}