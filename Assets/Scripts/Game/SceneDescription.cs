using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDescription : MonoBehaviour {
    public static SceneDescription GetSceneDescrpiptor {
        get {
            if (!currentScene) {
                currentScene = GameObject.FindObjectOfType<SceneDescription>();
            }

            return currentScene;
        }
    }

    static SceneDescription currentScene;

    public SceneType Type;

    void Awake() {
        currentScene = this;
    }

    private void OnDestroy() {
        currentScene = null;
    }
}

public enum SceneType {
    Menu,
    Game,
    EndScreen,
    Debug
}
