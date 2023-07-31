using UnityEngine;
using Sirenix.OdinInspector;

public enum weaponColor {
    Gray,
    Red,
    Green,
    Blue,
    Pink
}

[CreateAssetMenu(fileName = "Weapon", menuName = "RPM/Game/Weapon")]
public class Weapon : ScriptableObject {
    [LabelText("Name")]
    public string weaponName;

    [LabelText("GFX")]
    public Sprite GFX;

    [LabelText("Code Runner")]
    public GameObject coderunner;

    [Multiline(10)]
    public string Description;

    [LabelText("Color")]
    public weaponColor color;

    [LabelText("Level")]
    public int level;

    [LabelText("Rarity")]
    public int rarity;
}