using UnityEngine;

[System.Serializable, CreateAssetMenu(menuName = "Overdrive/Data/Object Pool")]
public class ObjectPoolData : ScriptableObject {
    public GameObject Object;
    public string Key;
    public int InitialCount;
    public bool AutoReinitialize;
}
