using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ThunderRock : ODEnemy {
    [FoldoutGroup("MovementParams")] public float hSpd;
    [FoldoutGroup("MovementParams")] public float hAccel;
    [FoldoutGroup("MovementParams")] public float vSpd;
    [FoldoutGroup("MovementParams")] public float vAccel;
    [FoldoutGroup("MovementParams")] public float randomMovementMax;
    [FoldoutGroup("MovementParams")] public Vector2 randomMovementVariance;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireSpeed;
    [FoldoutGroup("CombatParams")] public int normalFireNumTotal;
    [FoldoutGroup("CombatParams")] public int normalFireSetTotal;
    [FoldoutGroup("CombatParams")] public float waitTime;

    [FoldoutGroup("CombatVars")] public float fireTime;
    [FoldoutGroup("CombatVars")] public int stunFireNum;
    [FoldoutGroup("CombatVars")] public int normalFireNum;
    [FoldoutGroup("CombatVars")] public int normalFireSetNum;
    [FoldoutGroup("CombatVars")] public float timeSinceWait;

    [FoldoutGroup("Resources")] public GameObject normalBullet;
    [FoldoutGroup("Resources")] public ThunderRing ring;

    PlayerController playerTarget;
    float randomMovementLast;
    CensusTaker cs;

    public override void Awake() {
        base.Awake();
        ring = GetComponentInChildren<ThunderRing>();

    }
    public override void Start() {
        BasicUI.Instance.SetBoss(this, "Starbird");
        cs = CensusTaker.Instance;
    }

    public override void Update() {
        base.Update();

        switch (Mode) {
            case 0:
                State_Normal();
                break;
        }
    }

    public override void OnDestroy() {
        BasicUI.Instance.UnsetBoss();
        base.OnDestroy();

    }

    public void State_Normal() {
        switch (SubMode) {
            case 0:
                if(GameManager.Instance.timeSince(timeSinceWait) < waitTime) {
                    return;
                }
                if (playerTarget) { //If we have a target, move towards the target
                    playerTarget = PlayerController.Instance;
                    Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), playerTarget.transform.position.ConvertTo2D());
                    velocity.x = Mathf.MoveTowards(velocity.x, Mathf.Sign(offset.x) * hSpd, hAccel * Time.deltaTime);
                    velocity.y = Mathf.MoveTowards(velocity.y, Mathf.Sign(offset.y) * vSpd, vAccel * Time.deltaTime);
                }
                else { //If there is no target, search for one
                    playerTarget = PlayerController.Instance;
                }

                if (GameManager.Instance.timeSince(fireTime) >= fireRate) { //Fire at the player sometimes
                    fireTime = GameManager.Instance.unpausedTime;
                    ring.Fire();
                    normalFireNum++;

                    if(normalFireNum > normalFireNumTotal) {
                        normalFireSetNum++;
                        normalFireNum = 0;
                        timeSinceWait = GameManager.Instance.unpausedTime;
                        if (normalFireSetNum >= normalFireSetTotal)
                            SubMode = 1;
                    }
                }
                break;
            case 1:
                if (GameManager.Instance.timeSince(timeSinceWait) < waitTime) {
                    return;
                }
                if (playerTarget) { //If we have a target, move towards the target
                    playerTarget = PlayerController.Instance;
                    Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), playerTarget.transform.position.ConvertTo2D());
                    velocity.x = Mathf.MoveTowards(velocity.x, Mathf.Sign(offset.x) * hSpd, hAccel * Time.deltaTime);
                    velocity.y = Mathf.MoveTowards(velocity.y, Mathf.Sign(offset.y) * vSpd, vAccel * Time.deltaTime);
                }
                else { //If there is no target, search for one
                    playerTarget = PlayerController.Instance;
                }

                if (GameManager.Instance.timeSince(fireTime) >= fireRate*4) { //Fire at the player sometimes
                    fireTime = GameManager.Instance.unpausedTime;
                    ring.StunFire(0);
                    stunFireNum++;
                    if (stunFireNum >= 4) {
                        timeSinceWait = GameManager.Instance.unpausedTime;
                        SubMode = 0;
                        stunFireNum = 0;
                        normalFireSetNum = 0;
                    }
                }
                break;
        }
    }
}
