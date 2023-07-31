using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseHandler : MonoBehaviour {

    bool paused;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            paused = !paused;
            Time.timeScale = (!paused).ConvertToInt();
        }
    }

    public void OnReseed(){
        try{
            SceneManager.LoadScene(0);
            paused = false;
            Time.timeScale = 1f;
        }catch{
            return;
        }
    }
}