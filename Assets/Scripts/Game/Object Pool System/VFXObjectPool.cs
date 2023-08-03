using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class VFXObjectPool : MonoBehaviour {
    [LabelText("VFX Pools")]
    public ObjectPoolData[] VFXPools;

    private void Awake() {
        for (int i = 0; i < VFXPools.Length; i++) {
            ObjectPool.Instance.CreateObjectPool(VFXPools[i]);
        }
    }

    public static GameObject SpawnVFX(string key, Vector2 position) {
        GameObject gameObject = ObjectPool.Instance.GetNextObject(key);
        gameObject.transform.position = position;
        gameObject.SetActive(true);

        return gameObject;
    }
}
