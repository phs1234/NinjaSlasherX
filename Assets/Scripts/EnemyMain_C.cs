using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMain_C : EnemyMain
{
    public int aiIFATTACKONSIGHT = 50;
    public int aiIFRUNTOPLAYER = 30;
    public int aiIFESCAPE = 10;
    public int aiIfRETURNTODOGPILE = 10;
    public float aiPlayerEscapeDistance = 0.0f;

    public int damageAttack_A = 1;

    public int fireAttack_A = 3;
    public float waitAttack_A = 10.0f;

    int fireCountAttack_A = 0;

    public override void FixedUpdateAI()
    {
        enemyCtrl.ActionMoveToFar(player, aiPlayerEscapeDistance);

        switch (aiState) {
            case ENEMYAISTS.ACTIONSELECT:
                int n = SelectRandomAIState();

                if (n < aiIFATTACKONSIGHT)
                {
                    SetAIState(ENEMYAISTS.ATTACKONSIGTH, 5.0f);
                }
                else if (n < aiIFATTACKONSIGHT + aiIFRUNTOPLAYER)
                {
                    SetAIState(ENEMYAISTS.RUNTOPLAYER, 3.0f);
                }
                else if (n < aiIFATTACKONSIGHT + aiIFRUNTOPLAYER + aiIFESCAPE)
                {
                    SetAIState(ENEMYAISTS.ESCAPE, Random.Range(2.0f, 5.0f));
                }
                else if (n < aiIFATTACKONSIGHT + aiIfRETURNTODOGPILE + aiIFESCAPE + aiIfRETURNTODOGPILE) {
                    if (dogPile)
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
                enemyCtrl.ActionLookup(player, 0.0f);
                enemyCtrl.ActionMove(0.0f);
                break;

            case ENEMYAISTS.ATTACKONSIGTH:
                Attack_A();
                break;

            case ENEMYAISTS.RUNTOPLAYER:
                if (!enemyCtrl.ActionMoveToNear(player, 5.0f)) {
                    Attack_A();
                }
                break;

            case ENEMYAISTS.ESCAPE:
                if (!enemyCtrl.ActionMoveToFar(player, 4.0f))
                {
                    Attack_A();
                }
                break;

            case ENEMYAISTS.RETURNTODOGPILE:
                if (enemyCtrl.ActionMoveToNear(dogPile, 2.0f))
                {
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

        switch (aiState)
        {
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

        fireCountAttack_A++;
        if (fireCountAttack_A >= fireAttack_A) {
            fireCountAttack_A = 0;
            SetAIState(ENEMYAISTS.FREEZ, waitAttack_A);
        }
    }
}
