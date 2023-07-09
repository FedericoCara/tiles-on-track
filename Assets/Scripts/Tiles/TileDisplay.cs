using System;
using System.Collections.Generic;
using Mimic;
using UnityEngine;

[Serializable]
public class TileDisplay {
    [SerializeField] private List<SpriteRenderer> mainSpriteRenderers;
    [SerializeField] private List<SpriteRenderer> backgroundSpriteRenderers;
    
    public void MakeCorrectPreview() {
        foreach (var spriteRenderer in mainSpriteRenderers) {
            spriteRenderer.color = spriteRenderer.color.GetColorWithAlpha(0.5f);
        }
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            spriteRenderer.color = spriteRenderer.color.GetColorWithAlpha(0.5f);
        }
    }

    public void MakeWrongPreview() {
        foreach (var spriteRenderer in mainSpriteRenderers) {
            spriteRenderer.color = spriteRenderer.color.GetColorWithAlpha(0.5f) + Color.red*0.4f;
        }
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            spriteRenderer.color = spriteRenderer.color.GetColorWithAlpha(0.5f) + Color.red*0.4f;
        }
    }

    public void DestroyBackgroundSprites() {
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            if (spriteRenderer != null && spriteRenderer.gameObject != null)
                GameObject.Destroy(spriteRenderer.gameObject);
        }
        backgroundSpriteRenderers.Clear();
    }

    public void ChangeSortLayer(string sortLayerName) {
        foreach (var spriteRenderer in mainSpriteRenderers) {
            spriteRenderer.sortingLayerName = sortLayerName;
        }
        foreach (var spriteRenderer in backgroundSpriteRenderers) {
            spriteRenderer.sortingLayerName = sortLayerName;
        }
    }
}