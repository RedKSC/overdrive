using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlkylEntity : MonoBehaviour {
    public int Type;
    public int SubType;
    public int Mode{
        get{
            return mode;
        }
        set{
            if (Mode != value) {
                PrevMode = mode;
                mode = value;
            }
        }
    }

    public int SubMode{
        get{
            return subMode;
        }
        set{
            PrevSubMode = subMode;
            subMode = value;
        }
    }

    int mode;
    int subMode;

    public int PrevMode { get; private set; }
    public int PrevSubMode { get; private set; }

    /* RESERVED MODES
     * 4 = Dead
     * 5 = Stunned
     */

    public AlkylHealth Health { get; private set; }
    public AnimatorPassthrough Passthrough { get; private set; }
    public AlkylStateMachine StateMachine { get; private set; }

    public delegate void OnHealthSetupEvent();
    public delegate void OnAnimatorPassthroughSetupEvent();
    public delegate void OnStateMachineSetupEvent();

    public event OnHealthSetupEvent OnHealthSetup;
    public event OnAnimatorPassthroughSetupEvent OnAnimatorSetup;
    public event OnStateMachineSetupEvent OnStateMachineSetup;

    public virtual void Awake() {
        Health = GetComponent<AlkylHealth>();
        Passthrough = GetComponent<AnimatorPassthrough>();
        StateMachine = GetComponent<AlkylStateMachine>();

        if (Health) {
            HealthSetup();
        }

        if (Passthrough) {
            AnimatorSetup();
        }
    }

    public virtual void Start() {
        if(StateMachine){
            StateMachineSetup();
        }
    }

    [ObsoleteAttribute("You can just set Mode now")]
    public void SetMode(int mode) {
        if (mode != Mode) {
            PrevMode = Mode;
        }
        Mode = mode;
    }

    public void RevertMode() {
        Mode = PrevMode;
    }

    public virtual void HealthSetup() {
        OnHealthSetup?.Invoke();
     }
    public virtual void AnimatorSetup() { 
        OnAnimatorSetup?.Invoke();
    }
    public virtual void StateMachineSetup() {
        OnStateMachineSetup?.Invoke();
     }
}