using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponActivationType {
    Press,
    Hold
}

public class PlayerWeaponBase : MonoBehaviour
{
    public float drainSpeed;        //Percent per second if HOLD type, or percent total if PRESS type
    public float rechargeSpeed;     //Percent per second
    public float canUseThreshold;   //Percent 

    public WeaponActivationType activateType;

    public AudioSource audioSource;
    public AudioClip shootSound;

    public Weapon weaponData;

    public float currentCharge;            //Percent

    public float fireRate;
    float fireTimeLast;

    bool active;

    public virtual bool Activate() {
        if (activateType == WeaponActivationType.Press && active) return false;

        active = true;
        if (currentCharge >= canUseThreshold 
            && currentCharge >= drainSpeed * (activateType == WeaponActivationType.Hold ? Time.deltaTime : 1)
            && GameManager.Instance.timeSince(fireTimeLast) >= fireRate) 
        {
            currentCharge -= drainSpeed * (activateType == WeaponActivationType.Hold ? Time.deltaTime : 1);
            fireTimeLast = GameManager.Instance.unpausedTime;
            SpawnBullet();
            
            return true;
        } else {
            Deactivate();
        }
        return false;
    }

    public virtual void Deactivate() {
        active = false;
    }

    public void Update() {
        currentCharge = Mathf.MoveTowards(currentCharge, 1, rechargeSpeed * Time.deltaTime);
    }

    public virtual void SpawnBullet() { }
}
