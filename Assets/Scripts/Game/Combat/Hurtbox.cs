using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public ODEntity entity;
    public void Awake() {
        entity = GetComponentInParent<ODEntity>();
    }
    public void OnTriggerEnter2D(Collider2D collision) {
        Hitbox hb = collision.GetComponent<Hitbox>();
        if(hb) {
            entity.OnHurt(hb.damage, hb.knockbackSpd, hb.stunTime, hb.type, hb.burn, hb.knockbackInfluence);
            hb.OnHit(entity);
        }
    }
}
