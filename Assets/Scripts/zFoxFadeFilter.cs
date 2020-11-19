using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FOXFADE_STATE { 
    NON,
    IN,
    OUT
}

public class zFoxFadeFilter : MonoBehaviour {
    public static zFoxFadeFilter instance = null;

    public GameObject fadeFilterObject = null;
    public string attacheObject = "FadeFilterPoint";

    [System.NonSerialized] public FOXFADE_STATE fadeState;

    private float startTime;
    private float fadeTime;
    private Color fadeColor;

    void Awake() {
        instance = this;
        fadeState = FOXFADE_STATE.NON;

        fadeFilterObject.GetComponent<Renderer>().sortingLayerName = "GUI";
        fadeFilterObject.GetComponent<Renderer>().sortingOrder = 0;
    }

    void SetFadeAction(FOXFADE_STATE state, Color color, float time) {
        fadeState = state;
        startTime = Time.time;
        fadeTime = time;
        fadeColor = color;
    }

    public void FadeIn(Color color, float time) {
        SetFadeAction(FOXFADE_STATE.IN, color, time);
    }

    public void FadeOut(Color color, float time) {
        SetFadeAction(FOXFADE_STATE.OUT, color, time);
    }

    void setFadeFilterColor(bool enabled, Color color) {
        if (fadeFilterObject) {
            fadeFilterObject.GetComponent<Renderer>().enabled = enabled;
            fadeFilterObject.GetComponent<Renderer>().material.color = color;

            SpriteRenderer sprite = fadeFilterObject.GetComponent<SpriteRenderer>();

            if (sprite) {
                sprite.enabled = enabled;
                sprite.color = color;
                fadeFilterObject.SetActive(enabled);
            }
        }
    }

    void Update() {
        if (attacheObject != null) {
            GameObject go = GameObject.Find(attacheObject);
            fadeFilterObject.transform.position = go.transform.position;
        }

        switch (fadeState) {
            case FOXFADE_STATE.NON:
                break;

            case FOXFADE_STATE.IN:
                fadeColor.a = 1.0f - ((Time.time - startTime) / fadeTime);

                if (fadeColor.a > 1.0f || fadeColor.a < 0.0f) {
                    fadeColor.a = 0.0f;
                    fadeState = FOXFADE_STATE.NON;
                    setFadeFilterColor(false, fadeColor);
                    break;
                }

                setFadeFilterColor(true, fadeColor);
                break;

            case FOXFADE_STATE.OUT:
                fadeColor.a = (Time.time - startTime) / fadeTime;

                if (fadeColor.a > 1.0f || fadeColor.a < 0.0f) {
                    fadeColor.a = 1.0f;
                    fadeState = FOXFADE_STATE.NON;
                }

                setFadeFilterColor(true, fadeColor);
                break;


        }
    }
}
