using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CallScreen : ShopScreenBase
{
    [FoldoutGroup("States")] public int mPos;
    [FoldoutGroup("States")] public int state;

    [FoldoutGroup("Elements")] public GameObject SelectionSquare;
    [FoldoutGroup("Elements")] public Canvas storeCanvas;
    [FoldoutGroup("Elements")] public AnselOverviewScreen anselScreen;


    public override void Activate() {
        base.Activate();
        state = 0;
        mPos = 0;
    }

    public void Update() {
        if (!active) return;
        switch(state) {
            case 0:
       
                int min = 0;
                int max = 4;

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

                Transform selectedItem = transform.GetChild(1).GetChild(mPos);

                SelectionSquare.transform.position = selectedItem.position;

                if (PlayerInput.GetButtonDown(PlayerInputKey.Shoot)) {
                    StartCoroutine(FlashSelect());
                    state = 2;
                }
                if (PlayerInput.GetButtonDown(PlayerInputKey.Dash)) {
                    mPos = max - 1;
                }
                break;
            case 1:
                if(!DialogueEventDelegator.Instance.talking) {
                    storeCanvas.enabled = true;
                    switch(mPos) {
                        case 0:
                            active = false;
                            gameObject.SetActive(false);

                            anselScreen.gameObject.SetActive(true);
                            anselScreen.Activate();
                            break;
                    }
                }
                break;
        }
    }

    public void MoveOn() {
        switch (mPos) {
            case 0: // Call on Ansel
                state = 1;
                DialogueSpawner.Instance.SpawnShopDialogue(DialogueCharacter.Ansel);
                storeCanvas.enabled = false;
                break;
            case 3: // Call on Ansel
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
