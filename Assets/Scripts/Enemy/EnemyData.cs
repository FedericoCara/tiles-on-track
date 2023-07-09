using UnityEngine;

[CreateAssetMenu(fileName = "Enemy data")]
public class EnemyData : ScriptableObject {
    [SerializeField] private int enemyPointIndex;
    [SerializeField] private int playerPointIndex;
    [SerializeField] private Enemy enemyPrefab;
}