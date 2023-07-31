using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBullet : MonoBehaviour
{
    public ParticleSystem phase1;
    public ParticleSystem phase2;
    public ParticleSystem phase3;

    int state;
    float counter;

    public float persistTime;
    public float healTime;
    public int healAmount;

    bool touchPlayer;

    public float speed;
    public float angle; //In Degrees
    public float dragRate;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        switch(state) {
            case 0:

                transform.position += GameMath.ConvertToVector(angle).ConvertTo3D() * Time.deltaTime * speed;
                speed = Mathf.MoveTowards(speed, 0, dragRate * Time.deltaTime);

                if (touchPlayer && counter > 0.5f) {
                    counter = 0;
                    state = 1;
                    phase1.Stop();
                    phase2.Play();
                }
                if(counter > persistTime) {
                    Destroy(gameObject);
                }
                break;
            case 1:
                if (!touchPlayer) {
                    Destroy(gameObject);
                }
                if (counter > healTime) {
                    phase2.Stop();
                    phase3.Play();
                    state = 2;
                    PlayerController.Instance.simpleHealth += 1;
                    if(PlayerController.Instance.simpleHealth > PlayerController.Instance.maxHealth) {
                        PlayerController.Instance.simpleHealth = PlayerController.Instance.maxHealth;
                    }
                }
                break;
            case 2:
                if(phase3.particleCount == 0) {
                    Destroy(gameObject);
                }
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if(collision.GetComponent<Hurtbox>()) {
            touchPlayer = true;
        }
    }
    public void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<Hurtbox>()) {
            touchPlayer = false;
        }
    }
}
