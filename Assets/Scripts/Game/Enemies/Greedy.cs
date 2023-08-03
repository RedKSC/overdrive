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
    [FoldoutGroup("MovementVars")] public int lowerShipState;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireSpeed;
    [FoldoutGroup("CombatParams")] public float loopBackRange;
    [FoldoutGroup("CombatParams")] public float droneMaxCount;
    [FoldoutGroup("CombatParams")] public float timeBtwnDroneSpawns;
    [FoldoutGroup("CombatParams")] public int firstDroneNumberThresh; //For charge lightning attack
    [FoldoutGroup("CombatParams")] public int secondDroneNumberThresh; //For finding planet to absorb if one exists
    [FoldoutGroup("CombatParams")] public int hpThreshToStartEatingPlanets;
    [FoldoutGroup("CombatParams")] public float planetGrabOffset;
    [FoldoutGroup("CombatParams")] public float groundingTimeMax;
    [FoldoutGroup("CombatParams")] public float punishmentWhileEatingPlanetMax;
    [FoldoutGroup("CombatParams")] public Vector2 planetEatingTimeRange;

    [FoldoutGroup("CombatVars")] public float fireTime;
    [FoldoutGroup("CombatVars")] public int subState;
    [FoldoutGroup("CombatVars")] public float randomMovementLast;
    [FoldoutGroup("CombatVars")] public float droneSpawnedLast;
    [FoldoutGroup("CombatVars")] public float droneCurrentNum;
    [FoldoutGroup("CombatVars")] public float droneTotalNum;
    [FoldoutGroup("CombatVars")] public bool attackingWithWave;
    [FoldoutGroup("CombatVars")] public Planet planetTarget;
    [FoldoutGroup("CombatVars")] public float punishmentRecievedWhileEatingPlanet;
    [FoldoutGroup("CombatVars")] public float planetEatingTimeWaitStart;
    [FoldoutGroup("CombatVars")] public float planetEatingTimeWaitMax;

    [FoldoutGroup("ManualResources")] public GameObject normalBullet;
    [FoldoutGroup("ManualResources")] public GameObject homingBullet;
    [FoldoutGroup("ManualResources")] public GameObject stunBullet;
    [FoldoutGroup("ManualResources")] public GameObject drone;
    [FoldoutGroup("ManualResources")] public VisualEffect rushFX;
    [FoldoutGroup("ManualResources")] public GameObject hb;
    [FoldoutGroup("ManualResources")] public Animator upperShip;
    [FoldoutGroup("ManualResources")] public Animator lowerShip;
    [FoldoutGroup("ManualResources")] public Animator chargeFX;
    [FoldoutGroup("ManualResources")] public GameObject upperHurtbox;
    [FoldoutGroup("ManualResources")] public GameObject lowerClosedHurtbox;
    [FoldoutGroup("ManualResources")] public GameObject lowerOpenHurtbox;
    [FoldoutGroup("ManualResources")] public GameObject myFavoriteLittleExtraLayer;
    [FoldoutGroup("ManualResources")] public Transform Barrier;

    PlayerController playerTarget;

    public static Greedy Instance;
    float groundTime;

    public override void Awake() {
        base.Awake();
        Instance = this;
    }

    public override void Start() {
        BasicUI.Instance.SetBoss(this, "Greedy");
        cs = CensusTaker.Instance;
        
        movement = new Vector2(1, 1);
        CloseLowerShip();
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
        Instance = null;
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
            case 2:
                State_Normal_DestroyPlanet();
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
            

            if (droneTotalNum >= firstDroneNumberThresh && !attackingWithWave) {
                StartCoroutine(UseStunWave());
            }

            if(!attackingWithWave && GameManager.Instance.timeSince(planetEatingTimeWaitStart) > planetEatingTimeWaitMax && simpleHealth < maxHealth * 0.75f) {
                subState = 1;
                planetEatingTimeWaitMax = Random.Range(planetEatingTimeRange.x, planetEatingTimeRange.y);
            }

        }
        void State_Normal_MoveTwdPlanet() {
            attackingWithWave = false;
            if (planetTarget) { //If we have a target, move towards the target
                Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), planetTarget.transform.position.ConvertTo2D() + Vector2.up * planetGrabOffset);

                velocity.x = Mathf.MoveTowards(velocity.x, Mathf.Clamp(offset.x, -hSpd, hSpd), hAccel * Time.deltaTime);
                velocity.y = Mathf.MoveTowards(velocity.y, Mathf.Clamp(offset.y, -hSpd, hSpd), hAccel * Time.deltaTime);

                if (offset.magnitude < 0.1f) { //Reached the target.
                    subState = 2; 
                    groundTime = GameManager.Instance.unpausedTime;
                    StartCoroutine(OpenForPlanetConsumption());
                    punishmentRecievedWhileEatingPlanet = 0;
                }
            }
            else { //If there is no target, search for one
                planetTarget = NearestPlanet();

                if (planetTarget) { //Found a target, check if it's been tagged. If tagged, ignore target
                    if (!planetTarget.GetComponentInParent<Planet>().taggedForCapture) {
                        planetTarget.GetComponentInParent<Planet>().taggedForCapture = true;
                    }
                    else {
                        planetTarget = null;
                    }
                }
                if (planetTarget == null) { //If no suitable target, move randomly
                    planetTarget = null;
                    if (GameManager.Instance.timeSince(randomMovementLast) > randomMovementMax) {
                        randomMovementLast = GameManager.Instance.unpausedTime + Random.Range(randomMovementVariance.x, randomMovementVariance.y);
                        velocity.x = Mathf.Sign(Random.Range(-1f, 1f));
                        velocity.y = Mathf.Sign(Random.Range(-1f, 1f));
                    }
                }
            }
        }

        void State_Normal_DestroyPlanet() { //Destroy the victim planet
            transform.position = planetTarget.transform.position + Vector3.up * planetGrabOffset;
            velocity = Vector2.zero;
            if (GameManager.Instance.timeSince(groundTime) > groundingTimeMax) {
                Destroy(planetTarget.gameObject);
                subState = 1;
                planetTarget = null;
                planetEatingTimeWaitStart = GameManager.Instance.unpausedTime;
                CloseLowerShip();
            }
            if(punishmentRecievedWhileEatingPlanet > punishmentWhileEatingPlanetMax) {
                subState = 1;
                planetTarget = null;
                CloseLowerShip();
                velocity.y = 2;
                planetEatingTimeWaitStart = GameManager.Instance.unpausedTime;
            }
        }


        if (droneCurrentNum < droneMaxCount && !attackingWithWave) {
            if (GameManager.Instance.timeSince(droneSpawnedLast) > timeBtwnDroneSpawns) {
                Instantiate(drone, transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(1.5f, 2.5f), 0), Quaternion.identity);
                droneCurrentNum++;
                droneTotalNum++;
                droneSpawnedLast = GameManager.Instance.unpausedTime;
            }
        }

    }

    public override void OnHurt(float damage, Vector2 knockbackSpd, float stunTime, DamageType type, float burn, float knockbackOverride, Hurtbox hurtbox) {
        //Debug.Log(hurtbox);
        if (hurtbox.gameObject == lowerOpenHurtbox || hurtbox.gameObject == lowerClosedHurtbox) {
            lowerShip.SetTrigger("Hurt");
            Debug.Log("Damage to lower half");
        }
        base.OnHurt(damage, knockbackSpd, stunTime, type, burn, knockbackOverride);
        punishmentRecievedWhileEatingPlanet += damage;
    }

    void CloseLowerShip() {
        lowerClosedHurtbox.SetActive(true);
        lowerOpenHurtbox.SetActive(false);
        lowerShip.SetBool("Open", false);
        Barrier.gameObject.SetActive(false);
    }
    void OpenLowerShip() {
        lowerClosedHurtbox.SetActive(false);
        lowerOpenHurtbox.SetActive(true);
        lowerShip.SetBool("Open", true);

        if(simpleHealth < maxHealth/2) {
            Barrier.transform.localScale = new Vector3(Random.Range(0, 2) * 2 - 1, 1, 1);
            Barrier.gameObject.SetActive(true);
        }
    }

    public IEnumerator UseStunWave() {
        float t = 0;
        attackingWithWave = true;

        while (t < 4f) {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        while (t < 0.5f) {
            myFavoriteLittleExtraLayer.transform.localPosition = new Vector3(0, 0, 0);
            myFavoriteLittleExtraLayer.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.05f, 0.05f), 0);
            t += Time.deltaTime;
            yield return null;
        }
        myFavoriteLittleExtraLayer.transform.localPosition = new Vector3(0, 0, 0);
        OpenLowerShip();

        t = 0;
        while (t < 0.2f) {
            myFavoriteLittleExtraLayer.transform.localPosition = new Vector3(0, 0, 0);
            myFavoriteLittleExtraLayer.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.05f, 0.05f), 0);
            t += Time.deltaTime;
            yield return null;
        }

        myFavoriteLittleExtraLayer.transform.localPosition = new Vector3(0, 0, 0);

        chargeFX.gameObject.SetActive(true);
        chargeFX.SetTrigger("Charge");

        t = 0;
        while (t < 0.5f) {
            t += Time.deltaTime;
            yield return null;
        }

        chargeFX.gameObject.SetActive(false);

        Vector3 pos = transform.position;
        Vector3 vel = Vector3.right * fireSpeed;
        SimpleBullet bullet = Instantiate(stunBullet, pos, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;
        bullet.transform.localScale = new Vector3(1, 1, 1);
        bullet.damage = 0;

        vel = Vector3.right * -1f * fireSpeed;
        bullet = Instantiate(stunBullet, pos, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.velocity = vel;
        bullet.transform.localScale = new Vector3(-1, 1, 1);
        bullet.damage = 0;

        t = 0;
        while (t < 4f) {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        while (t < 0.5f) {
            myFavoriteLittleExtraLayer.transform.localPosition = new Vector3(0, 0, 0);
            myFavoriteLittleExtraLayer.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.05f, 0.05f), 0);
            t += Time.deltaTime;
            yield return null;
        }
        myFavoriteLittleExtraLayer.transform.localPosition = new Vector3(0, 0, 0);
        CloseLowerShip();

        t = 0;
        while (t < 0.2f) {
            myFavoriteLittleExtraLayer.transform.localPosition = new Vector3(0, 0, 0);
            myFavoriteLittleExtraLayer.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.05f, 0.05f), 0);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        while (t < 0.5f) {
            t += Time.deltaTime;
            yield return null;
        }

        attackingWithWave = false;
        droneTotalNum = 0;
    }

    public IEnumerator OpenForPlanetConsumption() {
        float t = 0;

        while (t < 0.5f) {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        while (t < 0.5f) {
            myFavoriteLittleExtraLayer.transform.localPosition = new Vector3(0, 0, 0);
            myFavoriteLittleExtraLayer.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.05f, 0.05f), 0);
            t += Time.deltaTime;
            yield return null;
        }
        myFavoriteLittleExtraLayer.transform.localPosition = new Vector3(0, 0, 0);
        OpenLowerShip();
    }
}
