using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const int WEAPONS_COUNT = 4;

    public static GameManager Instance;
    public static GameManager GetInstance() => Instance;

    public float unpausedTime = 0;
    public bool pause;
    public float gm2unityConv = 1 / 16f;
    public float deltaCoef = 60;
    public long Points = 0;

    public bool InCutcene { get; private set; }
    public bool AllowPlayerAttacks => !InCutcene && !ShopBrain.Instance.active;

    long displayPoints = 0;

    public float timeSince(float time) {
        return unpausedTime - time;
    }
    public float timeLerp(float startTime, float totalTime) {
        return timeSince(startTime) / totalTime;
    }

    public string GetDisplayScore(bool includeScoreText = true) {

        string empty = "00000000";
        string scoreString = displayPoints.ToString();
        int startReplaceIndex = empty.Length - scoreString.Length;

        if (startReplaceIndex < 0) {
            return "A FUCK TON";
        }

        empty = empty.Remove(startReplaceIndex, scoreString.Length);
        empty += scoreString;

        return (includeScoreText ? "Score: " : "") + empty;
    }

    void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Points += 100000;
    }

    void Update() {
        if(!pause) {
            unpausedTime += Time.deltaTime;
        }

        if (displayPoints != Points) {
            long pointsToMove = (Points - displayPoints) / 10L;
            if (pointsToMove == 0L) {
                pointsToMove = (Points - displayPoints) % 10L;
            }
            displayPoints += pointsToMove;
        }
    }

    public void SetCutsceneState(bool cutscene) => InCutcene = cutscene;
}
