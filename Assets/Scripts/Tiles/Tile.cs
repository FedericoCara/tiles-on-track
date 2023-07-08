using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Tile followingTile;

    private TilePoint[] points;
    
    private void Awake() {
        InitializePoints();
    }

    public Tile GetFollowingTile() {
        return followingTile;
    }

    public TilePoint GetNextPoint(int currentIndex) {
        if (currentIndex + 1 >= lineRenderer.positionCount)
            return null;
        return points[currentIndex + 1];
    }

    private void InitializePoints() {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        points = new TilePoint[lineRenderer.positionCount];
        for (int i = 0; i < lineRenderer.positionCount; i++) {
            points[i] = new TilePoint {
                point = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(i)),
                isLast = i == lineRenderer.positionCount - 1
            };
        }
    }
}

public class TilePoint {
    public Vector2 point;
    public bool isLast;
}
