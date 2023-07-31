using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShield : PlayerWeaponBase
{
    public GameObject normalBullet;
    public GameObject currentShield;

    public override void SpawnBullet() {
        base.SpawnBullet();

        Transform playertrans = PlayerController.Instance.transform;
        TimedTrackingObject bullet = Instantiate(normalBullet, playertrans.position, Quaternion.identity).GetComponent<TimedTrackingObject>();
        bullet.follow = playertrans;

    }
}
