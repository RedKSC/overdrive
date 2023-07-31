using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ShopItem", menuName = "RPM/Game/Shop Item")]
public class ShopItem : ScriptableObject {
    [LabelText("Name")]
    public string ItemName;

    [LabelText("Base Cost")]
    public long Cost;

    [LabelText("GFX")]
    public Sprite GFX;

    [LabelText("Multiplier Coefficient")]
    public float multipliercoeff;

    [LabelText("Max Purchases")]
    public int max;

    [Multiline(10)]
    public string Description;
}