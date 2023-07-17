using System;
using System.Collections.Generic;
using Mimic;
using UnityEngine;

[Serializable]
public class TileDisplay2D : TileDisplayBase {
    [SerializeField] private List<SpriteRenderer> mainSpriteRenderers;
    [SerializeField] private List<SpriteRenderer> backgroundSpriteRenderers;
    [SerializeField] private Tile tile;
    
    public override GameObject MakeCorrectTilePreview() {
        var preview = GameObject.Instantiate(tile);
        preview.Display.MakeCorrectPreview();
        SpawnEnemyPreviewIfNecessary(preview);
        return preview.gameObject;
    }

    public override GameObject MakeWrongTilePreview() {
        var preview = GameObject.Instantiate(tile);
        preview.Display.MakeWrongPreview();
        SpawnEnemyPreviewIfNecessary(preview);
        return preview.gameObject;
    }

    public override void MakeDraggablePreview(Transform previewParent, string sortLayerName) {
        var previewTileComponent = GameObject.Instantiate(tile, previewParent);
        previewTileComponent.Display.DestroyBackgroundSprites();
        previewTileComponent.Display.ChangeSortLayer(sortLayerName);
        if (previewTileComponent.HasEnemy) {
            SpawnEnemyPreviewIfNecessary(previewTileComponent);
            previewTileComponent.EnemyInTile.EnemyDisplay.ChangeSortLayer(sortLayerName);
        }else if (previewTileComponent.HasPotion) {
            previewTileComponent.PotionInTile.ChangeSortLayer(sortLayerName);
            GameObject.Destroy(previewTileComponent.PotionInTile.Potion);
        }

        GameObject.Destroy(previewTileComponent);
        GameObject.Destroy(previewTileComponent.GetComponentInChildren<Collider2D>());
    }


    private void SpawnEnemyPreviewIfNecessary(Tile previewTileComponent) {
        if (previewTileComponent.HasEnemy) {
            var enemyInTile = previewTileComponent.EnemyInTile;
            var enemyPosition = previewTileComponent.GetNextPointPosition(enemyInTile.EnemyPointIndex);
            var enemyPreviewComponent =  enemyInTile.EnemyDisplay.SpawnEnemyPreview(enemyInTile.EnemyPrefab, enemyPosition, previewTileComponent.transform);
            GameObject.Destroy(enemyPreviewComponent);
        }
    }
    
    public override void MakeCorrectPreview() {
        foreach (var spriteRenderer in mainSpriteRenderers) {
            spriteRenderer.color = spriteRenderer.color.GetColorWithAlpha(0.5f);
        }
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            spriteRenderer.color = spriteRenderer.color.GetColorWithAlpha(0.5f);
        }
    }

    public override void MakeWrongPreview() {
        foreach (var spriteRenderer in mainSpriteRenderers) {
            spriteRenderer.color = spriteRenderer.color.GetColorWithAlpha(0.5f) + Color.red*0.4f;
        }
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            spriteRenderer.color = spriteRenderer.color.GetColorWithAlpha(0.5f) + Color.red*0.4f;
        }
    }

    public override void DestroyBackgroundSprites() {
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            if (spriteRenderer != null && spriteRenderer.gameObject != null)
                GameObject.Destroy(spriteRenderer.gameObject);
        }
        backgroundSpriteRenderers.Clear();
    }

    public override void ChangeSortLayer(string sortLayerName) {
        foreach (var spriteRenderer in mainSpriteRenderers) {
            spriteRenderer.sortingLayerName = sortLayerName;
        }
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            spriteRenderer.sortingLayerName = sortLayerName;
        }
    }
}