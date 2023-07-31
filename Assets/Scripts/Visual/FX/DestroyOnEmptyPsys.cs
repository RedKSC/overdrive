using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEmptyPsys : MonoBehaviour {
    public ParticleSystem psys;
    float delay = 5;

    void Update() {
        if (delay > 0)
            delay--;
        else {

            if (psys.particleCount == 0) {
                psys.Stop();
                Destroy(gameObject);
            }
        }
    }
    private void OnEnable() {
        psys.Play();
    }
}
