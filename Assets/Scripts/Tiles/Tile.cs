using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Tile : MonoBehaviour {

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Tile followingTile;
    [SerializeField] private TileDisplay display;
    [SerializeField] private TileDirection nextTileDirection;
    private TilePoint[] points;
    public Tile FollowingTile => followingTile;
    public TileDirection NextTileDirection => nextTileDirection;
    
    private void Awake() {
        InitializePoints();
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

    public GameObject MakeCorrectPreview() {
        var preview = Instantiate(this);
        preview.display.MakeCorrectPreview();
        return preview.gameObject;
    }
    
    public GameObject MakeWrongPreview() {
        var preview = Instantiate(this);
        preview.display.MakeWrongPreview();
        return preview.gameObject;
    }
}

public enum TileDirection {
    RIGHT,
    UP,
    DOWN
}

public class TilePoint {
    public Vector2 point;
    public bool isLast;
}
