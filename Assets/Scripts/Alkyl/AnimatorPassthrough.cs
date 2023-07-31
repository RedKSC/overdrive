using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Takes in any number fo animator and passes all the animator variables
//assuming they are all usign the master animators
public class AnimatorPassthrough : MonoBehaviour {
    public Animator[] Passes;

    List<AnimatorTag> Tags = new List<AnimatorTag>();

    public int AddTag(string Name) {
        Tags.Add(new AnimatorTag(Name));
        return Tags.Count - 1;
    }

    public void SetFloat(int ID, float value) {
        AnimatorTag tag = Tags[ID];
        foreach (Animator anim in Passes) {
            anim.SetFloat(tag.ID, value);
        }
    }

    public void SetInt(int ID, int value) {
        AnimatorTag tag = Tags[ID];
        foreach (Animator anim in Passes) {
            anim.SetInteger(tag.ID, value);
        }
    }

    public void SetBool(int ID, bool value) {
        AnimatorTag tag = Tags[ID];
        foreach (Animator anim in Passes) {
            anim.SetBool(tag.ID, value);
        }
    }

    public void SetTrigger(int ID) {
        AnimatorTag tag = Tags[ID];
        foreach (Animator anim in Passes) {
            anim.SetTrigger(tag.ID);
        }
    }
}

[System.Serializable]
public class AnimatorTag {
    public AnimatorTag(string name) {
        name = Name;
        Calculate();
    }

    public string Name;
    [HideInInspector]
    public int ID { get; private set; }

    public void Calculate() {
        ID = Animator.StringToHash(Name);
    }
}