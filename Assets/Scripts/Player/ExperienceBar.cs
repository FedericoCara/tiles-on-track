
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour {
    [SerializeField] private Image barFillImage;
    [SerializeField] private Player player;

    void Start()
    {
        player.OnExperienceGained+=UpdateExperience;
        barFillImage.fillAmount = 0;
    }

    private void UpdateExperience(ExperienceGained experienceGained) {
        barFillImage.fillAmount = experienceGained.CurrentExperience / (float) experienceGained.TotalExperienceForNextLevel;
    }

}