using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BombConveyorBelt : MonoBehaviour {
    
    [SerializeField] private Player player;
    [SerializeField] private ConveyorBelt conveyorBelt;
    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
        player.OnPlayerHealthChanged += OnPlayerHealthChanged;
    }

    private void OnPlayerHealthChanged(int current, int max) {
        button.interactable = current / (float)max > 0.5f;
    }

    public void Bomb() {
        player.ReceiveDamage(player.MaxHealth / 2);
        conveyorBelt.Clear();
    }
}
