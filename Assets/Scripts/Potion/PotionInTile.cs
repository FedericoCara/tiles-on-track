using System;
using UnityEngine;

[Serializable]
public class PotionInTile : MonoBehaviour {
    
    [SerializeField] private int playerPointIndex = -1;
    public int PlayerPointIndex => playerPointIndex;
    
    [SerializeField] private Potion potion;
    public Potion Potion => potion;
    private const string previewSortLayer = "Conveyor Belt";

    public void ChangeSortLayer() {
        foreach (var spriteRenderer in Potion.GetComponentsInChildren<SpriteRenderer>()) {
            spriteRenderer.sortingLayerName = previewSortLayer;
        }
    }

    public void SetAsDraggablePreview() {
        ChangeSortLayer();
        Destroy(Potion);
    }
}