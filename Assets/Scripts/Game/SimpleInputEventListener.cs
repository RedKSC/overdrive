using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SimpleInputEventListener : UltEvents.UltEventHolder {
    [PropertyOrder(-999)]
    public KeyCode[] KeysToCheck;
    private void Update() {
        for(int i = 0; i < KeysToCheck.Length; i++) {
            if (Input.GetKeyDown(KeysToCheck[i])) {
                Invoke();
                return;
            }
        }
    }
}
