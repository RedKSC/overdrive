using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderAnimScripts : MonoBehaviour
{
    public ODEntity lander;
    public void Awake() {
        lander = GetComponentInParent<ODEntity>();
    }
    public void AnimFunc1() {
        lander.AnimFunc1();
    }
}
