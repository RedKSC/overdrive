using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebreath : PlayerWeaponBase
{
    public GameObject normalBullet;
    public float bulletSpeed;

    public override void SpawnBullet() {
        base.SpawnBullet();

        Transform playertrans = PlayerController.Instance.transform;
        Vector3 pos = playertrans.position + new Vector3(playertrans.localScale.x * 7.5f / 16f, -1.5f / 16f, 0);
        Vector3 vel = Vector3.right * playertrans.localScale.x * bulletSpeed * 0.7f * (0.5f + (weaponData.level+1) * 0.5f);
        SimpleBullet bullet = Instantiate(normalBullet, pos, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;
        bullet.transform.localScale = playertrans.localScale;
        bullet.damage = 0.1f * (weaponData.level+1);
        bullet.burn = 0.35f * (weaponData.level+1);

        audioSource.PlayOneShot(shootSound, 1);
    }
}
