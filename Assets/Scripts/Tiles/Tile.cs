using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Tile : MonoBehaviour {

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Tile followingTile;
    [SerializeField] private TileDisplay display;
    [SerializeField] private TileDirection comingTileDirection;
    [SerializeField] private TileDirection nextTileDirection;
    private TilePoint[] points;
    public Tile FollowingTile => followingTile;
    public TileDirection ComingTileDirection => comingTileDirection;
    public TileDirection NextTileDirection => nextTileDirection;

    public bool IsNextTileInReverse => FollowingTile != null &&
                                       (FollowingTile.IsReversable &&
                                        IsLastPointTheLastPointOfTheOtherTile() ||
                                        IsReversable && !FollowingTile.IsReversable &&
                                        IsFirstPointTheFirstPointOfTheOtherTile() ||
                                        IsReversable && FollowingTile.IsReversable &&
                                        IsFirstPointTheLastPointOfTheOtherTile());

    private bool IsLastPointTheLastPointOfTheOtherTile() =>
        Vector2.SqrMagnitude(GetLastPointPosition() - FollowingTile.GetLastPointPosition()) < 0.05f;
    private bool IsFirstPointTheFirstPointOfTheOtherTile() =>
        Vector2.SqrMagnitude(GetFirstPointPosition() - FollowingTile.GetFirstPointPosition()) < 0.05f;
    private bool IsFirstPointTheLastPointOfTheOtherTile() =>
        Vector2.SqrMagnitude(GetFirstPointPosition() - FollowingTile.GetLastPointPosition()) < 0.05f;

    private Vector2 GetLastPointPosition() => points[^1].point;
    private Vector2 GetFirstPointPosition() => points[0].point;


    public bool IsReversable => comingTileDirection == TileDirection.UP && nextTileDirection == TileDirection.DOWN ||
                                comingTileDirection == TileDirection.DOWN && nextTileDirection == TileDirection.UP;
    
    private void Start() {
        InitializePoints();
    }


    private void InitializePoints() {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        points = new TilePoint[lineRenderer.positionCount];
        for (int i = 0; i < lineRenderer.positionCount; i++) {
            points[i] = new TilePoint {
                point = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(i)),
                isLast = i == lineRenderer.positionCount - 1,
                isFirst = i == 0
            };
        }
    }

    public TilePoint GetNextPoint(int currentIndex) {
        if (currentIndex + 1 >= lineRenderer.positionCount)
            return null;
        return points[currentIndex + 1];
    }

    public TilePoint GetPreviousPoint(int currentIndex) {
        if (currentIndex == 0)
            return null;
        return points[currentIndex - 1];
    }

    public void SetFollowingTile(Tile tile) {
        if (followingTile == null)
            followingTile = tile;
        else
            Debug.LogError("Following tile already set");
    }

    public GameObject MakeCorrectTilePreview() {
        var preview = Instantiate(this);
        preview.display.MakeCorrectPreview();
        return preview.gameObject;
    }

    public GameObject MakeWrongTilePreview() {
        var preview = Instantiate(this);
        preview.display.MakeWrongPreview();
        return preview.gameObject;
    }

    public void MakeDraggablePreview(Transform previewParent, string sortLayerName) {
        var preview = Instantiate(this, previewParent);
        preview.display.DestroyBackgroundSprites();
        preview.display.ChangeSortLayer(sortLayerName);
        Destroy(preview);
        Destroy(preview.GetComponentInChildren<Collider2D>());
    }

    public bool CanConnectWith(TileDirection otherTileEntryDirection, TileDirection otherTilePutDirection, bool otherTileIsReversable) =>
        followingTile == null &&
        (AreOpposite(NextTileDirection,otherTileEntryDirection) && NextTileDirection == otherTilePutDirection ||
         IsReversable && AreOpposite(ComingTileDirection, otherTileEntryDirection) && ComingTileDirection == otherTilePutDirection ||
         otherTileIsReversable && NextTileDirection == otherTileEntryDirection && NextTileDirection == otherTilePutDirection ||
         IsReversable && otherTileIsReversable && ComingTileDirection == otherTileEntryDirection && ComingTileDirection == otherTilePutDirection);

    private bool AreOpposite(TileDirection direction1, TileDirection direction2) =>
        direction1 == TileDirection.UP && direction2 == TileDirection.DOWN ||
        direction1 == TileDirection.DOWN && direction2 == TileDirection.UP ||
        direction1 == TileDirection.LEFT && direction2 == TileDirection.RIGHT ||
        direction1 == TileDirection.RIGHT && direction2 == TileDirection.LEFT;

    public int GetLastPointIndex() => points.Length - 1;
}

public enum TileDirection {
    RIGHT,
    UP,
    DOWN,
    LEFT
}

public class TilePoint {
    public Vector2 point;
    public bool isLast;
    public bool isFirst;
}
