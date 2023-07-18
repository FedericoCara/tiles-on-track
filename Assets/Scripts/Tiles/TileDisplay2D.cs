using System;
using System.Collections.Generic;
using Mimic;
using UnityEngine;

[Serializable]
public class TileDisplay2D : TileDisplayBase {
    [SerializeField] private List<SpriteRenderer> mainSpriteRenderers;
    [SerializeField] private List<SpriteRenderer> backgroundSpriteRenderers;
    private const string previewSortLayer = "Conveyor Belt";

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

    public void DestroyBackgroundSprites() {
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            if (spriteRenderer != null && spriteRenderer.gameObject != null)
                Destroy(spriteRenderer.gameObject);
        }
        backgroundSpriteRenderers.Clear();
    }

    internal override void SetAsDraggablePreview() {
        DestroyBackgroundSprites();
        ChangeSortLayer(previewSortLayer);
    }
}