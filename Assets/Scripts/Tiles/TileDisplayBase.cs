using UnityEngine;

public abstract class TileDisplayBase : MonoBehaviour {
    public abstract GameObject MakeCorrectTilePreview();
    public abstract GameObject MakeWrongTilePreview();
    public abstract void MakeDraggablePreview(Transform previewParent);
    public abstract void MakeCorrectPreview();
    public abstract void MakeWrongPreview();
    public abstract void DestroyBackgroundSprites();
}