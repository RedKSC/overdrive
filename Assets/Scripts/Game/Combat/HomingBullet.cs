using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HomingBullet : SimpleBullet
{

    [FoldoutGroup("MovementParams")] public float hSpd;
    [FoldoutGroup("MovementParams")] public float hAccel;
    [FoldoutGroup("MovementParams")] public float vSpd;
    [FoldoutGroup("MovementParams")] public float vAccel;

    Transform playerTarget;
    public TargetFinder finder;

    public float speed;
    public float accel;
    public override void Update() {

        playerTarget = PlayerController.Instance.transform;

        if (playerTarget) { //If we have a target, move towards the target
            Vector2 targ = finder.GetNearestCopyLocation(playerTarget);
            float difX = targ.x - transform.position.x;
            float difY = targ.y - transform.position.y;
            velocity.x = Mathf.MoveTowards(velocity.x, Mathf.Sign(difX) * hSpd, hAccel * Time.deltaTime);
            velocity.y = Mathf.MoveTowards(velocity.y, Mathf.Sign(difY) * vSpd, vAccel * Time.deltaTime);
        }

        base.Update();
    }
}
