
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class QuitButton : MonoBehaviour
{
    private void Awake() {
        GetComponent<Button>().onClick.AddListener(QuitCmd);
    }

    private void QuitCmd() {
        Application.Quit();
    }
}