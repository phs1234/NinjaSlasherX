using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zFoxScreenAdjust : MonoBehaviour {
    public float aspectWH = 1.0f;
    public float aspectAdd = 0.05f;

    public bool startScreenAdjust = true;
    public bool updateScreenAdjust = false;

    Vector3 localScale;
    void Start() {
        localScale = transform.localScale;
        
        if (startScreenAdjust) {
            ScreenAdjust();
        }
    }

    void Update() {
        if (updateScreenAdjust) {
            ScreenAdjust();
        }
    }

    void ScreenAdjust() {
        float wh = (float)Screen.width / (float)Screen.height;

        if (wh < aspectWH) {
            transform.localScale = new Vector3(
                localScale.x - (aspectWH - wh) + aspectAdd,
                localScale.y, localScale.z);
        } else {
            transform.localScale = localScale;
        }
    }
}
