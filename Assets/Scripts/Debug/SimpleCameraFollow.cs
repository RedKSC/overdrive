using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour {
    public Transform Target;

    private void Update() {
        transform.position = new Vector3(Target.position.x, Target.position.y, -10f);
    }
}