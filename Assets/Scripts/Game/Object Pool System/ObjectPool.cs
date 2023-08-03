using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    public static ObjectPool Instance {
        get {
            if (!instance) {
                instance = GameObject.Find("Object Pool").GetComponent<ObjectPool>();
            }

            return instance;
        }
    }
    static ObjectPool instance;

    readonly Dictionary<string, Queue<GameObject>> poolLookup = new();
    readonly Dictionary<Queue<GameObject>, ObjectPoolData> poolDataLookup = new();

    //Creates and initializes an object pool
    public void CreateObjectPool(ObjectPoolData dta, bool initialize = true) {
        if (poolLookup.ContainsKey(dta.Key)) {
            Debug.LogWarning($"Object pool with the key '{dta.Key}' already exists!", gameObject);
            return;
        }

        poolLookup.Add(dta.Key, new Queue<GameObject>());
        poolDataLookup.Add(poolLookup[dta.Key], dta);

        if (initialize) {
            InitializePool(dta.Key, dta.Object, dta.InitialCount);
        }
    }

    //Dequeues the next object in the list and returns it
    public GameObject GetNextObject(string poolKey) {
        if (!poolLookup.ContainsKey(poolKey)) {
            return null;
        }

        Queue<GameObject> pool = poolLookup[poolKey];

        if (pool.Count <= 0) {
            ObjectPoolData data = poolDataLookup[pool];

            if (data.AutoReinitialize) {
                InitializePool(data.Key, data.Object, data.InitialCount);
                return pool.Dequeue();
            }

            Debug.LogWarning($"Pool {data.Key}' is out of objects! No more will spawn");
            return null;
        }

        GameObject go = pool.Dequeue();

        return go;
    }

    //Creates a new game object and sets it up to work with the OP system
    GameObject CreateNewObj(GameObject obj) {
        GameObject go = Instantiate(obj);
        go.transform.parent = transform;
        go.SetActive(false);

        return go;
    }
    
    //Spawns a given number of objects and adds them to the given object pool
    public void InitializePool(string poolID, GameObject obj, int count) {
        for (int i = 0; i < count; i++) {
            GameObject go = CreateNewObj(obj);
            poolLookup[poolID].Enqueue(go);
        }
    }
}
