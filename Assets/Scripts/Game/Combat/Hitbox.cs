using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType {
    Blast,
    Slice
}
public class Hitbox : MonoBehaviour
{
    public float damage;
    public bool relativeKnockback;
    public Vector2 knockbackSpd;
    public float knockbackInfluence;
    public float stunTime;
    public float burn;
    public DamageType type;
    public bool DestroyProjectiles;
    public bool DestroyIfNotKilled; //destroy bullet if enemy isn't killed
    public virtual void OnHit(ODEntity entity) {
        if(DestroyIfNotKilled && entity.simpleHealth > 0) {
            Destroy(gameObject);
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision) {
        
        
    }
}
