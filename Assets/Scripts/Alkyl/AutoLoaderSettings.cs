using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Alkyl/Utility/Auto Loader Settings", fileName = "Auto Loader Settings")]
public class AutoLoaderSettings : ScriptableObject {
    [HideLabel]
    public GameObject[] Objects;
}