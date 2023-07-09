using System;
using UnityEngine;

public class CheckForQuit : MonoBehaviour {
    private void Update() {
        if(Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();
    }
}