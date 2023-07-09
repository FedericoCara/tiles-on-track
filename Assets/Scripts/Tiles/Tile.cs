using System;
using System.Collections;
using System.Collections.Generic;
using Mimic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Tile : MonoBehaviour {

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Tile followingTile;
    [SerializeField] private TileDisplay display;
    [SerializeField] private TileDirection comingTileDirection;
    [SerializeField] private TileDirection nextTileDirection;
    [SerializeField] private bool hasEnemy = false;
    [SerializeField] private EnemyInTile enemyData;
    [SerializeField] private PotionInTile potionInTile;

    private TilePoint[] points;
    private Enemy _enemy;

    private TilePoint[] Points {
        get {
            if(points==null)
                InitializePoints();
            return points;
        }
    }
    public Tile FollowingTile => followingTile;
    public TileDirection ComingTileDirection => comingTileDirection;
    public TileDirection NextTileDirection => nextTileDirection;
    public bool HasEnemy => hasEnemy;

    public bool IsNextTileInReverse => FollowingTile != null &&
                                       (FollowingTile.IsReversable &&
                                        IsLastPointTheLastPointOfTheOtherTile() ||
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
    public bool HasPotion => potionInTile.Potion != null;

    public Enemy Enemy => _enemy;
    public Potion Potion => potionInTile.Potion;

    private void Start() {
        InitializePoints();
    }


    private void InitializePoints() {
        if(points!=null)
            return;
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        points = new TilePoint[lineRenderer.positionCount];
        for (int i = 0; i < lineRenderer.positionCount; i++) {
            points[i] = new TilePoint {
                point = lineRenderer.transform.TransformPoint(lineRenderer.GetPosition(i)),
                isLast = i == lineRenderer.positionCount - 1,
                isFirst = i == 0,
                stopsForEnemy = hasEnemy && i == enemyData.PlayerPointIndex,
                stopsForPotion = HasPotion && i == potionInTile.PlayerPointIndex
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
        SpawnEnemyPreviewIfNecessary(preview);
        return preview.gameObject;
    }

    public GameObject MakeWrongTilePreview() {
        var preview = Instantiate(this);
        preview.display.MakeWrongPreview();
        SpawnEnemyPreviewIfNecessary(preview);
        return preview.gameObject;
    }

    public void MakeDraggablePreview(Transform previewParent, string sortLayerName) {
        var previewTileComponent = Instantiate(this, previewParent);
        previewTileComponent.display.DestroyBackgroundSprites();
        previewTileComponent.display.ChangeSortLayer(sortLayerName);
        if (previewTileComponent.hasEnemy) {
            SpawnEnemyPreviewIfNecessary(previewTileComponent);
            previewTileComponent.enemyData.EnemyDisplay.ChangeSortLayer(sortLayerName);
        }else if (previewTileComponent.HasPotion) {
            previewTileComponent.potionInTile.ChangeSortLayer(sortLayerName);
            Destroy(previewTileComponent.potionInTile.Potion);
        }

        Destroy(previewTileComponent);
        Destroy(previewTileComponent.GetComponentInChildren<Collider2D>());
    }


    private void SpawnEnemyPreviewIfNecessary(Tile previewTileComponent) {
        if (previewTileComponent.hasEnemy) {
            var enemyPreviewComponent = previewTileComponent.SpawnEnemy();
            Destroy(enemyPreviewComponent);
        }
    }

    public Enemy SpawnEnemy() {
        _enemy = Instantiate(enemyData.EnemyPrefab, transform);
        _enemy.transform.position = Points[enemyData.EnemyPointIndex].point;
        enemyData.EnemyDisplay.SetEnemy(_enemy);
        return _enemy;
    }

    public bool CanConnectWith(TileDirection otherTileEntryDirection, TileDirection otherTilePutDirection, bool otherTileIsReversable) =>
        followingTile == null &&
        (AreOpposite(NextTileDirection,otherTileEntryDirection) && NextTileDirection == otherTilePutDirection ||
         IsReversable && AreOpposite(ComingTileDirection, otherTileEntryDirection) && ComingTileDirection == otherTilePutDirection ||
         otherTileIsReversable && NextTileDirection == otherTileEntryDirection && NextTileDirection == otherTilePutDirection ||
         IsReversable && otherTileIsReversable && ComingTileDirection == otherTileEntryDirection && ComingTileDirection == otherTilePutDirection);

    public void SetEnemy(Enemy enemy) {
        enemyData.SetEnemy(enemy);
    }

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

[Serializable]
public class TilePoint {
    public Vector2 point;
    public bool isLast;
    public bool isFirst;
    public bool stopsForEnemy;
    public bool stopsForPotion;
}
