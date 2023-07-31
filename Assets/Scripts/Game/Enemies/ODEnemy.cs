using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class ODEnemy : ODEntity
{
    [HideInInspector] public TargetFinder finder;
    [HideInInspector] public Spawner spawner;
    [FoldoutGroup("CombatParams")] public long pointWorth = 100;

    [FoldoutGroup("Resources")] public Sprite topHalf;
    [FoldoutGroup("Resources")] public Sprite bottomHalf;
    [HideInInspector] public GameObject sliceSprite;
    public override void Awake() {
        base.Awake();
        sliceSprite = Resources.Load<GameObject>("Slice Sprite");
        WaveHandler.enemies.Add(gameObject);
    }
    public override void Update() {
        base.Update();
    }
    public override void OnDestroy() {
        WaveHandler.enemies.Remove(gameObject);

        if (killed) {

            AudioSource oneshot = Instantiate(oneShotPlayer, transform.position, Quaternion.identity).GetComponent<AudioSource>();
            oneshot.clip = killSound;
            oneshot.Play();

            GameManager.Instance.Points += pointWorth;
            switch(lastDamageTypeInflicted) {
                case DamageType.Blast:
                    Instantiate(burst, transform.position, Quaternion.identity);
                    break;
                case DamageType.Slice:
                    SliceSprite slice = Instantiate(sliceSprite, transform.position, Quaternion.identity).GetComponent<SliceSprite>();
                    slice.side = 1;
                    slice.GetComponent<SpriteRenderer>().sprite = topHalf;
                    slice = Instantiate(sliceSprite, transform.position, Quaternion.identity).GetComponent<SliceSprite>();
                    slice.side = -1;
                    slice.GetComponent<SpriteRenderer>().sprite = bottomHalf;
                    break;
            }
        }
           
    }
}
