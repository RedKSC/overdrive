using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZooIntControl : ZooControl {
    public TMP_Text NumberText;

    int currentNum;

    public void ChangeNumber(int changeAmount) {
        currentNum += changeAmount;
        NumberText.text = currentNum.ToString();
        Anim.SetFloat(AnimID, (float)currentNum);
    }
}
