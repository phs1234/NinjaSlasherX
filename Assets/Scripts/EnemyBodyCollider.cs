using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyCollider : MonoBehaviour
{

    EnemyController enemyCtrl;
    Animator playerAnim;
    int attackHash = 0;

    void Awake()
    {
        enemyCtrl = GetComponentInParent<EnemyController>();
        playerAnim = PlayerController.GetAnimator();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);

        if (attackHash != 0 && stateInfo.nameHash == PlayerController.ANISTS_Idle) {
            attackHash = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerArm") {
            AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);

            if (attackHash != stateInfo.nameHash) {
                attackHash = stateInfo.nameHash;
                enemyCtrl.ActionDamage();
                Camera.main.GetComponent<CameraFollow>().AddCameraSize(-0.01f, -0.3f);
            }
        } else if (other.tag == "PlayerArmBullet") {
            Destroy(other.gameObject);
            enemyCtrl.ActionDamage();
        }
    }
}