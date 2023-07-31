using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBrightness : MonoBehaviour {
    public SpriteRenderer CloudRenderer;
    readonly Color colR = new Color(1f, 0.8f, 0.8f, 1f);
    readonly Color colB = new Color(0.8f, 0.8f, 1f, 1f);

    //yeah fuck it just hard code everything why not
    private void Awake() {
        float size = Random.Range(0f, 1f);

        float scale = Mathf.Lerp(0.1f, 0.4f, size);
        transform.localScale = new Vector3(scale, scale, scale);

        float alpha = Mathf.Lerp(0.1f, 1f, size);
        Color col = Color.Lerp(colR, colB, alpha);
        Color finalCol = new Color(col.r, col.g, col.b, alpha/4);
        GetComponent<SpriteRenderer>().color = finalCol;

        finalCol.a /= 2f;
        float cloudScale = Mathf.Lerp(0.3f, 2f, size);
        //CloudRenderer.transform.localScale = new Vector3(cloudScale, cloudScale, cloudScale);
        CloudRenderer.color = finalCol;

        if(GetComponent<FakeCopyLoop>()) {
            GetComponent<FakeCopyLoop>().ParallaxAmount = -((1-size) * 0.25f);
        }
    }
}