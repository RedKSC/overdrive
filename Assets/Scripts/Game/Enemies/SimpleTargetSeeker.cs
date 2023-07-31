using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTargetSeeker : MonoBehaviour {
    public float Speed;

    PlayerController player;
    CensusTaker cs;

    private void Start() {
        cs = CensusTaker.Instance;
        player = PlayerController.Instance;
    }

    private void Update() {

        transform.position += cs.OffsetCyclical(transform.position.ConvertTo2D(), player.transform.position.ConvertTo2D()).ConvertTo3D() * Time.deltaTime * Speed;
    }
}