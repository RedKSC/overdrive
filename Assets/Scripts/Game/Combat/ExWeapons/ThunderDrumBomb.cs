using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderDrumBomb : Hitbox
{
    float lastBurstTime;
    public float burstDelay;
    public int totalBursts;
    int bursts;

    public ParticleSystem psys;
    public Collider2D coll;
    public VisualObjectDupe dupe;

    public void Update() {
        coll.enabled = false;
        if(GameManager.Instance.timeSince(lastBurstTime) > burstDelay) {
            if(bursts > totalBursts) {
                Destroy(gameObject);
                return;
            }
            lastBurstTime = GameManager.Instance.unpausedTime;
            coll.enabled = true;
            psys.Play();
            for(int i = 0; i < dupe.copies.Length; i++) {
                dupe.copies[i].GetComponent<ParticleSystem>().Play();
            }
            bursts++;
        }
    }
}
