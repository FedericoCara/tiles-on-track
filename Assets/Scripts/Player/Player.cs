using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour, IPlayerEvents {

    [SerializeField] private PlayerData playerData;
    public event Action<ExperienceGained> OnExperienceGained = gained => { };
    public event Action<int> OnLevelGained = newLevel => { };
    public event Action OnPlayerDied = () => { };
    public event Action<int, int> OnPlayerHealthChanged = (current, max) => { };
    
    private PlayerMovement _playerMovement;
    public int Level => playerData.Level;
    public PlayerAttack PlayerAttack => playerData.PlayerAttack;
    public bool IsDead => playerData.Health<=0;
    public float HealthPercentage => playerData.Health / (float) playerData.MaxHealth;

    private void Awake() {
        playerData.OnExperienceGained += experienceGained => OnExperienceGained(experienceGained);
        playerData.OnLevelGained += newLevel => OnLevelGained(newLevel);
        playerData.OnPlayerDied += HandlePlayerDead;
        playerData.OnPlayerHealthChanged += (current,max) => OnPlayerHealthChanged(current, max);
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void HandlePlayerDead() {
        Destroy(gameObject);
        OnPlayerDied();
    }

    public void AddExperience(int exp) => playerData.AddExperience(exp);

    public void ReceiveDamage(int opponentDamage) => playerData.ReceiveDamage(opponentDamage);

    public void GainLife(int healAmount) => playerData.GainLife(healAmount);
}
