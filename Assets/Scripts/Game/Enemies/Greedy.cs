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

    [FoldoutGroup("CombatVars")] public float fireTime;
    [FoldoutGroup("CombatVars")] public int subState;
    [FoldoutGroup("CombatVars")] public float randomMovementLast;
    [FoldoutGroup("CombatVars")] public float droneSpawnedLast;
    [FoldoutGroup("CombatVars")] public float droneCurrentNum;
    [FoldoutGroup("CombatVars")] public float droneTotalNum;
    [FoldoutGroup("CombatVars")] public bool attackingWithWave;

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

    CensusTaker cs;
    PlayerController playerTarget;

    public static Greedy Instance;

    public override void Awake() {
        base.Awake();
        Instance = this;
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

            if(droneCurrentNum < droneMaxCount && !attackingWithWave) {
                if(GameManager.Instance.timeSince(droneSpawnedLast) > timeBtwnDroneSpawns) {
                    Instantiate(drone, transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(1.5f, 2.5f), 0), Quaternion.identity);
                    droneCurrentNum++;
                    droneTotalNum++;
                    droneSpawnedLast = GameManager.Instance.unpausedTime;
                }
            }

            
        }
        void State_Normal_MoveTwdPlanet() {
            playerTarget = PlayerController.Instance;
            Vector2 offset = cs.OffsetCyclical(transform.position.ConvertTo2D(), playerTarget.transform.position.ConvertTo2D());

            if (playerTarget) {

            }
        }

        if (droneTotalNum >= firstDroneNumberThresh && !attackingWithWave) {
            StartCoroutine(UseStunWave());

        }

    }

    public override void OnHurt(float damage, Vector2 knockbackSpd, float stunTime, DamageType type, float burn, float knockbackOverride, Hurtbox hurtbox) {
        //Debug.Log(hurtbox);
        if (hurtbox.gameObject == lowerOpenHurtbox || hurtbox.gameObject == lowerClosedHurtbox) {
            lowerShip.SetTrigger("Hurt");
            Debug.Log("Damage to lower half");
        }
        base.OnHurt(damage, knockbackSpd, stunTime, type, burn, knockbackOverride);

    }

    void CloseLowerShip() {
        lowerClosedHurtbox.SetActive(true);
        lowerOpenHurtbox.SetActive(false);
        lowerShip.SetBool("Open", false);
    }
    void OpenLowerShip() {
        lowerClosedHurtbox.SetActive(false);
        lowerOpenHurtbox.SetActive(true);
        lowerShip.SetBool("Open", true);
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
}
