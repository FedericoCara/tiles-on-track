
using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Player))]
public class PlayerDrinkingManager : MonoBehaviour{
    
    private Player _player;
    private PlayerMovement _playerMovement;
    private Potion _drinkingPotion;

    private float _timeDrinking = 0;
    private bool _drinking = false;
    
    private void Awake() {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.OnDrinkingStarted += StartDrinking;
        _player = GetComponent<Player>();
    }

    private void Update() {
        if(!_drinking)
            return;

        _timeDrinking += Time.deltaTime;
        if (_timeDrinking >= _drinkingPotion.DrinkingDuration) {
            _timeDrinking = 0;
            _drinking = false;
            _drinkingPotion.Heal(_player);
            _playerMovement.SetDrinkingFinished();
        }
    }

    private void StartDrinking(Potion potion) {
        _drinking = true;
        _timeDrinking = 0;
        _drinkingPotion = potion;
    }
}
