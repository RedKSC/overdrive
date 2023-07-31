using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberBomb : MonoBehaviour
{
    public float downSpeed;
    public SpriteRenderer warning;
    float birthTime;
    public float maxTime;
    public GameObject bomberExplosion;
    void Start()
    {
        birthTime = GameManager.Instance.unpausedTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * downSpeed * Time.deltaTime;
        downSpeed = Mathf.MoveTowards(downSpeed, 0, 5 * Time.deltaTime);

        Color col = warning.color;
        warning.color = new Color(col.r, col.g, col.b, GameManager.Instance.timeLerp(birthTime, maxTime));
        if(GameManager.Instance.timeSince(birthTime) >= maxTime) {
            Destroy(gameObject);
            Instantiate(bomberExplosion, transform.position, Quaternion.identity);
        }
    }
}
