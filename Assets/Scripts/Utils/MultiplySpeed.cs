using UnityEngine;

public class MultiplySpeed : MonoBehaviour
{
    [SerializeField] private float multipliedScale = 2;

    public void OnToggleValueChanged(bool isOn) {
        Time.timeScale = isOn ? multipliedScale : 1;
    }
}
