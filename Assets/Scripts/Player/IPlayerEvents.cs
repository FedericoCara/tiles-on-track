using System;

public interface IPlayerEvents {
    event Action<ExperienceGained> OnExperienceGained;
    event Action<int> OnLevelGained;
    event Action<int, int> OnPlayerHealthChanged;
    public void AddExperience(int exp);
}