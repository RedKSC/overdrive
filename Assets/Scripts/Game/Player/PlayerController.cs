using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.Universal;

public class PlayerController : ODEntity
{
    [FoldoutGroup("MovementParams")] public float maxhor = 4f;
    [FoldoutGroup("MovementParams")] public float accelhor = 0.1f;
    [FoldoutGroup("MovementParams")] public float decelhor = 0.1f;
    [FoldoutGroup("MovementParams")] public float maxvert = 2f;
    [FoldoutGroup("MovementParams")] public float accelvert = 0.12f;
    [FoldoutGroup("MovementParams")] public float decelvert = 0.12f;
    [FoldoutGroup("MovementParams")] public bool canControl = true;

    [FoldoutGroup("CombatParams")] public float bulletSpeed;
    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public float fireBufferMax;
    [FoldoutGroup("CombatParams")] public float stunChargeTime;
    [FoldoutGroup("CombatParams")] public float dashFullCharge;
    [FoldoutGroup("CombatParams")] public float dashSpeed;
    [FoldoutGroup("CombatParams")] public float dashTimeMax;        //For how long the dash lasts
    [FoldoutGroup("CombatParams")] public float fireBreathRate;
    [FoldoutGroup("CombatParams")] public float rollTime;
    [FoldoutGroup("CombatParams")] public float rollSpeed;
    [FoldoutGroup("CombatParams")] public float rollDecel;

    [FoldoutGroup("CombatVars")] public float fireFullCharge;
    [FoldoutGroup("CombatVars")] float timeSinceFire;
    [FoldoutGroup("CombatVars")] float timeSinceDashStart;
    [FoldoutGroup("CombatVars")] public float dashChargeTime;     //For using dash
    [FoldoutGroup("CombatVars")] float timeSinceStun;
    [FoldoutGroup("CombatVars")] float timeSinceFireBreath;
    [FoldoutGroup("CombatVars")] float timeSinceRoll;
    [FoldoutGroup("CombatVars")] public float invincibleTimeStart;
    [FoldoutGroup("CombatVars")] public float invincibleTimeMax;

    [FoldoutGroup("Resources")] public GameObject normalBullet;
    [FoldoutGroup("Resources")] public GameObject stunWave;
    [FoldoutGroup("Resources")] public GameObject fireBreath;
    [FoldoutGroup("Resources")] public Animator fireAnim;
    [FoldoutGroup("Resources")] public WeaponsInventory wpnInv;
    [FoldoutGroup("Resources")] public GameObject dashHitBox;

    [FoldoutGroup("SFX")] public AudioClip laserShoot;

    bool usingFire;    //For the fire breath attack
    float fireCharge;
    bool fireActive;    //For ship rocket animations
    ChargeBarHandler barHandler;
    int fireChargeBarID;

    [HideInInspector] public static PlayerController Instance;

    float TimeSinceFire {
        get { return GameManager.Instance.unpausedTime - timeSinceFire;  }
        set { timeSinceFire = GameManager.Instance.unpausedTime + value; }
    }

    public UIInfo playerUIInfo =>  new UIInfo() {
        healthPercent = Mathf.Clamp(simpleHealth / maxHealth, 0, 1),
        dashPercent = dashChargeTime / dashFullCharge,
        stunPercent = Mathf.Clamp(GameManager.Instance.timeLerp(timeSinceStun, stunChargeTime), 0, 1),
        firePercent = fireCharge / fireFullCharge,
        gasPercent = 0,
    };

    public override void Awake() {
        base.Awake();
        Instance = this;
        barHandler = GetComponent<ChargeBarHandler>();
    }

    public override void Start() {
        base.Start();
        timeSinceFire = 0;
        wpnInv = WeaponsInventory.Instance;
    }

    public override void OnDestroy() {
        if (killed) {
            WaveEvents.PlayerKilled();
            BasicUI.Instance.GameOver();
        }
    }

    public override void Update()
    {
        base.Update();
        switch(Mode) {
            case 0:
                State_Normal();
                break;
            case 1:
                State_Dash();
                break;
            case 2:
                State_Roll();
                break;
        }
        anim.SetFloat("VertSpeed", velocity.y);
    }

    void State_Normal() {


        // MOVEMENT //
		float hor = canControl ? PlayerInput.GetAxis(PlayerInputKey.Horizontal) : 0;
        bool horBurst = canControl ? Mathf.Abs(PlayerInput.GetAxisDelta(PlayerInputKey.Horizontal)) > 0.3f && (Mathf.Abs(hor) > 0.6f) : false;

        if (horBurst) {
            velocity.x = Mathf.MoveTowards(velocity.x, maxhor * hor, accelhor * 6 * Time.deltaTime * GameManager.Instance.deltaCoef);
        }

        if (hor != 0) {
			velocity.x = Mathf.MoveTowards(velocity.x, maxhor * hor, accelhor * Time.deltaTime * GameManager.Instance.deltaCoef);
			transform.localScale = new Vector3(1 * Mathf.Sign(hor), 1, 1);
		}
		else {
			velocity.x = Mathf.MoveTowards(velocity.x, 0, decelhor * Time.deltaTime * GameManager.Instance.deltaCoef);
		}


		var vert = canControl ? PlayerInput.GetAxis(PlayerInputKey.Vertical) : 0;

        if(PlayerInput.GetButton(PlayerInputKey.Roll) && canControl) {
            if (!GameManager.Instance.AllowPlayerAttacks) {
                Debug.Log("NoRoll");
                return;
            }
            if(vert > 0.5f) {
                anim.SetTrigger("RollUp");
                velocity.y = rollSpeed;
                velocity.x = velocity.x / 2;
                timeSinceRoll = GameManager.Instance.unpausedTime;
                Mode = 2;
            }
            if (vert < -0.5f) {
                anim.SetTrigger("RollDown");
                velocity.y = -rollSpeed;
                velocity.x = velocity.x / 2;
                timeSinceRoll = GameManager.Instance.unpausedTime;
                Mode = 2;
            }
        }

		if (vert != 0) {
			velocity.y = Mathf.MoveTowards(velocity.y, maxvert * vert, accelvert * Time.deltaTime * GameManager.Instance.deltaCoef);
		}
		else {
			velocity.y = Mathf.MoveTowards(velocity.y, 0, decelvert * Time.deltaTime * GameManager.Instance.deltaCoef);
		}

       

        if(fireCharge < fireFullCharge && !barHandler.UpdateChargeBar(fireChargeBarID, fireCharge)) {
            fireChargeBarID = barHandler.AddChargeBar(Color.red, fireCharge, fireFullCharge);
        }

        if(canControl) {
            Attack();
        }

        // ANIMATIONS //
        bool horizontal = hor == 0;
        if(fireActive == horizontal){
            fireAnim.SetTrigger(horizontal ? "Stop" : "On");
            fireActive = !horizontal;
        }
	}

    void State_Dash() {
        if (!GameManager.Instance.AllowPlayerAttacks) {
            Mode = 0;
            return;
        }

        invincibleTimeStart = GameManager.Instance.unpausedTime;
        var vert = PlayerInput.GetAxis(PlayerInputKey.Vertical);
        if (vert != 0) {
            velocity.y = Mathf.MoveTowards(velocity.y, maxvert * vert, accelvert * Time.deltaTime * GameManager.Instance.deltaCoef);
        }
        else {
            velocity.y = Mathf.MoveTowards(velocity.y, 0, decelvert * Time.deltaTime * GameManager.Instance.deltaCoef);
        }
        velocity.x = dashSpeed * transform.localScale.x;

        if (GameManager.Instance.timeSince(timeSinceDashStart) > dashTimeMax) {
            velocity.x = velocity.x / 2;
            Mode = 0;
            dashHitBox.SetActive(false);
        }
    }

    void State_Roll() {
        if(GameManager.Instance.timeSince(timeSinceRoll) >= rollTime) {
            Mode = 0;
        }
        velocity.y = Mathf.MoveTowards(velocity.y, 0, rollDecel * Time.deltaTime * GameManager.Instance.deltaCoef);

        float hor = PlayerInput.GetAxis(PlayerInputKey.Horizontal);

        if (hor != 0) {
            velocity.x = Mathf.MoveTowards(velocity.x, maxhor * hor, accelhor * Time.deltaTime * GameManager.Instance.deltaCoef);
            transform.localScale = new Vector3(1 * Mathf.Sign(hor), 1, 1);
        }
        else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, decelhor * Time.deltaTime * GameManager.Instance.deltaCoef);
        }

        Attack();
    }

    void Attack() {        

        if (PlayerInput.GetButtonBuffer(PlayerInputBuffer.ShootPressed) <= fireBufferMax) {
            wpnInv.ActivateWeapon(0);
            PlayerInput.NullifyButtonBuffer(PlayerInputBuffer.ShootPressed);
        }

        if (PlayerInput.GetButton(PlayerInputKey.ExWpn1)) {
            wpnInv.ActivateWeapon(1);
        }
        if (PlayerInput.GetButton(PlayerInputKey.ExWpn2)) {
            wpnInv.ActivateWeapon(2);
        }
        if (PlayerInput.GetButton(PlayerInputKey.ExWpn3)) {
            wpnInv.ActivateWeapon(3);
        }

        if (PlayerInput.GetButtonUp(PlayerInputKey.Shoot)) {
            wpnInv.DeactivateWeapon(0);
        }
        if (PlayerInput.GetButtonUp(PlayerInputKey.ExWpn1)) {
            wpnInv.DeactivateWeapon(1);
        }
        if (PlayerInput.GetButtonUp(PlayerInputKey.ExWpn2)) {
            wpnInv.DeactivateWeapon(2);
        }
        if (PlayerInput.GetButtonUp(PlayerInputKey.ExWpn3)) {
            wpnInv.DeactivateWeapon(3);
        }

        // WEAPONS //

        /*
        if (wpnInv.currentLaserData.enabled) {
            if (PlayerInput.GetButtonBuffer(PlayerInputBuffer.ShootPressed) <= fireBufferMax && TimeSinceFire > fireRate) {
                int mult = wpnInv.WeaponLevels["Laser Multiplier"];
                float startPos = (mult - 1) * 0.15f;
                TimeSinceFire = 0;

                for (int i = 0; i < mult; i++) {
                    Vector3 pos = transform.position + new Vector3(transform.localScale.x * 7.5f / 16f, -1.5f / 16f - startPos + 0.3f * i, 0);
                    Vector3 vel = Vector3.right * transform.localScale.x * bulletSpeed;
                    SimpleBullet bullet = Instantiate(normalBullet, pos, Quaternion.identity).GetComponent<SimpleBullet>();
                    bullet.velocity = vel;
                    bullet.transform.localScale = transform.localScale;
                    bullet.damage = wpnInv.WeaponLevels["Laser Power"] - (0.2f * (mult-1));
                }

                audioSource.PlayOneShot(laserShoot, 1);
            }
        }

        if (wpnInv.currentStunWaveData.enabled) {
            if (PlayerInput.GetButtonBuffer(PlayerInputBuffer.StunPressed) <= fireBufferMax && GameManager.Instance.timeSince(timeSinceStun) > stunChargeTime) {
                timeSinceStun = GameManager.Instance.unpausedTime;
                Vector3 pos = transform.position + new Vector3(transform.localScale.x * 7.5f / 16f, -1.5f / 16f, 0);
                Vector3 vel = Vector3.right * bulletSpeed * 0.75f;
                SimpleBullet bullet = Instantiate(stunWave, pos, Quaternion.identity).GetComponent<SimpleBullet>();
                bullet.velocity = vel;
                bullet.transform.localScale = new Vector3(1, 1, 1);
                bullet.damage = 0;

                vel = Vector3.right * -1f * bulletSpeed * 0.75f;
                bullet = Instantiate(stunWave, pos, Quaternion.identity).GetComponent<SimpleBullet>();
                bullet.velocity = vel;
                bullet.transform.localScale = new Vector3(-1, 1, 1);
                bullet.damage = 0;
                barHandler.AddTimedChargeBar(stunChargeTime, Color.yellow);
            }
        }

        if (wpnInv.currentStarcutterData.enabled) {
            dashChargeTime = Mathf.MoveTowards(dashChargeTime, dashFullCharge, Time.deltaTime);
            if (PlayerInput.GetButtonBuffer(PlayerInputBuffer.DashPressed) <= fireBufferMax && dashChargeTime >= dashFullCharge / wpnInv.WeaponLevels["Starcutter Use"]) {
                dashChargeTime -= dashFullCharge / wpnInv.WeaponLevels["Starcutter Use"];
                Mode = 1;
                timeSinceDashStart = GameManager.Instance.unpausedTime;
                dashHitBox.SetActive(true);
                barHandler.AddTimedChargeBar(dashChargeTime, Color.magenta);
            }
        }

        if (wpnInv.currentFlamethrowerData.enabled) {
            if (PlayerInput.GetButton(PlayerInputKey.Fire)
                && (fireCharge == fireFullCharge || usingFire)
                && GameManager.Instance.timeSince(timeSinceFireBreath) > fireBreathRate
                && fireCharge > 0) {

                timeSinceFireBreath = 0;
                usingFire = true;
                Vector3 pos = transform.position + new Vector3(transform.localScale.x * 7.5f / 16f, -1.5f / 16f, 0);
                Vector3 vel = Vector3.right * transform.localScale.x * bulletSpeed * 0.7f * (0.5f + wpnInv.WeaponLevels["Firebreath Range"] * 0.5f);
                SimpleBullet bullet = Instantiate(fireBreath, pos, Quaternion.identity).GetComponent<SimpleBullet>();
                bullet.velocity = vel;
                bullet.transform.localScale = transform.localScale;
                bullet.damage = 0.1f * wpnInv.WeaponLevels["Firebreath Power"];
                bullet.burn = 0.2f * wpnInv.WeaponLevels["Firebreath Power"];

                fireCharge = Mathf.MoveTowards(fireCharge, 0, Time.deltaTime / wpnInv.WeaponLevels["Firebreath Use"]);

            }
            else {
                fireCharge = Mathf.MoveTowards(fireCharge, fireFullCharge, Time.deltaTime * 0.5f);
            }

            if (!PlayerInput.GetButton(PlayerInputKey.Fire) && usingFire || fireCharge == 0) {
                usingFire = false;
            }
        }

        */
    }

    public override void OnHurt(float damage, Vector2 knockbackSpd, float stunTime, DamageType type, float burn, float knockbackOverride) {
        if (GameManager.Instance.timeSince(invincibleTimeStart) > invincibleTimeMax) {
            if (Mode == 2) Mode = 1;
            base.OnHurt(damage, knockbackSpd, stunTime, type, burn, knockbackOverride);
            invincibleTimeStart = GameManager.Instance.unpausedTime;
            invincibleTimeMax = 0.5f;
        }
    }
}
