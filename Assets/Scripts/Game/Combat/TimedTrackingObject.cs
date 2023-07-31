using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTrackingObject : MonoBehaviour
{
    public float lifeTime;
    float birthTime;
    public Transform follow;
    public ParticleSystemRenderer visualElement;

    // Start is called before the first frame update
    void Start()
    {
        birthTime = GameManager.Instance.unpausedTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.position;
        if(GameManager.Instance.timeSince(birthTime) > lifeTime) {
            Destroy(gameObject);
        }
        if (GameManager.Instance.timeSince(birthTime) > lifeTime * 0.8f) {
            visualElement.enabled = (Mathf.Round(GameManager.Instance.timeSince(birthTime)*20) % 2 == 0);
        }
    }
}
