using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class AnselBuyWeaponScreen : ShopScreenBase
{
    [FoldoutGroup("States")] public int mPos;
    [FoldoutGroup("States")] public int state;
    [FoldoutGroup("States")] public int revealState; //0 -- unknown // 1 -- revealed // 2 -- bought

    [FoldoutGroup("Elements")] public GameObject SelectionSquare;
    [FoldoutGroup("Elements")] public Canvas storeCanvas;
    [FoldoutGroup("Elements")] public Image itemImage;
    [FoldoutGroup("Elements")] public TextMeshProUGUI itemName;
    [FoldoutGroup("Elements")] public TextMeshProUGUI revealOption;
    [FoldoutGroup("Elements")] public Sprite unknownItemSprite;

    [FoldoutGroup("Resources")] public Weapon[] weaponBases;
    [FoldoutGroup("Resources")] public Weapon currentWeapon;
    [FoldoutGroup("Resources")] public int rememberedSeed;
    [FoldoutGroup("Resources")] public WeaponsInventory wpnInventory;


    public override void Activate() {
        base.Activate();
        state = 0;
        if(rememberedSeed != brain.shopSeed) {
            rememberedSeed = brain.shopSeed;
            RollWeapon();
        }
    }
    public void Awake() {
        weaponBases = Resources.LoadAll<Weapon>("Weapons");
    }
    public override void Start() {
        base.Start();
        wpnInventory = WeaponsInventory.Instance;
    }

    public void RollWeapon() {
        revealState = 0;
        revealOption.text = "REVEAL (1000 PTS)";
        int i = Random.Range(0, weaponBases.Length - 1);
        itemName.text = "???";
        itemImage.sprite = unknownItemSprite;
        Debug.Log("RollWeapon");
        currentWeapon = weaponBases[i];
    }

    public void Update() {
        if (!active) return;
        switch(state) {
            case 0:
                int min = 0;
                int max = 2;

                if (PlayerInput.GetButtonDown(PlayerInputKey.Horizontal))
                    mPos = min;
                if (PlayerInput.GetRevButtonDown(PlayerInputKey.Horizontal))
                    mPos = max - 1;

                if (PlayerInput.GetButtonDown(PlayerInputKey.Vertical))
                    mPos -= 1;
                if (PlayerInput.GetRevButtonDown(PlayerInputKey.Vertical))
                    mPos += 1;

                if (mPos >= max) mPos = min;
                if (mPos < min) mPos = max - 1;

                Transform selectedItem = transform.GetChild(0).GetChild(mPos);

                SelectionSquare.transform.position = selectedItem.position;

                if (PlayerInput.GetButtonDown(PlayerInputKey.Shoot)) {
                    bool cancel = false;
                    if(mPos == 0) {
                        if(revealState == 2 || (GameManager.Instance.Points < 1000 * (2 - revealState)) || wpnInventory.InventoryFull()) {
                            cancel = true;
                        }
                    }
                    if (!cancel) {
                        StartCoroutine(FlashSelect());
                        state = 1;
                    }
                }
                if (PlayerInput.GetButtonDown(PlayerInputKey.Dash)) {
                    mPos = max - 1;
                }
                break;
        }
    }

    public void MoveOn() {
        switch (mPos) {
            case 0: // Reveal/Purchase
                switch(revealState) {
                    case 0:
                        revealState = 1;
                        itemImage.sprite = currentWeapon.GFX;
                        itemName.text = currentWeapon.weaponName;
                        revealOption.text = "BUILD (1000 PTS)";
                        GameManager.Instance.Points -= 1000;
                        state = 0;
                        break;
                    case 1:
                        revealState = 2;
                        itemImage.sprite = currentWeapon.GFX;
                        itemName.text = currentWeapon.weaponName;
                        revealOption.text = "BUILT!";
                        GameManager.Instance.Points -= 1000;
                        wpnInventory.AddToInventory(currentWeapon);
                        state = 0;
                        break;
                }
                break;
            case 1:
                Exit();
                break;
        }
    }

    public IEnumerator FlashSelect() {
        float t = 0;
        int q = 0;
        SelectionSquare.SetActive(false);
        while (q < 4) {
            while (t < 0.1f) {
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;
            SelectionSquare.SetActive(!SelectionSquare.activeInHierarchy);
            q++;
            yield return null;
        }
        SelectionSquare.SetActive(true);
        MoveOn();
    }
}
