using System;
using UnityEngine;

[Serializable]
public class EnemyInTile {
    [SerializeField] private int enemyPointIndex = -1;
    public int EnemyPointIndex => enemyPointIndex;
    [SerializeField] private int playerPointIndex = -1;
    public int PlayerPointIndex => playerPointIndex;
    [SerializeField] private Enemy enemyPrefab;
    public Enemy EnemyPrefab => enemyPrefab;
    [SerializeField] private EnemyDisplay enemyDisplay;
    public EnemyDisplay EnemyDisplay => enemyDisplay;
    public void SetEnemy(Enemy enemy) => enemyPrefab = enemy;
}