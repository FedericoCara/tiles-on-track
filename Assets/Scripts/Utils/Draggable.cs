using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour {
    private bool _dragging = false;
    public bool Dragging => _dragging;
    private Vector2 _offset;
    public event Action OnDraggableFinished = () => { };
    
    void Update() {
        if (_dragging)
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector2)_offset;
    }

    private void OnMouseDown() {
        _offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _dragging = true;
    }

    private void OnMouseUp() {
        _dragging = false;
        OnDraggableFinished();
    }
}
