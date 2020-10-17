using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject_MoveBlock : MonoBehaviour
{
    public Vector3 velocityA = new Vector3(1.0f, 0.0f, 0.0f);
    public Vector3 velocityB = new Vector3(-1.0f, 0.0f, 0.0f);
    public float switchingTime = 5.0f;

    bool turnA;
    float changeTime = 0.0f;

    void Awake()
    {
        changeTime = Time.fixedTime;
        GetComponent<Rigidbody2D>().velocity = velocityA;
        turnA = true;
    }

    void FixedUpdate()
    {
        if (Time.fixedTime > changeTime + switchingTime) {
            turnA = !turnA;

            GetComponent<Rigidbody2D>().velocity = turnA ? velocityA : velocityB;
            changeTime = Time.fixedTime;
        }
    }
}
