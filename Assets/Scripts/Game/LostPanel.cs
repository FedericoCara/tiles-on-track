using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostPanel : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private GameObject mask;
    [SerializeField] private GameObject panel;
    
    private void Awake() {
        player.OnPlayerFinishedDying += HandleDead;
    }

    private void HandleDead() {
        if(player.KilledByBoss)
            return;
        mask.SetActive(true);
        panel.SetActive(true);
    }
}
