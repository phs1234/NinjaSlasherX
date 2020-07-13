using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    PlayerController playerCtrl;
    bool actionEtcRun = true;

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
            return;
        }

        if (Input.GetButtonDown("Fire1")) {
            playerCtrl.ActionAttack();
        }

        if (Input.GetButtonDown("Fire2")) {
            playerCtrl.ActionAttackJump();
        }

        if (Input.GetAxisRaw("Vertical") > 0.7f)
        {
            if (actionEtcRun)
            {
                playerCtrl.ActionEtc();
                actionEtcRun = false;
            }
        }
        else {
            actionEtcRun = true;
        }
    }
}
