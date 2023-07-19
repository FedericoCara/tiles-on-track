using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour {

    [SerializeField] private Player player;
    [SerializeField] private PlayerFightManager playerFightManager;
    [SerializeField] private float dyingAnimationDurationSec = 1f;
    [SerializeField] private float playerStartAttackAnticipation = 0.5f;
    private Animator _animator;
    private static readonly int AttackHash = Animator.StringToHash("attack");
    private static readonly int ReceiveDamageHash = Animator.StringToHash("receive_damage");
    private static readonly int DieHash = Animator.StringToHash("die");

    private void Awake() {
        _animator = GetComponent<Animator>();
        playerFightManager.OnPlayerStartAttack += StartAttack;
        player.OnPlayerReceivedDamage += ReceiveDamage;
        player.OnPlayerDied += Die;

        playerFightManager.PlayerStartAttackAnticipation = playerStartAttackAnticipation;
    }

    private void StartAttack() {
        _animator.SetTrigger(AttackHash);
    }

    private void ReceiveDamage() {
        _animator.SetTrigger(ReceiveDamageHash);
    }

    private float Die() {
        _animator.SetTrigger(DieHash);
        return dyingAnimationDurationSec;
    }

}
