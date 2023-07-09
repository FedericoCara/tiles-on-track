using System.Collections.Generic;
using Mimic;
using UnityEngine;

public class ConveyorTileStrategy : MonoBehaviour {
    
    [SerializeField] private List<Tile> possibleTiles;
    [SerializeField] private List<Tile> possibleEnemyTiles;
    [SerializeField] private List<Tile> possiblePotionTiles;
    [SerializeField] private Tile finalBossTile;
    [SerializeField] private List<Enemy> possibleEnemies;
    [SerializeField] private float enemyFrequency = 0.3f;
    [SerializeField] private float potionFrequency = 0.1f;
    private List<Tile> _tilesToGive = new();
    private List<Tile> _enemyTilesToGive = new();
    private List<Tile> _potionTilesToGive = new();
    private float enemyFrequencyAccumulated = 0;
    private float potionFrequencyAccumulated = 0;
    private bool finalBossDelivered = false;

    public Tile CalculateTile(int playerLevel, float healthPercentage) {
        if (_tilesToGive.IsEmpty()) {
            _tilesToGive.AddRange(possibleTiles);
        }

        return  ShouldGiveEnemy(playerLevel) ?? ShouldGivePotion(healthPercentage) ??_tilesToGive.RemoveElementAtRandom();
    }

    private Tile ShouldGivePotion(float healthPercentage) {
        potionFrequencyAccumulated += potionFrequency * (2-healthPercentage);
        if (Random.value < potionFrequencyAccumulated) {
            potionFrequencyAccumulated = 0;
            if(_potionTilesToGive.IsEmpty())
                _potionTilesToGive.AddRange(possiblePotionTiles);

            return _potionTilesToGive.RemoveElementAtRandom();
        }

        return null;
    }

    private Tile ShouldGiveEnemy(int playerLevel) {
        enemyFrequencyAccumulated += enemyFrequency;
        if (Random.value < enemyFrequencyAccumulated) {
            enemyFrequencyAccumulated = 0;
            if(_enemyTilesToGive.IsEmpty())
                _enemyTilesToGive.AddRange(possibleEnemyTiles);
            var enemy = GetEnemy(playerLevel);
            
            if (enemy.IsFinalBoss) {
                finalBossDelivered = true;
                return finalBossTile;
            }

            var enemyTile = _enemyTilesToGive.RemoveElementAtRandom();
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