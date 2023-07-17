using System.Collections.Generic;
using Mimic;
using UnityEngine;

public class ConveyorTileStrategy : MonoBehaviour {
    
    [SerializeField] private List<Tile> possibleTiles;
    [SerializeField] private List<Tile> possibleEnemyTiles;
    [SerializeField] private List<Tile> possiblePotionTiles;
    [SerializeField] private Tile finalBossTile;
    [SerializeField] private Enemy finalBoss;
    [SerializeField] private List<Enemy> possibleEnemies;
    [SerializeField] private float enemyFrequency = 0.3f;
    [SerializeField] private float potionFrequency = 0.1f;
    private List<Tile> _tilesToGive = new();
    private List<Tile> _enemyTilesToGive = new();
    private List<Tile> _potionTilesToGive = new();
    private float enemyFrequencyAccumulated = 0;
    private float potionFrequencyAccumulated = 0;
    private bool finalBossDelivered = false;

    public TileSpawnData CalculateTile(int playerLevel, float healthPercentage) {
        if (_tilesToGive.IsEmpty()) {
            _tilesToGive.AddRange(possibleTiles);
        }

        var potionTile = ShouldGivePotion(healthPercentage);
        if (!potionTile.IsEmpty)
            return potionTile;

        var enemyTile = ShouldGiveEnemy(playerLevel);
        if (!enemyTile.IsEmpty)
            return enemyTile;

        return GiveNeutralTile();
    }

    private TileSpawnData GiveNeutralTile() => new TileSpawnData(_tilesToGive.RemoveElementAtRandom());

    private TileSpawnData ShouldGivePotion(float healthPercentage) {
        potionFrequencyAccumulated += potionFrequency * (2-healthPercentage);
        if (Random.value < potionFrequencyAccumulated) {
            potionFrequencyAccumulated = 0;
            if(_potionTilesToGive.IsEmpty())
                _potionTilesToGive.AddRange(possiblePotionTiles);

            return new TileSpawnData(_potionTilesToGive.RemoveElementAtRandom());
        }

        return TileSpawnData.Empty();
    }

    private TileSpawnData ShouldGiveEnemy(int playerLevel) {
        enemyFrequencyAccumulated += enemyFrequency;
        if (Random.value < enemyFrequencyAccumulated) {
            enemyFrequencyAccumulated = 0;
            if(_enemyTilesToGive.IsEmpty())
                _enemyTilesToGive.AddRange(possibleEnemyTiles);
            var enemy = GetEnemy(playerLevel);
            
            if (enemy.IsFinalBoss) {
                finalBossDelivered = true;
                return new TileSpawnData(finalBossTile, finalBoss);
            }

            var enemyTilePrefab = _enemyTilesToGive.RemoveElementAtRandom();
            return new TileSpawnData(enemyTilePrefab, enemy);
        }

        return TileSpawnData.Empty();
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