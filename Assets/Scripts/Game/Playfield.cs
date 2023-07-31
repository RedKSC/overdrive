using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Playfield : MonoBehaviour {
    public static Playfield Instance;

    public PlayfieldSettings Settings {get; private set;}

    public Vector2 PlayfieldSize => new Vector2(Settings.BottomRightCorner.x - Settings.TopLeftCorner.x, Settings.TopLeftCorner.y - Settings.BottomRightCorner.y);
    public Vector2 GetPlayfieldPercent(Vector2 pos) {
        pos.x /= PlayfieldSize.x;
        pos.y /= PlayfieldSize.y;

        return pos;
    }
    
    public int Seed;

    private void Awake() {
        if(Seed == 0){
            Seed = Random.Range(int.MinValue, int.MaxValue);
        }
        if(Instance){
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Settings = Resources.Load<PlayfieldSettings>("Playfield Settings");
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += SceneChange;
    }

    void SceneChange(Scene old, Scene newScene) {
        Random.InitState(Seed);
    }

    private void OnDrawGizmos() {
        if(!Application.isPlaying){
            return;
        }

        //Lord help me
        Vector3 pos = new Vector3(Mathf.Lerp(Settings.TopLeftCorner.x, Settings.BottomRightCorner.x, 0.5f), Mathf.Lerp(Settings.TopLeftCorner.y, Settings.BottomRightCorner.y, 0.5f));

        Gizmos.color = Color.yellow;

        pos.y += Playfield.Instance.PlayfieldSize.y;
        pos.x -= Playfield.Instance.PlayfieldSize.x;
        Gizmos.DrawWireCube(pos, Playfield.Instance.PlayfieldSize);

        pos.x += Playfield.Instance.PlayfieldSize.x;
        Gizmos.DrawWireCube(pos, Playfield.Instance.PlayfieldSize);

        pos.x += Playfield.Instance.PlayfieldSize.x;
        Gizmos.DrawWireCube(pos, Playfield.Instance.PlayfieldSize);

        pos.x -= Playfield.Instance.PlayfieldSize.x;
        pos.x -= Playfield.Instance.PlayfieldSize.x;
        pos.y -= Playfield.Instance.PlayfieldSize.y;
        Gizmos.DrawWireCube(pos, Playfield.Instance.PlayfieldSize);

        pos.x += Playfield.Instance.PlayfieldSize.x;
        pos.x += Playfield.Instance.PlayfieldSize.x;
        Gizmos.DrawWireCube(pos, Playfield.Instance.PlayfieldSize);

        pos.x -= Playfield.Instance.PlayfieldSize.x;
        pos.x -= Playfield.Instance.PlayfieldSize.x;
        pos.y -= Playfield.Instance.PlayfieldSize.y;
        Gizmos.DrawWireCube(pos, Playfield.Instance.PlayfieldSize);

        pos.x += Playfield.Instance.PlayfieldSize.x;
        Gizmos.DrawWireCube(pos, Playfield.Instance.PlayfieldSize);

        pos.x += Playfield.Instance.PlayfieldSize.x;
        Gizmos.DrawWireCube(pos, Playfield.Instance.PlayfieldSize);

        Gizmos.color = Color.green;
        pos.y += Playfield.Instance.PlayfieldSize.y;
        pos.x -= Playfield.Instance.PlayfieldSize.x;
        Gizmos.DrawWireCube(pos, Playfield.Instance.PlayfieldSize);
    }
}