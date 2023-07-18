using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyDisplay2D : EnemyDisplayBase {
    [SerializeField] private List<SpriteRenderer> mainSpriteRenderers;
    private const string previewSortLayer = "Conveyor Belt";
    
    public void ChangeSortLayer(string sortLayerName) {
        foreach (var spriteRenderer in mainSpriteRenderers) {
            spriteRenderer.sortingLayerName = sortLayerName;
        }
    }

    public override Enemy SpawnEnemyPreview(Enemy enemyPrefab, Vector2 enemyPosition,  Transform parent) {
        var _enemy = Instantiate(enemyPrefab, parent);
        _enemy.transform.position = enemyPosition;
        _enemy.transform.localPosition = (Vector2)_enemy.transform.localPosition;
        _enemy.EnemyFightDisplay.Hide();
        return _enemy;
    }

    internal override void SetAsDraggablePreview() {
        ChangeSortLayer(previewSortLayer);
    }
}