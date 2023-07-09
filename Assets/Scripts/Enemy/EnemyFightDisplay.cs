using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EnemyFightDisplay {
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image lifeBar;
    [SerializeField] private Image attackBar;

    public void SetLifePercentage(float percentage) => lifeBar.fillAmount = percentage;
    public void SetAttackPercentage(float percentage) => attackBar.fillAmount = percentage;

    public void Hide() => canvas.SetActive(false);

    public void Show() => canvas.SetActive(true);
}