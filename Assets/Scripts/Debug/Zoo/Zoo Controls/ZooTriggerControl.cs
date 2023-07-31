using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZooTriggerControl : ZooControl {
    public Toggle CheckBox;

    public void Change(bool newVal) {
        if (!newVal) {
            return;
        }

        Anim.SetTrigger(AnimID);
        CheckBox.isOn = false;
    }
}
