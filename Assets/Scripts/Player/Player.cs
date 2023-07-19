using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour, IPlayerEvents {

    [SerializeField] private PlayerData playerData;
    public event Action<ExperienceGained> OnExperienceGained = gained => { };
    public event Action<int> OnLevelGained = newLevel => { };
    public delegate float PlayerDying();
    public event PlayerDying OnPlayerDied = () => 0;
    public event Action OnPlayerFinishedDying = () => { };
    public event Action<int, int> OnPlayerHealthChanged = (current, max) => { };
    public event Action OnPlayerReceivedDamage = () => { };
    
    public int Level => playerData.Level;
    public PlayerAttack PlayerAttack => playerData.PlayerAttack;
    public bool IsDead => playerData.Health<=0;
    public float HealthPercentage => playerData.Health / (float) playerData.MaxHealth;
    public int MaxHealth => playerData.MaxHealth;

    public bool KilledByBoss { get; private set; }

    private void Awake() {
        playerData.OnExperienceGained += experienceGained => OnExperienceGained(experienceGained);
        playerData.OnLevelGained += newLevel => OnLevelGained(newLevel);
        playerData.OnPlayerDied += HandlePlayerDead;
        playerData.OnPlayerHealthChanged += (current,max) => OnPlayerHealthChanged(current, max);
    }

    private void HandlePlayerDead(Enemy source) {
        float timeUntilFinishedDying = OnPlayerDied();
        KilledByBoss = source != null && source.IsFinalBoss;
        Invoke(nameof(CallOnPlayerFinishedDying), timeUntilFinishedDying);
    }

    private void CallOnPlayerFinishedDying() => OnPlayerFinishedDying();

    public void AddExperience(int exp) => playerData.AddExperience(exp);

    public void ReceiveDamage(int opponentDamage, Enemy source) {
        playerData.ReceiveDamage(opponentDamage, source);
        OnPlayerReceivedDamage();
    }

    public void GainLife(int healAmount) => playerData.GainLife(healAmount);
    
}
