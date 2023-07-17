using UnityEngine;

public abstract class TileDisplayBase : MonoBehaviour {
    public abstract GameObject MakeCorrectTilePreview(Enemy enemyPrefab);
    public abstract GameObject MakeWrongTilePreview(Enemy enemyPrefab);
    public abstract void MakeDraggablePreview(Transform previewParent, Enemy enemyPrefab);
    public abstract void MakeCorrectPreview();
    public abstract void MakeWrongPreview();
}