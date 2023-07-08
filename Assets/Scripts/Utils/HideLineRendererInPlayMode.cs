using UnityEngine;

public class HideLineRendererInPlayMode : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        #endif
    }

    private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
    {
        if(lineRenderer==null)
            return;
        
        if (state == UnityEditor.PlayModeStateChange.ExitingEditMode)
        {
            lineRenderer.enabled = false;
        }
        else if (state == UnityEditor.PlayModeStateChange.EnteredEditMode)
        {
            lineRenderer.enabled = true;
        }
    }
}