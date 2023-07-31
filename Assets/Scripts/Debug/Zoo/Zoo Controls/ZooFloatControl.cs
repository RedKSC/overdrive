using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZooFloatControl : ZooControl {
    public TMP_Text NumberText;

    float currentNum;

    public void ChangeNumber(float changeAmount) {
        currentNum += changeAmount;
        NumberText.text = currentNum.ToString("N1");
        Anim.SetFloat(AnimID, currentNum);
    }
}
