using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingBullet : SimpleBullet
{
    public Vector3 startSize;
    public Vector3 endSize;
    public void Awake() {
        transform.localScale = new Vector3(startSize.x * Mathf.Sign(transform.localScale.x), startSize.y, startSize.z);
    }
    public override void Update() 
    {
        base.Update();
        transform.localScale = Vector3.Lerp(new Vector3(startSize.x * Mathf.Sign(transform.localScale.x), startSize.y, startSize.z), new Vector3(endSize.x * Mathf.Sign(transform.localScale.x), endSize.y, endSize.z), GameManager.Instance.timeSince(birthTime) / lifeTime);
    }
}
