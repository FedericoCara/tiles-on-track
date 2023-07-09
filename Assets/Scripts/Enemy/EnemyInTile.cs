using System;
using UnityEngine;

[Serializable]
public class EnemyInTile {
    [SerializeField] private int enemyPointIndex = -1;
    public int EnemyPointIndex => enemyPointIndex;
    [SerializeField] private int playerPointIndex;
    public int PlayerPointIndex => playerPointIndex = -1;
    [SerializeField] private Enemy enemyPrefab;
    public Enemy EnemyPrefab => enemyPrefab;
    public void SetEnemy(Enemy enemy) => enemyPrefab = enemy;
}