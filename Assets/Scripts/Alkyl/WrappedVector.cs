using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappedVector2 {
    public WrappedVector2(Vector2 xLimits, Vector2 yLimits) {
        xWrapped = new WrappedValue(xLimits.x, xLimits.y);
        yWrapped = new WrappedValue(yLimits.x, yLimits.y);
    }

    public float x{
        get{
            return xWrapped;
        }
        set{
            xWrapped.Value = value;;
        }
    }

    public float y{
        get{
            return yWrapped;
        }
        set{
            yWrapped.Value = value;;
        }
    }

    WrappedValue xWrapped;
    WrappedValue yWrapped;
}

public class WrappedVector3 {
    public WrappedVector3(Vector2 xLimits, Vector2 yLimits, Vector2 zLimits) {
        xWrapped = new WrappedValue(xLimits.x, xLimits.y);
        yWrapped = new WrappedValue(yLimits.x, yLimits.y);
        zWrapped = new WrappedValue(zLimits.x, zLimits.y);
        
        xWrapped.SetOnWrapAction(xWrap);
        yWrapped.SetOnWrapAction(yWrap);
        zWrapped.SetOnWrapAction(zWrap);
    }

    public Action<bool> OnXWrapAction { get; private set; }
    public Action<bool> OnYWrapAction { get; private set; }
    public Action<bool> OnZWrapAction { get; private set; }

    public float x {
        get{
            return xWrapped;
        }
        set{
            xWrapped.Value = value;
        }
    }

    public float y {
        get{
            return yWrapped;
        }
        set{
            yWrapped.Value = value;
        }
    }

    public float z {
        get{
            return zWrapped;
        }
        set{
            zWrapped.Value = value;
        }
    }

    public Vector3 Vector{
        get{
            return new Vector3(xWrapped, yWrapped, zWrapped);
        }
        set{
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }

    WrappedValue xWrapped;
    WrappedValue yWrapped;
    WrappedValue zWrapped;

    public void SetupOnWrapActions(Action<bool> onXWrap, Action<bool> onYWrap, Action<bool> onZWrap){
        OnXWrapAction = onXWrap;
        OnYWrapAction = onYWrap;
        OnZWrapAction = onZWrap;
    }

    void xWrap(bool lowToHight){
        if(OnXWrapAction != null){
            OnXWrapAction.Invoke(lowToHight);
        }
    }

    void yWrap(bool lowToHight){
        if(OnYWrapAction != null){
            OnYWrapAction.Invoke(lowToHight);
        }
    }

    void zWrap(bool lowToHight){
        if(OnZWrapAction != null){
            OnZWrapAction.Invoke(lowToHight);
        }
    }
}