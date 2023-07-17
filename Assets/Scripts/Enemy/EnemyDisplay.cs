using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyDisplay {
    [SerializeField] private List<SpriteRenderer> mainSpriteRenderers;
    
    public void ChangeSortLayer(string sortLayerName) {
        foreach (var spriteRenderer in mainSpriteRenderers) {
            spriteRenderer.sortingLayerName = sortLayerName;
        }
    }

    public void SetEnemy(Enemy enemy) {
        mainSpriteRenderers.AddRange(enemy.GetComponentsInChildren<SpriteRenderer>());
    }

    public Enemy SpawnEnemyPreview(Enemy enemyPrefab, Vector2 enemyPosition,  Transform parent) {
        var _enemy = Enemy.Instantiate(enemyPrefab, parent);
        _enemy.transform.position = enemyPosition;
        SetEnemy(_enemy);
        _enemy.EnemyFightDisplay.Hide();
        return _enemy;
    }
}