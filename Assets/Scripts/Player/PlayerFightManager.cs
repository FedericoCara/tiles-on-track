using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Player))]
public class PlayerFightManager : MonoBehaviour {

    public event Action OnBossDefeated = () => { };
    public event Action OnPlayerStartAttack = () => { };

    private PlayerAttack _playerAttack;
    private Player _player;
    private PlayerMovement _playerMovement;
    
    private bool _fighting;
    private Enemy _opponent;
    private float _playerPreparingAttack = 0;
    private float _opponentPreparingAttack = 0;
    private bool startingAttack = false;
    private bool _isTraversingInReverse;

    public float PlayerStartAttackAnticipation { get; set; }

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

            if (!startingAttack &&
                _playerPreparingAttack >= _playerAttack.FrequencySeconds - PlayerStartAttackAnticipation) {
                OnPlayerStartAttack();
                startingAttack = true;
            }

            if (_playerPreparingAttack >= _playerAttack.FrequencySeconds) {
                startingAttack = false;
                _opponent.ReceiveDamage(_playerAttack.Damage);
                _playerPreparingAttack -= _playerAttack.FrequencySeconds;
                if (_opponent.IsDead) {
                    _player.AddExperience(_opponent.ExperienceGiven);
                    _fighting = false;
                    if (_opponent.IsFinalBoss) {
                        OnBossDefeated();
                    } else {
                        _playerMovement.SetFightFinished();
                    }

                    return;
                }
            }

            if (_opponentPreparingAttack >= _opponent.FrequencySeconds) {
                _player.ReceiveDamage(_opponent.Damage, _opponent);
                _opponentPreparingAttack -= _opponent.FrequencySeconds;
                if (_player.IsDead) {
                    _fighting = false;
                    return;
                }
            }
            
            _opponent.EnemyFightDisplay.SetLifePercentage(_opponent.HealthPercentage);
            _opponent.EnemyFightDisplay.SetAttackPercentage(_opponentPreparingAttack/_opponent.FrequencySeconds);
        }
    }

    private void StartFight(Enemy enemy, bool isTraversingInReverse) {
        _isTraversingInReverse = isTraversingInReverse;
        _fighting = true;
        _opponent = enemy;
        _playerPreparingAttack = 0;
        _opponentPreparingAttack = 0;
        _opponent.EnemyFightDisplay.Show();
        if (_isTraversingInReverse) {
            _playerMovement.Turn180();
        }
    }
}