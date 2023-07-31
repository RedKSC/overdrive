using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunwave : PlayerWeaponBase
{
    public GameObject normalBullet;
    public float bulletSpeed;

    public override void SpawnBullet() {
        base.SpawnBullet();

        Transform playertrans = PlayerController.Instance.transform;

        Vector3 pos = playertrans.position + new Vector3(playertrans.localScale.x * 7.5f / 16f, -1.5f / 16f, 0);
        Vector3 vel = Vector3.right * bulletSpeed * 0.75f;
        SimpleBullet bullet = Instantiate(normalBullet, pos, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;
        bullet.transform.localScale = new Vector3(1, 1, 1);
        bullet.damage = 0;

        vel = Vector3.right * -1f * bulletSpeed * 0.75f;
        bullet = Instantiate(normalBullet, pos, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;
        bullet.transform.localScale = new Vector3(-1, 1, 1);
        bullet.damage = 0;

        audioSource.PlayOneShot(shootSound, 1);
    }
}
