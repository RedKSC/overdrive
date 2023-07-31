using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponsInventory : MonoBehaviour
{
    public static WeaponsInventory Instance;

    public List<Weapon> inventory;
    public Weapon[] equipped;



    // Index 0 -- Laser (Can be swapped, but not removed)
    // Index 1, 2, 3 -- EX weapons
    // Index 4 -- Dash
    public bool[] valid; //For faster checking if something is equipped to a certain slot
    public GameObject[] weaponSlots;

    // This is meant to be a collection of the base scriptable objects for each weapon. But, I don't think this is actually the right place for it. You be the judge.
    public Weapon[] weaponBaseSOs;

    public int inventoryLimit = 32;

    public void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        DebugInventoryFill();
    }

    public void Start() {
        

    }

    void DebugInventoryFill() {
        Equip(Instantiate(Resources.Load<Weapon>("Weapons/Laser")), 0);
        Equip(Instantiate(Resources.Load<Weapon>("Weapons/WaveBeam")), 1);
        Equip(Instantiate(Resources.Load<Weapon>("Weapons/LaserKnife")), 2);
        Equip(Instantiate(Resources.Load<Weapon>("Weapons/RegenField")), 3);

        /*string[] names = { "Laser", "Stunwave", "Firebreath" };

        for (int i = 0; i < names.Length; i++) {
            for (int ii = 0; ii < 3; ii++) {
                for (int iii = 0; iii < 3; iii++) {
                    Weapon wpn = Instantiate(Resources.Load<Weapon>("Weapons/" + names[i]));
                    wpn.level = ii;
                    wpn.rarity = iii;
                    AddToInventory(wpn);
                }
            }
        }*/
        Weapon wpn = Resources.Load<Weapon>("Weapons/Stunwave");
        AddToInventory(wpn);
    }

    // PASSTHROUGH FUNCTIONS
    public bool ActivateWeapon(int index) { // "Button pressed down"
        if(valid[index]) {
            return weaponSlots[index].GetComponentInChildren<PlayerWeaponBase>().Activate();
        }
        return false;
    }
    public void DeactivateWeapon(int index) { // "Button released"
        if (valid[index]) {
            weaponSlots[index].GetComponentInChildren<PlayerWeaponBase>().Deactivate();
        }
    }
     
    // INVENTORY FUNCTIONS
    public bool InventoryFull() {
        return (inventory.Count >= inventoryLimit);
    }
    public bool AddToInventory(Weapon weapon, bool overrideLimit = false) { //overrideLimit exist so that the swap function can work if the inventory is full
        if (inventory.Count < inventoryLimit || overrideLimit) {
            inventory.Add(Instantiate(weapon));
            return true;
        }
        return false;
    }
    public Weapon RemoveFromInventory(int index) {
        Weapon wpn = inventory[index];
        inventory.RemoveAt(index);
        return wpn;
    }

    public bool Equip(Weapon weapon, int slot) { //Equip from defined scriptable object 
        if (valid[slot]) return false;
        valid[slot] = true;
        equipped[slot] = weapon;

        Instantiate(equipped[slot].coderunner, (weaponSlots[slot].transform));

        return true;
    }
    public bool Equip(int inventoryIndex, int slot) { //Equip from inventory
        if (valid[slot]) return false;
        valid[slot] = true;
        equipped[slot] = RemoveFromInventory(inventoryIndex);

        Instantiate(equipped[slot].coderunner, (weaponSlots[slot].transform));

        return true;
    }

    public bool Unequip(int slot, bool overrideLimit = false, bool destroy = false) {
        if (!valid[slot]) return false;
        if(destroy) {
            valid[slot] = false;
            equipped[slot] = null;

            Destroy(weaponSlots[slot].transform.GetChild(0).gameObject);
            return true;
        }
        if (AddToInventory(equipped[slot], overrideLimit)) {

            valid[slot] = false;
            equipped[slot] = null;

            Destroy(weaponSlots[slot].transform.GetChild(0).gameObject);
            return true;
        }

        return false;
    }

    public void SwapEquip(int inventoryIndex, int slot) {
        Unequip(slot, true);
        Equip(inventoryIndex, slot);
    }
}
