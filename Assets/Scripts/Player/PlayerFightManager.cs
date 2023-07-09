using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Player))]
public class PlayerFightManager : MonoBehaviour {

    private PlayerAttack _playerAttack;
    private Player _player;
    private PlayerMovement _playerMovement;
    
    private bool _fighting;
    private Enemy _opponent;
    private float _playerPreparingAttack = 0;
    private float _opponentPreparingAttack = 0;

    private void Awake() {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.OnFightStarted += StartFight;
        _player = GetComponent<Player>();
        _playerAttack = _player.PlayerAttack;
    }

    private void Update() {
        if (_fighting) {
            _playerPreparingAttack += Time.deltaTime;
            _opponentPreparingAttack += Time.deltaTime;

            if (_playerPreparingAttack >= _playerAttack.FrequencySeconds) {
                _opponent.ReceiveDamage(_playerAttack.Damage);
                _playerPreparingAttack -= _playerAttack.FrequencySeconds;
                if (_opponent.IsDead) {
                    _player.AddExperience(_opponent.ExperienceGiven);
                    _playerMovement.SetFightFinished();
                    _fighting = false;
                    return;
                }
            }

            if (_opponentPreparingAttack >= _opponent.FrequencySeconds) {
                _player.ReceiveDamage(_opponent.Damage);
                _opponentPreparingAttack -= _opponent.FrequencySeconds;
                if (_player.IsDead) {
                    _fighting = false;
                }
            }
        }
    }

    private void StartFight(Enemy enemy) {
        _fighting = true;
        _opponent = enemy;
        _playerPreparingAttack = 0;
        _opponentPreparingAttack = 0;
    }
}