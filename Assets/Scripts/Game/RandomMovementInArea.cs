using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RandomMovementInArea : MonoBehaviour{

    public Vector2 Speed, Factor;



    void Update() {
        float x = (Mathf.PerlinNoise(Time.time * Speed.x, transform.position.y) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(transform.position.x, Time.time * Speed.y) - 0.5f) * 2f;
        transform.localPosition = new Vector2(x * Factor.x, y * Factor.y);
    }
}
