using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryObject : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI level;
    public Image icon;
    public Weapon weapon;

    public Color defaultCol;
    public Color selectCol;

    public void initGraphics() {
        if (weapon != null) {
            title.text = weapon.weaponName;
            icon.sprite = weapon.GFX;
            level.text = "Level " + weapon.level.ToString() + " / Rarity " + weapon.rarity.ToString();
        }
    }
}
