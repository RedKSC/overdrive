using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GreedyDrone : ODEnemy
{
    [FoldoutGroup("MovementParams")] public float hSpd;
    [FoldoutGroup("MovementParams")] public float hAccel;
    [FoldoutGroup("MovementParams")] public float vSpd;
    [FoldoutGroup("MovementParams")] public float vAccel;
    [FoldoutGroup("MovementParams")] public float randomMovementMax;
    [FoldoutGroup("MovementParams")] public Vector2 randomMovementVariance;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireSpeed;
    [FoldoutGroup("CombatParams")] public Hurtbox hb;

    [FoldoutGroup("CombatVars")] public float fireTime;

    [FoldoutGroup("Resources")] public GameObject normalBullet;
    [FoldoutGroup("Resources")] public Animator CannonAnim;
    [FoldoutGroup("Resources")] public Animator JetAnim;

    CensusTaker cs;

    PlayerController playerTarget;
    float randomMovementLast;

    Vector3 greedySpawnOffset;

    public override void Awake() {
        base.Awake();

    }
    public override void Start() {
        base.Start();
        cs = CensusTaker.Instance;
        Mode = 1;
        GameObject greedyTarg = Greedy.Instance.gameObject;
        greedySpawnOffset = new Vector3(transform.position.x - greedyTarg.transform.position.x, transform.position.y - greedyTarg.transform.position.y, 0);

        fireTime = GameManager.Instance.unpausedTime + 1;
    }
    public override void Update() {
        GameObject greedyTarg = Greedy.Instance.gameObject;
        if(greedyTarg == null) {
            Destroy(gameObject);
            return;
        }
        base.Update();

        switch(Mode) {
            case 0:
                State_Normal();
                break;
            case 1:
                State_Spawn();
                break;
        }
    }

    public override void OnDestroy() {
        base.OnDestroy();
        Greedy greedyTarg = Greedy.Instance;
        if(greedyTarg != null)
            greedyTarg.droneCurrentNum--;
    }
    public void State_Spawn() {
        GameObject greedyTarg = Greedy.Instance.gameObject;
        transform.position = greedyTarg.transform.position + greedySpawnOffset;
    } 
    public void State_Normal() {
        playerTarget = PlayerController.Instance;
        
        if (playerTarget) { //If we have a target, move towards the target
            Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), playerTarget.transform.position.ConvertTo2D());

            float circleRange = 4;

            if (offset.magnitude > circleRange) {
                velocity.x = Mathf.MoveTowards(velocity.x, Mathf.Sign(offset.x) * hSpd, hAccel * Time.deltaTime);
                velocity.y = Mathf.MoveTowards(velocity.y, Mathf.Sign(offset.y) * vSpd, vAccel * Time.deltaTime);
            }
            else {
                velocity.x = Mathf.MoveTowards(velocity.x, Mathf.Sign(offset.y) * hSpd, hAccel * Time.deltaTime);
                velocity.y = Mathf.MoveTowards(velocity.y, Mathf.Sign(offset.x) * vSpd, vAccel * Time.deltaTime);
            }

            CannonAnim.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg);
            JetAnim.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);

        }


        if (GameManager.Instance.timeSince(fireTime) >= fireRate) { //Fire at the player sometimes
            fireTime = GameManager.Instance.unpausedTime + Random.Range(-0.4f, 0.4f);
            CannonAnim.SetTrigger("Fire");
            //AnimFunc1();
        }
    }

    public override void AnimFunc1() {
        if (!playerTarget) {
            return;
        }
        Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), PlayerController.Instance.transform.position.ConvertTo2D());

        Vector2 vel = offset.normalized * fireSpeed;
        SimpleBullet bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;

        velocity -= offset.normalized * 1;
    }
    public override void AnimFunc2() {
        Mode = 0;
        hb.gameObject.SetActive(true);
        JetAnim.gameObject.SetActive(true);
        CannonAnim.gameObject.SetActive(true);
    }
}
