using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostAtBossPanel : MonoBehaviour {
    [SerializeField] private PlayerFightManager fightManager;
    [SerializeField] private GameObject mask;
    [SerializeField] private GameObject panel;
    
    private void Awake() {
        fightManager.OnPlayerDefeated += HandleDefeat;
    }

    private void HandleDefeat() {
        mask.SetActive(true);
        panel.SetActive(true);
    }
}
