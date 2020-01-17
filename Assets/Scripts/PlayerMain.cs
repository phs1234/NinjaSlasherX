using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    PlayerController playerCtrl;

    void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!playerCtrl.activeSts) {
            return;
        }

        float joyMv = Input.GetAxis("Horizontal");
        playerCtrl.ActionMove(joyMv);

        if (Input.GetButtonDown("Jump")) {
            playerCtrl.ActionJump();
        }

        if (Input.GetButtonDown("Fire1")) {
            playerCtrl.ActionAttack();
        }
    }
}
