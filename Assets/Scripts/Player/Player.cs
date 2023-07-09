using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake() {
        playerData.OnExperienceGained += experienceGained => OnExperienceGained(experienceGained);
        playerData.OnLevelGained += newLevel => OnLevelGained(newLevel);
        playerData.OnPlayerDied += () => OnPlayerDied();
        playerData.OnPlayerHealthChanged += (current,max) => OnPlayerHealthChanged(current, max);
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update() {
        
    }
}
