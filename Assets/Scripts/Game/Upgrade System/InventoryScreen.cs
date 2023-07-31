using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;


public class InventoryScreen : ShopScreenBase
{

    // The higher level purpose for currently being in the inventory screen
    [FoldoutGroup("States")] public int inventoryContext;
    //0 -- equip equipment // 1 -- remove equipment // 2 -- upgrade equipment

    // The part of the menu you're currently selecting on 
    [FoldoutGroup("States")] public int selectionSide;              
    [FoldoutGroup("States")] public int savedSelectionSide;              
    //0 -- inventory // 1 -- equipment  // 2 -- context menu // 3 -- equipment, but you can't select index 0 or 4.

    [FoldoutGroup("States")] public int mPos;

    [FoldoutGroup("Elements")] public GameObject InventoryObjects;
    [FoldoutGroup("Elements")] public GameObject EquipmentObjects;
    [FoldoutGroup("Elements")] public GameObject SelectionSquare;
    [FoldoutGroup("Elements")] public GameObject[] ContextMenus;
    [FoldoutGroup("Elements")] public TextMeshProUGUI description;
    [FoldoutGroup("Elements")] public TextMeshProUGUI cost;
    [FoldoutGroup("Elements")] public TextMeshProUGUI points;
    [FoldoutGroup("Elements")] public TextMeshProUGUI deduction;

    [FoldoutGroup("Cache")] public WeaponsInventory wpnInventory;
    [FoldoutGroup("Cache")] public int inventoryMPos;

    [FoldoutGroup("Resources")] public GameObject InventoryObject;

    //The inventory screen can get called on by different parts of the shop, and is meant to be adaptable to different contexts. This includes:
    // Swapping current weapons with ones in inventory -- Ansel
    // Upgrading or fusing weapons in inventory -- Ansel
    // Deploying weapons in inventory as automatic defenses -- Stella

    public override void Start() {
        base.Start();
        wpnInventory = WeaponsInventory.Instance;
        UpdateInventory();
    }
    public override void Activate() {
        base.Activate();
        mPos = 0;
        active = true;
        selectionSide = 0;
        wpnInventory = WeaponsInventory.Instance;
        UpdateInventory();
    }

    // Self explanatory function, updates the icons in the inventory screen.
    public void UpdateInventory() {
        foreach (Transform child in InventoryObjects.transform) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in EquipmentObjects.transform) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < wpnInventory.inventory.Count; i++) {
            InventoryObject thisWeapon = Instantiate(InventoryObject, InventoryObjects.transform).GetComponent<InventoryObject>();
            thisWeapon.weapon = wpnInventory.inventory[i];
            thisWeapon.initGraphics();
        }
        for (int i = 0; i < 5; i++) {
            InventoryObject thisWeapon = Instantiate(InventoryObject, EquipmentObjects.transform).GetComponent<InventoryObject>();
            thisWeapon.weapon = wpnInventory.equipped[i];
            thisWeapon.initGraphics();
        }

    }

    int CalculateUpgradeCost() {
        Weapon thisWeapon = (savedSelectionSide == 0 ? wpnInventory.inventory[inventoryMPos] : wpnInventory.equipped[inventoryMPos]);
        int cost = (thisWeapon.level + 3 * thisWeapon.rarity) * 500;
        return cost;
    }

    public void Update() {

        if (!active) return;

        if (PlayerInput.GetButtonDown(PlayerInputKey.Horizontal))
            mPos += 1;
        if (PlayerInput.GetRevButtonDown(PlayerInputKey.Horizontal))
            mPos -= 1;

        if (PlayerInput.GetButtonDown(PlayerInputKey.Vertical))
            mPos -= 1 + (selectionSide == 0 ? 1 : 0);
        if (PlayerInput.GetRevButtonDown(PlayerInputKey.Vertical))
            mPos += 1 + (selectionSide == 0 ? 1 : 0);

        // Alright, these got kind of out of hand as I added contexts
        // Basically your min and max mpos depends on where you are in the menu.
        // The only really weird thing is if your selection side is 3 you can only select the slots reserved for Ex Weapons
        int min = (selectionSide == 3 ? 1 : 0);
        int max = (selectionSide == 0 ? wpnInventory.inventory.Count : selectionSide == 1 ? 5 : selectionSide == 2 ? 5 : 4);
        if (mPos >= max) mPos = min;
        if (mPos < min) mPos = max - 1;

        Transform selectedItem = (selectionSide == 0 ? InventoryObjects.transform.GetChild(mPos) 
            : (selectionSide == 1 || selectionSide == 3) ? EquipmentObjects.transform.GetChild(mPos) 
            : ContextMenus[inventoryContext].transform.GetChild(mPos));

        SelectionSquare.transform.position = selectedItem.position;

        switch (selectionSide) {
            case 0:     //Inventory Side
                savedSelectionSide = 0;
                if (PlayerInput.GetButtonDown(PlayerInputKey.Shoot)) { // Open up context menu
                    savedSelectionSide = selectionSide;
                    selectionSide = 2;
                    ContextMenus[inventoryContext].SetActive(true);
                    inventoryMPos = mPos;
                    mPos = 0;
                }
                if (PlayerInput.GetButtonDown(PlayerInputKey.ExWpn1)) { // Open up context menu
                    Exit();
                }
                if (PlayerInput.GetButtonDown(PlayerInputKey.Dash)) { // Switch to the equipment side
                    selectionSide = 1;

                    // Contexts 0 and 1 are actually accessed the same way, since they're used for swapping equipment. 
                    // But context 0 is for EQUIPPING an item from inventory into a weapon slot, and context 1 is for UNEQUIPPING an item from a weapon slot into inventory
                    if (inventoryContext == 0) inventoryContext = 1;
                }

                description.text = wpnInventory.inventory[mPos].Description;
                if (inventoryContext < 1) { // This part is gonna have to be changed later. Basically, only show relevent information (price, etc) on the correct contexts
                    points.text = "";
                    cost.text = "";
                    deduction.text = "";
                } else {

                    cost.text = "Cost:    " + CalculateUpgradeCost().ToString();
                    deduction.text = "(" + ((int)(GameManager.Instance.Points - CalculateUpgradeCost())).ToString() + ")";
                    points.text = "POINTS:  " + GameManager.Instance.GetDisplayScore(false);
                }
                break;
            case 1:     //Equipment Side
                savedSelectionSide = 1;
                if (PlayerInput.GetButtonDown(PlayerInputKey.Shoot) && wpnInventory.equipped[mPos] != null) { // Open up context menu
                    savedSelectionSide = selectionSide;
                    selectionSide = 2;
                    ContextMenus[inventoryContext].SetActive(true);
                    inventoryMPos = mPos;
                    mPos = 0;
                }
                if (PlayerInput.GetButtonDown(PlayerInputKey.ExWpn1)) { // Open up context menu
                    Exit();
                }
                if (PlayerInput.GetButtonDown(PlayerInputKey.Roll)) { // Switch to inventory side
                    selectionSide = 0;
                    if (inventoryContext == 1) inventoryContext = 0;
                }

                description.text = wpnInventory.equipped[mPos].Description;
                if (inventoryContext < 1) {
                    points.text = "";
                    cost.text = "";
                    deduction.text = "";
                }
                else {
                    cost.text = "Cost:    " + CalculateUpgradeCost().ToString();
                    deduction.text = "(" + ((int)(GameManager.Instance.Points - CalculateUpgradeCost())).ToString() + ")";
                    points.text = "POINTS:  " + GameManager.Instance.GetDisplayScore(false);
                }
                break;
            case 2:     //Context Menu  
                if (PlayerInput.GetButtonDown(PlayerInputKey.Shoot)) {
                    switch (inventoryContext) {
                        case 0:     //EQUIP ITEM CONTEXT
                            switch (mPos) {
                                case 0:

                                    //If the weapon is a dash or laser (pink or grey color), it can be equipped right away
                                    //Otherwise, you go to a state where you can select which Ex Weapon slot the item goes into
                                    Weapon wpn = wpnInventory.inventory[inventoryMPos];
                                    if (wpn.color == weaponColor.Gray) {
                                        wpnInventory.SwapEquip(inventoryMPos, 0);

                                        selectionSide = 0;
                                        mPos = inventoryMPos;
                                        ContextMenus[inventoryContext].SetActive(false);

                                        UpdateInventory();
                                    }
                                    else if (wpn.color == weaponColor.Pink) {
                                        wpnInventory.SwapEquip(inventoryMPos, 4);

                                        selectionSide = 0;
                                        mPos = inventoryMPos;
                                        ContextMenus[inventoryContext].SetActive(false);

                                        UpdateInventory();
                                    } else {
                                        selectionSide = 3;
                                        mPos = 0;
                                        ContextMenus[inventoryContext].SetActive(false);
                                    }
                                    break;
                                case 1:
                                    // Drop from inventory
                                    wpnInventory.inventory.RemoveAt(inventoryMPos);
                                    UpdateInventory();

                                    selectionSide = 0;
                                    mPos = inventoryMPos;
                                    ContextMenus[inventoryContext].SetActive(false);
                                    break;
                                case 2:
                                    // Cancel the context menu
                                    selectionSide = 0;
                                    mPos = inventoryMPos;
                                    ContextMenus[inventoryContext].SetActive(false);
                                    break;
                            }
                            break;
                        case 1:         // UNEQUIP ITEM CONTEXT
                            switch (mPos) {
                                case 0:
                                    // Just take it out of equipment and move it into inventory.
                                    wpnInventory.Unequip(inventoryMPos);
                                    UpdateInventory();

                                    selectionSide = 1;
                                    mPos = inventoryMPos;
                                    ContextMenus[inventoryContext].SetActive(false);
                                    break;
                                case 1:
                                    // Unequip and chuck the item away
                                    wpnInventory.Unequip(inventoryMPos, false, true);
                                    UpdateInventory();

                                    selectionSide = 1;
                                    mPos = inventoryMPos;
                                    ContextMenus[inventoryContext].SetActive(false);
                                    break;
                                case 2:
                                    // Cancel the context menu
                                    selectionSide = 1;
                                    mPos = inventoryMPos;
                                    ContextMenus[inventoryContext].SetActive(false);
                                    break;
                            }
                            break;
                        case 2:         // UPGRADE ITEM CONTEXT
                            switch (mPos) {
                                case 0:
                                    //This should have you pay points to increase a weapon's LEVEL
                                    //The cost is a function of the weapon's LEVEL, the weapon's base upgrade price, the weapon's upgrade price multiplier, and the RARITY.
                                    Weapon thisWeapon = (savedSelectionSide == 0 ? wpnInventory.inventory[inventoryMPos] : wpnInventory.equipped[inventoryMPos]);
                                    if (GameManager.Instance.Points >= CalculateUpgradeCost() && thisWeapon.level < 3) {
                                        GameManager.Instance.Points -= CalculateUpgradeCost();
                                        thisWeapon.level++;

                                        cost.text = "Cost:    " + CalculateUpgradeCost().ToString();
                                        deduction.text = "(" + ((int)(GameManager.Instance.Points - CalculateUpgradeCost())).ToString() + ")";
                                        points.text = "POINTS:  " + GameManager.Instance.GetDisplayScore(false);

                                        UpdateInventory();
                                    }
                                    break;
                                case 1:
                                    //Only available if the item is at max LEVEL (which is level 3, attained after 3 upgrades.)
                                    //Selecting this should have you attempt to select two other weapons to fuse it with. The two other weapons must be the same RARITY, max LEVEL, and DIFFERENT COLORS
                                    break;
                                case 2:
                                    selectionSide = savedSelectionSide;
                                    mPos = inventoryMPos;
                                    ContextMenus[inventoryContext].SetActive(false);
                                    break;
                            }
                            break;
                        case 3:         // DEPLOY DEFENSE CONTEXT
                            switch (mPos) {
                                case 0:
                                    //Selecting this should let you pick a planet to deploy the weapon on as an automatic defense. Each planet can have two defenses.
                                    break;
                                case 1:
                                    //Guess we don't need this one? IDK.
                                    break;
                                case 2:
                                    selectionSide = 1;
                                    mPos = inventoryMPos;
                                    ContextMenus[inventoryContext].SetActive(false);
                                    break;
                            }
                            break;

                    }
                }
                break;
            case 3:     //Equipment side if you're selecting an ex weapon slot to equip an ex weapon to.
                if (PlayerInput.GetButtonDown(PlayerInputKey.Shoot)) {
                    wpnInventory.SwapEquip(inventoryMPos, mPos);

                    UpdateInventory();
                    selectionSide = 0;
                    mPos = inventoryMPos;
                }
                break;
        }
    }
}
