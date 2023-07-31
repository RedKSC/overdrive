using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents {
    public delegate void OnCutsceneStateChangedEvent(bool inCutscene);

    public static event OnCutsceneStateChangedEvent OnCutsceneStateChanged;

    public delegate void OnShopStateChangeEvent(bool isOpen);

    public static event OnShopStateChangeEvent OnShopStateChange;

    public static void SetCutsceneState(bool inCutscene) {
        GameManager.Instance.SetCutsceneState(inCutscene);
        OnCutsceneStateChanged?.Invoke(inCutscene);
    }
    public static void SetShopState(bool isOpen) {
        OnShopStateChange?.Invoke(isOpen);
    }
}

