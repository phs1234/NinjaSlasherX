using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageA_Sun : MonoBehaviour
{
    Transform playerTrfm;

    void Awake()
    {
        playerTrfm = PlayerController.GetTransform();
    }

    void Update()
    {
        Vector3 targetPosition = playerTrfm.position + new Vector3(8.0f, 5.0f, 0.0f);
        transform.position = new Vector3(targetPosition.x, Mathf.Lerp(transform.position.y, targetPosition.y, 0.001f), targetPosition.z);
    }
}
