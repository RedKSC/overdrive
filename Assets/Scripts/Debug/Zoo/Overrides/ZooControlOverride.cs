using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ZooControlOverride : MonoBehaviour {
    [OnValueChanged("UpdateOverrides")]
    public ZooControls Controls;

    [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ListElementLabelName = "ReferenceName")]
    public ZooOverrideControlData[] ControlsOverride;

    public virtual ZooOverrideControlData GetOverrideControl(string animName) => default;

    [Button, InfoBox("WILL RESET OVERRIDE SETTINGS")]
    public virtual void UpdateOverrides() {
        if (!Controls) {
            ControlsOverride = new ZooOverrideControlData[0];
            return;
        }
        
        ControlsOverride = new ZooOverrideControlData[Controls.Controls.Length];

        for (int i = 0; i < ControlsOverride.Length; i++) {
            ControlsOverride[i].ReferenceName = Controls.Controls[i].ControlName;
        }
    }
}

[System.Serializable]
public struct ZooOverrideControlData {
    [HideInInspector]
    public string ReferenceName;

    public string AnimName;
    public Animator AnimatorReference;
}
