using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnAnimEnd : MonoBehaviour
{
    public void destroySelf() {
        Destroy(gameObject);
    }
}
