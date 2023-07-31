using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderRing : MonoBehaviour
{
    public float currentRotSpeed;
    public float rotSpeed;
    public float fireSpeed;

    public PlayerController playerTarget;
    public GameObject normalBullet;
    public GameObject stunBullet;

    public void Start() {
        playerTarget = PlayerController.Instance;
    }

    public void Update() {
        currentRotSpeed = Mathf.Lerp(currentRotSpeed, rotSpeed, 0.9f * Time.deltaTime * 60);
        transform.Rotate(new Vector3(0, 0, currentRotSpeed * Time.deltaTime));
    }

    public void Fire() {
        for(int i = 0; i < 4; i++) {
            Vector2 offset = new Vector2(Mathf.Cos((transform.rotation.eulerAngles.z + 90*i) * Mathf.Deg2Rad), Mathf.Sin((transform.rotation.eulerAngles.z + 90 * i) * Mathf.Deg2Rad));
            Vector2 vel = offset * fireSpeed;
            SimpleBullet bullet = Instantiate(normalBullet, transform.position + new Vector3(offset.x, offset.y, 0), Quaternion.identity).GetComponent<SimpleBullet>();
            bullet.velocity = vel;
        }
    }

    public void StunFire(int dir) {
        for (int i = 0; i < 2; i++) {
            Vector2 offset = new Vector2(Mathf.Cos((transform.rotation.eulerAngles.z + 180 * i + dir * 90) * Mathf.Deg2Rad), Mathf.Sin((transform.rotation.eulerAngles.z + 180 * i + dir * 90) * Mathf.Deg2Rad));
            Vector2 vel = offset * fireSpeed;
            ThunderRockStunbomb bullet = Instantiate(stunBullet, transform.position + new Vector3(offset.x, offset.y, 0), Quaternion.identity).GetComponent<ThunderRockStunbomb>();
            bullet.velocity = vel;
        }
    }
}
