using UnityEngine;

public class Potion : MonoBehaviour {

    [SerializeField] private int healAmount = 30;
    [SerializeField] private float drinkingDuration = 3f;
    public float DrinkingDuration => drinkingDuration;
    
    public void Heal(Player player) {
        player.GainLife(healAmount);
        AnimatePotionUse();
    }

    private void AnimatePotionUse() {
        Destroy(gameObject);
    }
}