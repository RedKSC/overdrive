using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ShopScreenBase : MonoBehaviour
{

    [FoldoutGroup("Resources")] public ShopBrain brain;
    [FoldoutGroup("Resources")] public ShopScreenBase parent;

    [FoldoutGroup("States")] public bool active = false;

    bool inCutscene;

    public virtual void Start() {
        brain = ShopBrain.Instance;
    }

    public virtual void Activate() {
        active = true;
    }

    public virtual void Exit() {
        if (inCutscene) {
            active = false;
            gameObject.SetActive(false);
            return;
        }

        if (parent == null) {
            ShopBrain.Instance.CloseShop();
        }
        else {
            parent.gameObject.SetActive(true);
            parent.Activate();
        }
        active = false;
        gameObject.SetActive(false);
    }

    public void CutsceneState(bool cutscene) => inCutscene = cutscene;
}
