using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedRepeatAction : MonoBehaviour {

    System.Action action;

    //I'm not kidding
    LTDescr currentTimer;
    float time;

    public void Setup(System.Action Action, float Time, bool autoStart = false) {
        action = Action;
        time = Time;

        if(autoStart) {
            Start();
        }
    }

    public void Start() {
        currentTimer = LeanTween.value(gameObject, 0f, 1f, time).setOnComplete(() => {
            action.Invoke();
            Start();
        });
    }

    public void Stop() {
        LeanTween.cancel(currentTimer.id);
    }
}
