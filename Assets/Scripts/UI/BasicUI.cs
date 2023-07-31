using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct UIInfo { //Convenient way to gather relevant UI info from the player
    public float healthPercent;
    public float dashPercent;
    public float stunPercent;
    public float firePercent;
    public float gasPercent;

    public bool dashEnabled;
    public bool stunEnabled;
    public bool fireEnabled;
    public bool gasEnabled;
}
public class BasicUI : MonoBehaviour
{
    public static BasicUI Instance;

    public GameObject dashUI;
    public GameObject stunUI;
    public GameObject fireUI;
    public GameObject gasUI;

    public RectTransform healthBar;
    public RectTransform dashBar;
    public RectTransform stunBar;
    public RectTransform fireBar;
    public RectTransform gasBar;
    public RectTransform enemyBar;

    public TextMeshProUGUI dashText;
    public TextMeshProUGUI stunText;
    public TextMeshProUGUI fireText;
    public TextMeshProUGUI gasText;
    public TextMeshProUGUI enemyText;

    public ODEntity boss;
    public GameObject bossUI;

    public TextMeshProUGUI score;

    public PlayerController player;

    public GameObject gameOverUI;

    public void Awake() {
        Instance = this;
    }
    public void Start() {
        player = PlayerController.Instance;
        
    }

    public void Update() {
        UIInfo uiInfo = player.playerUIInfo; //Fetch a struct of UIinfo from the playercontroller instance

        dashUI.SetActive(uiInfo.dashEnabled);
        fireUI.SetActive(uiInfo.fireEnabled);
        stunUI.SetActive(uiInfo.stunEnabled);
        gasUI.SetActive(uiInfo.gasEnabled);

        healthBar.sizeDelta     = new Vector2(100 * uiInfo.healthPercent, 100);
        dashBar.sizeDelta       = new Vector2(100 * uiInfo.dashPercent, 100);
        stunBar.sizeDelta       = new Vector2(100 * uiInfo.stunPercent, 100);
        fireBar.sizeDelta       = new Vector2(100 * uiInfo.firePercent, 100);
        gasBar.sizeDelta        = new Vector2(100 * uiInfo.gasPercent, 100);

        if(boss)
            enemyBar.sizeDelta      = new Vector2(200 * boss.simpleHealth / boss.maxHealth, 100);

        dashText.color          = uiInfo.dashPercent == 1 ? Color.white : Color.black;
        stunText.color          = uiInfo.stunPercent == 1 ? Color.white : Color.black;
        fireText.color          = uiInfo.firePercent == 1 ? Color.white : Color.black;
        gasText.color           = uiInfo.gasPercent == 1 ? Color.white : Color.black;

        score.text = GameManager.Instance.GetDisplayScore(false);
    }

    public void SetBoss(ODEntity theBoss, string name) {
        boss = theBoss;
        enemyText.text = name;
        bossUI.SetActive(true);
    }
    public void UnsetBoss() {
        boss = null;
        bossUI.SetActive(false);
    }

    public void GameOver() {
        gameOverUI.SetActive(true);
    }
}
