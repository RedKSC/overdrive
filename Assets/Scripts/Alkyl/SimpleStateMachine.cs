using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStateMachine : MonoBehaviour {
    Action currentState;

    private void Update() {
        if(currentState == null) {
            return;
        }

        currentState.Invoke();
    }

    public void SetState(Action newState) => currentState = newState;
}
