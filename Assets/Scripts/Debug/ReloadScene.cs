using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour {
    public string SceneOverride;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if (!Debug.isDebugBuild) {
            return;
        }
        
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.P)){
            SceneManager.LoadScene(string.IsNullOrEmpty(SceneOverride) ? SceneManager.GetActiveScene().name : SceneOverride);
        }
    }
}