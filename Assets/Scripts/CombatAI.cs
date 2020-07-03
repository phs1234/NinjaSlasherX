using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAI : MonoBehaviour
{
    public int freeAIMax = 3;
    public int blockAttackAIMax = 10;

    void FixedUpdate()
    {
        // var? javascript?
        var activeEnemyMainList = new List<EnemyMain>();


        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemyList == null) {
            return;
        }

        foreach (GameObject enemy in enemyList) {
            EnemyMain enemyMain = enemy.GetComponent<EnemyMain>();

            if (enemyMain != null)
            {
                if (enemyMain.combatAIOrder && enemyMain.cameraEnabled)
                {
                    activeEnemyMainList.Add(enemyMain);
                }
            }
            else {

            }
        }


        //공격하는 적을 억제한다
        int i = 0;

        foreach (EnemyMain enemyMain in activeEnemyMainList) {
            if (i < freeAIMax)
            {
                // 자유롭게 공격
            }
            else if (i < freeAIMax + blockAttackAIMax)
            {
                if (enemyMain.aiState == ENEMYAISTS.RUNTOPLAYER)
                {
                    enemyMain.SetCombatAIState(ENEMYAISTS.WAIT);
                }
            }
            else {
                if (enemyMain.aiState != ENEMYAISTS.WAIT) {
                    enemyMain.SetCombatAIState(ENEMYAISTS.WAIT);
                }
            }

            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
