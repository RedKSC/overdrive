using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserKnife : PlayerWeaponBase
{
    public GameObject normalBullet;
    public float bulletSpeed;

    public override void SpawnBullet() {
        base.SpawnBullet();

        Transform playertrans = PlayerController.Instance.transform;

        Vector3 pos = playertrans.position + new Vector3(playertrans.localScale.x * 7.5f / 16f, -1.5f / 16f, 0);
        Vector3 vel = Vector3.right * bulletSpeed;
        Hitbox bullet = Instantiate(normalBullet, pos, Quaternion.identity).GetComponent<Hitbox>();
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 - 90 * playertrans.localScale.x));
        bullet.knockbackSpd = 2 * new Vector3(playertrans.localScale.x, 0, 0);
        bullet.transform.localScale = new Vector3(1, 1, 1);

        audioSource.PlayOneShot(shootSound, 1);
    }
}
