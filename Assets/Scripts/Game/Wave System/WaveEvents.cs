using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaveEvents {
    public delegate void OnGameStartEvent();
    public delegate void OnShopCloseEvent();
    public delegate void OnWaveStartEvent();
    public delegate void OnWaveEndEvent();
    public delegate void OnWaveCompleteEvent();
    public delegate void OnPlayerKilledEvent();

    public static event OnGameStartEvent OnGameStart;
    public static event OnShopCloseEvent OnShopClose;
    public static event OnWaveStartEvent OnWaveStart;
    public static event OnWaveEndEvent OnWaveEnd;
    public static event OnWaveEndEvent OnWaveComplete;
    public static event OnPlayerKilledEvent OnPlayerKilled;

    public static void GameStart() => OnGameStart?.Invoke();
    public static void ShopClose() => OnShopClose?.Invoke();
    public static void WaveStart() => OnWaveStart?.Invoke();
    public static void WaveEnd() => OnWaveEnd?.Invoke();
    public static void WaveComplete() => OnWaveComplete?.Invoke();
    public static void PlayerKilled() => OnPlayerKilled?.Invoke();
}