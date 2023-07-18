using UnityEngine;

public abstract class TileDisplayBase : MonoBehaviour {
    
    [SerializeField] protected Tile tile;
    
    public abstract void MakeDraggablePreview(Transform previewParent, Enemy enemyPrefab);
    public abstract void MakeCorrectPreview();
    public abstract void MakeWrongPreview();


    public virtual GameObject MakeCorrectTilePreview(Enemy enemyPrefab) {
        var preview = Instantiate<Tile>(tile);
        preview.Display.MakeCorrectPreview();
        SpawnEnemyPreviewIfNecessary(preview, enemyPrefab);
        return preview.gameObject;
    }

    public virtual GameObject MakeWrongTilePreview(Enemy enemyPrefab) {
        var preview = Instantiate<Tile>(tile);
        preview.Display.MakeWrongPreview();
        SpawnEnemyPreviewIfNecessary(preview, enemyPrefab);
        return preview.gameObject;
    }

    protected void SpawnEnemyPreviewIfNecessary(Tile previewTileComponent, Enemy enemyPrefab) {
        if (previewTileComponent.HasEnemy) {
            var enemyInTile = previewTileComponent.EnemyInTile;
            enemyInTile.SetEnemy(enemyPrefab);
            var enemyPosition = previewTileComponent.GetNextPointPosition(enemyInTile.EnemyPointIndex);
            var enemyPreviewComponent =  enemyInTile.EnemyDisplay.SpawnEnemyPreview(enemyPrefab, enemyPosition, previewTileComponent.transform);
            Destroy(enemyPreviewComponent);
        }
    }
}