using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LutTester : MonoBehaviour {
    [Range(0f, 1f)]
    public float Input;
    public int ReferenceIndex;
    public Vector2 Output;

    private void OnDrawGizmos() {
        ReferenceIndex = Mathf.RoundToInt(Input * (SinCosinLookup.SinLookup.Length - 1));
        Output = Input.ConvertToVector(1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere((Vector2)transform.position + Output, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere((Vector2)transform.position + new Vector2(Mathf.Cos(Input * (Mathf.PI * 2f)), Mathf.Sin(Input * (Mathf.PI * 2f))), 0.1f);
    }
}
