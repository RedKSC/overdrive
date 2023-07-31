using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCopyLoop : MonoBehaviour {
    public float ParallaxAmount;

    SpriteRenderer sprite;

    Vector2[] LoopPositions;
    Camera cam;

    Vector2 getOffset(int ID) {
        switch(ID){
            case 0:
            return new Vector2(-1f, 1f);
            case 1:
            return new Vector2(0f, 1f);
            case 2: 
            return new Vector2(1f, 1f);
            case 3:
            return new Vector2(-1f, 0f);
            case 4:
            return new Vector2(1f, 0f);
            case 5:
            return new Vector2(-1f, -1f);
            case 6:
            return new Vector2(0f, -1f);
            case 7:
            return new Vector2(1f, -1f);
        }
        return Vector2.zero;
    }

    private void Awake() {
        cam = Camera.main;
        sprite = GetComponent<SpriteRenderer>();
        LoopPositions = new Vector2[9];

        LoopPositions[0] = transform.position;

        for(int i = 1; i < LoopPositions.Length; i++) {
            LoopPositions[i] = (Vector2)transform.position + (Playfield.Instance.PlayfieldSize * getOffset(i - 1));
        }
    }

    private void LateUpdate() {
        float lastDist = float.MaxValue;
        int index = 0;
        for(int i = 0; i < LoopPositions.Length; i++) {
            float dist = Vector2.Distance(LoopPositions[i], cam.transform.position);
            if(dist < lastDist) {
                lastDist = dist;
                index = i;
            }
        }

        Vector2 parallax = ((Vector2)cam.transform.InverseTransformPoint(LoopPositions[index]) * ParallaxAmount);

        transform.position = LoopPositions[index] + parallax;
    }
}