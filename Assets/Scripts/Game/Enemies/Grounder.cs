using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Grounder : ODEnemy {
    [FoldoutGroup("MovementParams")] public float speed;
    [FoldoutGroup("MovementParams")] public float randomMovementMax;
    [FoldoutGroup("MovementParams")] public Vector2 randomMovementVariance;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireSpeed;
    [FoldoutGroup("CombatParams")] public float planetGrabOffset;
    [FoldoutGroup("CombatParams")] public float groundingTimeMax;
    [FoldoutGroup("CombatParams")] public float startDelayAbduction; //How long to wait until starting abductions

    [FoldoutGroup("CombatVars")] public float fireTime;

    [FoldoutGroup("Resources")] public GameObject normalBullet;

    public Planet planetTarget;
    float randomMovementLast;
    float birthTime;
    float groundTime;
    Vector2 velDir;
    CensusTaker cs;

    public override void Awake() {
        base.Awake();
        birthTime = GameManager.Instance.unpausedTime;
    }
    public override void Start() {
        velDir.x = Mathf.Sign(Random.Range(-1f, 1f));
        velDir.y = Mathf.Sign(Random.Range(-1f, 1f));
        cs = CensusTaker.Instance;
    }
    public override void Update() {
        base.Update();

        switch (Mode) {
            case 0:
                State_Normal();
                break;
            case 1:
                State_Destroy();
                break;
        }
    }

    public override void OnDestroy() {
        if (planetTarget) {
            planetTarget.GetComponentInParent<Planet>().taggedForCapture = false;
        }
        base.OnDestroy();

    }

    public void State_Normal() {

        if (planetTarget) { //If we have a target, move towards the target
            Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), planetTarget.transform.position.ConvertTo2D() + Vector2.up * planetGrabOffset);

            velDir.x = Mathf.Sign(offset.x);
            velDir.y = Mathf.Sign(offset.y);
            if (offset.magnitude < 0.1f) { //Reached the target.
                Mode = 1;
                anim.SetTrigger("Drill");
                groundTime = GameManager.Instance.unpausedTime;
            }
        }
        else { //If there is no target, search for one
            planetTarget = NearestPlanet();

            if (planetTarget && GameManager.Instance.timeSince(birthTime) > startDelayAbduction) { //Found a target, check if it's been tagged. If tagged, ignore target
                if (!planetTarget.GetComponentInParent<Planet>().taggedForCapture) {
                    planetTarget.GetComponentInParent<Planet>().taggedForCapture = true;
                }
                else {
                    planetTarget = null;
                }
            }
            if (planetTarget == null || GameManager.Instance.timeSince(birthTime) < startDelayAbduction) { //If no suitable target, move randomly
                planetTarget = null;
                if (GameManager.Instance.timeSince(randomMovementLast) > randomMovementMax) {
                    randomMovementLast = GameManager.Instance.unpausedTime + Random.Range(randomMovementVariance.x, randomMovementVariance.y);
                    velDir.x = Mathf.Sign(Random.Range(-1f, 1f));
                    velDir.y = Mathf.Sign(Random.Range(-1f, 1f));
                }
            }
        }

        if (GameManager.Instance.timeSince(fireTime) >= fireRate) { //Fire at the player sometimes
            fireTime = GameManager.Instance.unpausedTime + Random.Range(-0.4f, 0.4f);
            Fire();
        }

        velocity = velDir * speed;
    }

    public void State_Destroy() { //Destroy the victim planet
        transform.position = planetTarget.transform.position + Vector3.up * planetGrabOffset;
        velocity = Vector2.zero;
        if(GameManager.Instance.timeSince(groundTime) > groundingTimeMax) {
            Destroy(planetTarget.gameObject);
            Mode = 0;
            planetTarget = null;
            anim.SetTrigger("DoneDrilling");
        }

        if (GameManager.Instance.timeSince(fireTime) >= fireRate) { //Fire at the player sometimes
            fireTime = GameManager.Instance.unpausedTime + Random.Range(-0.4f, 0.4f);
            Fire();
        }
    }

    public void Fire() {
        //whatever, man.

        Vector2 vel = new Vector2(0, 1).normalized * fireSpeed;
        SimpleBullet bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;

        vel = new Vector2(1, 1).normalized * fireSpeed;
        bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;

        vel = new Vector2(1, 0).normalized * fireSpeed;
        bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;

        vel = new Vector2(1, -1).normalized * fireSpeed;
        bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;

        vel = new Vector2(0, -1).normalized * fireSpeed;
        bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;

        vel = new Vector2(-1, -1).normalized * fireSpeed;
        bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;

        vel = new Vector2(-1, 0).normalized * fireSpeed;
        bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;

        vel = new Vector2(-1, 1).normalized * fireSpeed;
        bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;

        vel = new Vector2(0, 1).normalized * fireSpeed;
        bullet = Instantiate(normalBullet, transform.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;
    }

    public Planet NearestPlanet() {
        float closestDist = float.MaxValue;
        Planet closestPlanet = null;
        for (int i = 0; i < cs.planets.Count; i++) {
            float thisDist = cs.OffsetCyclical(transform.position.ConvertTo2D(), cs.planets[i].transform.position.ConvertTo2D()).magnitude;
            if (thisDist < closestDist) {
                closestDist = thisDist;
                closestPlanet = cs.planets[i];
            }
        }
        return closestPlanet;
    }
}
