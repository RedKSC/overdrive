using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZooBoolControl : ZooControl {
    bool currentVal;

    public void Change(bool newVal) {
        currentVal = newVal;
        Anim.SetBool(AnimID, currentVal);
    }
}
