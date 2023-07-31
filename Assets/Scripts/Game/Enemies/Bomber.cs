using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Bomber : ODEnemy
{
    [FoldoutGroup("MovementParams")] public float speed;
    [FoldoutGroup("MovementParams")] public float randomMovementMax;
    [FoldoutGroup("MovementParams")] public Vector2 randomMovementVariance;

    [FoldoutGroup("CombatParams")] public float fireRate;
    [FoldoutGroup("CombatParams")] public Vector2 fireRateVariance;

    [FoldoutGroup("CombatVars")] public float fireTime;

    [FoldoutGroup("Resources")] public GameObject bomb;

    public Transform citizenTarget;
    float randomMovementLast;
    Vector2 velDir;

    public override void Awake() {
        base.Awake();
        transform.localScale = new Vector3(Mathf.Sign(Random.Range(-1f, 1f)), 1, 1);
        if(transform.localScale.x == 0) transform.localScale = new Vector3(1, 1, 1);

        velDir.x = transform.localScale.x;
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
        base.OnDestroy();
        
    }

    public void State_Normal() {
        if (GameManager.Instance.timeSince(randomMovementLast) > randomMovementMax) {
            randomMovementLast = GameManager.Instance.unpausedTime + Random.Range(randomMovementVariance.x, randomMovementVariance.y);
            velDir.y = Mathf.Sign(Random.Range(-1f, 1f)) / 2;
        }
        if (GameManager.Instance.timeSince(fireTime) > fireRate) {
            fireTime = GameManager.Instance.unpausedTime + Random.Range(fireRateVariance.x, fireRateVariance.y);
            DropBomb();
        }

        velocity = velDir;
    }

    public void DropBomb() {
        Instantiate(bomb, transform.position + Vector3.down * 0.5f, Quaternion.identity);
    }
}
