using System;
using UnityEngine;

[Serializable]
public class PlayerData {
    [SerializeField] private int experience;
    public int Experience => experience;
    public Action<ExperienceGained> OnExperienceGained => experienceGained => { }; 
    public Action<int> OnLevelGained => newLevel => { }; 
    
    [SerializeField] private int health;
    public int Health => health;

    [SerializeField] private int level;
    public int Level => level;

    [SerializeField] private PlayerLevelStrategy playerLevelStrategy;

    public void AddExperience(int exp) {
        experience += exp;
        UpdateLevelIfNecessary();
    }

    private int CalculateExperienceForNextLevel() => playerLevelStrategy.CalculateExperienceForNextLevel(level);

    private void UpdateLevelIfNecessary() {
        var experienceForNextLevel = CalculateExperienceForNextLevel();
        if (experience > experienceForNextLevel) {
            experience -= experienceForNextLevel;
            level++;
            OnLevelGained(level);
        }else
            OnExperienceGained(new ExperienceGained(experience, experienceForNextLevel));
    }
}

public struct ExperienceGained {
    private int _currentExperience;
    public int CurrentExperience => _currentExperience;
    private int _totalExperienceForNextLevel;
    public int TotalExperienceForNextLevel => _totalExperienceForNextLevel;

    public ExperienceGained(int currentExperience, int totalExperienceForNextLevel) {
        _currentExperience = currentExperience;
        _totalExperienceForNextLevel = totalExperienceForNextLevel;
    }
}
