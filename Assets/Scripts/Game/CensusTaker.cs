using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CensusTaker : MonoBehaviour
{
    public List<Citizen> citizens;
    public List<Planet> planets;
    public static CensusTaker Instance;

    public void Awake() {
        Instance = this;
    }

    public Vector2 OffsetCyclical(Vector2 fromPos, Vector2 toPos) {
        Playfield play = Playfield.Instance;
        Vector2 offset = toPos - fromPos;
        if(offset.x > play.PlayfieldSize.x/2) {
            offset.x -= play.PlayfieldSize.x;
        }
        if (offset.x < -play.PlayfieldSize.x/2) {
            offset.x += play.PlayfieldSize.x;
        }
        if (offset.y > play.PlayfieldSize.y/2) {
            offset.y -= play.PlayfieldSize.y;
        }
        if (offset.y < -play.PlayfieldSize.y/2) {
            offset.y += play.PlayfieldSize.y;
        }
        return offset;
    }

   
}

