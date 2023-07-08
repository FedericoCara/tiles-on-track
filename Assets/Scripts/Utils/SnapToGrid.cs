using System;
using UnityEngine;

[RequireComponent(typeof(Draggable))]
public class SnapToGrid : MonoBehaviour {

    public event Action OnSnap = () => { };
    public event Action OnCellChanged = () => { };

    private Draggable _draggable;
    private Grid _mainGrid;
    private Vector2Int currentCell;

    private void Awake() {
        _draggable = GetComponent<Draggable>();
        _draggable.OnDraggableFinished += Snap;
        _mainGrid = FindObjectOfType<Grid>();
    }

    private void Update() {
        if (_draggable.Dragging) {
            UpdateCurrentCell();
        }
    }

    public Vector2Int GetCurrentCellPosition() => (Vector2Int) _mainGrid.WorldToCell(transform.position);
    
    public Vector3 GetCurrentSnappingPosition() {
        var cell = GetCurrentCellPosition();
        return _mainGrid.GetCellCenterWorld((Vector3Int) cell);
    }

    private void Snap() {
        transform.position = GetCurrentSnappingPosition();
        OnSnap();
    }

    private void UpdateCurrentCell() {
        var actualCell = (Vector2Int)_mainGrid.WorldToCell(transform.position);
        if (actualCell != currentCell) {
            currentCell = actualCell;
            OnCellChanged();
        }
    }
}