using System;
using TMPro;
using UnityEngine;

public class LevelDisplay : MonoBehaviour {

    [SerializeField] private TMP_Text levelTxt;
    [SerializeField] private Player player;

    private void Start() {
        player.OnLevelGained += UpdateLevel;
    }

    private void UpdateLevel(int newLevel) {
        levelTxt.text = $"Level: {newLevel:00}";
    }
}