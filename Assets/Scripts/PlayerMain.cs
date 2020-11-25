using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour {
    PlayerController playerCtrl;
    zFoxVirtualPad vPad;

    bool actionEtcRun = true;

    void Awake() {
        playerCtrl = GetComponent<PlayerController>();
        vPad = GameObject.FindObjectOfType<zFoxVirtualPad>();
    }

    void Update() {
        if (!playerCtrl.activeSts) {
            return;
        }

        float vpad_vertical = 0.0f;
        float vpad_horizontal = 0.0f;

        zFOXPAD_BUTTON vpad_btnA = zFOXPAD_BUTTON.NON;
        zFOXPAD_BUTTON vpad_btnB = zFOXPAD_BUTTON.NON;

        if (vPad != null) {
            vpad_vertical = vPad.vertical;
            vpad_horizontal = vPad.horizontal;
            vpad_btnA = vPad.buttonA;
            vpad_btnB = vPad.buttonB;
        }

        float joyMv = Input.GetAxis("Horizontal");

        float vpadMv = vpad_horizontal;
        vpadMv = Mathf.Pow(Mathf.Abs(vpadMv), 1.5f) * Mathf.Sign(vpadMv);
        playerCtrl.ActionMove(joyMv + vpadMv);

        if (Input.GetButtonDown("Jump") || vpad_btnA == zFOXPAD_BUTTON.DOWN) {
            playerCtrl.ActionJump();
            return;
        }

        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3") || vpad_btnB == zFOXPAD_BUTTON.DOWN) {
            if (Input.GetAxisRaw("Vertical") + vpad_vertical < 0.5f) {
                playerCtrl.ActionAttack();
            } else {
                playerCtrl.ActionAttackJump();
            }
        }

        if (Input.GetAxisRaw("Vertical") > 0.7f) {
            if (actionEtcRun) {
                playerCtrl.ActionEtc();
                actionEtcRun = false;
            }
        } else {
            actionEtcRun = true;
        }
    }
}
