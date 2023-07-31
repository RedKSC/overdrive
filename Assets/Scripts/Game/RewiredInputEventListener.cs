using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RewiredInputEventListener : UltEvents.UltEventHolder {
    [PropertyOrder(-99)]
    public PlayerInputKey[] ButtonsToCheck;
    private void Update() {
        for (int i = 0; i < ButtonsToCheck.Length; i++) {
            if (PlayerInput.GetButtonDown(ButtonsToCheck[i])) {
                Invoke();
                return;
            }
        }
    }
}
