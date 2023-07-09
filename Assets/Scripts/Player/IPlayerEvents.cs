using System;

public interface IPlayerEvents {
    event Action<ExperienceGained> OnExperienceGained;
    event Action<int> OnLevelGained;
    event Action OnPlayerDied;
    event Action<int, int> OnPlayerHealthChanged;
}