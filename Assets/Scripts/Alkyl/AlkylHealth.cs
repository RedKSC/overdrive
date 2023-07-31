using System;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

//[RequireComponent(typeof(AlkylEntity))]
#if ODIN_INSPECTOR
public class AlkylHealth : SerializedMonoBehaviour {
#else
public class AlkylHealth : MonoBehaviour {
#endif  
    //[SerializeField, ShowIf("DrawDebug"), ReadOnly]
    Dictionary<int, DamageTag> damageTags = new Dictionary<int, DamageTag>();

    private void Update() {
        foreach (KeyValuePair<int, DamageTag> tag in damageTags) {
            tag.Value.UpdateTag();
        }
    }
    

    /// <summary>
    ///Adds on to a damage tag.
    /// </summary>
    /// <param name="tag">
    /// The tag data
    /// </param>
    public void AddTag(DamageTag tag) => AddTag(tag.ID, tag.Amount);

    public void CreateTag(int ID, float initialAmount) => CreateTag(ID, initialAmount, null, null);

    public void CreateTag(int ID, float initialAmount, Action<DamageTag> UpdateAction, Action<DamageTag> OnValueChangedAction){
        damageTags.Add(ID, new DamageTag(initialAmount, ID));
        damageTags[ID].UpdateAction = UpdateAction;
        damageTags[ID].OnValueChangedAction = OnValueChangedAction;
    }
    
    /// <summary>
    ///Adds on to a tag by the given DamageTag and Amount
    /// </summary>
    /// <param name="tag">
    /// The tag to add on to
    /// </param>
    /// <param name="Amount">
    /// The amount to add on to
    /// </param>
    public void AddTag(int tag, float Amount){
        damageTags[tag] += Amount;
    }
    
    /// <summary>
    ///Adds multiple damage tags at once.
    /// </summary>
    /// <param name="tags">
    /// The tags to add on to.
    /// </param>
    public void AddTags(DamageTag[] tags){
        foreach(DamageTag tag in tags){
            AddTag(tag);
        }
    }

    public void MultiplyTag(int type, float Amount) {
        damageTags[type] *= Amount;
    }
    
    /// <summary>
    ///Hard sets a tags value, optionally also triggers tag changed event
    /// </summary>
    /// <param name="tag">
    /// The tage to set
    /// </param>
    /// <param name="Amount">
    /// The new value
    /// </param>
    public void SetTag(int tag, float Amount){
        damageTags[tag].SetAmount(Amount);
    }

    /// <summary>
    /// Gets the amount a certian tag has
    /// </summary>
    /// <param name="type">
    /// The tag to get
    /// </param>
    /// <returns>
    /// The given tag
    /// </returns>
    public DamageTag GetTag(int type) => damageTags[type];
}