using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawner : MonoBehaviour {
    public GameObject Star;
    public int StarCount;

    public void Awake(){
        Vector2 size = Playfield.Instance.PlayfieldSize / 2f;
        for(int i = 0; i < StarCount; i++){
            Instantiate(Star, new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * size, Quaternion.identity, transform);
        }
    }
}
