using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WonPanel : MonoBehaviour {
    [SerializeField] private PlayerFightManager fightManager;
    [SerializeField] private GameObject mask;
    [SerializeField] private GameObject panel;
    
    private void Awake() {
        fightManager.OnBossDefeated += HandleVictory;
    }

    private void HandleVictory() {
        mask.SetActive(true);
        panel.SetActive(true);
    }
}
