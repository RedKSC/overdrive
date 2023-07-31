using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    SpriteRenderer sprite;
    public CircleCollider2D coll;
    float birthTime;
    public float maxTime;
    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<CircleCollider2D>();
    }
    void Start()
    {
        birthTime = GameManager.Instance.unpausedTime;
    }

    void Update()
    {
        Color col = sprite.color;
        sprite.color = new Color(col.r, col.g, col.b, 1-GameManager.Instance.timeLerp(birthTime, maxTime));
        if (GameManager.Instance.timeSince(birthTime) > 0.1f) {
            coll.enabled = false;
        }
        if (GameManager.Instance.timeSince(birthTime) >= maxTime) {
            Destroy(gameObject);
        }
    }
}
