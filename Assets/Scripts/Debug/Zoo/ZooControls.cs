using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ZooControls", menuName = "Overdrive/Zoo Controls")]
public class ZooControls : ScriptableObject {
    [Required]
    public string EntityName;
    public GameObject ObjectOverride;

    [Required]
    public RuntimeAnimatorController Anim;
    public ZooControlData[] Controls;
}

[System.Serializable]
public struct ZooControlData {
    public string ControlName;
    public AnimationControlType ControlType;

    [ShowIf("@ControlType == AnimationControlType.OVERRIDE"), Required]
    public ZooControl OverrideControl;
}
