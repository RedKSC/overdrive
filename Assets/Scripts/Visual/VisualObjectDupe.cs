using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class VisualObjectDupe : MonoBehaviour {
    [AssetsOnly]
    public GameObject[] copies;

    //Hee hee funny look up table
    Vector2 getOffset(int ID) {
        switch (ID) {
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
        copies = new GameObject[8];

        Vector2 playfield = Playfield.Instance.PlayfieldSize;

        for (int i = 0; i < copies.Length; i++) {
            copies[i] = Instantiate(gameObject.transform.GetChild(0).gameObject, (Vector2)transform.position + (getOffset(i) * playfield), Quaternion.identity, transform);
        }
    }

    private void Update() {
        Vector2 playfield = Playfield.Instance.PlayfieldSize;

        for (int i = 0; i < copies.Length; i++) {
            copies[i].transform.localPosition = ( (getOffset(i) * playfield) / new Vector2(transform.localScale.x, transform.localScale.y)).ConvertTo3D();
        }
    }
}