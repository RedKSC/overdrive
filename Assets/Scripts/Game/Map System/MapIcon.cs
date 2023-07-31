using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapIcon : MonoBehaviour {
    [AssetsOnly, Required]
    public RectTransform Icon;
    public bool Passive;
    public bool MakeCopies;
    public bool AutoDestroy;
    public Transform ReferenceOverride;

    private void Awake() {
        if (!ReferenceOverride) {
            ReferenceOverride = transform;
        }

        ODEntity ent = GetComponent<ODEntity>();

        if (ent) {
            ent.OnEntDestroyed += Destroy;
        }
    }

    private void Start() {
        Map.Instance.AddIcon(Icon, ReferenceOverride, Passive, MakeCopies);
    }

    private void Destroy() {
        if(AutoDestroy) {
            Map.Instance.RemoveIcon(ReferenceOverride, Passive);
        }

        GetComponent<ODEntity>().OnEntDestroyed -= Destroy;
    }
}
