using System;
using System.Collections.Generic;
using Mimic;
using UnityEngine;

[Serializable]
public class TileDisplay {
    [SerializeField] private List<SpriteRenderer> spriteRenderers;
    
    public void MakeCorrectPreview() {
        foreach (var spriteRenderer in spriteRenderers) {
            spriteRenderer.color = spriteRenderer.color.GetColorWithAlpha(0.5f);
        }
    }

    public void MakeWrongPreview() {foreach (var spriteRenderer in spriteRenderers) {
            spriteRenderer.color = spriteRenderer.color.GetColorWithAlpha(0.5f) + Color.red*0.4f;
        }
    }
}