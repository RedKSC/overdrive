using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadBullet : MonoBehaviour
{
    public float speed;
    public float angle; // in degrees
    public float lifeTime;
    float birthTime;

    public GameObject payload;
    public virtual void Start() {
        birthTime = GameManager.Instance.unpausedTime;
    }
    public virtual void Update() {
        transform.position += GameMath.ConvertToVector(angle).ConvertTo3D() * Time.deltaTime * speed;
        if (GameManager.Instance.timeSince(birthTime) > lifeTime) {
            DeliverPayload();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<Hurtbox>()) {
            DeliverPayload();
        }
    }

    public void DeliverPayload() {
        Instantiate(payload, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
