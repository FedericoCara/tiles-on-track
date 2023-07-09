using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TryAgainButton : MonoBehaviour
{
    private void Awake() {
        GetComponent<Button>().onClick.AddListener(TryAgainCmd);
    }

    private void TryAgainCmd() {
        SceneManager.LoadScene(0);
    }
}
