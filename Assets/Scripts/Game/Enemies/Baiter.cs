using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Baiter : ODEnemy
{
    [FoldoutGroup("MovementParams")] public float hSpd;
    [FoldoutGroup("MovementParams")] public float hAccel;
    [FoldoutGroup("MovementParams")] public float vSpd;
    [FoldoutGroup("MovementParams")] public float vAccel;
    [FoldoutGroup("MovementParams")] public float randomMovementMax;
    [FoldoutGroup("MovementParams")] public Vector2 randomMovementVariance;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireSpeed;

    [FoldoutGroup("CombatVars")] public float fireTime;

    [FoldoutGroup("Resources")] public GameObject normalBullet;

    CensusTaker cs;

    PlayerController playerTarget;
    float randomMovementLast;

    public override void Awake() {
        base.Awake();

    }
    public override void Start() {
        base.Start();
        cs = CensusTaker.Instance;
    }
    public override void Update() {
        base.Update();

        switch(Mode) {
            case 0:
                State_Normal();
                break;
        }
    }

    public override void OnDestroy() {
        base.OnDestroy();
        
    }

    public void State_Normal() {
        playerTarget = PlayerController.Instance;
        if (playerTarget) { //If we have a target, move towards the target
            Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), playerTarget.transform.position.ConvertTo2D());

            velocity.x = Mathf.MoveTowards(velocity.x, Mathf.Sign(offset.x) * hSpd, hAccel * Time.deltaTime);
            velocity.y = Mathf.MoveTowards(velocity.y, Mathf.Sign(offset.y) * vSpd, vAccel * Time.deltaTime);
        }


        if (GameManager.Instance.timeSince(fireTime) >= fireRate) { //Fire at the player sometimes
            fireTime = GameManager.Instance.unpausedTime + Random.Range(-0.4f, 0.4f);
            anim.SetTrigger("Fire");
        }
    }

    public override void AnimFunc1() {
        if (!playerTarget) {
            return;
        }
        Vector2 vel = new Vector2(playerTarget.transform.position.x - transform.position.x, playerTarget.transform.position.y - transform.position.y).normalized * fireSpeed;
        SimpleBullet bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;
    }
}
