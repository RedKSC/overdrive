using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolSpawner : MonoBehaviour {
    public float SpawnTime;

    float timer;

    private void Update() {
        if (timer < SpawnTime) {
            timer += Time.deltaTime;
            return;
        }

        VFXObjectPool.SpawnVFX("Test-Obj", transform.position);
        timer = 0f;
    }
}
