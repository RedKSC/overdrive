using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnSoundEnd : MonoBehaviour
{
    public AudioSource audioSource;
    float start;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        start = Time.time;
    }
    void Update() {
        if (Time.time - start >= audioSource.clip.length)
            Destroy(gameObject);
    }

}
