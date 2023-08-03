using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {
    public float TimeToDestroy;

    float timer;

    private void Update() {
        if (timer < TimeToDestroy) {
            timer += Time.deltaTime;
            return;
        }

        Destroy(gameObject);
    }
}
