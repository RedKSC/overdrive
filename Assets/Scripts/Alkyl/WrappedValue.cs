using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappedValue {
    public WrappedValue(float min, float max) {
        MinValue = min;
        MaxValue = max;
    }

    public WrappedValue(float min, float max, float value) {
        MinValue = min;
        MaxValue = max;
        this.Value = value;
    }

    public float MinValue;

    public float MaxValue;

    public Action<bool> OnWrapAction { get; private set; }

    public float Value {
        get {
            return val;
        }
        set {
            val = value.Wrap(MinValue, MaxValue);
            
            if(OnWrapAction != null && (value > MaxValue || value < MinValue)){
                OnWrapAction.Invoke(value < MinValue);
            }
        }
    }

    float val;

    public void SetOnWrapAction(Action<bool> action) => OnWrapAction = action;

    #region Self Operations
    public static bool operator ==(WrappedValue lhs, WrappedValue rhs) => lhs.Value == rhs.Value;
    public static bool operator !=(WrappedValue lhs, WrappedValue rhs) => lhs.Value != rhs.Value;
    public static bool operator >(WrappedValue lhs, WrappedValue rhs) => lhs.Value > rhs.Value;
    public static bool operator <(WrappedValue lhs, WrappedValue rhs) => lhs.Value < rhs.Value;
    public static bool operator >=(WrappedValue lhs, WrappedValue rhs) => lhs.Value >= rhs.Value;
    public static bool operator <=(WrappedValue lhs, WrappedValue rhs) => lhs.Value <= rhs.Value;

    public static WrappedValue operator +(WrappedValue lhs, WrappedValue rhs) {
        lhs.Value += rhs.Value;
        return lhs;
    }
    public static WrappedValue operator -(WrappedValue lhs, WrappedValue rhs) {
        lhs.Value -= rhs.Value;
        return lhs;
    }
    public static WrappedValue operator *(WrappedValue lhs, WrappedValue rhs) {
        lhs.Value *= rhs.Value;
        return lhs;
    }
    public static WrappedValue operator /(WrappedValue lhs, WrappedValue rhs) {
        lhs.Value /= rhs.Value;
        return lhs;
    }
    #endregion

    #region Float Operations
    public static bool operator ==(WrappedValue lhs, float rhs) => lhs.Value == rhs;
    public static bool operator !=(WrappedValue lhs, float rhs) => lhs.Value != rhs;
    public static bool operator >(WrappedValue lhs, float rhs) => lhs.Value > rhs;
    public static bool operator <(WrappedValue lhs, float rhs) => lhs.Value < rhs;
    public static bool operator >=(WrappedValue lhs, float rhs) => lhs.Value >= rhs;
    public static bool operator <=(WrappedValue lhs, float rhs) => lhs.Value <= rhs;

    public static WrappedValue operator +(WrappedValue lhs, float rhs) {
        lhs.Value += rhs;
        return lhs;
    }
    public static WrappedValue operator -(WrappedValue lhs, float rhs) {
        lhs.Value -= rhs;
        return lhs;
    }
    public static WrappedValue operator *(WrappedValue lhs, float rhs) {
        lhs.Value *= rhs;
        return lhs;
    }
    public static WrappedValue operator /(WrappedValue lhs, float rhs) {
        lhs.Value /= rhs;
        return lhs;
    }

    public static bool operator ==(float lhs, WrappedValue rhs) => lhs == rhs.Value;
    public static bool operator !=(float lhs, WrappedValue rhs) => lhs != rhs.Value;
    public static bool operator >(float lhs, WrappedValue rhs) => lhs > rhs.Value;
    public static bool operator <(float lhs, WrappedValue rhs) => lhs < rhs.Value;
    public static bool operator >=(float lhs, WrappedValue rhs) => lhs >= rhs.Value;
    public static bool operator <=(float lhs, WrappedValue rhs) => lhs <= rhs.Value;

    public static WrappedValue operator +(float lhs, WrappedValue rhs) {
        rhs.Value += lhs;
        return rhs;
    }
    public static WrappedValue operator -(float lhs, WrappedValue rhs) {
        rhs.Value -= lhs;
        return rhs;
    }
    public static WrappedValue operator *(float lhs, WrappedValue rhs) {
        rhs.Value *= lhs;
        return rhs;
    }
    public static WrappedValue operator /(float lhs, WrappedValue rhs) {
        rhs.Value /= lhs;
        return rhs;
    }
    #endregion

    #region Int Operations
    public static bool operator ==(WrappedValue lhs, int rhs) => lhs.Value == rhs;
    public static bool operator !=(WrappedValue lhs, int rhs) => lhs.Value != rhs;
    public static bool operator >(WrappedValue lhs, int rhs) => lhs.Value > rhs;
    public static bool operator <(WrappedValue lhs, int rhs) => lhs.Value < rhs;
    public static bool operator >=(WrappedValue lhs, int rhs) => lhs.Value >= rhs;
    public static bool operator <=(WrappedValue lhs, int rhs) => lhs.Value <= rhs;

    public static WrappedValue operator +(WrappedValue lhs, int rhs) {
        lhs.Value += rhs;
        return lhs;
    }
    public static WrappedValue operator -(WrappedValue lhs, int rhs) {
        lhs.Value -= rhs;
        return lhs;
    }
    public static WrappedValue operator *(WrappedValue lhs, int rhs) {
        lhs.Value *= rhs;
        return lhs;
    }
    public static WrappedValue operator /(WrappedValue lhs, int rhs) {
        lhs.Value /= rhs;
        return lhs;
    }

    public static bool operator ==(int lhs, WrappedValue rhs) => lhs == rhs.Value;
    public static bool operator !=(int lhs, WrappedValue rhs) => lhs != rhs.Value;
    public static bool operator >(int lhs, WrappedValue rhs) => lhs > rhs.Value;
    public static bool operator <(int lhs, WrappedValue rhs) => lhs < rhs.Value;
    public static bool operator >=(int lhs, WrappedValue rhs) => lhs >= rhs.Value;
    public static bool operator <=(int lhs, WrappedValue rhs) => lhs <= rhs.Value;

    public static WrappedValue operator +(int lhs, WrappedValue rhs) {
        rhs.Value += lhs;
        return rhs;
    }
    public static WrappedValue operator -(int lhs, WrappedValue rhs) {
        rhs.Value -= lhs;
        return rhs;
    }
    public static WrappedValue operator *(int lhs, WrappedValue rhs) {
        rhs.Value *= lhs;
        return rhs;
    }
    public static WrappedValue operator /(int lhs, WrappedValue rhs) {
        rhs.Value /= lhs;
        return rhs;
    }
    #endregion

    #region Casts
    public static implicit operator float(WrappedValue input) => input.Value;

    public static implicit operator int(WrappedValue input) => (int)input.Value;
    public static explicit operator WrappedValue(float input) => new WrappedValue(float.MinValue, float.MaxValue, input);
    public static explicit operator WrappedValue(int input) => new WrappedValue(float.MinValue, float.MaxValue, input);
    #endregion

    public override bool Equals(object obj) {
        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override string ToString() {
        return Value.ToString();
    }
}