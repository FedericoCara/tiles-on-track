using UnityEngine;

public class TileDisplay3D : TileDisplayBase {
    public override void MakeDraggablePreview(Transform previewParent, Enemy enemyPrefab) {
        var previewTileComponent = Instantiate(tile, previewParent);
        var display3D = previewTileComponent.Display as TileDisplay3D;
        //display3D.DestroyBackgroundSprites();
        //display3D.ChangeSortLayer(previewSortLayer);
        if (previewTileComponent.HasEnemy) {
            SpawnEnemyPreviewIfNecessary(previewTileComponent, enemyPrefab);
            var enemyDisplay2D = previewTileComponent.EnemyInTile.EnemyDisplay as EnemyDisplay2D;
            //enemyDisplay2D.ChangeSortLayer(previewSortLayer);
        }else if (previewTileComponent.HasPotion) {
            //previewTileComponent.PotionInTile.ChangeSortLayer(previewSortLayer);
            Destroy(previewTileComponent.PotionInTile.Potion);
        }

        Destroy(previewTileComponent);
        Destroy(previewTileComponent.GetComponentInChildren<Collider2D>());
    }

    public override void MakeCorrectPreview() {
        
    }

    public override void MakeWrongPreview() {
    
    }
}