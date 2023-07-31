using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPM/Game/Playfield Settings")]
public class PlayfieldSettings : ScriptableObject {
    public Vector2 TopLeftCorner;
    public Vector2 BottomRightCorner;
}
