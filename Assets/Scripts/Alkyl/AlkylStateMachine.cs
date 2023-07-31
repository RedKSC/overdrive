using System.IO.Enumeration;
using System.Data;
using System.Reflection;
using System.Net.NetworkInformation;
using System;
using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AlkylEntity))]
public class AlkylStateMachine : MonoBehaviour {
    public string EntityName;

    const string StateFormat = "State_*_*";

    AlkylStateData dta;

    List<List<Action>> States = new List<List<Action>>();
    List<Action> GlobalStates = new List<Action>();
    AlkylEntity ent;

    //Thanks ChatGPT (not kidding)
    public static bool MatchFormat(string format, string input) {
        // Check if the input string is empty
        if (string.IsNullOrEmpty(input)) {
            return false;
        }

        // Split the format string and the input string into parts
        string[] formatParts = format.Split('_');
        string[] inputParts = input.Split('_');

        // Check if the number of parts in the format string and the input string are the same
        if (formatParts.Length != inputParts.Length) {
            return false;
        }

        // Check if each part in the input string matches the corresponding part in the format string
        for (int i = 0; i < formatParts.Length; i++) {
            if (formatParts[i] != "*" && formatParts[i] != inputParts[i]) {
                return false;
            }
        }

        return true;
    }

    private void Awake() {
        ent = GetComponent<AlkylEntity>();

        //Grab the state data
        dta = Resources.Load<AlkylStateData>("Entity State Data/" + EntityName);

        if(!dta){
            Debug.LogError($"[AlkylStateMachine@{gameObject.name}]::No state data found!(Did you get the name wronge?)", gameObject);
        }
    }

    private void Update() {
        //Yes it is really this easy after doing all that to bind
        int mode = ent.Mode.Clamp(0, States.Count - 1);
        int subMode = ent.SubMode.Clamp(0, States[mode].Count - 1);

        States[mode][subMode].Invoke();
    }

    public void BindModeToFunctions(object obj) {
        //Now the hard part
        //Using reflection I can grab all methods by name (THEY MUST BE PUBLIC MAKE SURE THAT ALL STATES ARE PUBLIC METHODS)
        //Then compare the name of the function against the format string using the function
        //If it matches I need to add it to a specific list then format that in a way that'll work in the dictionary

        //The dictionary here just keeps track of how many sub-states exist for a given state
        Dictionary<string, int> stateLookup = new Dictionary<string, int>();

        foreach(var method in obj.GetType().GetMethods()) {
            if(!FileSystemName.MatchesSimpleExpression(StateFormat, method.Name)) {
                continue;
            }

            string stateName = method.Name.Replace("State_", "");
            stateName = stateName.Replace(stateName.Substring(stateName.LastIndexOf('_')), "");

            if(!stateLookup.ContainsKey(stateName)){
                stateLookup.Add(stateName, 0);
            }

            stateLookup[stateName]++;
        }

        //Now we can actually bind the methods to the mode
        foreach(string s in dta.States){
            if(!stateLookup.ContainsKey(s)){
                Debug.LogWarning($"[AlkylStateMachine@{gameObject.name}]::Failed to bind state '{s}'");
                continue;
            }

            States.Add(new List<Action>());

            for(int i = 0; i < stateLookup[s]; i++){
                Action action = (Action)obj.GetType().GetMethod($"State_{s}_{i}").CreateDelegate(typeof(Action), obj);
                States[States.Count - 1].Add(action);
            }
        }
    }
}