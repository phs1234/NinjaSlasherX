using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain_A : EnemyMain
{
    public int aiIfRUNTOPLAYER = 20;
    public int aiIfJUMPTOPLAYER = 30;
    public int aiIfESCAPE = 10;
    public int aiIfRETURNTODOGPILE = 10;

    public int damageAttack_A = 1;

    // AI state에 맞춰서 작동
    public override void FixedUpdateAI() {
        switch (aiState) {
            case ENEMYAISTS.ACTIONSELECT:
                int n = SelectRandomAIState();

                if (n < aiIfRUNTOPLAYER)
                {
                    SetAIState(ENEMYAISTS.RUNTOPLAYER, 3.0f);
                }
                else if (n < aiIfRUNTOPLAYER + aiIfJUMPTOPLAYER)
                {
                    SetAIState(ENEMYAISTS.JUMPTOPLAYER, 1.0f);
                }
                else if (n < aiIfRUNTOPLAYER + aiIfJUMPTOPLAYER + aiIfESCAPE)
                {
                    SetAIState(ENEMYAISTS.ESCAPE, 3.0f);
                }
                else if (n < aiIfRUNTOPLAYER + aiIfJUMPTOPLAYER + aiIfESCAPE + aiIfRETURNTODOGPILE)
                {
                    if (dogPile != null)
                    {
                        SetAIState(ENEMYAISTS.RETURNTODOGPILE, 3.0f);
                    }
                }
                else {
                    SetAIState(ENEMYAISTS.WAIT, 1.0f + Random.Range(0.0f, 1.0f));
                }

                enemyCtrl.ActionMove(0.0f);
                break;

            case ENEMYAISTS.WAIT:
                enemyCtrl.ActionLookup(player, 0.1f);
                enemyCtrl.ActionMove(0.0f);
                break;

            case ENEMYAISTS.RUNTOPLAYER:
                // 플레이어가 떨어져 있다면 다가가기 
                if (GetDistancePlayer() > 3.0f) {
                    SetAIState(ENEMYAISTS.JUMPTOPLAYER, 1.0f);
                }

                // 가까이 있으면 공격하기
                if (!enemyCtrl.ActionMoveToNear(player, 2.0f)) {
                    Attack_A();
                }

                break;

            case ENEMYAISTS.JUMPTOPLAYER:
                //가까이서 움직이면 공격하기 
                if (GetDistancePlayer() < 2.0f && IsChangeDistancePlayer(0.5f)) {
                    Attack_A();
                    break;
                }

                enemyCtrl.ActionJump();
                enemyCtrl.ActionMoveToNear(player, 0.1f);
                SetAIState(ENEMYAISTS.FREEZ, 0.5f);
                break;

            case ENEMYAISTS.ESCAPE:
                //너무 멀리 떨어지면 고민하기 
                if (!enemyCtrl.ActionMoveToFar(player, 7.0f)) {
                    SetAIState(ENEMYAISTS.ACTIONSELECT, 1.0f);
                }

                break;

            case ENEMYAISTS.RETURNTODOGPILE:
                if (enemyCtrl.ActionMoveToNear(dogPile, 2.0f))
                {
                    //? 실행이 안 되지 않나?
                    if (GetDistancePlayer() < 2.0f)
                    {
                        Attack_A();
                    }
                }
                else {
                    SetAIState(ENEMYAISTS.ACTIONSELECT, 1.0f);
                }

                break;
        }
    }


    public override void SetCombatAIState(ENEMYAISTS sts)
    {
        base.SetCombatAIState(sts);

        switch (aiState) {
            case ENEMYAISTS.ACTIONSELECT: break;
            case ENEMYAISTS.WAIT:
                aiActionTimeLength = 1.0f + Random.Range(0.0f, 1.0f); break;
            case ENEMYAISTS.RUNTOPLAYER: aiActionTimeLength = 3.0f; break;
            case ENEMYAISTS.JUMPTOPLAYER: aiActionTimeLength = 1.0f; break;
            case ENEMYAISTS.ESCAPE:
                aiActionTimeLength = Random.Range(2.0f, 5.0f); break;
            case ENEMYAISTS.RETURNTODOGPILE: aiActionTimeLength = 3.0f; break;
        }
    }

    void Attack_A() {
        enemyCtrl.ActionLookup(player, 0.1f);
        enemyCtrl.ActionMove(0.0f);
        enemyCtrl.ActionAttack("Attack_A", damageAttack_A);
        SetAIState(ENEMYAISTS.WAIT, 2.0f);
    }
}
