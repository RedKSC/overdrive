using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderDrum : PlayerWeaponBase
{
    public GameObject normalBullet;
    public float bulletSpeed;

    public override void SpawnBullet() {
        base.SpawnBullet();

        Transform playertrans = PlayerController.Instance.transform;

        Vector3 pos = playertrans.position + new Vector3(playertrans.localScale.x * 7.5f / 16f, -1.5f / 16f, 0);
        Vector3 vel = Vector3.right * bulletSpeed;
        PayloadBullet bullet = Instantiate(normalBullet, pos, Quaternion.identity).GetComponent<PayloadBullet>();
        bullet.speed = bulletSpeed * playertrans.localScale.x;
        bullet.transform.localScale = new Vector3(1, 1, 1);

        audioSource.PlayOneShot(shootSound, 1);
    }
}
