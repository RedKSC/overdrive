using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ArtWrapOffset : MonoBehaviour {
    public GameCamera cam;
    public Playfield play;
    public Vector3 initPos;
    public Vector3 addPos;
    public Vector2 offset;

    private void Awake() {
        //WrappedTransform wrapTrans = transform.parent.GetComponent<WrappedTransform>();
        //wrapTrans.OnLateUpdate += SetWrap;
    }
    void Start() {
        cam = GameCamera.Instance;
        play = Playfield.Instance;
        initPos = transform.localPosition;

        cam.OnCameraLateUpdate += SetWrap;
        //cam.OnSetWrap += SetWrap;

    }
    public void OnDestroy() {
        cam.OnCameraLateUpdate -= SetWrap;
    }
    public void LateUpdate() {
        SetWrap();
    }

    public void SetWrap() {
        float difX = (cam.transform.position.x - transform.parent.position.x) / (play.PlayfieldSize.x / 2);
        offset.x = Mathf.Floor(Mathf.Abs(difX)) * Mathf.Sign(difX);

        float difY = (cam.transform.position.y - transform.parent.transform.position.y) / (play.PlayfieldSize.y / 2);
        offset.y = Mathf.Floor(Mathf.Abs(difY)) * Mathf.Sign(difY);

        float bestPosx = offset.x * play.PlayfieldSize.x;// * Mathf.Sign(transform.parent.localScale.x);
        float bestPosy = offset.y * play.PlayfieldSize.y;// * Mathf.Sign(transform.parent.localScale.y);
        transform.position =
            transform.parent.position +
            initPos +
            addPos +
            new Vector3(bestPosx, bestPosy, 0);
    }
}
