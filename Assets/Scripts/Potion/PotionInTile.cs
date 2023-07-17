using System;
using UnityEngine;

[Serializable]
public class PotionInTile : MonoBehaviour {
    
    [SerializeField] private int playerPointIndex = -1;
    public int PlayerPointIndex => playerPointIndex;
    
    [SerializeField] private Potion potion;
    public Potion Potion => potion;

    public void ChangeSortLayer(string sortLayerName) {
        foreach (var spriteRenderer in Potion.GetComponentsInChildren<SpriteRenderer>()) {
            spriteRenderer.sortingLayerName = sortLayerName;
        }
    }
}