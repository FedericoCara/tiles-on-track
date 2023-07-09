using System.Collections.Generic;
using Mimic;
using UnityEngine;

public class ConveyorTileStrategy : MonoBehaviour {
    
    [SerializeField] private List<Tile> possibleTiles;
    [SerializeField] private List<Tile> possibleEnemyTiles;
    [SerializeField] private Tile finalBossTile;
    [SerializeField] private List<Enemy> possibleEnemies;
    [SerializeField] private float enemyFrequency = 0.3f;
    private List<Tile> tilesToGive = new();
    private List<Tile> enemyTilesToGive = new();
    private float enemyFrequencyAccumulated = 0;
    private bool finalBossDelivered = false;

    public Tile CalculateTile(int playerLevel) {
        if (tilesToGive.IsEmpty()) {
            tilesToGive.AddRange(possibleTiles);
        }

        Tile enemyTile = ShouldGiveEnemy(playerLevel);
        
        return enemyTile ?? tilesToGive.RemoveElementAtRandom();
    }

    private Tile ShouldGiveEnemy(int playerLevel) {
        enemyFrequencyAccumulated += enemyFrequency;
        if (Random.value < enemyFrequencyAccumulated) {
            enemyFrequencyAccumulated = 0;
            if(enemyTilesToGive.IsEmpty())
                enemyTilesToGive.AddRange(possibleEnemyTiles);
            var enemy = GetEnemy(playerLevel);
            
            if (enemy.IsFinalBoss) {
                finalBossDelivered = true;
                return finalBossTile;
            }

            var enemyTile = enemyTilesToGive.RemoveElementAtRandom();
            enemyTile.SetEnemy(enemy);
            return enemyTile;
        }

        return null;
    }

    private Enemy GetEnemy(int playerLevel) {
        Enemy highestLevelEnemyResistedByPlayer = null;
        foreach (var enemy in possibleEnemies) {
            if (enemy.MinPlayerLevel <= playerLevel &&
                (highestLevelEnemyResistedByPlayer == null ||
                 highestLevelEnemyResistedByPlayer.MinPlayerLevel < enemy.MinPlayerLevel && 
                 (!enemy.IsFinalBoss||!finalBossDelivered) )) {
                highestLevelEnemyResistedByPlayer = enemy;
            }
        }

        return highestLevelEnemyResistedByPlayer;
    }
}