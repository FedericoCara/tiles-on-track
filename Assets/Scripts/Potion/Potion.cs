using UnityEngine;

public class Potion : MonoBehaviour {

    [SerializeField] private int healAmount = 30;

    public void Heal(Player player) {
        player.GainLife(healAmount);
        AnimatePotionUse();
    }

    private void AnimatePotionUse() {
        Destroy(gameObject);
    }
}