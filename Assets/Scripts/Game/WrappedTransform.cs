using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WrappedTransform : MonoBehaviour {
    public bool AutoSetLimits;

    [HideIf("AutoSetLimits"), SerializeField, FoldoutGroup("Limits"), LabelText("X")]
    Vector2 XLimits;
    [HideIf("AutoSetLimits"), SerializeField, FoldoutGroup("Limits"), LabelText("Y")]
    Vector2 YLimtits;
    [SerializeField, FoldoutGroup("Limits"), LabelText("Z")]
    Vector2 ZLimits = new Vector2(-1f, 1f);

    public WrappedVector3 Position;

    public delegate void OnXWrapEvent(bool lowToHigh);
    public delegate void OnYWrapEvent(bool lowToHigh);

    public event OnXWrapEvent OnXWrap;
    public event OnYWrapEvent OnYWrap;

    public delegate void OnLateUpdateEvent();
    public event OnLateUpdateEvent OnLateUpdate;

    Vector3 lastPos;

    private void Awake() {
        if(AutoSetLimits) {
            PlayfieldSettings settings = Playfield.Instance.Settings;
            XLimits = new Vector2(settings.TopLeftCorner.x, settings.BottomRightCorner.x);
            YLimtits = new Vector2(settings.BottomRightCorner.y, settings.TopLeftCorner.y);
        }

        Position = new WrappedVector3(XLimits, YLimtits, ZLimits);
        Position.Vector = transform.position;
        lastPos = transform.position;
        Position.SetupOnWrapActions(
            (bool lowToHigh) => {
                OnXWrap?.Invoke(lowToHigh);
            },
            (bool lowToHigh) => {
                OnYWrap?.Invoke(lowToHigh);
            },
            null
        );
    }

    private void Update() {
        if(lastPos != transform.position){
            Position.Vector = transform.position;
            lastPos = transform.position;
        }
    }

    public void LateUpdate() {
        transform.position = Position.Vector;
        OnLateUpdate?.Invoke();
    }
}