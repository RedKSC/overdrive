using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(WrappedTransform))]
public class ODEntity : AlkylEntity
{
    [FoldoutGroup("MovementVar")] public Vector2 velocity;

    [FoldoutGroup("CombatParams")] public float maxHealth;
    [FoldoutGroup("CombatParams")] public float StunResist;
    [FoldoutGroup("CombatParams")] public float KnockbackResist;

    [FoldoutGroup("CombatVars")] public float simpleHealth;
    [FoldoutGroup("CombatVars")] public float burnAmount;
    [FoldoutGroup("CombatVars")] public float stunTimeStart;
    [FoldoutGroup("CombatVars")] public float stunTimeMax;
    [FoldoutGroup("CombatVars")] public DamageType lastDamageTypeInflicted;
    

    [FoldoutGroup("Resources")] public Animator anim;
    [FoldoutGroup("Resources")] public GameObject burst;
    [FoldoutGroup("Resources")] public GameObject oneShotPlayer;
    [FoldoutGroup("Resources")] public UnityEngine.VFX.VisualEffect entityVFX;
    [FoldoutGroup("Resources")] public SpriteRenderer matSprite;
    [FoldoutGroup("Resources")] public Material mat;

    [FoldoutGroup("SFX")] public AudioSource audioSource;
    [FoldoutGroup("SFX")] public AudioClip killSound;

    [HideInInspector] public bool killed;
    [HideInInspector] public CensusTaker cs;

    public delegate void OnEntityDestroyedEvent();
    public event OnEntityDestroyedEvent OnEntDestroyed;

    [HideInInspector] public Vector3 artInitPos;
    public override void Awake() {
        base.Awake();
        if(anim == null) {
            anim = transform.GetChild(0).GetComponent<Animator>();
        }
        
        artInitPos = anim.transform.localPosition;

        simpleHealth = maxHealth;
        burst = Resources.Load<GameObject>("Burst FX");
        oneShotPlayer = Resources.Load<GameObject>("Oneshot Player");
        entityVFX = Instantiate(Resources.Load<GameObject>("Entity VFX"), anim.transform).GetComponent<UnityEngine.VFX.VisualEffect>();
        entityVFX.transform.localPosition = Vector3.zero;
        audioSource = GetComponent<AudioSource>();
        if(matSprite == null)
            mat = anim.GetComponent<SpriteRenderer>().material;
        else {
            mat = matSprite.material;
        }
    }
    public virtual void Update() {

        transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime * GameManager.Instance.deltaCoef * GameManager.Instance.gm2unityConv;

        switch (Mode) {
            case 4:
                State_Dead();
                break;
            case 5:
                State_Stunned();
                break;
        }
        burnAmount = Mathf.Clamp(Mathf.MoveTowards(burnAmount, 0, Time.deltaTime * 2), 0, 10);
        
        mat.SetFloat("_BurnLevel", burnAmount / 10);
        entityVFX.GetComponent<UnityEngine.VFX.VisualEffect>().SetInt("Density", Mathf.CeilToInt(burnAmount * 2));
        simpleHealth -= burnAmount * Time.deltaTime * 0.25f;
        if (simpleHealth <= 0) {
            stunTimeMax = 0.1f;
            Mode = 4;
        }
    }

    public override void Start() {
        cs = CensusTaker.Instance;
        base.Start();
    }

    void OnApplicationQuit() {
    }
    public virtual void OnDestroy() {
        if (killed) {
            Instantiate(burst, transform.position, Quaternion.identity);
            AudioSource oneshot = Instantiate(oneShotPlayer, transform.position, Quaternion.identity).GetComponent<AudioSource>();
            oneshot.clip = killSound;
            oneshot.Play();
        }
    }

    public virtual void State_Stunned() {
        anim.transform.localPosition = artInitPos;
        anim.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.05f, 0.05f), 0);
        if (GameManager.Instance.timeSince(stunTimeStart) > stunTimeMax) {
            anim.transform.localPosition = artInitPos;
            RevertMode();
        }
    }
    public virtual void State_Dead() {
        velocity = Vector2.zero;
        if (GameManager.Instance.timeSince(stunTimeStart) > stunTimeMax) {
            killed = true;
            OnEntDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }

    public virtual void OnHurt(float damage, Vector2 knockbackSpd, float stunTime, DamageType type, float burn, float knockbackOverride, Hurtbox hurtbox = null) {
        if(simpleHealth <= 0) { return; }

        simpleHealth -= damage;
        burnAmount += burn;
        if(stunTime > 0)
            velocity = Vector3.Lerp(knockbackSpd, velocity, Mathf.Clamp(KnockbackResist - knockbackOverride, 0, 1));
        stunTimeStart = GameManager.Instance.unpausedTime;
        stunTimeMax = stunTime / StunResist;
        lastDamageTypeInflicted = type;
        anim.SetTrigger("Hurt");
        
        if (simpleHealth <= 0) {
            stunTimeMax = 0.1f;
            Mode = 4;
        }
        else {
            Mode = 5;
        }
    }

    public virtual void AnimFunc1() {}
    public virtual void AnimFunc2() { }

    public Planet NearestPlanet() {
        if(cs == null) {
            cs = CensusTaker.Instance;
        }
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
