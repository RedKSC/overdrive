using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.VFX;

public class Greedy : ODEnemy
{
    [FoldoutGroup("MovementParams")] public float hSpd;
    [FoldoutGroup("MovementParams")] public float hAccel;
    [FoldoutGroup("MovementParams")] public float vSpd;
    [FoldoutGroup("MovementParams")] public float vAccel;
    [FoldoutGroup("MovementParams")] public float randomMovementMax;
    [FoldoutGroup("MovementParams")] public Vector2 randomMovementVariance;

    [FoldoutGroup("MovementVars")] public Vector2 movement;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireSpeed;
    [FoldoutGroup("CombatParams")] public float loopBackRange;

    [FoldoutGroup("CombatVars")] public float fireTime;
    [FoldoutGroup("CombatVars")] public int subState;
    [FoldoutGroup("CombatVars")] public float randomMovementLast;

    [FoldoutGroup("Resources")] public GameObject normalBullet;
    [FoldoutGroup("Resources")] public GameObject homingBullet;
    [FoldoutGroup("Resources")] public VisualEffect rushFX;
    [FoldoutGroup("Resources")] public GameObject hb;

    CensusTaker cs;
    PlayerController playerTarget;

    public override void Awake() {
        base.Awake();

    }

    public override void Start() {
        BasicUI.Instance.SetBoss(this, "Greedy");
        cs = CensusTaker.Instance;

        movement = new Vector2(1, 1);
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
        BasicUI.Instance.UnsetBoss();
        base.OnDestroy();
        
    }

    public void State_Normal() {

        switch(subState) {
            case 0:
                State_Normal_MoveRandomly();
                break;
            case 1:
                State_Normal_MoveTwdPlanet();
                break;
        }
        

        void State_Normal_MoveRandomly() {
            playerTarget = PlayerController.Instance;
            Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), playerTarget.transform.position.ConvertTo2D());

            if (playerTarget) {
                if(GameManager.Instance.timeSince(randomMovementLast) > randomMovementMax) {
                    randomMovementMax = Random.Range(randomMovementVariance.x, randomMovementVariance.y);
                    randomMovementLast = GameManager.Instance.unpausedTime;

                    int pick = Random.Range(0, 2);
                    if (pick == 0) movement.x = -Mathf.Sign(movement.x);
                    if (pick == 1) movement.y = -Mathf.Sign(movement.y);

                }
                velocity.x = Mathf.MoveTowards(velocity.x, movement.x * hSpd, hAccel * Time.deltaTime);
                velocity.y = Mathf.MoveTowards(velocity.y, movement.y * hSpd, hAccel * Time.deltaTime);
            }
        }
        void State_Normal_MoveTwdPlanet() {
            playerTarget = PlayerController.Instance;
            Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), playerTarget.transform.position.ConvertTo2D());

            if (playerTarget) {

            }
        }

    }

}
