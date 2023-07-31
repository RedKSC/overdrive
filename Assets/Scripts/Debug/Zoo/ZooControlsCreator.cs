using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

[RequireComponent(typeof(Animator))]
public class ZooControlsCreator : SerializedMonoBehaviour
{
    public ZooControls[] Controls;
    
    [FoldoutGroup("Setup")]
    public TMP_Text TitleText;

    [FoldoutGroup("Setup")]
    public RectTransform ControlsPivot;

    [FoldoutGroup("Setup")]
    public GameObject EntityBackground;

    [FoldoutGroup("Setup")]
    public float EntityBackgroundScaleSpeed;

    [FoldoutGroup("Setup")]
    public Dictionary<AnimationControlType, ZooControl> ControlLookup = new Dictionary<AnimationControlType, ZooControl>();

    public int CurrentEnt {
        get => ent;
        set {
            #region Constrain new valu
            int newVal = value;

            if (newVal < 0) {
                newVal += Controls.Length;
            }

            if (newVal >= Controls.Length) {
                newVal -= Controls.Length;
            }
            #endregion

            #region Destroy old shit
            if (overrideRef) {
                Destroy(overrideRef);
            }

            for(int i = ControlsPivot.childCount - 1; i >= 0; i--) {
                if (!ControlsPivot.GetChild(i)) {
                    return;
                }

                Destroy(ControlsPivot.GetChild(i).gameObject);
            }
            #endregion

            defaultRenderer.enabled = false;

            if (currentEntityBackgroundTween != null) {
                LeanTween.cancel(currentEntityBackgroundTween.id);
            }

            currentEntityBackgroundTween = LeanTween.scale(EntityBackground, Vector3.zero, EntityBackgroundScaleSpeed).setEaseInCubic().setOnComplete(() => {
                ent = newVal;

                currentEntityBackgroundTween = LeanTween.scale(EntityBackground, Vector3.one, EntityBackgroundScaleSpeed).setEaseOutCubic().setOnComplete(() => {
                    RefreshEnt();
                    currentEntityBackgroundTween = null;
                });
            });
        }
    }
    int ent;

    SpriteRenderer defaultRenderer;
    Animator anim;
    GameObject overrideRef;
    LTDescr currentEntityBackgroundTween;
    bool recievedInput;

    ZooControls GetControls => Controls[CurrentEnt];

    void Awake() {
        anim = GetComponent<Animator>();
        anim.fireEvents = false;
        defaultRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        RefreshEnt();
    }

    void Update() {
        if (Input.GetAxisRaw("Horizontal") == 0f) {
            recievedInput = false;
            return;
        }

        if (recievedInput) {
            return;
        }

        CurrentEnt += Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        recievedInput = true;
    }

    public void ChangeEntity(int change) => CurrentEnt += change;

    public void RefreshEnt() {
        Animator animRef = anim;
        
        if (GetControls.Anim) {
            animRef.runtimeAnimatorController = GetControls.Anim;
        }

        //Make new fancy shit
        TitleText.text = GetControls.EntityName;
        defaultRenderer.enabled = !GetControls.ObjectOverride;

        //The overrides, the hardest part of this system
        if (GetControls.ObjectOverride) {
            overrideRef = Instantiate(GetControls.ObjectOverride, transform.position, Quaternion.identity);
            animRef = overrideRef.GetComponent<Animator>();

            ZooControlOverride controlOverride = overrideRef.GetComponent<ZooControlOverride>();

            for (int i = 0; i < GetControls.Controls.Length; i++) {
                string animName = GetControls.Controls[i].ControlName;
                Animator overrideAnim = animRef;

                if (!string.IsNullOrEmpty(controlOverride.ControlsOverride[i].AnimName)) {
                    animName = controlOverride.ControlsOverride[i].AnimName;
                }

                if (controlOverride.ControlsOverride[i].AnimatorReference) {
                    overrideAnim = controlOverride.ControlsOverride[i].AnimatorReference;
                }

                if (GetControls.Controls[i].ControlType == AnimationControlType.OVERRIDE) {
                    ZooControl control = Instantiate(GetControls.Controls[i].OverrideControl, ControlsPivot);

                    control.Anim = anim;
                    control.AnimID = Animator.StringToHash(animName);
                    control.TitleText.text = animName;
                    continue;
                }

                CreateControl(GetControls.Controls[i].ControlType, GetControls.Controls[i].ControlName, Animator.StringToHash(animName), overrideAnim);
            }

            return;
        }

        //Default behaviour, really not a lot should get here
        for (int i = 0; i < GetControls.Controls.Length; i++) {
            if (GetControls.Controls[i].ControlType == AnimationControlType.OVERRIDE) {
                ZooControl control = Instantiate(GetControls.Controls[i].OverrideControl, ControlsPivot);

                control.Anim = anim;
                control.AnimID = Animator.StringToHash(GetControls.Controls[i].ControlName);
                control.TitleText.text = GetControls.Controls[i].ControlName;
                continue;
            }

            CreateControl(GetControls.Controls[i].ControlType, GetControls.Controls[i].ControlName, Animator.StringToHash(GetControls.Controls[i].ControlName), animRef);
        }
    }

    void CreateControl(AnimationControlType type, string animName, int animID, Animator anim) {
        ZooControl control = Instantiate(ControlLookup[type], ControlsPivot);

        control.Anim = anim;
        control.AnimID = animID;
        control.TitleText.text = animName;
    }
}

public enum AnimationControlType {
    Integer,
    Float,
    Bool,
    Trigger,
    OVERRIDE
}
