using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
//using Unity.VisualScripting;

//[IncludeInSettings(true)]
public class DamageTag {
    public int ID;

    public float Amount {
        get {
            return amount;
        }
        private set {
            Last = amount;
            amount = value;
            OnValueChanged?.Invoke(this);
            if(OnValueChangedAction != null) {
                OnValueChangedAction.Invoke(this);
            }
        }
    }

    public float Resistance = 1f;

    public float Last { get { return last; } private set { last = value; } }

    public delegate void OnValueChangeEvent(DamageTag tag);
    public delegate void OnUpdateEvent(DamageTag tag);

    public event OnValueChangeEvent OnValueChanged;
    public event OnUpdateEvent OnUpdate;
    public Action<DamageTag> UpdateAction;
    public Action<DamageTag> OnValueChangedAction;

    [SerializeField]
    float amount;
#if ODIN_INSPECTOR
    [SerializeField, ReadOnly]
#else
    [SerializeField]
#endif
    float last;

    /// <summary>
    ///Returns if the amount is greater than 0.
    /// </summary>
    public bool IsActive => Amount > 0f;
    
    /// <summary>
    ///Compares the tags amount to a given value.
    /// </summary>
    /// <param name="check">
    /// The amount to check by.
    /// </param>
    /// <returns>
    /// True if the Amount is greater than the given value.
    /// </returns>
    public bool Compare(float check) => Amount >= check;
    
    /// <summary>
    ///A new Damage Tag
    /// </summary>
    /// <param name="Amount">
    /// The initial amount of the tag
    /// </param>
    /// <param name="tag">
    /// The IS of the tag
    /// </param>
    public DamageTag(float Amount, int tag){
        this.Amount = Amount;
        ID = tag;
    }
    
    public DamageTag(){
        Amount = 0f;
    }
    
    /// <summary>
    ///Adds the given amount to the tag
    /// </summary>
    /// <param name="amount">
    /// The amount to add by.
    /// </param>
    /// <returns>
    /// The new amount of the tag.
    /// </returns>
    public float AddAmount(float amount) {
        if (amount > 0f) {
            amount *= Resistance;
        }
        Amount += amount;
        return Amount;
    }
    
    /// <summary>
    ///Sets the amount of the tag, overriding the previous value. Does not activate OnValueChanged event
    /// </summary>
    /// <param name="amount">
    /// The new amount of the tag.
    /// </param>
    public void SetAmount(float amount) => this.amount = amount;

    public void UpdateTag() {
        OnUpdate?.Invoke(this);
        UpdateAction.Invoke(this);
    }

    #region Operators
    //operators for calculations
    public static DamageTag operator +(DamageTag lhs, DamageTag rhs){
        lhs.AddAmount(rhs.Amount);
        return lhs;
    }
    
    public static DamageTag operator -(DamageTag lhs, DamageTag rhs) {
        lhs.AddAmount(-rhs.Amount);
        return lhs;
    }
    
    public static DamageTag operator +(DamageTag lhs, float rhs){
        lhs.AddAmount(rhs);
        return lhs;
    }
    
    public static DamageTag operator -(DamageTag lhs, float rhs){
        lhs.AddAmount(-rhs);
        return lhs;
    }
    
    public static DamageTag operator *(DamageTag lhs, float rhs){
        lhs.SetAmount(lhs.Amount * rhs);
        return lhs;
    }

    public static DamageTag operator *(float rhs, DamageTag lhs) {
        lhs.SetAmount(lhs.Amount * rhs);
        return lhs;
    }

    public static DamageTag operator /(DamageTag lhs, float rhs){
        lhs.SetAmount(lhs.Amount / rhs);
        return lhs;
    }

    public static bool operator >(DamageTag lhs, float rhs) => lhs.Amount > rhs;
    public static bool operator <(DamageTag lhs, float rhs) => lhs.Amount < rhs;
    public static bool operator >=(DamageTag lhs, float rhs) => lhs.Amount >= rhs;
    public static bool operator <=(DamageTag lhs, float rhs) => lhs.Amount <= rhs;
    public static bool operator !=(DamageTag lhs, float rhs) => lhs.Amount != rhs;
    public static bool operator ==(DamageTag lhs, float rhs) => lhs.Amount == rhs;
    public static bool operator >(float rhs, DamageTag lhs) => rhs > lhs.Amount;
    public static bool operator <(float rhs, DamageTag lhs) => rhs < lhs.Amount;
    public static bool operator >=(float rhs, DamageTag lhs) => rhs >= lhs.Amount;
    public static bool operator <=(float rhs, DamageTag lhs) => rhs <= lhs.Amount;
    public static bool operator !=(float rhs, DamageTag lhs) => rhs != lhs.Amount;
    public static bool operator ==(float rhs, DamageTag lhs) => rhs == lhs.Amount;
    #endregion

    public override bool Equals(object obj) {
        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override string ToString() {
        return Amount.ToString();
    }
}