using System;
using UnityEngine;

[Serializable]
public class EnemyInTile : MonoBehaviour{
    [SerializeField] private int enemyPointIndex = -1;
    public int EnemyPointIndex => enemyPointIndex;
    [SerializeField] private int playerPointIndex = -1;
    public int PlayerPointIndex => playerPointIndex;
    [SerializeField] private Enemy enemyPrefab;
    public Enemy EnemyPrefab => enemyPrefab;
    public EnemyDisplayBase EnemyDisplay => enemyPrefab.EnemyDisplay;
    public void SetEnemy(Enemy enemy) => enemyPrefab = enemy;
}