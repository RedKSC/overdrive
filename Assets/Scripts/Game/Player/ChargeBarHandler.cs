using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class ChargeBarHandler : MonoBehaviour {
    public Transform BarParent;

    [AssetsOnly]
    public GameObject ChargeBar;

    Dictionary<int, ChargeBarState> barStates = new Dictionary<int, ChargeBarState>();
    List<int> chargeBarLookup = new List<int>();

    private void Update() {
        BarParent.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1f, 1f);
        for(int i = 0; i < chargeBarLookup.Count; i++) {
            if(barStates[chargeBarLookup[i]].LifePercent <= 0f || (barStates[chargeBarLookup[i]].Type == ChargeBarType.Percent && barStates[chargeBarLookup[i]].LifePercent >= 0.99f)) {
                Destroy(barStates[chargeBarLookup[i]].Bar.transform.parent.gameObject);
                barStates.Remove(chargeBarLookup[i]);
                chargeBarLookup.Remove(chargeBarLookup[i]);
                continue;
            }

            if(barStates[chargeBarLookup[i]].Type == ChargeBarType.Timed){
                barStates[chargeBarLookup[i]].TimeLeft -= Time.deltaTime;
            }

            barStates[chargeBarLookup[i]].Bar.transform.localScale = new Vector3(barStates[chargeBarLookup[i]].LifePercent, 1f, 1f);
        }
    }

    public bool UpdateChargeBar(int barID, float amount) {
        if(!barStates.ContainsKey(barID)){
            return false;
        }

        barStates[barID].TimeLeft = amount;
        return true;
    }

    public void AddTimedChargeBar(float time, Color color) {
        ChargeBarState state = new ChargeBarState(){
            Type = ChargeBarType.Timed,
            TimeLeft = time,
            TimeAlive = time,
            Bar = Instantiate(ChargeBar, BarParent).transform.GetChild(0).gameObject,
        };

        state.Bar.GetComponent<Image>().color = color;

        int barID = Random.Range(int.MinValue, int.MaxValue);

        chargeBarLookup.Add(barID);
        barStates.Add(barID, state);
    }

    public int AddChargeBar(Color color, float currentAmount, float maxAmount) { 
        if(currentAmount <= 0f || currentAmount >= 0.99f) {
            return -1;
        }
        ChargeBarState state = new ChargeBarState() {
            Type = ChargeBarType.Percent,
            TimeLeft = currentAmount,
            TimeAlive = maxAmount,
            Bar = Instantiate(ChargeBar, BarParent).transform.GetChild(0).gameObject,
        };

        state.Bar.GetComponent<Image>().color = color;
        
        int barID = Random.Range(int.MinValue, int.MaxValue);

        chargeBarLookup.Add(barID);
        barStates.Add(barID, state);

        return barID;
    }
}

public class ChargeBarState {
    public ChargeBarType Type;
    public float TimeLeft;
    public float TimeAlive;
    public float LifePercent => TimeLeft / TimeAlive;
    public GameObject Bar;
}

public enum ChargeBarType{
    Timed,
    Percent
}