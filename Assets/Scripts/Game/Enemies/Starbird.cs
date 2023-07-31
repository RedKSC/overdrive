using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.VFX;

public class Starbird : ODEnemy
{
    [FoldoutGroup("MovementParams")] public float hSpd;
    [FoldoutGroup("MovementParams")] public float hAccel;
    [FoldoutGroup("MovementParams")] public float vSpd;
    [FoldoutGroup("MovementParams")] public float vAccel;
    [FoldoutGroup("MovementParams")] public float randomMovementMax;
    [FoldoutGroup("MovementParams")] public Vector2 randomMovementVariance;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireSpeed;
    [FoldoutGroup("CombatParams")] public float loopBackRange;

    [FoldoutGroup("CombatVars")] public float fireTime;

    [FoldoutGroup("Resources")] public GameObject normalBullet;
    [FoldoutGroup("Resources")] public GameObject homingBullet;
    [FoldoutGroup("Resources")] public VisualEffect rushFX;
    [FoldoutGroup("Resources")] public GameObject hb;

    CensusTaker cs;

    PlayerController playerTarget;
    float randomMovementLast;
    int fireSpurtNum;
    float loopBackLast;

    public override void Awake() {
        base.Awake();

    }

    public override void Start() {
        BasicUI.Instance.SetBoss(this, "Starbird");
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
        BasicUI.Instance.UnsetBoss();
        base.OnDestroy();
        
    }

    public void State_Normal() {
        playerTarget = PlayerController.Instance;
        Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), playerTarget.transform.position.ConvertTo2D());

        if (playerTarget) { //If we have a target, move towards the target
            bool speeding = Mathf.Abs(velocity.x) > hSpd * 0.55f;
            
            velocity.x = Mathf.MoveTowards(velocity.x, Mathf.Sign(offset.x) * hSpd , hAccel * Time.deltaTime * (Mathf.Sign(offset.x) == transform.localScale.x || !speeding ? 1 : 0.3f));
            velocity.y = Mathf.MoveTowards(velocity.y, Mathf.Sign(offset.y) * vSpd, vAccel * Time.deltaTime);

            transform.localScale = new Vector3(Mathf.Sign(velocity.x), 1, 1);

            if (speeding) rushFX.Play();
            else rushFX.Stop();

            hb.SetActive(speeding);
        }

        if (GameManager.Instance.timeSince(loopBackLast) > 1) {
            if (offset.magnitude > loopBackRange) {
                loopBackLast = GameManager.Instance.unpausedTime;
                transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
                velocity.x = hSpd * transform.localScale.x * 0.5f;
                transform.position = playerTarget.transform.position + Vector3.right * loopBackRange * 0.8f * -transform.localScale.x;
                fireTime = GameManager.Instance.unpausedTime + Random.Range(-0.4f, 0.4f);
            }
        }

        if (GameManager.Instance.timeSince(fireTime) >= fireRate) { //Fire at the player sometimes
            if(fireSpurtNum != 2) {
                fireSpurtNum++;
                fireTime = GameManager.Instance.unpausedTime - fireRate + 0.1f;
            }
            else {
                fireSpurtNum = 0;
                fireTime = GameManager.Instance.unpausedTime + Random.Range(-0.4f, 0.4f);
            }

            if (Mathf.Sign(offset.x) == transform.localScale.x) {
                float xpos = transform.position.x + 0.684f * transform.localScale.x;
                float ypos = transform.position.y + 0.3f;
                Vector2 vel = new Vector2(transform.localScale.x, 0) * fireSpeed;
                SimpleBullet bullet = Instantiate(normalBullet, new Vector3(xpos, ypos, 0), Quaternion.identity).GetComponent<SimpleBullet>();
                bullet.velocity = vel;

                xpos = transform.position.x + 0.684f * transform.localScale.x;
                ypos = transform.position.y - 0.3f;
                vel = new Vector2(transform.localScale.x, 0) * fireSpeed;
                bullet = Instantiate(normalBullet, new Vector3(xpos, ypos, 0), Quaternion.identity).GetComponent<SimpleBullet>();
                bullet.velocity = vel;
            }
            else {
                float xpos = transform.position.x + 0.684f * transform.localScale.x;
                float ypos = transform.position.y + 0.3f;
                Vector2 vel = new Vector2(transform.localScale.x, 0.3f) * fireSpeed * 0.5f;
                SimpleBullet bullet = Instantiate(homingBullet, new Vector3(xpos, ypos, 0), Quaternion.identity).GetComponent<SimpleBullet>();
                bullet.velocity = vel;

                xpos = transform.position.x + 0.684f * transform.localScale.x;
                ypos = transform.position.y - 0.3f;
                vel = new Vector2(transform.localScale.x, -0.3f) * fireSpeed * 0.5f;
                bullet = Instantiate(homingBullet, new Vector3(xpos, ypos, 0), Quaternion.identity).GetComponent<SimpleBullet>();
                bullet.velocity = vel;
            }
        }
    }

}
