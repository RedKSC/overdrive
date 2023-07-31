using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Map : MonoBehaviour {
    public static Map Instance;

    [SceneObjectsOnly]
    public RectTransform MapImage;

    [SceneObjectsOnly]
    public Transform IconHolder;

    [Range(0.1f, 4f)]
    public float MapScale = 1f;
    public float MapYSize = 200f;

    List<MapIconData> activeIcons = new List<MapIconData>();
    public List<RectTransform> iconCopies = new List<RectTransform>();
    List<MapIconData> passiveIcons = new List<MapIconData>();

    Vector2 getOffset(int ID){
        switch(ID){
            case 0:
            return new Vector2(-1f, 1f);
            case 1:
            return new Vector2(0f, 1f);
            case 2: 
            return new Vector2(1f, 1f);
            case 3:
            return new Vector2(-1f, 0f);
            case 4:
            return new Vector2(1f, 0f);
            case 5:
            return new Vector2(-1f, -1f);
            case 6:
            return new Vector2(0f, -1f);
            case 7:
            return new Vector2(1f, -1f);
        }
        return Vector2.zero;
    }

    private void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        float playfieldRatio = Playfield.Instance.PlayfieldSize.x / Playfield.Instance.PlayfieldSize.y;

        MapImage.sizeDelta = new Vector2(MapYSize * playfieldRatio * MapScale, MapYSize * MapScale);
    }

    private void Update() {
        for (int i = 0; i < activeIcons.Count; i++) {
            UpdateIcon(activeIcons[i]);
        }
    }

    public void AddIcon(RectTransform iconPrefab, Transform position, bool passive, bool makeCopies) {
        MapIconData dta = new MapIconData() {
            ReferenceObject = position,
            IconObject = Instantiate(iconPrefab, IconHolder),
            CopyID = makeCopies ? iconCopies.Count / 8 : -1,
        };

        (passive ? passiveIcons : activeIcons).Add(dta);

        if (makeCopies) {
            for (int i = 0; i < 8; i++) {
                iconCopies.Add(Instantiate(iconPrefab, IconHolder));
            }
        }

        UpdateIcon(dta);
    }

    public void RemoveIcon(Transform position, bool passive) {
        List<MapIconData> icons = (passive ? passiveIcons : activeIcons);

        for (int i = 0; i < icons.Count; i++) {
            if (icons[i].ReferenceObject == position) {
                Destroy(icons[i].IconObject.gameObject);
                icons.RemoveAt(i);
                return;
            }
        }

        Debug.LogWarning($"No match found for '{position.name}'! Ignoring...");
    }

    void UpdateIcon(MapIconData data) {
        //Debug.Log($"Update Icon: {data}");

        if (!data.ReferenceObject) {
            return;
        }

        Vector2 percent = Playfield.Instance.GetPlayfieldPercent(data.ReferenceObject.position);

        data.IconObject.anchoredPosition = MapImage.sizeDelta * percent;
        data.IconObject.eulerAngles = new Vector3(0f, 0f, data.ReferenceObject.eulerAngles.z);
        data.IconObject.localScale = data.ReferenceObject.lossyScale;

        if (data.CopyID == -1) {
            return;
        }

        for (int i = data.CopyID * 8; i < (data.CopyID * 8) + 8; i++) {
            Vector2 offset = getOffset(i - (data.CopyID * 8));

            iconCopies[i].anchoredPosition = MapImage.sizeDelta * (percent + offset);
            iconCopies[i].eulerAngles = data.IconObject.eulerAngles;
            iconCopies[i].localScale = data.ReferenceObject.localScale;
        }
    }
}

[System.Serializable]
public class MapIconData {
    public Transform ReferenceObject;
    public RectTransform IconObject;
    public int CopyID = -1;

    public override string ToString()
    {
        return $"Icon data: {ReferenceObject.name}:{CopyID}";
    }
}
