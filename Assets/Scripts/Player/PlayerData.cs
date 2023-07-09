using System;
using UnityEngine;

[Serializable]
public class PlayerData : IPlayerEvents{
    public event Action<ExperienceGained> OnExperienceGained = experienceGained => { };
    public event Action<int> OnLevelGained = newLevel => { };
    public event Action OnPlayerDied = () => { };
    public event Action<int, int> OnPlayerHealthChanged = (health, maxHealth) => { };
    
    [SerializeField] private int experience;
    public int Experience => experience;
    
    [SerializeField] private int health;
    public int Health => health;
    public int MaxHealth { get; private set; }

    [SerializeField] private int level;
    public int Level => level;

    [SerializeField] private PlayerAttack playerAttack;
    public PlayerAttack PlayerAttack => playerAttack;

    [SerializeField] private PlayerLevelStrategy playerLevelStrategy;

    private int experienceForNextLevel;

    public void AddExperience(int exp) {
        experience += exp;
        UpdateLevelIfNecessary();
    }

    public void ReceiveDamage(int damage) {
        ValidateMaxHealth();
        health -= damage;
        if (health <= 0) {
            OnPlayerDied();
        }else
            NotifyHealthUpdated();
    }

    public void GainLife(int healAmount) {
        ValidateMaxHealth();
        health = Mathf.Clamp(health + healAmount, 0, MaxHealth);
        NotifyHealthUpdated();
    }

    private void UpdateLevelIfNecessary() {
        SetStartingRequiredExperienceIfNecessary();

        if (experience > experienceForNextLevel) {
            experience -= experienceForNextLevel;
            level++;
            experienceForNextLevel = CalculateExperienceForNextLevel();
            MaxHealth = CalculateHealthForNextLevel();
            playerAttack.Damage = CalculateDamageForNextLevel();
            RestoreHealth();

            OnLevelGained(level);
            OnExperienceGained(new ExperienceGained(experience, experienceForNextLevel));
        }else
            OnExperienceGained(new ExperienceGained(experience, experienceForNextLevel));
    }

    private void RestoreHealth() {
        health = MaxHealth;
        NotifyHealthUpdated();
    }

    private void NotifyHealthUpdated() {
        OnPlayerHealthChanged(Health, MaxHealth);
    }

    private void SetStartingRequiredExperienceIfNecessary() {
        if (experienceForNextLevel <= 0)
            experienceForNextLevel = CalculateExperienceForNextLevel();
    }

    private int CalculateExperienceForNextLevel() => playerLevelStrategy.CalculateExperienceForNextLevel(experienceForNextLevel);

    private int CalculateHealthForNextLevel() => playerLevelStrategy.CalculateHealthForNextLevel(MaxHealth);

    private int CalculateDamageForNextLevel() => playerLevelStrategy.CalculateDamageForNextLevel(playerAttack.Damage);


    private void ValidateMaxHealth() {
        if (MaxHealth <= 0)
            MaxHealth = health;
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
