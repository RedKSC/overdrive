using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SnakeByte : ODEnemy
{
    [FoldoutGroup("MovementParams")] public float moveUpdateSpeed;
    [FoldoutGroup("MovementParams")] public int movesToDirectionChange;
    [FoldoutGroup("MovementParams")] public Vector2Int movesToDirectionChangeVariance;

    [FoldoutGroup("MovementVars")] public float moveTime;
    [FoldoutGroup("MovementVars")] public int moves;
    [FoldoutGroup("MovementVars")] public Vector2 myDirection;
    [FoldoutGroup("MovementVars")] public Vector2 myMove;
    [FoldoutGroup("MovementVars")] public Vector2 myLastMove;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireSpeed;
    [FoldoutGroup("CombatParams")] public int tailsToCreate;

    [FoldoutGroup("CombatVars")] public float fireTime;

    [FoldoutGroup("Resources")] public GameObject normalBullet;
    [FoldoutGroup("Resources")] public SnakeByte head;
    [FoldoutGroup("Resources")] public SnakeByte tail;
    [FoldoutGroup("Resources")] public GameObject snakeByte;

    CensusTaker cs;

    public override void Awake() {
        base.Awake();

    }

    public override void Start() {
        base.Start();
        cs = CensusTaker.Instance;
        fireTime = GameManager.Instance.unpausedTime + Random.Range(-0.4f, 0.4f);
    }
    public override void Update() {
        velocity = Vector2.zero;

        base.Update();

        if(tailsToCreate > 0) {
            tail = Instantiate(snakeByte, transform.position, Quaternion.identity).GetComponent<SnakeByte>();
            tail.tailsToCreate = tailsToCreate - 1;
            tail.head = this;
            tailsToCreate = 0;
        }

        switch(Mode) {
            case 0:
                State_Normal();
                break;
        }

        anim.SetFloat("Type", head == null ? 0 : 1);
    }

    public override void OnDestroy() {
        base.OnDestroy();
        
    }

    public void State_Normal() {

        if (head == null) {

            moveUpdateSpeed = (MyTails() + 2) * 0.07f;

            if (GameManager.Instance.timeSince(moveTime) > moveUpdateSpeed) {
                moves++;
                if(moves >= movesToDirectionChange) {

                    if (PlayerController.Instance.gameObject) {
                        Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), PlayerController.Instance.transform.position.ConvertTo2D());

                        moves = Random.Range(movesToDirectionChangeVariance.x, movesToDirectionChangeVariance.y);
                        if (myDirection.x != 0) {
                            myDirection = new Vector2(0, Mathf.Sign(offset.y));
                        } else {
                            myDirection = new Vector2(Mathf.Sign(offset.x), 0);
                        }
                    }
                }
                Move(myDirection);
                Debug.Log(MyTails());
                moveTime = GameManager.Instance.unpausedTime;
            }
        }

        if (GameManager.Instance.timeSince(fireTime) >= fireRate) { //Fire at the player sometimes
            fireTime = GameManager.Instance.unpausedTime + Random.Range(-0.4f, 0.4f);
            //anim.SetTrigger("Fire");
            AnimFunc1();
        }
    }

    public override void AnimFunc1() {

        Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), PlayerController.Instance.transform.position.ConvertTo2D());

        Vector2 vel = offset.normalized * fireSpeed;
        SimpleBullet bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;
    }

    public void Move(Vector2 move) {
        myLastMove = myMove;
        myMove = move;
        transform.position += myMove.ConvertTo3D();

        if(tail != null) {
            tail.Move(myLastMove);
        }
    }

    public int MyTails() {
        if(tail == null) {
            return 0;
        }
        else {
            return tail.MyTails() + 1;
        }
    }

}
