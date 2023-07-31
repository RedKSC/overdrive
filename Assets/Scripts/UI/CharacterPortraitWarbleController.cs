using System.Net.Mime;
using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CharacterPortraitWarbleController : MonoBehaviour {

    [LabelText("CA Warble"), MinMaxSlider(0f, 1f)]
    public Vector2 CAWarbleMinMax;
    [LabelText("CA Time"), MinMaxSlider(0f, 5f)]
    public Vector2 CATimeMinMax;

    [LabelText("CA Change Time"), MinMaxSlider(0f, 2f)]
    public Vector2 CAChangeTimeMinMax;

    Material mat;
    TimedRepeatAction repeater;
    float currentCA;
    float targetCA;

    private void Awake() {
        mat = GetComponent<Image>().material;
        repeater = GetComponent<TimedRepeatAction>();
        repeater.Setup(OnTimer, Random.Range(CATimeMinMax.x, CATimeMinMax.y));
    }

    void OnTimer() {
        LeanTween.value(0f, 1f, Random.Range(CAChangeTimeMinMax.x, CAChangeTimeMinMax.y)).setEaseInOutBounce().setOnStart(() => {
            targetCA = Random.Range(CAWarbleMinMax.x, CAWarbleMinMax.y);
        }).setOnUpdate((float t) => {
            mat.SetFloat("_CA_Amount", Mathf.Lerp(currentCA, targetCA, t));
        }).setOnComplete(() => {
            currentCA = targetCA;
        });

        //rerun the setup command to make it happen again at a different time
        repeater.Setup(OnTimer, Random.Range(CATimeMinMax.x, CATimeMinMax.y));
    }
}
