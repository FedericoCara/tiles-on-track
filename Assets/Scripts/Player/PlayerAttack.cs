using System;

[Serializable]
public class PlayerAttack {
    private int damage = 5;
    public int Damage {
        get => damage;
        set => damage = value;
    }
    private float frequencySeconds = 1;  
    public float FrequencySeconds {
        get => frequencySeconds;
        set => frequencySeconds = value;
    }
}