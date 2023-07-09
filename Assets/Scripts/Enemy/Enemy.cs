using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private int minPlayerLevel = 1;
    public int MinPlayerLevel => minPlayerLevel;
    [SerializeField] private int health = 5;
    private int Health => health;
    [SerializeField] private int damage = 5;
    public int Damage => damage;
    [SerializeField] private float frequencySeconds = 2;
    public float FrequencySeconds => frequencySeconds;
    [SerializeField] private int experienceGiven = 5;
    public int ExperienceGiven => experienceGiven;
    [SerializeField] private bool isFinalBoss = false;
    public bool IsFinalBoss => isFinalBoss;
}