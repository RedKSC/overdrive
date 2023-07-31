using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceSprite : MonoBehaviour
{
    public int side;

    public GameObject burst;
    float birthTime;

    [HideInInspector] public bool cancelExplosion;
    public void Awake() {
        burst = Resources.Load<GameObject>("Burst FX");
    }
    public void Start() {
        birthTime = GameManager.Instance.unpausedTime;
    }
    void Update()
    {
        transform.position += Vector3.up * side * Time.deltaTime * 2 * GameManager.Instance.timeLerp(birthTime, 0.25f);
        if (GameManager.Instance.timeSince(birthTime) > 0.25f) {
            Instantiate(burst, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
