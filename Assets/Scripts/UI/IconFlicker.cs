using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class IconFlicker : MonoBehaviour {
    public int FrameMax;

    Image img;
    int counter;
    int next;

    IEnumerator Flicker() {
        img.enabled = false;
        yield return null;
        img.enabled = true;    
    }

    void Awake() {
        img = GetComponent<Image>();
    }

    void Update() {
        if (counter < next) {
            counter++;
            return;
        }

        counter = 0;
        
        next = Random.Range(0, FrameMax);
        StartCoroutine(Flicker());
    }
}
