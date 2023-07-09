using UnityEngine;

[CreateAssetMenu(fileName = "Player Level Strategy")]
public class PlayerLevelStrategy : ScriptableObject {
    [SerializeField] private int level2ExperienceRequired = 50;
    [SerializeField] private float expIncrementPerLevel = 0.1f;
    [SerializeField] private float attackIncrementPerLevel = 0.1f;
    [SerializeField] private float healthIncrementPerLevel = 0.1f;
    
    public int CalculateExperienceForNextLevel(int previousLevelRequiredExperience) {
        return Mathf.RoundToInt(previousLevelRequiredExperience<=0?level2ExperienceRequired : previousLevelRequiredExperience * (1+expIncrementPerLevel));
    }

    public int CalculateDamageForNextLevel(int currentAttackAmount) {
        return Mathf.RoundToInt(currentAttackAmount * (1 + attackIncrementPerLevel));
    }

    public int CalculateHealthForNextLevel(int currentHealth) {
        return Mathf.RoundToInt(currentHealth * (1 + healthIncrementPerLevel));
    }
}