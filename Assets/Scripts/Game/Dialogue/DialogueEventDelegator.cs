using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEventDelegator : MonoBehaviour
{
    public static DialogueEventDelegator Instance;
    public bool talking;

    void OnConversationEnd(Transform actor) {
        talking = false;
    }
    void OnConversationStart(Transform actor) {
        talking = true;
    }

    public void Awake() {
        Instance = this;
    }
}
