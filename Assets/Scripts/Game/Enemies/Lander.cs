using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Lander : ODEnemy
{
    [FoldoutGroup("MovementParams")] public float speed;
    [FoldoutGroup("MovementParams")] public float randomMovementMax;
    [FoldoutGroup("MovementParams")] public Vector2 randomMovementVariance;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireSpeed;
    [FoldoutGroup("CombatParams")] public float citizenGrabOffset;
    [FoldoutGroup("CombatParams")] public float startDelayAbduction; //How long to wait until starting abductions
    [FoldoutGroup("CombatParams")] public float searchRadius;

    [FoldoutGroup("CombatVars")] public float fireTime;
    [FoldoutGroup("CombatVars")] public bool grabbedTarget;

    [FoldoutGroup("Resources")] public GameObject normalBullet;
    [FoldoutGroup("Resources")] public GameObject mutant;

    public Citizen citizenTarget;
    float randomMovementLast;
    float birthTime;
    Vector2 velDir;

    CensusTaker cs;

    public override void Awake() {
        base.Awake();
        birthTime = GameManager.Instance.unpausedTime;

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
            case 1:
                State_Return();
                break;
        }
    }

    public override void OnDestroy() {
        if (citizenTarget) {
            citizenTarget.GetComponentInParent<Citizen>().taggedForCapture = false;
            if (grabbedTarget) { //If we'd grabbed the target, set the target to mode 0 (floating)
                citizenTarget.GetComponentInParent<Citizen>().Mode = 0;
            }
        }
        base.OnDestroy();
        
    }

    public void State_Normal() {

        if (citizenTarget) { //If we have a target, move towards the target
            Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), citizenTarget.transform.position.ConvertTo2D());

            velDir.x = Mathf.Sign(offset.x);
            velDir.y = Mathf.Sign(offset.y);

            if (offset.magnitude < 0.2f) { //Reached the target.
                citizenTarget.GetComponentInParent<Citizen>().Mode = 2;
                Mode = 1;
                grabbedTarget = true;
            }
            
        }
        else { //If there is no target, search for one
            citizenTarget = NearestCitizen();
            if (citizenTarget && GameManager.Instance.timeSince(birthTime) > startDelayAbduction) { //Found a target, check if it's been tagged. If tagged, ignore target
                if (!citizenTarget.taggedForCapture) {
                    citizenTarget.taggedForCapture = true;
                }
                else {
                    citizenTarget = null;
                }
            }
            if (citizenTarget == null || GameManager.Instance.timeSince(birthTime) < startDelayAbduction) { //If no suitable target, move randomly
                citizenTarget = null;
                if (GameManager.Instance.timeSince(randomMovementLast) > randomMovementMax) {
                    randomMovementLast = GameManager.Instance.unpausedTime + Random.Range(randomMovementVariance.x, randomMovementVariance.y);
                    velDir.x = Mathf.Sign(Random.Range(-1f, 1f));
                    velDir.y = Mathf.Sign(Random.Range(-1f, 1f));
                }
            }
        }

        if (GameManager.Instance.timeSince(fireTime) >= fireRate) { //Fire at the player sometimes
            fireTime = GameManager.Instance.unpausedTime + Random.Range(-0.4f, 0.4f);
            anim.SetTrigger("Fire");
        }

        velocity = velDir * speed;
    }

    public void State_Return() { //Return to the spawner with the victim
        if(!citizenTarget) { Mode = 0; grabbedTarget = false; }

        Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), spawner.transform.position.ConvertTo2D());

        citizenTarget.transform.position = transform.position + Vector3.down * citizenGrabOffset;

        if (offset.magnitude < 0.2f) { //Reached the spawner with the victim
            Destroy(citizenTarget.gameObject);
            Destroy(gameObject);
            Instantiate(mutant, transform.position, Quaternion.identity, transform.parent);
        }

        velDir.x = Mathf.Sign(offset.x);
        velDir.y = Mathf.Sign(offset.y);
        velocity = velDir * speed;
    }

    public override void AnimFunc1() {

        Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), PlayerController.Instance.transform.position.ConvertTo2D());

        Vector2 vel = offset.normalized * fireSpeed;
        SimpleBullet bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;
    }

    public Citizen NearestCitizen() {
        float closestDist = float.MaxValue;
        Citizen closestCitizen = null;
        for (int i = 0; i < cs.citizens.Count; i++) {
            float thisDist = cs.OffsetCyclical(transform.position.ConvertTo2D(), cs.citizens[i].transform.position.ConvertTo2D()).magnitude;
            if (thisDist > searchRadius) continue;
            if (thisDist < closestDist) {
                closestDist = thisDist;
                closestCitizen = cs.citizens[i];
            }
        }
        return closestCitizen;
    }
}
