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

    Dictionary<string, int> poolLookup = new Dictionary<string, int>();

    Dictionary<int, Queue<GameObject>> pools = new Dictionary<int, Queue<GameObject>>();

    //Creates and initializes an object pool
    public int CreateObjectPool(ObjectPoolData dta, bool initialize = true) {
        if (poolLookup.ContainsKey(dta.Key)) {
            Debug.LogWarning($"Object pool with the key '{dta.Key}' already exists!", gameObject);
            return -1;
        }

        int hash = dta.Key.GetHashCode();

        poolLookup.Add(dta.Key, hash);

        pools.Add(hash, new Queue<GameObject>());

        if (initialize) {
            InitializePool(hash, dta.Object, dta.InitialCount);
        }

        return hash;
    }

    //Dequeues the next object in the list and returns it
    public GameObject GetNextObject(string pool) {
        if (!poolLookup.ContainsKey(pool)) {
            return null;
        }

        int key = poolLookup[pool];

        if (pools[key].Count <= 0) {
            return null;
        }

        GameObject go = pools[key].Dequeue();

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
    public void InitializePool(int poolID, GameObject obj, int count) {
        for (int i = 0; i < count; i++) {
            GameObject go = CreateNewObj(obj);
            pools[poolID].Enqueue(go);
        }
    }
}
