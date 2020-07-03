using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum zFoxTWEEN_VALUE {
    POSITION,
    POSITION_X,
    POSITION_Y,
    POSITION_Z,
    LOCALPOSITION_X,
    LOCALPOSITION_Y,
    LOCALPOSITION_Z,
    LOCALROTATION,
    LOCALROTATION_X,
    LOCALROTATION_Y,
    LOCALROTATION_Z,
    LOCALSCALE,
    LOCALSCALE_X,
    LOCALSCALE_Y,
    LOCALSCALE_Z,
    COLOR_R,
    COLOR_G,
    COLOR_B,
    COLOR_A,
    COLOR_RGB,
    COLOR_RGBA
}

public enum zFOXTWEEN_OPM {
    NON,
    REPEAT,
    PINGPONG,
    SMOOTHSTEP,
    SMOOTHSTEP_PINGPONG,
    SMOOTHDUMP,
    SMOOTHDUMP_PINGPONG,
    SIN,
    COS,
    TAN,
    RANDOM
}

public enum zFOXTWEEN_OUT {
    OVERRIDE,
    ADD,
    SUB,
    ADDxWEIGHT,
    SUBxWEIGHT
}

public enum zFOXTWEEN_FILTER {
    NON,
    MIN,
    MAX,
    MINMAX
}

public class zFoxTween : MonoBehaviour
{
    [System.Serializable]
    public class TweenItem {
        public bool enabled = true;
        public zFoxTWEEN_VALUE valueType = zFoxTWEEN_VALUE.LOCALSCALE;
        public zFOXTWEEN_OPM opmMode = zFOXTWEEN_OPM.NON;
        public zFOXTWEEN_OUT outMode = zFOXTWEEN_OUT.ADD;
        public float outWeight = 1.0f;
        public float va = 0.0f;
        public float vb = 1.0f;
        public float speed = 1.0f;
        public float vt = 0.0f;

        public float _smoothDumpVelocity = 0.0f;
        public float _smoothDumpMaxSpeed = 1.0f;

        public zFOXTWEEN_FILTER filterMode = zFOXTWEEN_FILTER.NON;
        public float filterMin = 0.0f;
        public float filterMax = 1.0f;
    }

    public TweenItem[] tweenItemList;

    Vector3 orgPosition;
    Vector3 orgLocalRotationEA;
    Vector3 orgLocalScale;
    Color orgColor;
    SpriteRenderer sprite;

    bool cameraVisible = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        orgPosition = transform.position;
        orgLocalRotationEA = transform.localRotation.eulerAngles;
        orgLocalScale = transform.localScale;

        if (sprite != null) {
            orgColor = GetComponent<SpriteRenderer>().color;
        }
    }

    private void OnBecameInvisible()
    {
        cameraVisible = true;
    }

    private void OnBecameVisible()
    {
        cameraVisible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float position_x = orgPosition.x;
        float position_y = orgPosition.y;
        float position_z = orgPosition.z;
        float localRotation_x = orgLocalRotationEA.x;
        float localRotation_y = orgLocalRotationEA.y;
        float localRotation_z = orgLocalRotationEA.z;

        float localScale_x = orgLocalScale.x;
        float localScale_y = orgLocalScale.y;
        float localScale_z = orgLocalScale.z;
        float color_r = orgColor.r;
        float color_g = orgColor.g;
        float color_b = orgColor.b;
        float color_a = orgColor.a;

        if (!cameraVisible || tweenItemList.Length <= 0) {
            return;
        }

        foreach (TweenItem tw in tweenItemList) {
            if (!tw.enabled) {
                continue;
            }

            switch (tw.valueType) {
                case zFoxTWEEN_VALUE.POSITION_X:
                    position_x = TweenFloat(tw, position_x, orgPosition.x); break;

                case zFoxTWEEN_VALUE.POSITION_Y:
                    position_y = TweenFloat(tw, position_y, orgPosition.y); break;

                case zFoxTWEEN_VALUE.POSITION_Z:
                    position_z = TweenFloat(tw, position_z, orgPosition.z); break;

                case zFoxTWEEN_VALUE.POSITION:
                    position_x = TweenFloat(tw, position_x, orgPosition.x);
                    position_y = TweenFloat(tw, position_y, orgPosition.y);
                    position_z = TweenFloat(tw, position_z, orgPosition.z);
                    break;

                case zFoxTWEEN_VALUE.LOCALROTATION_X:
                    localRotation_x = TweenFloat(tw, localRotation_x, localRotation_x); break;

                case zFoxTWEEN_VALUE.LOCALROTATION_Y:
                    localRotation_y = TweenFloat(tw, localRotation_y, localRotation_y); break;

                case zFoxTWEEN_VALUE.LOCALROTATION_Z:
                    localRotation_z = TweenFloat(tw, localRotation_z, localRotation_z); break;

                case zFoxTWEEN_VALUE.LOCALROTATION:
                    localRotation_x = TweenFloat(tw, localRotation_x, localRotation_x);
                    localRotation_y = TweenFloat(tw, localRotation_y, localRotation_y);
                    localRotation_z = TweenFloat(tw, localRotation_z, localRotation_z);
                    break;

                case zFoxTWEEN_VALUE.LOCALSCALE_X:
                    localScale_x += TweenFloat(tw, localScale_x, orgLocalScale.x); break;

                case zFoxTWEEN_VALUE.LOCALSCALE_Y:
                    localScale_y += TweenFloat(tw, localScale_y, orgLocalScale.y); break;

                case zFoxTWEEN_VALUE.LOCALSCALE_Z:
                    localScale_z += TweenFloat(tw, localScale_z, orgLocalScale.z); break;

                case zFoxTWEEN_VALUE.LOCALSCALE:
                    localScale_x += TweenFloat(tw, localScale_x, orgLocalScale.x);
                    localScale_y += TweenFloat(tw, localScale_y, orgLocalScale.y);
                    localScale_z += TweenFloat(tw, localScale_z, orgLocalScale.z);
                    break;

                case zFoxTWEEN_VALUE.COLOR_R:
                    color_r = TweenFloat(tw, color_r, orgColor.r); break;

                case zFoxTWEEN_VALUE.COLOR_G:
                    color_g = TweenFloat(tw, color_g, orgColor.g); break;

                case zFoxTWEEN_VALUE.COLOR_B:
                    color_b = TweenFloat(tw, color_b, orgColor.b); break;

                case zFoxTWEEN_VALUE.COLOR_A:
                    color_a = TweenFloat(tw, color_a, orgColor.a); break;

                case zFoxTWEEN_VALUE.COLOR_RGB:
                    color_r = TweenFloat(tw, color_r, orgColor.r);
                    color_g = TweenFloat(tw, color_g, orgColor.g);
                    color_b = TweenFloat(tw, color_b, orgColor.b);
                    break;

                case zFoxTWEEN_VALUE.COLOR_RGBA:
                    color_r = TweenFloat(tw, color_r, orgColor.r);
                    color_g = TweenFloat(tw, color_g, orgColor.g);
                    color_b = TweenFloat(tw, color_b, orgColor.b);
                    color_a = TweenFloat(tw, color_a, orgColor.a);
                    break;
            }

            transform.position = new Vector3(position_x, position_y, position_z);
            transform.localRotation = Quaternion.Euler(localRotation_x, localRotation_y, localRotation_z);
            transform.localScale = new Vector3(localScale_x, localScale_y, localScale_z);

            if (sprite != null) {
                sprite.color = new Color(color_r, color_g, color_b, color_a);
            }
        }
    }

    float TweenFloat(TweenItem tw, float nowN, float orgN) {
        float n = tw.va;
        float v = tw.vb - tw.va;
        float t = Time.time * tw.speed + tw.vt;

        switch (tw.opmMode) {
            case zFOXTWEEN_OPM.REPEAT:
                n = tw.va + Mathf.Repeat(t, v); break;
            case zFOXTWEEN_OPM.PINGPONG:
                n = tw.va + Mathf.PingPong(t, v); break;
            case zFOXTWEEN_OPM.SMOOTHSTEP:
                n = tw.va + Mathf.SmoothStep(tw.va, tw.vb, Mathf.PingPong(t, 1.0f)); break;
            case zFOXTWEEN_OPM.SMOOTHSTEP_PINGPONG:
                n = tw.va + Mathf.SmoothStep(tw.va, tw.vb, Mathf.PingPong(t, 1.0f)); break;
            case zFOXTWEEN_OPM.SMOOTHDUMP:
                n = tw.va + Mathf.SmoothDamp(tw.va, tw.vb, ref tw._smoothDumpVelocity,
                    tw._smoothDumpMaxSpeed, Mathf.Repeat(t, 1.0f));
                break;
            case zFOXTWEEN_OPM.SMOOTHDUMP_PINGPONG:
                n = tw.va + Mathf.SmoothDamp(tw.va, tw.vb, ref tw._smoothDumpVelocity,
                    tw._smoothDumpMaxSpeed, Mathf.PingPong(t, 1.0f));
                break;
            case zFOXTWEEN_OPM.SIN:
                n = tw.va + (v / 2.0f) * Mathf.Sin(t) + (v / 2.0f); break;
            case zFOXTWEEN_OPM.COS:
                n = tw.va + (v / 2.0f) * Mathf.Cos(t) + (v / 2.0f); break;
            case zFOXTWEEN_OPM.TAN:
                n = tw.va + (v / 2.0f) * Mathf.Tan(t) + (v / 2.0f); break;
            case zFOXTWEEN_OPM.RANDOM:
                n = Random.Range(tw.va, tw.vb); break;
        }

        switch (tw.outMode) {
            case zFOXTWEEN_OUT.OVERRIDE:
                nowN = orgN; break;
            case zFOXTWEEN_OUT.SUB:
                n = -n; break;
            case zFOXTWEEN_OUT.ADDxWEIGHT:
                n = n * tw.outWeight; break;
            case zFOXTWEEN_OUT.SUBxWEIGHT:
                n = -n * tw.outWeight; break;
        }

        switch (tw.filterMode) {
            case zFOXTWEEN_FILTER.MIN:
                n = Mathf.Min(n, tw.filterMin); break;
            case zFOXTWEEN_FILTER.MAX:
                n = Mathf.Max(n, tw.filterMax); break;
            case zFOXTWEEN_FILTER.MINMAX:
                n = Mathf.Clamp(n, tw.filterMin, tw.filterMax); break;


        }

        return nowN + n;
    }
}
