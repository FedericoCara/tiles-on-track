using UnityEngine;

public abstract class EnemyDisplayBase : MonoBehaviour {
    public abstract Enemy SpawnEnemyPreview(Enemy enemyPrefab, Vector2 enemyPosition,  Transform parent);
}