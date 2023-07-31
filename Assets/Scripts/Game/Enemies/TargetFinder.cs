using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour{
    List<Transform> CurrentTargets = new List<Transform>();

    //Hee hee funny look up table AGAIN
    //I should really put this somewhere accessible
    Vector2 getOffset(int ID){
        switch(ID){
            case 0:
            return Vector2.left + Vector2.up;
            case 1:
            return Vector2.up;
            case 2: 
            return Vector2.one;
            case 3:
            return Vector2.left;
            case 4:
            return Vector2.right;
            case 5:
            return Vector2.left + Vector2.down;
            case 6:
            return Vector2.down;
            case 7:
            return Vector2.right + Vector2.down;
        }
        return Vector2.zero;
    }

    public Transform GetClosestTarget() => GetClosestTarget(out Vector2 smthn);

    public Transform GetClosestTarget(out Vector2 Offset) {
        if(CurrentTargets.Count <= 0){
            Offset = Vector2.zero;
            return null;
        }

        float lastClosest = float.MaxValue;
        int index = -1;
        Vector2 offset = Vector2.zero;

        for(int i = 0; i < CurrentTargets.Count; i++){
            for(int x = 0; x < 9; x++){
                Vector2 checkOffset = getOffset(x) * Playfield.Instance.PlayfieldSize;
                float dist = Vector2.Distance(transform.position, (Vector2)CurrentTargets[i].position + checkOffset);
                if(dist < lastClosest) {
                    index = i;
                    lastClosest = dist;
                    offset = checkOffset;
                }
            }
        }

        Offset = offset;
        return CurrentTargets[index];
    }

    public Vector2 GetNearestCopyLocation(Transform check) {
        Vector2 pos = check.position;
        Vector2 selfPos = transform.position;

        Vector2 finalPos = Vector2.zero;
        float closestDist = float.MaxValue;
        for(int x = 0; x < 9; x++) {
                Vector2 checkOffset = getOffset(x) * Playfield.Instance.PlayfieldSize;
                float dist = Vector2.Distance(transform.position, pos + checkOffset);

                if(dist < closestDist){
                    closestDist = dist;
                    finalPos = pos + checkOffset;
                }
        }

        return finalPos;
    }

    private void OnTriggerEnter2D(Collider2D other) => CurrentTargets.Add(other.transform);

    private void OnTriggerExit2D(Collider2D other) => CurrentTargets.Remove(other.transform);
}