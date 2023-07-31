using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public bool taggedForCapture;
    void Start()
    {
        CensusTaker.Instance.planets.Add(this);
    }

    private void OnDestroy() {
        CensusTaker.Instance.planets.Remove(this);
    }
}
