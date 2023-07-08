using UnityEngine;

[CreateAssetMenu(fileName = "Level Configuration")]
public class LevelConfiguration : ScriptableObject {
    [SerializeField] private Vector2 gridOffset;
    public Vector3 GridOffset => gridOffset;
}