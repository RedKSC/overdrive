using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBullet : Hitbox
{
    public float knockbackMult;
    public Vector2 velocity;
    public float lifeTime;
    public bool destroyOnHit;
    [HideInInspector] public float birthTime;
    Hitbox hitbox;

    public float t;
    public float amplitude;
    public float period;
    public float direction; //in degrees

    private void Start() {
        birthTime = GameManager.Instance.unpausedTime;
        hitbox = GetComponent<Hitbox>();
    }
    public virtual void Update()
    {
        hitbox.knockbackSpd = velocity * GameManager.Instance.gm2unityConv * 2 * knockbackMult;
        transform.position += GameMath.ConvertTo3D(velocity) * Time.deltaTime;

        t += period * Time.deltaTime;

        transform.position += (GameMath.ConvertToVector(t) * GameMath.ConvertToVector(direction-90)).ConvertTo3D() * amplitude * Time.deltaTime;
        if(GameManager.Instance.timeSince(birthTime) > lifeTime) {
            Destroy(gameObject);
        }
    }
    public override void OnHit(ODEntity entity) {
        base.OnHit(entity);
        if(destroyOnHit) {
            Destroy(gameObject);
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision) {
        
        base.OnTriggerEnter2D(collision);
        Hitbox hb = collision.GetComponent<Hitbox>();
        if (hb && hb.DestroyProjectiles) {
            Destroy(gameObject);
        }
    }

}