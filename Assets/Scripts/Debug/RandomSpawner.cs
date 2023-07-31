using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour {
    public int Count;
    public float Size;
    public GameObject Prefab;

    private void Awake() {
        for(int i = 0; i < Count; i++){
            Instantiate(Prefab, Random.insideUnitCircle * Size, Quaternion.identity);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Size);
    }
}
