using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WrappedTransform))]
public class SimpleMovement : MonoBehaviour {
    public float Speed;

    private void Update() {
        transform.position += new Vector3(PlayerInput.GetAxis(PlayerInputKey.Horizontal), PlayerInput.GetAxis(PlayerInputKey.Vertical), 0f).normalized * Speed * Time.deltaTime;
    }
}