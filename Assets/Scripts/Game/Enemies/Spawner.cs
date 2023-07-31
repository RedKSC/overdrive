using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : AlkylEntity
{
    public List<GameObject> toSpawn;
    public List<GameObject> spawnedList;
    public GameObject baiter;
    public float spawnRate;
    public float baiterSpawnRate;

    float lastBaiterSpawn;
    float lastSpawn;
    int spawned;

    public override void Start() {
        base.Start();
        lastSpawn = GameManager.Instance.unpausedTime;
    }

    // Update is called once per frame
    void Update()
    {
        switch(Mode) {
            case 0:
                if (GameManager.Instance.timeSince(lastSpawn) > spawnRate) {
                    lastSpawn = GameManager.Instance.unpausedTime;
                    ODEnemy enemy = Instantiate(toSpawn[spawned], transform.position, Quaternion.identity, transform).GetComponent<ODEnemy>();
                    enemy.spawner = this;
                    spawnedList.Add(enemy.gameObject);
                    spawned++;
                    if (spawned >= toSpawn.Count) {
                        Mode = 1;
                        lastBaiterSpawn = GameManager.Instance.unpausedTime;
                    }
                }
                break;
            case 1:
                //Check for any alive enemies (not counting baiters). If not, destroy self
                bool found = false;
                for(int i = 0; i < spawnedList.Count; i++) {
                    if(spawnedList[i]) {
                        found = true;
                    }
                }
                if(!found) {
                    Destroy(gameObject);
                }

                //Pop out a baiter after being active for long enough
                if (GameManager.Instance.timeSince(lastBaiterSpawn) > baiterSpawnRate) {
                    lastBaiterSpawn = GameManager.Instance.unpausedTime;
                    ODEnemy enemy = Instantiate(baiter, transform.position, Quaternion.identity, transform).GetComponent<ODEnemy>();
                    enemy.spawner = this;
                    spawnedList.Add(enemy.gameObject);
                }
                break;
        }
        
    }
}
