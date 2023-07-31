using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    public float Speed;
    public float MinMoveDistance;
    public Vector2 LookAheadScale;
    public float LookAheadDistance;
    public Transform Target;

    Vector2 currentVelocity;
    Vector3 lastTargetPos;
    Vector2 wrapOffset;
    public static GameCamera Instance;
    Vector2 lookAhead => LookAheadScale * LookAheadDistance;
    bool ignoreWrap;

    public delegate void OnCameraLateUpdateEvent();
    public event OnCameraLateUpdateEvent OnCameraLateUpdate;

    private void Awake() {
        WrappedTransform trans = Target.GetComponent<WrappedTransform>();
        trans.OnXWrap += Target_xWrap;
        trans.OnYWrap += Target_yWrap;

        WrappedTransform myTrans = GetComponent<WrappedTransform>();

        myTrans.OnXWrap += Self_XWrap;
        myTrans.OnYWrap += self_yWrap;

        Instance = this;
    }

    private void Update() {
        if(!ignoreWrap){
            currentVelocity = Target.position - lastTargetPos;
        }
        ignoreWrap = false;

        Vector2 targetPos = (Vector2)Target.position + (currentVelocity * lookAhead) + wrapOffset;
        float dist = Vector2.Distance(transform.position, targetPos);

        if(dist < MinMoveDistance){
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPos, Speed * Time.deltaTime * dist);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    private void LateUpdate() {
        lastTargetPos = Target.position;
        OnCameraLateUpdate?.Invoke();
    }

    void Target_xWrap(bool lowToHigh) {
        wrapOffset.x -= (Playfield.Instance.PlayfieldSize.x * (lowToHigh ? 1f : -1f));
        ignoreWrap = true;
    }
    void Target_yWrap(bool lowToHigh) {
        wrapOffset.y -= Playfield.Instance.PlayfieldSize.y * (lowToHigh ? 1f : -1f);
        ignoreWrap = true;
    }

    void Self_XWrap(bool lowToHigh) => wrapOffset.x += Playfield.Instance.PlayfieldSize.x * (lowToHigh ? 1f : -1f);
    void self_yWrap(bool lowToHigh) => wrapOffset.y += Playfield.Instance.PlayfieldSize.y * (lowToHigh ? 1f : -1f);

    private void OnDrawGizmos() {
        if(!Application.isPlaying){
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere((Vector2)Target.position + (currentVelocity * lookAhead), 0.1f);
    }

}