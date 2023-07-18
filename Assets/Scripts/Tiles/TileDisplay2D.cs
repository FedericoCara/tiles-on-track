using System;
using System.Collections.Generic;
using Mimic;
using UnityEngine;

[Serializable]
public class TileDisplay2D : TileDisplayBase {
    [SerializeField] private List<SpriteRenderer> mainSpriteRenderers;
    [SerializeField] private List<SpriteRenderer> backgroundSpriteRenderers;
    [SerializeField] private string previewSortLayer = "Conveyor Belt";

    public override void MakeDraggablePreview(Transform previewParent, Enemy enemyPrefab) {
        var previewTileComponent = GameObject.Instantiate(tile, previewParent);
        var display2D = previewTileComponent.Display as TileDisplay2D;
        display2D.DestroyBackgroundSprites();
        display2D.ChangeSortLayer(previewSortLayer);
        if (previewTileComponent.HasEnemy) {
            SpawnEnemyPreviewIfNecessary(previewTileComponent, enemyPrefab);
            var enemyDisplay2D = previewTileComponent.EnemyInTile.EnemyDisplay as EnemyDisplay2D;
            enemyDisplay2D.ChangeSortLayer(previewSortLayer);
        }else if (previewTileComponent.HasPotion) {
            previewTileComponent.PotionInTile.ChangeSortLayer(previewSortLayer);
            Destroy(previewTileComponent.PotionInTile.Potion);
        }

        Destroy(previewTileComponent);
        Destroy(previewTileComponent.GetComponentInChildren<Collider2D>());
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

    public void ChangeSortLayer(string sortLayerName) {
        foreach (var spriteRenderer in mainSpriteRenderers) {
            spriteRenderer.sortingLayerName = sortLayerName;
        }
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            spriteRenderer.sortingLayerName = sortLayerName;
        }
    }

    private void DestroyBackgroundSprites() {
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            if (spriteRenderer != null && spriteRenderer.gameObject != null)
                Destroy(spriteRenderer.gameObject);
        }
        backgroundSpriteRenderers.Clear();
    }
}