using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderRockStunbomb : MonoBehaviour
{
    public Vector2 velocity;
    public GameObject stunWave;
    public float birthTime;
    public float lifeTime;
    public float bulletSpeed;
    public Animator anim;
    void Start()
    {
        velocity = Vector2.MoveTowards(velocity, Vector2.zero, 0.3f * Time.deltaTime);
        birthTime = GameManager.Instance.unpausedTime;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        transform.position += velocity.ConvertTo3D() * Time.deltaTime;
        if (GameManager.Instance.timeSince(birthTime) > lifeTime * 0.8f) {
            anim.SetTrigger("AlmostExplode");
        }
        if (GameManager.Instance.timeSince(birthTime) > lifeTime) {
            Destroy(gameObject);
            Vector3 pos = transform.position;
            Vector3 vel = Vector3.right * bulletSpeed;
            SimpleBullet bullet = Instantiate(stunWave, pos, Quaternion.identity).GetComponent<SimpleBullet>();
            bullet.velocity = vel;
            bullet.transform.localScale = new Vector3(1, 1, 1);
            bullet.damage = 0;

            vel = Vector3.right * -1f * bulletSpeed;
            bullet = Instantiate(stunWave, pos, Quaternion.identity).GetComponent<SimpleBullet>();
            bullet.velocity = vel;
            bullet.transform.localScale = new Vector3(-1, 1, 1);
            bullet.damage = 0;
        }
    }
}
