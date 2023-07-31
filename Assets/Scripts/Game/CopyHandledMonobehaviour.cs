using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyHandledMonobehaviour : MonoBehaviour {
    public CopyHandledMonobehaviour Original;
    public new Transform transform => Original ? Original.gameObject.transform : gameObject.transform;
    public new T GetComponent<T>() where T : MonoBehaviour => Original ? Original.GetComponent<T>() : GetComponent<T>();
    public T Get<T>() where T : CopyHandledMonobehaviour => (T)(Original ? Original : this);
}