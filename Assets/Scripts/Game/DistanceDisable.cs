using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceDisable : MonoBehaviour {
    public GameObject[] Disables;

    Transform Cam;
    bool inRange{
        get{
            
            Vector2 size = Playfield.Instance.PlayfieldSize;
            Vector2 XMinMax = new Vector2(Playfield.Instance.Settings.TopLeftCorner.x, Playfield.Instance.Settings.BottomRightCorner.x);
            Vector2 YMinMax = new Vector2(Playfield.Instance.Settings.BottomRightCorner.y, Playfield.Instance.Settings.TopLeftCorner.y);
            
            return Vector2.Distance(((Vector2)transform.position).Wrap(XMinMax, YMinMax), Cam.position) <= 15f;
        }
    }
    bool active = true;

    private void Awake() {
        Cam = Camera.main.transform;
    }

    private void Update() {
        if(active == inRange){
            return;
        }

        active = inRange;

        for(int i = 0; i < Disables.Length; i++){
            Disables[i].SetActive(active);
        }
    }
}
