using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Mutant : ODEnemy
{
    [FoldoutGroup("MovementParams")] public float speed;
    [FoldoutGroup("MovementParams")] public float randomMovementMax;
    [FoldoutGroup("MovementParams")] public Vector2 randomMovementVariance;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireSpeed;
    [FoldoutGroup("CombatParams")] public float citizenGrabOffset;

    [FoldoutGroup("CombatVars")] public float fireTime;

    [FoldoutGroup("Resources")] public GameObject normalBullet;

    PlayerController playerTarget;
    Vector2 velDir;

    public override void Awake() {
        base.Awake();

        velDir.x = Mathf.Sign(Random.Range(-1f, 1f));
        velDir.y = Mathf.Sign(Random.Range(-1f, 1f));


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

            velDir.x = Mathf.Sign(offset.x);
            velDir.y = Mathf.Sign(offset.y);
        }
        

        if (GameManager.Instance.timeSince(fireTime) >= fireRate) { //Fire at the player sometimes
            fireTime = GameManager.Instance.unpausedTime + Random.Range(-0.4f, 0.4f);
            anim.SetTrigger("Fire");
        }

        velocity = velDir * speed;
    }

    public override void AnimFunc1() {
        if (!playerTarget) {
            return;
        }
        Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), PlayerController.Instance.transform.position.ConvertTo2D());

        Vector2 vel = offset.normalized * fireSpeed;
        SimpleBullet bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;
    }
}
